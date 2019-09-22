using App4.sources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public Thread()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var uri = "https://adnmb2.com/api/thread/"+ (string)e.Parameter;
            //TextBlock textBlock = new TextBlock();
            //textBlock.Text = uri;
            //ContentStackPanel.Children.Add(textBlock);
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            var result = httpClient.GetStringAsync(uri).Result;
            httpClient.Dispose();
            ThreadItem threaditems = JsonConvert.DeserializeObject<ThreadItem>(result);
            foreach (var postcontent in threaditems.replys)
            {
                Grid grid = new Grid
                {
                    MinHeight = 100,
                    Margin = new Thickness(0, 2, 0, 2),
                    Background = new SolidColorBrush(new Windows.UI.Color { A = 208, R = 208, G = 208, B = 208 })
                };

                StackPanel rootStackPanel = new StackPanel();
                StackPanel contentStackPanel = new StackPanel();

                StackPanel infoStackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                TextBlock titleTextBlock = new TextBlock
                {
                    Text = postcontent.title,
                    Foreground = new SolidColorBrush(new Windows.UI.Color { B = 5, R = 204, G = 17, A = 255 })
                };
                infoStackPanel.Children.Add(titleTextBlock);

                TextBlock nameTextBlock = new TextBlock
                {
                    Text = "  " + postcontent.name,
                    Foreground = new SolidColorBrush(new Windows.UI.Color { R = 17, G = 119, B = 67, A = 255 })
                };
                infoStackPanel.Children.Add(nameTextBlock);

                TextBlock noTextBlock = new TextBlock
                {
                    Text = "  No." + postcontent.id,
                    Foreground = new SolidColorBrush(new Windows.UI.Color { R = 0, G = 85, B = 153, A = 255 })
                };
                infoStackPanel.Children.Add(noTextBlock);

                rootStackPanel.Children.Add(infoStackPanel);

                TextBlock infoTextBlock = new TextBlock();
                string info = postcontent.now + "  ID:" + postcontent.userid;
                infoTextBlock.Text = info;

                rootStackPanel.Children.Add(infoTextBlock);
                rootStackPanel.Children.Add(contentStackPanel);

                contentStackPanel.Orientation = Orientation.Horizontal;

                grid.Children.Add(rootStackPanel);

                //TextBlock textBlock = new TextBlock();
                //textBlock.TextWrapping = TextWrapping.Wrap;
                //textBlock.MaxWidth = 600;
                //textBlock.Text = postcontent.content.Replace("<br />", "");
                //textBlock.Margin = new Thickness(0, 0, 0, 2);
                WebView contentWebView = new WebView();
                string threadContent = string.Format("<html><body>{0}</body></html>", postcontent.content.Replace("<br />", ""));
                contentWebView.NavigateToString(threadContent);
                contentWebView.DefaultBackgroundColor = new Windows.UI.Color { A = 0, B = 255, R = 255, G = 255 };
                contentWebView.MinHeight = 200;
                contentWebView.MaxWidth = 600;
                contentWebView.MinWidth = 200;

                if (postcontent.img != "")
                {
                    Image image = new Image();
                    string imageSource = "https://nmbimg.fastmirror.org/image/" + postcontent.img.Replace("\\", "") + postcontent.ext;
                    BitmapImage bitmapImage = new BitmapImage
                    {
                        UriSource = new Uri(imageSource)
                    };
                    image.Source = bitmapImage;
                    image.MaxHeight = 250;
                    image.MaxWidth = 250;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    contentStackPanel.Children.Add(image);
                }
                contentStackPanel.Children.Add(contentWebView);

                ContentStackPanel.Children.Add(grid);
            }
        }
    }
}
