using RainMan.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace RainMan.Tasks
{
    class LocationsToEstimations
    {
        static String baseURLString = "http://dev.virtualearth.net/REST/V1/Routes/";
        static String driving_q = "Driving?output=xml&optmz=time";
        static String walking_q = "Walking?output=xml&optmz=time";
        static String bicycle_roads = "&avoid=highways";
        static String source_point = "&wp.0=";
        static String destination_point = "&wp.1=";
        static String comma = ",";
        static String get_only_summary = "&routeAttributes=routeSummariesOnly";
        static String key = "&key=AmJqOC6z5Nnf_tL1iajMSSLVyMoWRpwIBREiL1LE20_trwH1uFlK6yC5t0FrIqxD";
        static Stopwatch sw = new Stopwatch();

        public static async Task<double[]> getTimeAndDistance(Geopoint source, Geopoint destination, RainMan.Navigation.RouteKind kind)
        {





            sw.Start();
            XmlReader response = await getXmlReader(source, destination, kind);
            sw.Stop();
            TimeSpan time1 = sw.Elapsed;
            sw.Restart();
            double distance = 0.0;
            double time = 0.0;
            double[] pack = new double[2];
            while (response.Read())
            {
                // Only detect elements 
                if (response.IsStartElement())
                {
                    // Get element name and switch on it.
                    switch (response.Name)
                    {
                        case "TravelDistance":
                            //now read the attribute to distance 
                            response.Read();
                            distance = Convert.ToDouble(response.Value.Trim());
                            break;
                        case "TravelDuration":
                            //now read the attribute to distance 
                            response.Read();
                            time = Convert.ToDouble(response.Value.Trim());
                            break;
                        default:
                            break;
                    }
                }
            }

            response.Dispose();
            sw.Stop();
            TimeSpan time2 = sw.Elapsed;
            pack[0] = distance * 1000; // meters
            pack[1] = time / 60;      // minutes
            return pack;
        }

        public static async Task<double[]> getTimeAndDistance2(Geopoint source, Geopoint destination, RainMan.Navigation.RouteKind kind)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //---------------- PART 1 : Use BING API to complete the route from given waypoints -------------- //

            MapRouteFinderResult routeResult = null;
            if (kind == RouteKind.DRIVE)
            {
                // no restirctions
                routeResult =
                    await MapRouteFinder.GetDrivingRouteAsync(source, destination,
                    MapRouteOptimization.Time,
                    MapRouteRestrictions.None);
            }

            else if (kind == RouteKind.BIKE)
            {

                // avoid highways
                routeResult =
                    await MapRouteFinder.GetDrivingRouteAsync(source, destination,
                    MapRouteOptimization.Time,
                    MapRouteRestrictions.Highways);

            }
            else if (kind == RouteKind.WALK)
            {
                // walking 
                routeResult = await MapRouteFinder.GetWalkingRouteAsync(source, destination);
            }

            sw.Stop();
            TimeSpan TIME = sw.Elapsed;
            double[] pack = new double[2];
            pack[0] = routeResult.Route.LengthInMeters;
            pack[1] = routeResult.Route.EstimatedDuration.TotalMinutes;

            return pack;

        }

        private static async Task<XmlReader> getXmlReader(Geopoint source, Geopoint destination, RainMan.Navigation.RouteKind kind)
        {

            String query_url = makeURL(source, destination, kind);
            //String query_url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=Eiffel+Tower&wp.1=louvre+museum&optmz=distance&output=xml&key=AmJqOC6z5Nnf_tL1iajMSSLVyMoWRpwIBREiL1LE20_trwH1uFlK6yC5t0FrIqxD&routeAttributes=routeSummariesOnly";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(query_url);
            XmlReader reader = XmlReader.Create(response.Content.ReadAsStreamAsync().Result);
            return reader;
        }

        private static String makeURL(Geopoint source, Geopoint destination, RainMan.Navigation.RouteKind kind)
        {
            StringBuilder query_url = new StringBuilder();
            query_url.Append(baseURLString);

            if (kind == RainMan.Navigation.RouteKind.WALK)
            {
                query_url.Append(walking_q);
            }
            else
            {
                query_url.Append(driving_q);

                if (kind == RainMan.Navigation.RouteKind.BIKE)
                {
                    query_url.Append(bicycle_roads);
                }
            }

            query_url.Append(source_point);
            query_url.Append(source.Position.Latitude.ToString());
            query_url.Append(comma);
            query_url.Append(source.Position.Longitude.ToString());

            query_url.Append(destination_point);
            query_url.Append(destination.Position.Latitude.ToString());
            query_url.Append(comma);
            query_url.Append(destination.Position.Longitude.ToString());

            query_url.Append(key);

            query_url.Append(get_only_summary);

            return query_url.ToString();
        }


    }
}