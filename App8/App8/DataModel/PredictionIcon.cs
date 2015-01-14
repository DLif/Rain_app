using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace App8.DataModel
{
    public class PredictionIcon
    {

        public String Title { get; set; }
        public String Info { get; set;  }
        public String ImagePath { get; set; }
        

        public PredictionIcon(double averageRain, int timeIndex, DateTime time)
        {

            Title = time.ToString();
            InitImagePath(averageRain);

        }

   

        private void InitImagePath(double averageRain)
        {

            if (averageRain <= 0.2)
            {
                ImagePath = "Assets/rain/sun.png";
                Info = "Cloudy";
                // cloudy
                return;
            }

            if (averageRain <= 0.7)
            {
                // mildy rainy
                ImagePath = "Assets/rain/sun_cloud.png";
                Info = "Midly Rainy";
                return;
            }

            if (averageRain <= 2)
            {
                ImagePath ="Assets/rain/rain_weak.png";
                Info = "Weak rain";
                // rainy
                return;
            }

            if (averageRain <= 13)
            {
                // very rainy
                ImagePath = "Assets/rain/rain_strong.png";
                Info = "Rain";
                return;
            }
            else
            {
                ImagePath = "Assets/rain/rain_nax.png";
                // yellow and above
                // super rainy
                Info = "Strong rain";
            }

        }


    }

    public class PredictionCollection
    {
        public ObservableCollection<PredictionIcon> PredictionIcons { get; set; }


        public PredictionCollection(RadarMapManager mapManager, Geopoint userLocation)
        {

            // get predictions from each map...
            // 
            
            PredictionIcons = new ObservableCollection<PredictionIcon>();
            double rainAvg;
            for(int i = 0; i < 4; ++i)
            {
               //rainAvg = mapManager.Maps[i].getAverageRain(userLocation, 2);
                rainAvg = 0.2;
               PredictionIcons.Add(new PredictionIcon(rainAvg, i, mapManager.Maps[i].Time));
            }
             
        }



    }


    public sealed class PredictionIconDataSource
    {
        private static PredictionIconDataSource __dataSource = new PredictionIconDataSource();

        public static async Task<PredictionCollection> getData(RadarMapManager radarMapManager)
        {

            var res = await __dataSource.getPredictionCollection(radarMapManager);
            return res;
        }


        private async Task<PredictionCollection> getPredictionCollection(RadarMapManager radarMapManager)
        {

            // get my current location
            // locate current location
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

             var myPosition = await locator.GetGeopositionAsync(
                   maximumAge: TimeSpan.FromMinutes(10),
                  timeout: TimeSpan.FromSeconds(10)
             );

            var geopoint = new Geopoint(new BasicGeoposition
            {
                    Latitude = myPosition.Coordinate.Latitude,
                    Longitude = myPosition.Coordinate.Longitude
            });



            PredictionCollection collection = new PredictionCollection(radarMapManager, geopoint);
            return collection;


        }

    }
}
