using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using App4.sources;
using Newtonsoft.Json;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App4
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TimeLine : Page
    {
        private const string Uri = "https://nmb.fastmirror.org/Api/timeline";

        private static int _timeLinePage = 1;
        private ObservableCollection<PostContent> _postContents;

        /*  The current structure:
         *  ContentStackPanel -> grid -> rootStackPanel -> infoStackPanel -> titleTextBlock
         *                    |                         |                 -> nameTextBlock
         *                    |                         |                 -> noTextBlock
         *                    |                         |                 
         *                    |                         -> contentStackPanel -> image
         *                    |                                              -> textBlock
         *                    -> ...(same grid)
         *                    -> ...
         */
        public TimeLine()
        {
            InitializeComponent();
        }


        private string GetPostContent(string uri)
        {
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);
            httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            var result = httpClient.GetStringAsync(uri).Result;
            httpClient.Dispose();
            return result;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid;
            grid = sender as Grid;
            if (grid != null)
            {
                var id =
                    (((grid.Children.ToList()[0] as StackPanel).Children.ToList()[0] as StackPanel).Children
                        .ToList()[2] as TextBlock).Text;
                id = id.Replace("No.", "?id=");
                _timeLinePage = 1;
                Frame.Navigate(typeof(Thread), id);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _postContents = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = GetPostContent(Uri);
            _postContents =
                new ObservableCollection<PostContent>(JsonConvert.DeserializeObject<List<PostContent>>(result));
            ContentListView.ItemsSource = _postContents;
        }

        private void rootScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if ((sender as ScrollViewer).VerticalOffset == (sender as ScrollViewer).ScrollableHeight)
            {
                var newUri = $"{Uri}?page={++_timeLinePage}";
                var result = GetPostContent(newUri);
                foreach (var thread in new ObservableCollection<PostContent>(
                    JsonConvert.DeserializeObject<List<PostContent>>(result))) _postContents.Add(thread);
            }
        }

        private void ShowMenu(bool isTransient, object sender)
        {
            var myOption = new FlyoutShowOptions();
            myOption.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
            if (sender != null) CommandBarFlyout1.ShowAt(sender as Image, myOption);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            var test = ((CommandBarFlyout1.Target as Image).GetValue(Image.SourceProperty) as BitmapImage).UriSource;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(ImageDetail), test);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            var imageSave = new ImageSave();
            ImageSave.SaveImage(((CommandBarFlyout1.Target as Image).GetValue(Image.SourceProperty) as BitmapImage)
                .UriSource);
        }

        private void image_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            ShowMenu(false, null);
        }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ShowMenu(true, sender as Image);
            e.Handled = true;
        }

        private async void ThreadWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var webView = sender;

            int width;
            int height;

            // get the total width and height
            var widthString = await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollWidth.toString()"});
            var heightString = await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

            if (!int.TryParse(widthString, out width)) throw new Exception("Unable to get page width");
            if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");

            // resize the webview to the content
            webView.Width = width;
            webView.Height = height;
        }

        //private void contentStackPanel_Loading(FrameworkElement sender, object args)
        //{
        //    StackPanel stackPanel = sender as StackPanel;
        //    WebView webView = new WebView();
        //    webView.MinWidth = 600;
        //    webView.DefaultBackgroundColor = new Windows.UI.Color { R = 255, G = 255, B = 255, A = 60 };
        //    webView.SetValue(WebViewEx.UriProperty, "<html><body>res</body></html>");
        //    stackPanel.Children.Add(webView);
        //}
    }
}