using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using App4.sources;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImageDetail : Page
    {
        public ImageDetail()
        {
            this.InitializeComponent();
            ApplicationView applicationView = ApplicationView.GetForCurrentView();
            applicationView.Title = "查看图片";
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            imageDetail.Source = new BitmapImage(e.Parameter as Uri);
        }

        private void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ImageSave.SaveImage((imageDetail.GetValue(Image.SourceProperty) as BitmapImage).UriSource);
        }

    }
}
