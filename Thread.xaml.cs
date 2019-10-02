using App4.sources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Thread : Page
    {
        ThreadItem threaditems;
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
            var uri = "https://adnmb2.com/api/thread/"+ (string)e.Parameter;
            var result = GetPostContent(uri);
            threaditems = JsonConvert.DeserializeObject<ThreadItem>(result);
            contentListView.ItemsSource = threaditems.replys;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            threaditems.Dispose();
            threaditems = null;
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
    }
}
