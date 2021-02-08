using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Epicture.Droid
{
    [Activity(Label = "ImgurAuthInterceptor")]
    [
        IntentFilter
        (
            actions: new[] { Intent.ActionView },
            Categories = new[]
            {
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            },
            DataSchemes = new[]
            {
            // First part of the redirect url (Package name)
            "com.epitech.epicture"
            },
            DataPaths = new[]
            {
            // Second part of the redirect url (Path)
            "/imguroauth2redirect"
            }
        )
    ]
    class ImgurAuthInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Android.Net.Uri uri_android = Intent.Data;
            Uri uri_netfx = new Uri(uri_android.ToString());
//            AuthenticationState.Authenticator.OnPageLoading(uri_netfx);

            Xamarin.Forms.Application.Current.Properties["access_token"] 
                = HttpUtility.ParseQueryString(uri_netfx.Query).Get("access_token");
            Xamarin.Forms.Application.Current.Properties["refresh_token"]
                = HttpUtility.ParseQueryString(uri_netfx.Query).Get("refresh_token");
            Xamarin.Forms.Application.Current.Properties["token_type"]
                = HttpUtility.ParseQueryString(uri_netfx.Query).Get("token_type");
            Xamarin.Forms.Application.Current.Properties["account_id"]
                = HttpUtility.ParseQueryString(uri_netfx.Query).Get("account_id");
            Xamarin.Forms.Application.Current.Properties["account_username"]
                = HttpUtility.ParseQueryString(uri_netfx.Query).Get("account_username");
            Xamarin.Forms.Application.Current.Properties["expires_in"]
                = HttpUtility.ParseQueryString(uri_netfx.Query).Get("expires_in");
            Xamarin.Forms.Application.Current.SavePropertiesAsync();

            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            Finish();
            return;
        }
    }
}