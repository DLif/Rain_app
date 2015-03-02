using System;
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


namespace RainMan.Tasks
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
            this.Visibile = Visibility.Collapsed;
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

            if (prev != this.Visibile)
            {
                this.NotifyPropertyChanged("Visibile");
            }

        }

        public WriteableBitmap cropAndScale(GeoboundingBox bounds, int mapControlWidth, int mapControlHeight)
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

            int offset = (int)Math.Floor(((510.0 / 560.0) * line) / 2.0);

            Pixel northwestPixel = new Pixel(255 - offset, 255 - offset);
            Pixel southeastPixel = new Pixel(255 + offset, 255 + offset);


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
        public Color getColorAtPixel(int x, int y)
        {

            return Colors.Red;
        }

        public double getAverageRain(Geopoint location, int pixelRadius)
        {
           int image_size_x = 512;
           int image_size_y = 512;
           double power = 0;
           int locationPixel = PointTranslation.locationToPixel(location.Position.Latitude, location.Position.Longitude);
           int x_pixel = locationPixel % image_size_x;
           int y_pixel = (locationPixel - x_pixel) / image_size_x;
           using (var buffer = this.ReadableImage.PixelBuffer.AsStream())
           {
               Byte[] pixels = new Byte[4 * image_size_x * image_size_y];
                buffer.Read(pixels, 0, pixels.Length);
                power = ColorTranslator.power_to_radius(pixels,x_pixel,y_pixel,pixelRadius,this.ReadableImage.PixelWidth);
           }
           return power;

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

        // holds the index of the current radar map (current time map)
        private static int currentRadarMapIndex = 4;

        // how many MAPS (old+current+new) we provide
        public static int totalNumMaps = 8;

        // how many OLD images we provide
        public static int totalOldMaps = 3;

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
            if (__instance == null)
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
                if (resetSizes)
                {
                    foreach (RadarMap map in this.Maps)
                    {
                        map.Width = 512;
                        map.Height = 512;
                        map.setVisible(false);
                    }
                    Maps.ElementAt(totalOldMaps).setVisible(true);

                }
                return;
            }

            // may need to update the maps every now and  then
            RadarMap[] files = await fetchMaps();
            // the order of the images is such that  the oldest one if first, newest one is last
            for (int i = 0; i < totalNumMaps; ++i)
            {
                Maps.Add(files[i]);

            }

            // current map is set to visible
            Maps.ElementAt(totalOldMaps).setVisible(true);
            this.isSet = true;

            // notify icon source that it needs to update
            PredictionIconDataSource.NeedToUpdate = true;
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

            RadarMap[] files = new RadarMap[totalNumMaps];
   

            var seq = Enumerable.Range(0, totalNumMaps);
            var tasks = seq.Select(async i =>
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
                    catch (Exception e)
                    {
                        String msg = e.Message;
                    }
                    IRandomAccessStream accessStream = ms.AsRandomAccessStream();
                    accessStream.Seek(0);
                    BitmapImage imageSource = new BitmapImage();
                    await imageSource.SetSourceAsync(accessStream);

                    // download jpg image (only for maps we going to predict on)
                    WriteableBitmap writeableImage = null;
                    if (i <= currentRadarMapIndex)
                    {

                        blob = predictionsContainer.GetBlockBlobReference(jpgFormatImage);
                        await blob.DownloadToStreamAsync(ms.AsOutputStream());
                        accessStream = ms.AsRandomAccessStream();
                        accessStream.Seek(0);
                        writeableImage = new WriteableBitmap(512, 512);
                        await writeableImage.SetSourceAsync(accessStream);
                    }



                    DateTime time = DateTime.Now;
                    time = time.AddMinutes((-1) * (time.Minute % 10));

                    if (i > currentRadarMapIndex)
                    {
                        time = time.AddMinutes((-1) * 10 * (i - currentRadarMapIndex));
                    }
                    else if (i < currentRadarMapIndex)
                    {
                        time = time.AddMinutes(10 * (currentRadarMapIndex - i));
                    }

                    // flip the order
                    files[totalNumMaps - 1 - i] = new RadarMap(time, imageSource, RadarMapManager.center, writeableImage);

            });
            await Task.WhenAll(tasks);
            
            
            return files;

        }


      


    }
}
