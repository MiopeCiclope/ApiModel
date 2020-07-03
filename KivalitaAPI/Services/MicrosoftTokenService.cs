
using AutoMapper;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
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

        public MicrosoftTokenService(
            KivalitaApiContext context
            , MicrosoftTokenRepository baseRepository
            , IOptions<Settings> settings
            , IMapper mapper
        ) : base(context, baseRepository) {
            _myConfiguration = settings.Value;
            _mapper = mapper;
        }

        public bool Auth(MicrosoftAuthDTO auth, int userId)
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

            return true;
        }

        public bool RefreshToken(int userId)
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
                return true;
            }
            else
                return false;
        }
    }
}


