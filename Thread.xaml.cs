using App4.sources;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Thread : Page
    {
        ThreadItem _threaditems;
        public Thread()
        {
            this.InitializeComponent();

        }
        private string GetPostContent(string uri)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string result = httpClient.GetStringAsync(uri).Result;
            httpClient.Dispose();
            return result;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var uri = "https://adnmb2.com/api/thread/" + (string)e.Parameter;
            var result = GetPostContent(uri);
            _threaditems = JsonConvert.DeserializeObject<ThreadItem>(result);
            contentListView.ItemsSource = _threaditems.replys;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _threaditems.Dispose();
            _threaditems = null;
        }
        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout imageFlyout = new Flyout();
            Image orginImage = new Image();
            Button saveButton = new Button();
            StackPanel flyoutStackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            flyoutStackPanel.Children.Add(orginImage);
            orginImage.Source = (sender as Image).Source;


            flyoutStackPanel.Children.Add(saveButton);
            saveButton.Content = "Save";

            imageFlyout.Content = flyoutStackPanel;
            imageFlyout.ShowAt((FrameworkElement)sender);
            e.Handled = true;
        }

        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var webView = sender as WebView;

            int width;
            int height;

            // get the total width and height
            var widthString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
            var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            if (!int.TryParse(widthString, out width))
            {
                throw new Exception("Unable to get page width");
            }
            if (!int.TryParse(heightString, out height))
            {
                throw new Exception("Unable to get page height");
            }

            // resize the webview to the content
            webView.Width = width;
            webView.Height = height;
        }
    }
}
