using System;
using Xamarin.Forms;

namespace Xamarine
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeEmbeddedHost();
            var names = typeof(MainPage).Assembly.GetManifestResourceNames();
        }

        private void InitializeEmbeddedHost()
        {
            Startup.Initialize();
            WebView.Source = "http://localhost:9696/";
        }
    }
}
