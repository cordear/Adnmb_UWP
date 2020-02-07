﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace App4.sources
{
    internal class ImageSave
    {
        private static string Md5(string str)
        {
            var hashAlgorithm =
                HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var cryptographic = hashAlgorithm.CreateHash();
            var buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            cryptographic.Append(buffer);
            return CryptographicBuffer.EncodeToHexString(cryptographic.GetValueAndReset());
        }

        private static async Task<byte[]> ConvertIRandomAccessStreamByte(IRandomAccessStream stream)
        {
            var read = new DataReader(stream.GetInputStreamAt(0));
            await read.LoadAsync((uint) stream.Size);
            var temp = new byte[stream.Size];
            read.ReadBytes(temp);
            return temp;
        }

        public static async void SaveImage(Uri uri)
        {
            var http = new HttpClient();
            var buffer = await http.GetBufferAsync(uri);
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(buffer);
                stream.Seek(0);
                var fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                fileSavePicker.FileTypeChoices.Add("Image", new List<string> {".jpg", ".jpeg", ".png", ".bmp", ".gif"});
                fileSavePicker.SuggestedFileName = Md5(uri.AbsoluteUri) + uri.AbsoluteUri.Split('.')[1];
                try
                {
                    var file = await fileSavePicker.PickSaveFileAsync();
                    await FileIO.WriteBytesAsync(file, await ConvertIRandomAccessStreamByte(stream));
                }
                catch (Exception)
                {
                }
            }
        }
    }
}