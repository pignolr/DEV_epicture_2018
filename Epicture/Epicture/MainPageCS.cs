using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Epicture
{
    public class MainPageCS : TabbedPage
    {
        public MainPageCS()
        {
            var navigationPage = new NavigationPage(new SearchPage());

            Children.Add(navigationPage);
            Children.Add(navigationPage);
        }
    }
}
