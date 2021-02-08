using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models.Impl;
using Imgur.API.Authentication.Impl;
using System.Text.RegularExpressions;

namespace Epicture
{
    public partial class SearchPage : ContentPage
    {
        EpictureImgur eclient = new EpictureImgur();
        // Class for deserializing JSON list of sample bitmaps
        public SearchPage()
            {
                InitializeComponent();
                basicDisp();
            }
        private async void basicDisp()
        {
            var client = eclient.Client;
            var endpoint = eclient.GalleryEndpoint;
            var images = await endpoint.SearchGalleryAsync("viral");
            LoadGallery(images);
        }
        private void LoadGallery(IEnumerable<Imgur.API.Models.IGalleryItem> ImageGallery)
        {
            try
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, e) =>
                {
                    MyImage theImage = (MyImage)sender;
                    Navigation.PushAsync(new NavigationPage(new BigPicture(theImage)));
                };

                flexLayout.Children.Clear();
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
                        flexLayout.Children.Add(image);
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
                        flexLayout.Children.Add(image);
                    }
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

        private async void SearchComm(object sender, EventArgs e)
        {
            var text = BrowseImage.Text;
            var client = eclient.Client;
            var endpoint = eclient.GalleryEndpoint;

            if (text.Length <= 0 || text == string.Empty || text == null)
            {
                var images = await endpoint.GetRandomGalleryAsync();
                LoadGallery(images);
            }
            else
            {
                var images = await endpoint.SearchGalleryAsync(text);
                LoadGallery(images);
            }
        }
    }
}