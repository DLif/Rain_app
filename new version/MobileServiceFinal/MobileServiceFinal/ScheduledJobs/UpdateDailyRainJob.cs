using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using RainMan.DataModels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MobileServiceFinal.ScheduledJobs
{
    // A simple scheduled job which can be invoked manually by submitting an HTTP
    // POST request to the path "/jobs/sample".

    public class UpdateDailyRainJob : ScheduledJob
    {

        string accountName = "portalvhdszwvb89wr0jbcc";
        string accountKey = "zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==";
        String storageNameMinutes = "noclouds";
        String storageNameDaily = "daysums";
        String maxPixTextFileName = "Maxpic.txt";
        CloudStorageAccount account;
        CloudBlobClient client;
        int image_size_x = 512;
        int image_size_y = 512;
        int period = 30;


        /* needs to be run exactly at 23:55 */
        public override Task ExecuteAsync()
        {
            double[,] sums = new double[image_size_x,image_size_y]; /*represents the sum over all pixels*/
            double sum = 0;
            int x,y =0;
            string name;
         //   double[,] previousum = new double[image_size_x, image_size_y]; /*represents the sum over all pixels*/
            /*initialize array 
            for (x = 0; x < image_size_x; x++)
            {
                for (y = 0; y < image_size_y; y++)
                {
                    previousum[x, y] = 0;
                }

            }
             * */


            initializeBlobClient();
      //      for (int i = 0; i < 30; i++)
       //     {
            //    name = String.Format("{0}.jpg", i);
             //   previousum = newArray( 5656 - (24 * 6 * (i )), 5656 - (24 * 6 * (i + 1)), name, previousum);
           // }

     //       return Task.FromResult(true); 
            
            int max = getMaxIndex(maxPixTextFileName, storageNameMinutes);
         //   int num = 6 * 24; /* full day.. */
            int num = 5; //fixed me
            String currentName;

            /*initialize array */
            for (x = 0; x < image_size_x; x++)
            {
                for (y = 0; y < image_size_y; y++)
                {
                    sums[x, y] = 0;
                }

            }


            //threads....
          //  Parallel.For(0, num, i =>
               for (int i = 0 /*fix me - start with 0 */; i <num ; i++)
            {
                currentName = String.Format("{0}.jpg",(max-i) );

                Bitmap file = new Bitmap(GetStreamImage(currentName));

                /* bug in the picture */
                if (file.Height == 1)
                {
                       continue;
               //     return;

                }
                for (x = 0; x < image_size_x; x++)
                {
                    for (y = 0; y < image_size_y; y++)
                    {
                        try
                        {
                            Color RGB = file.GetPixel(x, y);
                            sums[x,y] += Models.ColorTranslator.RBG_to_power(RGB.R, RGB.G, RGB.B);
                            //  RBGArray = RGBFromImageBitmap(file, pixel);
                            //  sum += Models.ColorTranslator.RBG_to_power(RBGArray[0], RBGArray[1], RBGArray[2]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            continue;
                          //  return; /* Fix me before we handle the project - continue.. */
                        }
                    }
                }

                    }
          //  }); // Parallel.For

           
       //     exportArrayToDB( sums, "0.jpg");
            updateMonthBack(sums);




            return Task.FromResult(true);
        }


        double[,] newArray(int first,int last, string name,double[,] previoussum)
        {


            double[,] sums = new double[image_size_x, image_size_y]; /*represents the sum over all pixels*/
            double sum = 0;
            int x, y = 0;

            // int num = 6 * 24; /* full day.. */
            int num = first-last;
            String currentName;

            /*initialize array */
            for (x = 0; x < image_size_x; x++)
            {
                for (y = 0; y < image_size_y; y++)
                {
                    sums[x, y] = previoussum[x,y];
                }

            }


            //threads....
            //  Parallel.For(0, num, i =>
            for (int i = 0 /*fix me - start with 0 */; i < num; i++)
            {
                currentName = String.Format("{0}.jpg", (first - i));
                Bitmap file = new Bitmap(GetStreamImage(currentName));

                /* bug in the picture */
                if (file.Height == 1)
                {
                    continue;
                    //     return;

                }
                for (x = 0; x < image_size_x; x++)
                {
                    for (y = 0; y < image_size_y; y++)
                    {
                        try
                        {
                            Color RGB = file.GetPixel(x, y);
                            sums[x, y] += Models.ColorTranslator.RBG_to_power(RGB.R, RGB.G, RGB.B);
                            //  RBGArray = RGBFromImageBitmap(file, pixel);
                            //  sum += Models.ColorTranslator.RBG_to_power(RBGArray[0], RBGArray[1], RBGArray[2]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            continue;
                            //  return; /* Fix me before we handle the project - continue.. */
                        }
                    }
                }

            }



        exportArrayToDB( sums,name);
        return sums;

        }

        int updateMonthBack(double[,] sums)
        {
            int i = 0;
            int x, y;
            double[,] temp;
            string fileNameAbove;
            string fileName;
            for (i =(period-1);i> 0; i--)
            {
                fileNameAbove =  String.Format("{0}.jpg", i-1);
                fileName = String.Format("{0}.jpg", i );
                temp =  GetDoubleArray(fileNameAbove);
                
                for (x = 0; x < image_size_x; x++)
                {
                    for (y = 0; y < image_size_y; y++)
                    {
                        temp[x,y]+=sums[x,y];
                    }
                }
            //    exportArrayToDB(temp,fileName); fix me

            }
            /* i ==0 */
            fileName = String.Format("{0}.jpg", 0);
       //     exportArrayToDB(sums, fileName);  fix me


            return 1;

        }

        private int getMaxIndex(string maxPixTextFileName,string storageName)
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

        private int increaseMaxIndex(string maxPixTextFileName, string storageName)
        {
            try
            {


                int currentIndex = getMaxIndex(maxPixTextFileName, storageName);
                UnicodeEncoding uniEncoding = new UnicodeEncoding();
                MemoryStream ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                sw.Write(currentIndex + 1);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                CloudBlobContainer sampleContainer = client.GetContainerReference(storageName);
                sampleContainer.CreateIfNotExists();
                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(maxPixTextFileName);
               

                blob.UploadFromStream(ms);
                return 0;
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

        public async void exportArrayToDB(double[,] sums,string fileName)
        {

            try
            {

           //     increaseMaxIndex(maxPixTextFileName, storageNameDaily);
          //      int max = getMaxIndex(maxPixTextFileName, storageNameDaily);

              //  String fileName = String.Format("{0}.txt", max);


                IFormatter formatter = new BinaryFormatter();
             //   Stream stream = new FileStream(fileName,
                                    //     FileMode.Create,
                                      //   FileAccess.Write, FileShare.None);
                Stream stream = new MemoryStream();
                formatter.Serialize(stream, sums);
                stream.Flush();
                stream.Position = 0;
                
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageNameDaily);
                sampleContainer.CreateIfNotExists();

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(fileName);

                blob.UploadFromStream(stream);

                stream.Close();
                //  }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //  Console.WriteLine("Done... press a key to end.");
            //  Console.ReadKey();

        }

        public static Stream GetStreamImageFromUrl(string url)
        {
            {
                WebClient client = new WebClient();
                return client.OpenRead(url);
            }
        }
        /*  
          public static Stream GetStreamImageFromUrl(string url)
              {
                  HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

                      using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
                      {
                          using (Stream stream = httpWebReponse.GetResponseStream())
                          {
                              return stream;
                          }
                      }
              }
          }
         
         */
        
        // method connects to the blob and downloads the radar image 
        private Stream GetStreamImage(String fileName)
        {

            try
            {
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageNameMinutes);

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



      public double[,] GetDoubleArray(String fileName)
      {

            try
            {
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageNameDaily);

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(fileName);

                blob.FetchAttributes();
                int fileByteLength = (int)blob.Properties.Length;
                Byte[] myByteArray = new Byte[fileByteLength];
                Stream fileStream = new MemoryStream();
                blob.DownloadToStream(fileStream);
                fileStream.Position = 0;

                BinaryFormatter formatter = new BinaryFormatter();
                double[,] myArray = (double[,])formatter.Deserialize(fileStream);
                return myArray;
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
