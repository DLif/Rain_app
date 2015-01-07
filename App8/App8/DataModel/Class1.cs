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

namespace App8.DataModel
{
    public class RadarMap
    {

        public WriteableBitmap ImageSrc { get; set; }
         
        // 0, 1, 2 or 3 
        public int ImageIndex { get; set; }



        public RadarMap(int index, WriteableBitmap imgSrc)
        {
            this.ImageIndex = index;
            this.ImageSrc = imgSrc;


        }

        public WriteableBitmap cropAndScale(GeoboundingBox bounds, int mapControlWidth, int mapControlHeight )
        {

            

            /* get the corners */

            BasicGeoposition northwest = bounds.NorthwestCorner;
            BasicGeoposition southeast = bounds.SoutheastCorner;


            /* TO DO */
            
            // transfrom the corners to pixels

            double temp = this.distance(northwest.Latitude, northwest.Longitude, southeast.Latitude, southeast.Longitude, 'K');

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

            WriteableBitmap cropped = ImageSrc.Crop(northwestPixel.X, northwestPixel.Y, southeastPixel.X - northwestPixel.X, southeastPixel.X - northwestPixel.Y);

            // resize to fit the given dimensions

        //  var resized = cropped.Resize(mapControlWidth, mapControlHeight, WriteableBitmapExtensions.Interpolation.Bilinear);

            return cropped;



        }

        // need to implement
        public Color getColorAtPixel(Pixel pixel)
        {

            return new Color();
        }


        public Pixel transformLocationToPixel(double longtitue, double latitute)
        {
            return new Pixel(0, 0);
        }

 

 

private double distance(double lat1, double lon1, double lat2, double lon2, char unit) {

  double theta = lon1 - lon2;

  double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));

  dist = Math.Acos(dist);

  dist = rad2deg(dist);

  dist = dist * 60 * 1.1515;

  if (unit == 'K') {

    dist = dist * 1.609344;

  } else if (unit == 'N') {

    dist = dist * 0.8684;

    }

  return (dist);

}

 

//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//::  This function converts decimal degrees to radians             :::
//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

private double deg2rad(double deg) {

  return (deg * Math.PI / 180.0);

}

 

//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

//::  This function converts radians to decimal degrees             :::

//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

private double rad2deg(double rad) {

  return (rad / Math.PI * 180.0);

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

        public RadarMap[] Maps { get; set; }

        public RadarMapManager()
        {
            Maps = new RadarMap[4];
        }

        public async void updateMaps()
        {

            // update logic here
            var tmp = BitmapFactory.New(1, 1);
            String imgSrc = "radar/baseImage.jpg";
            // currently
            for(int i = 0; i < 4; ++i)
            {
                WriteableBitmap current =  await tmp.FromContent(new Uri(String.Format("ms-appx:///Assets/{0}", imgSrc)));
             //   WriteableBitmap current = await tmp.FromContent(new Uri(String.Format("ms-appx:///Assets/rain/rain_strong.png", imgSrc)));
                Maps[i] = new RadarMap(i, current);
            }


        }
    }
}
