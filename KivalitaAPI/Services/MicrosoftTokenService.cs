
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
            base.Add(entity);

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
            var graphToken = this.baseRepository.GetBy(token => token.UserId == userId).First();
            var token = (graphToken.ExpirationDate <= DateTime.UtcNow) ? this.RefreshToken(userId).AccessToken : graphToken.AccessToken;

            return new GraphServiceClient(new DelegateAuthenticationProvider(async request => {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }));
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
    }
}


