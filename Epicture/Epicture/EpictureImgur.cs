using System;
using System.Collections.Generic;
using System.Text;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models.Impl;
using System.Linq;
using Xamarin.Auth;

namespace Epicture
{
    public class EpictureImgur
    {
        public string ClientImgurId { get; }
        public string ClientImgurSecret { get; }
        public ImgurClient Client { get; private set; }
        public AccountEndpoint AccountEndpoint { get; private set; }
        public ImageEndpoint ImageEndpoint { get; private set; }
        public AlbumEndpoint AlbumEndpoint{ get; private set; }
        public GalleryEndpoint GalleryEndpoint { get; private set; }
        public OAuth2Endpoint OAuth2 { get; private set; }
        public OAuth2Token OAuth2Token { get; private set; }

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string TokenType { get; private set; }
        public string AccountId { get; private set; }
        public string AccountUsername { get; private set; }
        public int ExpiresIn { get; private set; }
        public bool Connected { get; private set; }

        public EpictureImgur()
        {
            ClientImgurId = "46fb201282dd862";
            ClientImgurSecret = "d2e6a2b4ffa3b636f12a3a84fb7ea0630a0e44b6";

            SetAuthentificationToken();
        }

        public void SetAuthentificationToken()
        {
            var properties = Xamarin.Forms.Application.Current.Properties;

            AccessToken = properties.ContainsKey("access_token")
                && properties["access_token"] != null ?
                properties["access_token"].ToString() : null;
            RefreshToken = properties.ContainsKey("refresh_token")
                && properties["refresh_token"] != null ?
                properties["refresh_token"].ToString() : null;
            TokenType = properties.ContainsKey("token_type")
                && properties["token_type"] != null ?
                properties["token_type"].ToString() : null;
            AccountId = properties.ContainsKey("account_id")
                && properties["account_id"] != null ?
                properties["account_id"].ToString() : null;
            AccountUsername = properties.ContainsKey("account_username")
                && properties["account_username"] != null ?
                properties["account_username"].ToString() : null;
            ExpiresIn = properties.ContainsKey("expires_in")
                && properties["expires_in"] != null ?
                int.Parse(properties["expires_in"].ToString()) : 0;

            if (AccessToken != null)
            {
                OAuth2Token = new OAuth2Token(AccessToken, RefreshToken, TokenType,
                    AccountId, AccountUsername, ExpiresIn);
                Connected = true;

                Client = new ImgurClient(ClientImgurId, OAuth2Token);
                OAuth2 = new OAuth2Endpoint(Client);
                AccountEndpoint = new AccountEndpoint(Client);
                ImageEndpoint = new ImageEndpoint(Client);
                GalleryEndpoint = new GalleryEndpoint(Client);
                AlbumEndpoint = new AlbumEndpoint(Client);
            }
            else
            {
                Connected = false;

                Client = new ImgurClient(ClientImgurId);
                OAuth2 = new OAuth2Endpoint(Client);
                AccountEndpoint = new AccountEndpoint(Client);
                ImageEndpoint = new ImageEndpoint(Client);
                GalleryEndpoint = new GalleryEndpoint(Client);
                AlbumEndpoint = new AlbumEndpoint(Client);
            }
        }

        public void DeleteAuthentificationToken()
        {
            var properties = Xamarin.Forms.Application.Current.Properties;
            properties["access_token"] = null;
            properties["refresh_token"] = null;
            properties["token_type"] = null;
            properties["account_id"] = null;
            properties["account_username"] = null;
            properties["expires_in"] = 0;
            Xamarin.Forms.Application.Current.SavePropertiesAsync();
            SetAuthentificationToken();
        }
    }
}
