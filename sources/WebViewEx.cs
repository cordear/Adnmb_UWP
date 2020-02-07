using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App4.sources
{
    public class WebViewEx
    {
        // Using a DependencyProperty as the backing store for WebViewUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.RegisterAttached("Uri", typeof(string), typeof(WebViewEx),
                new PropertyMetadata(null, PropertyChangedCallback));

        public static string GetUri(DependencyObject obj)
        {
            return (string) obj.GetValue(UriProperty);
        }

        public static void SetUri(DependencyObject obj, string value)
        {
            obj.SetValue(UriProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = d as WebView;
            webView.NavigateToString(e.NewValue.ToString());
        }
    }
}