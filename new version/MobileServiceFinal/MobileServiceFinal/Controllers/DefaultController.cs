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

namespace MobileServiceFinal.Controllers
{
    public class DefaultController : ApiController
    {
        public ApiServices Services { get; set; }
        string accountName = "portalvhdszwvb89wr0jbcc";
        string accountKey = "zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==";
        String storageName = "noclouds";
        CloudStorageAccount account;
        CloudBlobClient client;


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
        public string GetRainAmount(String places, String picturesNum)
        {
            double sum = 0;
            initializeBlobClient();
            Services.Log.Info("Trying to get the amount of rain");
            int max = getMaxIndex();
            int num = int.Parse(picturesNum);
            String currentName;
            for ( int i = 0; i<num ; i++)
            {
                currentName =  String.Format("{0}.png", (max - i));
                Task<Bitmap> file =  GetImageStreamImages(currentName);
                /* until david finishes
                RBG_to_power power = new RGB_to_power();
                for ( elemment in places)
                {
                    sum + = power.somename(element,file);
                }
                 */
            }

            return sum.ToString();
        }

        private int getMaxIndex()
        {
            byte[] int_byte = new byte[4];
            try
            {
                CloudBlobContainer sampleContainer = client.GetContainerReference(storageName);
                CloudBlockBlob blob = null;//sampleContainer.GetBlockBlobReference(fileName);
                Stream fileStream = new MemoryStream();
                blob.FetchAttributes();
                long fileByteLength = blob.Properties.Length;
                Byte[] myByteArray = new Byte[fileByteLength];
                blob.DownloadToStream(fileStream);
                fileStream.Read(int_byte, 0, 4);

                // If the system architecture is little-endian (that is, little end first), 
                // reverse the byte array. 
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(int_byte);

                return BitConverter.ToInt32(int_byte, 0); 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return 0;

        }
        // method connects to the blob and downloads the radar image streams
        private async Task<Bitmap> GetImageStreamImages(String fileName)
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
    }
}
