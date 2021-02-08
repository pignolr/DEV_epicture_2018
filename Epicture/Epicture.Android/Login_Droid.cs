using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Epicture.Droid.Login_Droid))]
namespace Epicture.Droid
{
    public class Login_Droid : ILogin
    {
        Intent Intent;
        EpictureImgur ImgurApi;
        public void GetLoginUI(string authorizationUrl,
            EpictureImgur injectedImgurApi,
            EventHandler<AuthenticatorCompletedEventArgs> callback)
        {
            var context = MainActivity.Instance;

            ImgurApi = injectedImgurApi;

            var auth = new OAuth2Authenticator(
                ImgurApi.ClientImgurId,
                ImgurApi.ClientImgurSecret,
                null,
                new Uri(authorizationUrl),
                new Uri("com.epitech.epicture://imguroauth2redirect"),
                new Uri("https://api.imgur.com/oauth2/token"),
                null,
                false)
            { AllowCancel = true, };

            auth.Completed += LoginComplete;
            auth.Completed += callback;

            auth.Error += LoginError;

            Intent = auth.GetUI(context);
            context.StartActivity(Intent);
        }
        private void LoginComplete(object sender, AuthenticatorCompletedEventArgs eventArgs)
        {
            if (eventArgs.IsAuthenticated)
            {
                //saves user. 
                Xamarin.Forms.Application.Current.Properties["access_token"]
                    = eventArgs.Account.Properties["access_token"];
                Xamarin.Forms.Application.Current.Properties["refresh_token"]
                    = eventArgs.Account.Properties["refresh_token"];
                Xamarin.Forms.Application.Current.Properties["token_type"]
                    = eventArgs.Account.Properties["token_type"];
                Xamarin.Forms.Application.Current.Properties["account_id"]
                    = eventArgs.Account.Properties["account_id"];
                Xamarin.Forms.Application.Current.Properties["account_username"]
                    = eventArgs.Account.Properties["account_username"];
                Xamarin.Forms.Application.Current.Properties["expires_in"]
                    = eventArgs.Account.Properties["expires_in"];
                Xamarin.Forms.Application.Current.SavePropertiesAsync();

                ImgurApi.SetAuthentificationToken();
            }
        }
        private void LoginError(object sender, AuthenticatorErrorEventArgs eventArgs)
        {
            // Error message
            StringBuilder sb = new StringBuilder();
            sb.Append("Error = ").AppendLine($"{eventArgs.Message}");

            // Dialog box
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(MainActivity.Instance);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("Authentication Error");
            alert.SetMessage(sb.ToString());
            alert.SetButton("OK", (c, ev) => { });
            alert.Show();

            return;
        }
    }
}
