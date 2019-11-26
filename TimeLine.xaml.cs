using App4.sources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AppStudio.Uwp.Controls;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TimeLine : Page
    {

        static int timeLinePage = 1;
        const string uri = "https://nmb.fastmirror.org/Api/timeline";
        ObservableCollection<PostContent> postContents;

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
            this.InitializeComponent();
        }



        private string GetPostContent(string uri)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(httpClientHandler);
            httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string result = httpClient.GetStringAsync(uri).Result;
            httpClient.Dispose();
            return result;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            var id = (((grid.Children.ToList()[0] as StackPanel).Children.ToList()[0] as StackPanel).Children.ToList()[2] as TextBlock).Text;
            id = id.Replace("No.", "?id=");
            timeLinePage = 1;
            Frame.Navigate(typeof(Thread), id);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            postContents = null;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = GetPostContent(uri);
            postContents = new ObservableCollection<PostContent>(JsonConvert.DeserializeObject<List<PostContent>>(result));
            contentListView.ItemsSource = postContents;
        }
        private void rootScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if ((sender as ScrollViewer).VerticalOffset == (sender as ScrollViewer).ScrollableHeight)
            {
                string newUri = string.Format("{0}?page={1}", uri, ++timeLinePage);
                var result = GetPostContent(newUri);
                foreach (var thread in new ObservableCollection<PostContent>(JsonConvert.DeserializeObject<List<PostContent>>(result)))
                {
                    postContents.Add(thread);
                }
            }
        }
        private void showMenu(bool isTransient, object sender)
        {
            FlyoutShowOptions myOption = new FlyoutShowOptions();
            myOption.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
            if (sender != null) CommandBarFlyout1.ShowAt(sender as Image, myOption);
        }
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            var test = ((CommandBarFlyout1.Target as Image).GetValue(Image.SourceProperty) as BitmapImage).UriSource;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(ImageDetail), test);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            imageSave imageSave = new imageSave();
            imageSave.SaveImage(((CommandBarFlyout1.Target as Image).GetValue(Image.SourceProperty) as BitmapImage).UriSource);
            
        }
        private void image_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            showMenu(false, null);

        }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            showMenu(true, (sender as Image));
            e.Handled = true;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            contentListView.Width = (sender as TimeLine).ActualWidth;
        }

        private async void  ThreadWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
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
