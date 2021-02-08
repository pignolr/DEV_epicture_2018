using Imgur.API.Models.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Epicture
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BigPicture : ContentPage
	{
        EpictureImgur ImgurApi { set; get; }
        MyImage TheImage { set; get; }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public BigPicture(MyImage theImage)
        {
            InitializeComponent();
            ImgurApi = new EpictureImgur();
            TheImage = theImage;
            displayImage();
            Favorite.IsVisible = false;
            Unfavorite.IsVisible = false;
            if (ImgurApi.Connected)
            {
                bool favorite = false;
                if (TheImage.Gallery != null && (TheImage.Gallery.GetType() == typeof(GalleryAlbum) || TheImage.Gallery.GetType() == typeof(GalleryImage)))
                {
                    if (TheImage.Gallery.GetType() == typeof(GalleryAlbum))
                        favorite = ((GalleryAlbum)TheImage.Gallery).Favorite == true;
                    else
                        favorite = ((GalleryImage)TheImage.Gallery).Favorite == true;
                    Favorite.IsVisible = !favorite;
                    Unfavorite.IsVisible = favorite;
                }
            }
        }
        public void displayImage()
        {
            if (TheImage.Gallery != null && TheImage.Gallery.GetType() == typeof(GalleryAlbum))
            {
                handleAlbum();
                return ;
            }
            var image = new Xamarin.Forms.Image
            {
                Source = ImageSource.FromUri(new Uri(TheImage.Link))
            };
            flexLayout.Children.Add(image);
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }

        public void handleAlbum()
        {
            try
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, e) =>
                {
                    MyImage anImage = (MyImage)sender;
                    Navigation.PushAsync(new NavigationPage(new BigPicture(anImage)));
                };
                foreach (var item in ((GalleryAlbum)TheImage.Gallery).Images.ToList())
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
                    flexLayout.Children.Add(image);
                }
            }
            catch
            {
                flexLayout.Children.Add(new Label
                {
                    Text = "Cannot access list of bitmap files"
                });
            }
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }
        public async void selfDestruct(object sender, EventArgs e)
        {
            try
            {
                bool favorite;
                if (TheImage.Gallery != null && (TheImage.Gallery.GetType() == typeof(GalleryAlbum) || TheImage.Gallery.GetType() == typeof(GalleryImage)))
                {
                    if (TheImage.Gallery.GetType() == typeof(GalleryAlbum))
                    {
                        string id = ((GalleryAlbum)TheImage.Gallery).Id;
                        ((GalleryAlbum)TheImage.Gallery).Favorite =
                            await ImgurApi.AlbumEndpoint.FavoriteAlbumAsync(id);
                        favorite = ((GalleryAlbum)TheImage.Gallery).Favorite == true;
                    }
                    else
                    {
                        string id = ((GalleryImage)TheImage.Gallery).Id;
                        ((GalleryImage)TheImage.Gallery).Favorite =
                            await ImgurApi.ImageEndpoint.FavoriteImageAsync(id);
                        favorite = ((GalleryImage)TheImage.Gallery).Favorite == true;
                    }
                    Favorite.IsVisible = !favorite;
                    Unfavorite.IsVisible = favorite;
                }
                else if (TheImage.ImageInf != null && TheImage.ImageInf.GetType() == typeof(Imgur.API.Models.IImage))
                {
                    string id = ((Imgur.API.Models.IImage)TheImage.ImageInf).Id;
                    ((Imgur.API.Models.IImage)TheImage.ImageInf).Favorite =
                        await ImgurApi.ImageEndpoint.FavoriteImageAsync(id);
                    favorite = ((Imgur.API.Models.IImage)TheImage.ImageInf).Favorite == true;
                    Favorite.IsVisible = !favorite;
                    Unfavorite.IsVisible = favorite;
                }
            }
            catch
            {
                await DisplayAlert("error", "Favorite fail", "OK");
            }
        }
	}
}