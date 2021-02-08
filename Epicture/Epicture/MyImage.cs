using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Epicture
{
    public class MyImage : Image
    {
        public Imgur.API.Models.IGalleryItem Gallery;
        public Imgur.API.Models.IImage ImageInf;
        public string Link;
        public string ImageId;
    }
}
