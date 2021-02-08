using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace Epicture
{
    public interface ILogin
    {
        void GetLoginUI(string authorizationUrl,
            EpictureImgur injectedImgurApi,
            EventHandler<AuthenticatorCompletedEventArgs> callback);
    }
}
