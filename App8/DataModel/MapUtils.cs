using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace App8.DataModel
{
    class MapUtils
    {




        public static double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {

            double theta = lon1 - lon2;

            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));

            dist = Math.Acos(dist);

            dist = rad2deg(dist);

            dist = dist * 60 * 1.1515;

            if (unit == 'K')
            {

                dist = dist * 1.609344;

            }
            else if (unit == 'N')
            {

                dist = dist * 0.8684;

            }

            return (dist);

        }



        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static double deg2rad(double deg)
        {

            return (deg * Math.PI / 180.0);

        }

        // color options: Colors.red, Colors.blue
        public static DependencyObject getMapPin(Geopoint point, Color color)
        {

            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);
            ImageBrush imgBrush = new ImageBrush();
            if (color == Colors.Red)
            {
                imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/mappin.png"));
            }
            else
            {
                imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/waypointpin.png"));
            }

            //Creating a Rectangle
            var myRectangle = new Rectangle { Fill = imgBrush, Height = 35, Width = 20 };
            myRectangle.SetValue(Grid.RowProperty, 0);
            myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            myGrid.Children.Add(myRectangle);

            return myGrid;

        }

        public static void addPinToMap(MapControl map, DependencyObject pin, Geopoint location)
        {
            // add pin to map
            map.Children.Add(pin);
            MapControl.SetLocation(pin, location);
            MapControl.SetNormalizedAnchorPoint(pin, new Point(0.5, 1));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //::  This function converts radians to decimal degrees             :::

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static double rad2deg(double rad)
        {

            return (rad / Math.PI * 180.0);

        }


        [DataContract]
        public class testClass
        {

            [DataMember]
            public string Text { set; get; }
            [DataMember]
            public int Num { set; get; }
            [DataMember]

            public testClass Obj { set; get; }

           // public testClass(string text, int num)
           // {
           //     this.Text = text;
           //     this.Num = num;
           // }

            public testClass()
            {

            }


        }

    }
}
