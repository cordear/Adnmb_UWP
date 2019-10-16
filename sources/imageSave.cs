using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace App4.sources
{
    class imageSave
    {
        private static string Md5(string str)
        {
            HashAlgorithmProvider hashAlgorithm =
            HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash cryptographic = hashAlgorithm.CreateHash();
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            cryptographic.Append(buffer);
            return CryptographicBuffer.EncodeToHexString(cryptographic.GetValueAndReset());
        }
        private static async Task<byte[]> ConvertIRandomAccessStreamByte(IRandomAccessStream stream)
        {
            DataReader read = new DataReader(stream.GetInputStreamAt(0));
            await read.LoadAsync((uint)stream.Size);
            byte[] temp = new byte[stream.Size];
            read.ReadBytes(temp);
            return temp;
        }
        public async void downLoadImage(Uri uri)
        {
            Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
            IBuffer buffer = await http.GetBufferAsync(uri);
            BitmapImage img = new BitmapImage();
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(buffer);
                stream.Seek(0);
                await img.SetSourceAsync(stream);
                await StorageImageFolder(stream, uri);
            }
        }
        private static async Task StorageImageFolder(IRandomAccessStream stream, Uri uri)

        {
            StorageFolder folder = await GetImageFolder();
            string image = Md5(uri.AbsolutePath) + ".png";
            try
            {
                StorageFile file = await folder.CreateFileAsync(image);
                await FileIO.WriteBytesAsync(file, await ConvertIRandomAccessStreamByte(stream));
            }
            catch (Exception)
            {
            }
        }
        private static async Task<StorageFolder> GetImageFolder()
        {
            //文件夹
            string name = "image";
            StorageFolder folder = null;
            //从本地获取文件夹
            try
            {
                folder = await ApplicationData.Current.LocalCacheFolder.GetFolderAsync(name);
            }
            catch (FileNotFoundException)
            {
                //没找到
                folder = await ApplicationData.Current.LocalCacheFolder.
                    CreateFolderAsync(name);
            }
            return folder;
        }
    }
}
