using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Imgur.API.Enums;
using Imgur.API.Models.Impl;
using Plugin.Media;

namespace Epicture
{
    public partial class MainPage : TabbedPage
    {
        EpictureImgur ImgurApi;
        public MainPage()
        {
            InitializeComponent();
            ImgurApi = new EpictureImgur();

            if (ImgurApi.Connected)
                SetLogoutButton();
            else
                SetLoginButton();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            var authorizationUrl = ImgurApi.OAuth2.GetAuthorizationUrl(OAuth2ResponseType.Token);

            DependencyService.Get<ILogin>()
                .GetLoginUI(authorizationUrl,
                ImgurApi,
                (object s, AuthenticatorCompletedEventArgs eventArgs) =>{
                    if (eventArgs.IsAuthenticated)
                        SetLogoutButton();
                });
        }

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            ImgurApi.DeleteAuthentificationToken();
            SetLoginButton();
        }

        private void SetLoginButton()
        {
            LoginButton.IsVisible = true;
            LogoutButton.IsVisible = false;
            Welcome.Text = "Welcome! You are not connected";
            UserGallery.IsVisible = false;
            FavoritesWelcome.Text = "You are not connected";
            UserFavorites.IsVisible = false;
            ReloadFavorites.IsVisible = false;
            UploadImage.IsVisible = false;
        }

        private void SetLogoutButton()
        {
            LoginButton.IsVisible = false;
            LogoutButton.IsVisible = true;
            Welcome.Text = $"Welcome! {ImgurApi.AccountUsername}";
            UserGallery.IsVisible = true;
            FavoritesWelcome.Text = "Your Favorites";
            UserFavorites.IsVisible = true;
            UploadImage.IsVisible = true;
            ReloadFavorites.IsVisible = true;
            AffUserGalleryAsync();
            AffUserFavortiesAsync();
        }

        private async Task AffUserFavortiesAsync()
        {
            try
            {
                IEnumerable<Imgur.API.Models.IGalleryItem> items =
                    await ImgurApi.AccountEndpoint.GetAccountFavoritesAsync();
                LoadUserFavorites(items);
            }
            catch
            {}
        }

        private async Task AffUserGalleryAsync()
        {
            try
            {
                IEnumerable<Imgur.API.Models.IImage> enumImages =
                    await ImgurApi.AccountEndpoint.GetImagesAsync();
                LoadUserGallery(enumImages);
            }
            catch
            { }
        }

        private async void UploadImage_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            Plugin.Media.Abstractions.MediaFile file =
                await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;
            ImgurApi.ImageEndpoint.UploadImageStreamAsync(file.GetStream()).GetAwaiter().GetResult();
            AffUserGalleryAsync();
        }

        private void LoadUserGallery(IEnumerable<Imgur.API.Models.IImage> ImageGallery)
        {
            activityUserGallery.IsRunning = true;
            activityUserGallery.IsVisible = true;
            try
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, e) =>
                {
                    MyImage theImage = (MyImage)sender;
                    Navigation.PushAsync(new NavigationPage(new BigPicture(theImage)));
                };

                UserGallery.Children.Clear();
                foreach (var item in ImageGallery.ToList())
                {
                    var image = new MyImage
                    {
                        Gallery = null,
                        ImageInf = item,
                        Link = item.Link,
                        ImageId = item.Id,
                        Source = ImageSource.FromUri(new Uri(Regex.Replace(item.Link, @"^(.*)(\.[^\.]*)$", "$1b$2")))
                    };
                    image.GestureRecognizers.Add(tapGestureRecognizer);
                    UserGallery.Children.Add(image);
                }
            }
            catch
            {
                UserGallery.Children.Add(new Label
                {
                    Text = "Cannot access list of bitmap files"
                });
            }
            activityUserGallery.IsRunning = false;
            activityUserGallery.IsVisible = false;
        }

        private void LoadUserFavorites(IEnumerable<Imgur.API.Models.IGalleryItem> ImageGallery)
        {
            activityUserFavorites.IsVisible = true;
            activityUserFavorites.IsRunning = true;
            try
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, e) =>
                {
                    MyImage theImage = (MyImage)sender;
                    Navigation.PushAsync(new NavigationPage(new BigPicture(theImage)));
                };

                UserFavorites.Children.Clear();
                foreach (var item in ImageGallery)
                {
                    if (item.GetType() == typeof(GalleryAlbum))
                    {
                        var image = new MyImage
                        {
                            Gallery = item,
                            Link = ((GalleryAlbum)item).Images.ToList()[0].Link,
                            ImageId = ((GalleryAlbum)item).Id,
                            Source = ImageSource.FromUri(new Uri(Regex.Replace(((GalleryAlbum)item).Images.ToList()[0].Link, @"^(.*)(\.[^\.]*)$", "$1b$2")))
                        };
                        image.GestureRecognizers.Add(tapGestureRecognizer);
                        UserFavorites.Children.Add(image);
                    }
                    else if (item.GetType() == typeof(GalleryImage))
                    {
                        var image = new MyImage
                        {
                            Gallery = item,
                            Link = ((GalleryImage)item).Link,
                            ImageId = ((GalleryImage)item).Id,
                            Source = ImageSource.FromUri(new Uri(Regex.Replace(((GalleryImage)item).Link, @"^(.*)(\.[^\.]*)$", "$1b$2")))
                        };
                        image.GestureRecognizers.Add(tapGestureRecognizer);
                        UserFavorites.Children.Add(image);
                    }
                }
            }
            catch
            {
                UserFavorites.Children.Add(new Label
                {
                    Text = "Cannot access list of bitmap files"
                });
            }
            activityUserFavorites.IsRunning = false;
            activityUserFavorites.IsVisible = false;
        }


        private void ReloadFavorites_Clicked(object sender, EventArgs e)
        {
            AffUserFavortiesAsync();
        }
    }
}
