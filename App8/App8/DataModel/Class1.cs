﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

using System.IO;
using Windows.Foundation;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace App8.DataModel
{
    public class RadarMap : INotifyPropertyChanged
    {

        // image source objet for drawing onto map (.png) format
        public BitmapImage ImageSrc { get; set; }

        // image source for pixel anaylsis
        public WriteableBitmap ReadableImage { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        // date and time of radar map
        public DateTime Time { get; set; }

        // visibillty of radar map on map control
        public Visibility Visibile { get; set; }

        // base point to draw the map
        public Geopoint Point { get; set; }

        public Point AnchorPoint { get; set; }

        private double width;
        private double height;

        // height and weight of the image
        public double Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                this.NotifyPropertyChanged("Width");
            }

        }

        
        public double Height 
        { 
            get 
            {
                return this.height;
            }
            set
            {
                this.height = value;
                this.NotifyPropertyChanged("Height");
            } 
        }


        public RadarMap(DateTime time, BitmapImage imgSrc, Geopoint center, WriteableBitmap writeableBitmap)
        {
            this.Time = time;

            this.ImageSrc = imgSrc;
            this.ReadableImage = writeableBitmap;
            this.Visibile = Visibility.Collapsed ;
            this.Point = center;
            // default anchor point
            this.AnchorPoint = new Point(0.5, 0.5);
            
            // default values
            this.Width = 512;
            this.Height = 512;

        }

        public void setVisible(Boolean value)
        {

            Visibility prev = this.Visibile;

            if (value)
                this.Visibile = Visibility.Visible;
            else
                this.Visibile = Visibility.Collapsed;

            if(prev != this.Visibile)
            {
                this.NotifyPropertyChanged("Visibile");
            }
            
        }

        public WriteableBitmap cropAndScale(GeoboundingBox bounds, int mapControlWidth, int mapControlHeight )
        {


            /* get the corners */

            BasicGeoposition northwest = bounds.NorthwestCorner;
            BasicGeoposition southeast = bounds.SoutheastCorner;


            /* TO DO */
            
            // transfrom the corners to pixels

            double temp = MapUtils.distance(northwest.Latitude, northwest.Longitude, southeast.Latitude, southeast.Longitude, 'K');

            if (temp > 791)
                temp = 791; // alahson

            double line = temp / Math.Sqrt(2);

            int offset =  (int)Math.Floor(((510.0 / 560.0) * line) / 2.0);

            Pixel northwestPixel = new Pixel(255-offset, 255-offset);
            Pixel southeastPixel = new Pixel(255+offset, 255+offset);


            // 510 pixels
           // int width, height = 


          //  Pixel northwestPixel = this.transformLocationToPixel(northwest.Longitude, northwest.Latitude);
           // Pixel southeastPixel = this.transformLocationToPixel(southeast.Longitude, southeast.Latitude);

            // crop the square from the map

           // WriteableBitmap cropped = ImageSrc.Crop(northwestPixel.X, northwestPixel.Y, southeastPixel.X - northwestPixel.X, southeastPixel.X - northwestPixel.Y);

            // resize to fit the given dimensions

        //  var resized = cropped.Resize(mapControlWidth, mapControlHeight, WriteableBitmapExtensions.Interpolation.Bilinear);

           // return cropped;

            return null;


        }

        // need to implement
        public Color getColorAtPixel(int x , int y)
        {

            return Colors.Red;
        }

        public double getAverageRain(Geopoint location, int pixelRadius)
        {

            int locationPixel = PointTranslation.locationToPixel(location.Position.Latitude, location.Position.Longitude);

            int width = this.ReadableImage.PixelWidth;
            int height = this.ReadableImage.PixelHeight;

            using (var buffer = ReadableImage.PixelBuffer.AsStream())
            {
                Byte[] pixels = new Byte[4 * width * height];
                buffer.Read(pixels, 0, pixels.Length);

                for (int x = 0; x < width; x++)
                {

                    for (int y = 0; y < height; y++)
                    {
                        int index = ((y * width) + x) * 4;




                        Byte b = pixels[index + 0];
                        Byte g = pixels[index + 1];
                        Byte r = pixels[index + 2];
                        Byte a = pixels[index + 3];

                        Color pixelColor = Color.FromArgb(a, r, g, b);


                    }
                }

                //buffer.Position = 0;
                //buffer.Write(pixels, 0, pixels.Length);

            }
            



            return 0;

        }

        


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

 


        
    }

    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Pixel(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

    }



    public class RadarMapManager
    {

        // singleton instance used througout the app
        private static RadarMapManager __instance;

        // are maps instances already created 
        private bool isSet = false;

        public static Geopoint center = new Geopoint(new BasicGeoposition() { Latitude = 32.006340, Longitude = 34.814471 });

        public ObservableCollection<RadarMap> Maps { get; set; }

        private RadarMapManager()
        {

            // by default, create the collections of maps we want to predict, four maps
            Maps = new ObservableCollection<RadarMap>();


        
       }

        public Boolean NeedToUpdate()
        {

            if (this.isSet)
                return false;
            return true;

        }

        public static RadarMapManager getRadarMapManager()
        {
            if(__instance == null)
            {
                __instance = new RadarMapManager();
            }
            return __instance;

        }

        public async Task updateRadarMaps(bool resetSizes)
        {

            if (this.isSet)
            {

                // no need to create the instances
                if(resetSizes)
                {
                    foreach(RadarMap map in this.Maps)
                    {
                        map.Width = 512;
                        map.Height = 512;
                        map.setVisible(false);
                    }
                    Maps.ElementAt(0).setVisible(true);

                }
                return;
            }

            // may need to update the maps every now and  then
                 
            RadarMap[] files = await fetchMaps();
            for (int i = 0; i < 4; ++i )
            {
                Maps.Add(files[i]);

            }
            Maps.ElementAt(0).setVisible(true);
            this.isSet = true;
        }

        // method connects to the blob and downloads the radar image streams
        private async Task<RadarMap[]> fetchMaps()
        {

            string accountName = "portalvhdszwvb89wr0jbcc";
            string accountKey = "zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==";

            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer predictionsContainer = client.GetContainerReference("predictions");

            RadarMap[] files = new RadarMap[4];

            for(int i = 0; i < 4; ++i )
            {
                String pngFormatImage = String.Format("{0}.png", i);
                CloudBlockBlob blob = predictionsContainer.GetBlockBlobReference(pngFormatImage);
                String jpgFormatImage = String.Format("{0}.jpg", i);

                // download png image
                var ms = new MemoryStream();
                try
                {
                    await blob.DownloadToStreamAsync(ms.AsOutputStream());
                }
                catch(Exception e)
                {
                    String msg = e.Message;
                }
                IRandomAccessStream accessStream = ms.AsRandomAccessStream();
                accessStream.Seek(0);
                BitmapImage imageSource = new BitmapImage();
                await imageSource.SetSourceAsync(accessStream);

                // download jpg image
                blob = predictionsContainer.GetBlockBlobReference(jpgFormatImage);
                await blob.DownloadToStreamAsync(ms.AsOutputStream());
                accessStream = ms.AsRandomAccessStream();
                accessStream.Seek(0);
                WriteableBitmap writeableImage = new WriteableBitmap(512, 512);
                await writeableImage.SetSourceAsync(accessStream);

                DateTime time = DateTime.Now;
               time = time.AddMinutes((-1) * (time.Minute % 10));
                time = time.AddMinutes((-1) * 10 * i);


                files[i] = new RadarMap(time, imageSource, RadarMapManager.center, writeableImage);
            }

            return files;

        }

    
        private static void makeTransparentBackground(WriteableBitmap image)
        {



        }

        
    }
}
