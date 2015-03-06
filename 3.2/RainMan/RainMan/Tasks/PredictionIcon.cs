using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RainMan.Tasks
{

    //Data-model for a single prediction icon
    public class PredictionIcon
    {

        public String Title { get; set; }
        public String Info { get; set; }
        public String ImagePath { get; set; }
        public String AvgInfo { get; set; }
        public Double Avg { set; get; }
        public Double ItemWidth { get; set; }
        public Double ItemHeight { get; set; }


        public PredictionIcon(double averageRain, int timeIndex, DateTime time)
        {

            Title = String.Format("{0}:{1}", time.Hour, time.Minute == 0 ? "00" : time.Minute.ToString());
            AvgInfo = String.Format(" MM/HOUR: {0:0.00}", averageRain);
            InitImagePath(averageRain);
            Avg = averageRain;
            ItemWidth = 170;
            ItemHeight = 130;
        }



        private void InitImagePath(double averageRain)
        {

            if (averageRain < 0.1)
            {
                ImagePath = "Assets/rain/sun.png";
                Info = "CLEAR";
                // cloudy
                return;
            }
            if (averageRain <= 0.1)
            {
                ImagePath = "Assets/rain/sun_cloud.png";
                Info = "SUNNY/CLOUDY";
                // cloudy
                return;

            }
            else if (averageRain <= 0.2)
            {
                ImagePath = "Assets/rain/sun_cloud.png";
                Info = "CLOUDY";
                // cloudy
                return;

            }

            else if (averageRain <= 0.7)
            {
                // mildy rainy
                ImagePath = "Assets/rain/sun_cloud.png";
                Info = "POSSIBLY RAINY";
                return;
            }

            else if (averageRain <= 2)
            {
                ImagePath = "Assets/rain/rain_weak.png";
                Info = "WEAK RAIN";
                // rainy
                return;
            }

            else if (averageRain <= 13)
            {
                // very rainy
                ImagePath = "Assets/rain/rain_strong.png";
                Info = "RAIN";
                return;
            }
            else
            {
                ImagePath = "Assets/rain/rain_nax.png";
                // yellow and above
                // super rainy
                Info = "STRONG RAIN";

            }


        }


    }

    // class represents a collection of prediction icons

    public class PredictionCollection
    {
        public ObservableCollection<PredictionIcon> PredictionIcons { get; set; }

        // for debugging purposes (?)
        public static int _X { get; set; }
        public static int _Y { get; set; }

        //public static double[] colors = new double[4];
        public PredictionCollection(RadarMapManager mapManager, Geopoint userLocation)
        {

            // get predictions from each map...
            PredictionIcons = new ObservableCollection<PredictionIcon>();
            double rainAvg;
            for (int i = RadarMapManager.totalOldMaps; i < RadarMapManager.totalNumMaps; ++i)
            {
                rainAvg = mapManager.Maps[i].getAverageRain(userLocation, 2);
                //colors[i] = rainAvg;
                PredictionIcons.Add(new PredictionIcon(rainAvg, i, mapManager.Maps[i].Time));
            }
         
        }



    }



    // this class represents a data source for prediction icons that appear on the main page
    // CurrentLocation represents the location for which we calculate the prediction at
    
    public sealed class PredictionIconDataSource
    {


        private static PredictionIconDataSource __dataSource = new PredictionIconDataSource();
        private static Geopoint currentLocation = null;
        public static Geopoint CurrentLocation
        {
            get
            {
                return currentLocation;
            }
            set
            {
                NeedToUpdate = true;
                currentLocation = value;
            }

        }
        public static Boolean NeedToUpdate { get; set; }
        public static async Task<PredictionCollection> getData(RadarMapManager radarMapManager)
        {

            var res = await __dataSource.getPredictionCollection(radarMapManager);
            if (res == null) return null;
            NeedToUpdate = false;
            return res;
        }


        private async Task<PredictionCollection> getPredictionCollection(RadarMapManager radarMapManager)
        {




            Geopoint geopoint;
            if (currentLocation == null)
            {


                try
                {

                    // get my current location
                    // locate current location
                    var locator = new Geolocator();
                    locator.DesiredAccuracyInMeters = 50;

                    var myPosition = await locator.GetGeopositionAsync(
                          maximumAge: TimeSpan.FromMinutes(10),
                         timeout: TimeSpan.FromSeconds(10)
                    );

                    geopoint = new Geopoint(new BasicGeoposition
                    {
                        Latitude = myPosition.Coordinate.Latitude,
                        Longitude = myPosition.Coordinate.Longitude
                    });
                }

                catch
                {
                    return null;
                }

            }
            else
            {
                geopoint = currentLocation;
            }

            PredictionCollection collection = new PredictionCollection(radarMapManager, geopoint);
            return collection;


        }

    }
}
