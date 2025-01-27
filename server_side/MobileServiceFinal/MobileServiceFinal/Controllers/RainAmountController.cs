﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing;
using Microsoft.WindowsAzure;
using MobileServiceFinal.Models;
//using MobileServiceFinal.Models.PixelsSeriallizer;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileServiceFinal.Controllers
{
    public class RainAmountController : ApiController
    {
        public ApiServices Services { get; set; }
        string accountName = "portalvhdszwvb89wr0jbcc";
        string accountKey = "zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==";
        String storageName = "noclouds";
        String maxPixTextFileName = "Maxpic.txt";
        CloudStorageAccount account;
        CloudBlobClient client;
        int image_size_x = 512;
        int image_size_y = 512;

        private void initializeBlobClient()
        {
            try
            {
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                account = new CloudStorageAccount(creds, useHttps: true);
                client = account.CreateCloudBlobClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        // GET api/Default
        public string GetRainAmount(String places , String picturesNum)
        {
            double sum = 0;
            initializeBlobClient();
            Services.Log.Info("Trying to get the amount of rain");
            int max = getMaxIndex();
            int num = int.Parse(picturesNum);
            String currentName;
        //    String places = RainApiSerializer.demorun();
            List<PixelRep> placesList = RainApiSerializer.DeserializeRequest(places).Pixels;
            int[] RBGArray;
            for (int i = 0; i < 4 /*FIx me after david inserts new pic     num */ ; i++)
            {
                currentName = String.Format("{0}.jpg", (max - i));
                //byte[] file = GetByteImage(currentName);
                Bitmap file = new Bitmap(GetStreamImage(currentName));
                foreach (PixelRep pixel in placesList)
                {
                    RBGArray = RGBFromImageBitmap(file, pixel);
                    sum += ColorTranslatorModule.RBG_to_power(RBGArray[0], RBGArray[1], RBGArray[2]);
                }

            }
            return sum.ToString();

        }



        private int[] RGBFromImageBitmap(Bitmap file, PixelRep pixel)
        {
            int[] result = new int[3];
            Color RGB = file.GetPixel(pixel.X, pixel.Y);
            result[0] = RGB.R;
            result[1] = RGB.G;
            result[2] = RGB.B;
            return result;
        }
        /*
        private int[] RGBFromImageByteArray(byte[] buffer,PixelRep pixel)
        {
            int initial_offset = 0;
            int[] result = new int[3];
            initial_offset += pixel.X * image_size_x * 4;
            int y = pixel.Y * 4;
            initial_offset = x+y;
            result[0] = buffer[initial_offset];
            result[1] = buffer[initial_offset+1];
            result[2] = buffer[initial_offset+2];
            return result;
        }
        */


        private int getMaxIndex()
        {
            try
            {
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageName);
                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(maxPixTextFileName);
                Stream fileStream = new MemoryStream();
                blob.FetchAttributes();
                int fileByteLength = (int)blob.Properties.Length;
                Byte[] intByte = new Byte[fileByteLength];
                Byte[] intReverse = new Byte[fileByteLength];
                blob.DownloadToStream(fileStream);
                fileStream.Position = 0;
                fileStream.Read(intByte, 0, fileByteLength);

                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array :((((((((((((. 
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(intByte);
                Array.Copy(intByte, intReverse, fileByteLength);

                int number = (int)ConvertLittleEndian(intReverse);
                return number;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }

        }

        int ConvertLittleEndian(byte[] array)
        {
            int pos = 0;
            int result = 0;
            foreach (byte by in array)
            {
                result += (by - 48) * (int)Math.Pow(10, pos);
                pos += 1;
            }
            return result;
        }
        /*
        // method connects to the blob and downloads the radar image streams
        private Bitmap GetImageStreamImage(String fileName)
        {

            try
            {
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageName);

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(fileName);

                blob.FetchAttributes();
                long fileByteLength = blob.Properties.Length;
                Byte[] myByteArray = new Byte[fileByteLength];
                blob.DownloadToByteArray(myByteArray, 0);
                return new Bitmap(new MemoryStream(myByteArray));


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
         */

        // method connects to the blob and downloads the radar image 
        private Stream GetStreamImage(String fileName)
        {

            try
            {
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageName);

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(fileName);

                blob.FetchAttributes();
                int fileByteLength = (int)blob.Properties.Length;
                Byte[] myByteArray = new Byte[fileByteLength];
                Stream fileStream = new MemoryStream();
                blob.DownloadToStream(fileStream);
                return fileStream;
                //  fileStream.Position = 0;
                //  fileStream.Read(myByteArray, 0, fileByteLength);
                //   return myByteArray;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
