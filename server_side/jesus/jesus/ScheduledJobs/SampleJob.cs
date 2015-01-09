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
using jesus.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace jesus.ScheduledJobs
{
    // A simple scheduled job which can be invoked manually by submitting an HTTP
    // POST request to the path "/jobs/sample".

    public class SampleJob : ScheduledJob
    {

        String RadarUrlPath = "http://www.ims.gov.il/Ims/Pages/RadarImage.aspx?Row=0&TotalImages=1&LangID=1&Location=";

        /*public override Task ExecuteAsync()
        {
            Services.Log.Info("Hello from scheduled job!");
            return Task.FromResult(true);
        }
        */

        public override Task ExecuteAsync()
        {
            Stream imageStram = GetStreamImageFromUrl(RadarUrlPath);
            exportPicToDB(imageStram);

            return Task.FromResult(true);
        }
        public async void exportPicToDB(Stream image)
        {

            string accountName = "portalvhdszwvb89wr0jbcc";
            string accountKey = "zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==";
            try
            {
                DateTime dt = DateTime.Now;
                String fileName = String.Format("{0:d/M/yyyy HH:mm:ss}", dt);

                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

                CloudBlobClient client = account.CreateCloudBlobClient();
                CloudBlobContainer sampleContainer = client.GetContainerReference("radarpics");
                sampleContainer.CreateIfNotExists();

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(fileName);
                //   using (Stream file = System.IO.File.OpenRead("MiniAzure.jpg"))
                // {
                blob.UploadFromStream(image);
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
    }
}
