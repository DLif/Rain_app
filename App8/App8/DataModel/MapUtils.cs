using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
