
using AutoMapper;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KivalitaAPI.Services
{

    public class MicrosoftTokenService : Service<MicrosoftToken, KivalitaApiContext, MicrosoftTokenRepository>
    {
        private readonly Settings _myConfiguration;
        private readonly string authUrl = "/oauth2/v2.0/token";
        private readonly IMapper _mapper;
        private readonly ILogger<MicrosoftTokenService> _logger;

        public MicrosoftTokenService(
            KivalitaApiContext context
            , MicrosoftTokenRepository baseRepository
            , IOptions<Settings> settings
            , IMapper mapper
            , ILogger<MicrosoftTokenService> logger
        ) : base(context, baseRepository) {
            _myConfiguration = settings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        public MicrosoftToken Auth(MicrosoftAuthDTO auth, int userId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("grant_type", "password");
            dict.Add("client_id", _myConfiguration.ClientId);
            dict.Add("scope", _myConfiguration.Scopes);
            dict.Add("userName", auth.login);
            dict.Add("password", auth.password);

            string url = $"{_myConfiguration.MicrosoftUrl}{_myConfiguration.TinantId}{authUrl}";
            var token = RestClient.PostFormUrlEncoded<GraphAuthDTO>(url, dict).Result;
            var entity = _mapper.Map<MicrosoftToken>(token);
            entity.UserId = userId;

            if (token.access_token == null) 
                return null;

            base.Add(entity);

            RegisterWebhookToAnsweredEmailsAsync(userId).Wait();

            return entity;
        }

        public MicrosoftToken RefreshToken(int userId)
        {
            var tokenQuery = base.baseRepository.GetBy(token => token.UserId == userId);
            if (tokenQuery.Any())
            {
                var storedToken = tokenQuery.First();

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("grant_type", "refresh_token");
                dict.Add("client_id", _myConfiguration.ClientId);
                dict.Add("scope", _myConfiguration.Scopes);
                dict.Add("refresh_token", storedToken.RefreshToken);

                string url = $"{_myConfiguration.MicrosoftUrl}{_myConfiguration.TinantId}{authUrl}";
                var token = RestClient.PostFormUrlEncoded<GraphAuthDTO>(url, dict).Result;
                var entity = _mapper.Map<MicrosoftToken>(token);

                storedToken.AccessToken = entity.AccessToken;
                storedToken.RefreshToken = entity.RefreshToken;
                storedToken.ExpirationDate = entity.ExpirationDate;

                base.Update(storedToken);
                return storedToken;
            }
            else
                return null;
        }

        public GraphServiceClient GetTokenClient(int userId)
        {
            var graphToken = this.baseRepository.GetBy(token => token.UserId == userId).FirstOrDefault();

            if (graphToken != null)
            {
                var token = (graphToken.ExpirationDate <= DateTime.UtcNow) ? this.RefreshToken(userId).AccessToken : graphToken.AccessToken;

                return new GraphServiceClient(new DelegateAuthenticationProvider(async request => {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }));
            }
           
            return null;
        }

        public bool SendMail(GraphServiceClient client, Message email, int userId)
        {
            try
            {
                client.Me
                    .SendMail(email, null)
                    .Request()
                    .PostAsync()
                    .Wait();

                return true;
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public bool ShoulSendMail(GraphServiceClient client, string leadMail, int userId) 
        {
            try
            {
                var leadDidAnswor = client.Me
                    .MailFolders
                    .Inbox
                    .Messages
                    .Request()
                    .Filter($"(from/emailAddress/address) eq '{leadMail}'")
                    .GetAsync()
                    .Result
                    .Any();

                return !leadDidAnswor;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public bool DidReply(GraphServiceClient client, string leadMail)
        {
            try
            {
                var leadDidAnswor = client.Me
                    .MailFolders
                    .Inbox
                    .Messages
                    .Request()
                    .Filter($"receivedDateTime ge 1900-01-01T00:00:00Z and (from/emailAddress/address) eq '{leadMail}'")
                    .OrderBy($"ReceivedDateTime desc")
                    .Select("internetMessageHeaders")
                    .GetAsync()
                    .Result
                    .ToList();

                var hasReplyEmail = leadDidAnswor?
                                    .Where(mail => mail.InternetMessageHeaders
                                                    .Where(header => header.Name == "In-Reply-To")
                                                    .Any()
                                                    )?.Any() ?? false;
                return hasReplyEmail;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public override MicrosoftToken Delete(int userId, int responsableId)
        {
            var tokenUserList = base.GetAll().Where(token => token.UserId == userId).ToList();
            base.DeleteRange(tokenUserList);
            return tokenUserList.First();
        }

        public Message BuildEmail(Leads lead, Template template, int taskId, string signature)
        {
            try
            {
                return new Message
                {
                    Subject = this.ReplaceVariables(template.Subject, lead),
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = $"{this.ReplaceVariables(template.Content, lead)}{GetTracker(lead, taskId)}{signature}"
                    },
                    ToRecipients = new List<Recipient>() {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = $"{lead.Email}"
                            //Address = $"romulo.carvalho@kivalita.com.br"
                        }
                    }
                }
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao criar e-mails");
                _logger.LogError($"{e.Message}");
                throw e;
            }
        }

        private string ReplaceVariables(string text, Leads lead)
        {
            try
            {
                var templateTransformService = new TemplateTransformService();

                return templateTransformService.TransformLead(text, lead);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao substituir variáveis");
                _logger.LogError($"{e.Message}");
                throw e;
            }
        }

        private string GetTracker(Leads lead, int taskId)
        {
            var text = $"{taskId}-{lead.Id}";
            var encriptKey = HttpUtility.UrlEncode(AesCripty.EncryptString(Setting.MailTrackSecret, text), Encoding.UTF8);
            var url = $"<img src = \"{Setting.SelfUrl}tracker/track?key={encriptKey}\" width=1 height=1 style=\"mso-hide:all; display:none; line-height: 0; font-size: 0; height: 0; padding: 0; visibility:hidden;\"/>";
            return url;
        }

        public async Task<IGraphServiceSubscriptionsCollectionPage> RegisterWebhookToAnsweredEmailsAsync(int userId)
        {
            var folderList = new List<string> { "PokeLead Bounce", "PokeLead Positivo", "PokeLead Negativo" };
            var graphClient = GetTokenClient(userId);

            foreach (var folderName in folderList)
            {
                var folders = await graphClient.Me.MailFolders.Request().Filter($"displayName eq '{folderName}'").GetAsync();
                MailFolder folder;

                if (folders.Count > 0)
                {
                    folder = folders.First();
                }
                else
                {
                    folder = await graphClient.Me.MailFolders.Request().AddAsync(new MailFolder
                    {
                        DisplayName = folderName
                    });
                }

                var subscription = new Subscription
                {
                    ChangeType = "created",
                    NotificationUrl = $"{Setting.SelfUrl}Graph/Webhook/{userId}/{folderName}/GetAnsweredEmailsQualified",
                    Resource = $"me/mailFolders('{folder.Id}')/messages",
                    ExpirationDateTime = DateTimeOffset.UtcNow.AddMinutes(4200)
                };

                await graphClient.Subscriptions.Request().AddAsync(subscription);
            }

            return await graphClient.Subscriptions.Request().GetAsync();
        }
    }
}


