using System;
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
using RainMan.DataModels;
using MobileServiceFinal.ScheduledJobs;

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
        public string GetRainAmount(String places,String numDaysString)
        {
            double a;
            double sum = 0;
            initializeBlobClient();
            Services.Log.Info("Trying to get the amount of rain");
            int max = getMaxIndex();
            int numMinutes = DateTime.Now.Minute / 10 + DateTime.Now.Hour * 6;
            int numDays = int.Parse(numDaysString);
           // String currentName;
            List<PixelRep> polygonPoints = RainApiSerializer.DeserializeRequest(places).Pixels;
            Polygon polygon_from_user = new Polygon(polygonPoints.Count, polygonPoints);
            List<PixelRep> placesList = PolygonPixels.getAllPointsInsidePolygon(polygon_from_user);
            double[] sum_array = new double[numMinutes];

            /*initialize the array*/
            for (int i = 0; i < numMinutes; i++)
            {
                sum_array[i] = 0.0;
            }


            //threads....
            Parallel.For(0, numMinutes, i =>
            {
               String currentName = String.Format("{0}.jpg", (max - i));
                try
                {
                    Bitmap file = new Bitmap(GetStreamImage(currentName));

                    /* bug in the picture */
                    if (file.Height == 1)
                    {
                        return;

                    }
                    foreach (PixelRep pixel in placesList)
                    {
                        try
                        {
                            Color RGB = file.GetPixel(pixel.X, pixel.Y);
                            sum_array[i] += Models.ColorTranslator.RGB_array_power(RGB.R, RGB.G, RGB.B);
                            /*
                            if((Models.ColorTranslator.RGB_array_power(RGB.R, RGB.G, RGB.B) >16))
                            {
                                a = Models.ColorTranslator.RGB_array_power(RGB.R, RGB.G, RGB.B);
                            }
                             * */
                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            return; 
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }); 

            /*adding all the thread results */
            for (int i = 0; i < numMinutes; i++)
            {
                sum += sum_array[i];
            }
            if (numDays - 1 == -1)
            {
                return sum.ToString();
            }


            /* getting the day result */
           String currentNameDay = String.Format("{0}.jpg", numDays-1);
           UpdateDailyRainJob job = new UpdateDailyRainJob();
           double[,] sumDays = job.GetDoubleArray(currentNameDay);
           foreach (PixelRep pixel in placesList)
           {
              try
              {
                  sum += sumDays[pixel.X, pixel.Y];
              }
              catch (Exception ex)
              {
                  Console.WriteLine(ex);
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


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
