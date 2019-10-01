﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using App4.sources;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Collections.ObjectModel;
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
            var result = GetPostContent(uri);
            postContents = new ObservableCollection<PostContent>(JsonConvert.DeserializeObject<List<PostContent>>(result));
            contentListView.ItemsSource = postContents;
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

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            var id = (((grid.Children.ToList()[0] as StackPanel).Children.ToList()[0] as StackPanel).Children.ToList()[2] as TextBlock).Text;
            id = id.Replace("No.", "?id=");
            timeLinePage = 1;
            postContents = null;
            Frame.Navigate(typeof(Thread), id);
        }

        private void rootScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if((sender as ScrollViewer).VerticalOffset == (sender as ScrollViewer).ScrollableHeight)
            {
                string newUri = string.Format("{0}?page={1}", uri, ++timeLinePage);
                var result = GetPostContent(newUri);
                foreach(var thread in new ObservableCollection<PostContent>(JsonConvert.DeserializeObject<List<PostContent>>(result)))
                {
                    postContents.Add(thread);
                }
            }
        }
    }
}
