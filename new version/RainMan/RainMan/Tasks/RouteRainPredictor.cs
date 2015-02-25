using RainMan.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace RainMan.Tasks
{


    public class RoutePredictorResult
    {

        // holds result for each starting time [0, 10, 20, 30]
        public List<Color>[] routesPerStartingTime = new List<Color>[4];

        public List<MapRouteView> routesCollection = new List<MapRouteView>();

        // holds the average rain over route per starting time
        public double[] averages;

    }


    public class RouteRainPredictor
    {

     
        public List<RadarMap> Maps { get; set; }

        // to how many time slots we devide a single path/leg
        public int NumTimeSlots { get; set; }

        // what type of routing should we use
        public RouteKind RouteKind { get; set; }

        public RouteRainPredictor(RadarMapManager manager, int numTimeSlots, RouteKind routeKind)
        {
           
            this.NumTimeSlots = numTimeSlots;
            this.RouteKind = routeKind;
            this.Maps = new List<RadarMap>();
            // save only the relevant maps
            for(int i = RadarMapManager.totalOldMaps; i < RadarMapManager.totalNumMaps; ++ i)
            {
                Maps.Add(manager.Maps.ElementAt(i));
            }
        }

        public double GeopathToAverageRain(Geopath path, RadarMap predictionMap)
        {
            int numPoints = 1;
            double avg = 0;

            // iterative mean, to prevent overflow
            foreach (BasicGeoposition position in path.Positions)
            {
                Geopoint geopoint = new Geopoint(position);
                double curr = predictionMap.getAverageRain(geopoint, 1);

                avg = (((double)numPoints - 1) / numPoints) * avg + (1.0 / numPoints) * curr;

                ++numPoints;

            }
            return avg;
        }

        public double GeopathToAverageRain(Geopath path, RadarMap predictionMap, int startIndex, int endIndex)
        {
            int numPoints = 1;
            double avg = 0;

            // iterative mean, to prevent overflow
            for (int i = startIndex; i <= endIndex; ++i)
            {
                BasicGeoposition position = path.Positions.ElementAt(i);
                Geopoint geopoint = new Geopoint(position);
                double curr = predictionMap.getAverageRain(geopoint, 1);

                avg = (((double)numPoints - 1) / numPoints) * avg + (1.0 / numPoints) * curr;

                ++numPoints;

            }
            return avg;
        }


        // in case the travelling time is small, map can be determined by the starting time only
        // time is given in minutes
        public int startingTimeToMapIndex(double startTime)
        {


            if (startTime < 10.0)
            {

                return 0;

            }
            if (startTime < 20.0)
            {
                return 1;
            }
            if (startTime < 30.0)
            {
                return 2;
            }

            // minutes >= 30.0
            return 3;


        }
        private double currentLegAvgRain = 0;


        public async Task<RoutePredictorResult> predictionsForLeg(MapRouteLeg leg, double startingMinute)
        {
            // prediction for each starting time offset
            List<Color>[] result = new List<Color>[4];
            double[] averages = new double[4];
            List<MapRouteView> routes = new List<MapRouteView>();

            for(int j = 0; j < result.Length; ++j)
            {
                result[j] = new List<Color>();
                averages[j] = 0;
            }


            int numPaths = 1;
            int i = 0;

            // how many points in a slot
            int slotSize = (leg.Path.Positions.Count / this.NumTimeSlots);

            currentLegAvgRain = 0;

            // speed fix for bikes
            int factor = 1;
            if (this.RouteKind == RouteKind.BIKE)
            {
                var drivingSpeed = (leg.LengthInMeters / 1000.0) / leg.EstimatedDuration.TotalHours; // km/h
                // 15.5 is the average bike speed
                factor = (int)(drivingSpeed / 15.5);

            }

          
            for (i = 1; i < leg.Path.Positions.Count; i = i + slotSize - 1)
            {
                Geopoint startPoint = new Geopoint(leg.Path.Positions.ElementAt(i - 1));
                Geopoint endPoint = new Geopoint(leg.Path.Positions.ElementAt(Math.Min(i + slotSize - 2, leg.Path.Positions.Count - 1)));

                // Get the route between the points.
                MapRouteFinderResult routeResult = null;

               
                    if (this.RouteKind == RouteKind.DRIVE)
                    {
                        // no restirctions
                        routeResult =
                            await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint,
                            MapRouteOptimization.Time,
                            MapRouteRestrictions.None);
                    }

                    else if (this.RouteKind == RouteKind.BIKE)
                    {

                        // avoid highways
                        routeResult =
                            await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint,

                            MapRouteOptimization.Time,
                            MapRouteRestrictions.Highways);

                    }
                    else if (this.RouteKind == RouteKind.WALK)
                    {
                        // walking 
                        routeResult = await MapRouteFinder.GetWalkingRouteAsync(startPoint, endPoint);
                    }


                    if (routeResult.Status != MapRouteFinderStatus.Success)
                        continue;

                routes.Add(new MapRouteView(routeResult.Route));
                

                for (int k = 0; k <= RadarMapManager.totalOldMaps; ++k)
                {



                    int predictionMapIndex = minutesToMapIndex(startingMinute + 10 * k, routeResult.Route.EstimatedDuration.TotalMinutes * factor);

                    // get average rain over path
                    double curr = GeopathToAverageRain(leg.Path, this.Maps[predictionMapIndex], i - 1, Math.Min(i + slotSize - 2, leg.Path.Positions.Count - 1));
                    //double curr = GeopathToAverageRain(routes[index].Path, this.Maps[predictionMapIndex]);
                    
                    // get avg color
                    Color color = ColorTranslator.rainToColor(curr);

                    result[k].Add(color);
                    averages[k] = (((double)numPaths - 1) / numPaths) * averages[k] + (1.0 / numPaths) * curr;
                }

                // advance time
                startingMinute += routeResult.Route.EstimatedDuration.TotalMinutes * factor;

                numPaths++;
               
            }

            RoutePredictorResult rpr = new RoutePredictorResult();
            rpr.routesPerStartingTime = result;
            rpr.averages = averages;
            rpr.routesCollection = routes;
            return rpr;
        }


        public async Task<List<MapRouteView>> LegToSubRoutes(MapRouteLeg leg, double startingMinute)
        {

            List<MapRouteView> result = new List<MapRouteView>();
            int numPaths = 1;
            int i = 0;

            int slotSize = (leg.Path.Positions.Count / this.NumTimeSlots);

            currentLegAvgRain = 0;

            int factor = 1;
            if (this.RouteKind == RouteKind.BIKE)
            {
                var drivingSpeed = (leg.LengthInMeters/1000.0) / leg.EstimatedDuration.TotalHours; // km/h
                // 15.5 is the average bike speed
                factor = (int)(drivingSpeed / 15.5);

            }

            for (i = 1; i < leg.Path.Positions.Count; i = i + slotSize - 1)
            {
                

                Geopoint startPoint = new Geopoint(leg.Path.Positions.ElementAt(i - 1));
                Geopoint endPoint = new Geopoint(leg.Path.Positions.ElementAt(Math.Min(i + slotSize - 2, leg.Path.Positions.Count - 1)));

                // Get the route between the points.
                MapRouteFinderResult routeResult = null;


                if (this.RouteKind == RouteKind.DRIVE)
                {
                    // no restirctions
                    routeResult =
                        await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint,
                        MapRouteOptimization.Time,
                        MapRouteRestrictions.None);
                }

                else if (this.RouteKind == RouteKind.BIKE)
                {

                    // avoid highways
                    routeResult =
                        await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint,
                       
                        MapRouteOptimization.Time,
                        MapRouteRestrictions.Highways);

                }
                else if (this.RouteKind == RouteKind.WALK)
                {
                    // walking 
                    routeResult = await MapRouteFinder.GetWalkingRouteAsync(startPoint, endPoint);
                }

                // get map index from current starting minute
                //int predictionMapIndex = startingTimeToMapIndex(startingMinute);


                int predictionMapIndex = minutesToMapIndex(startingMinute, routeResult.Route.EstimatedDuration.Minutes * factor);

                // get average rain over path
                double curr = GeopathToAverageRain(leg.Path, this.Maps[predictionMapIndex], i - 1, Math.Min(i + slotSize - 2, leg.Path.Positions.Count - 1));

                // get avg color
                Color color = ColorTranslator.rainToColor(curr);

                MapRouteView newView = new MapRouteView(routeResult.Route);
                newView.OutlineColor = color;
                newView.RouteColor = color;
                result.Add(newView);

                // advance time
                startingMinute += routeResult.Route.EstimatedDuration.Minutes;

                currentLegAvgRain = (((double)numPaths - 1) / numPaths) * currentLegAvgRain + (1.0 / numPaths) * curr;

                numPaths++;

            }
            return result;

        }

        public double TotalRouteAverage = 0;

        public async Task<List<MapRouteView>> routeToColoredRoutes(MapRoute route, int startingMapIndex)
        {
            // this determines the radar map we will be using
            double minutes = 0;
            List<MapRouteView> subRoutes = new List<MapRouteView>();

            // vars for calculating the mean
            double mean = 0;
            int numLegs = 1;

            double meanTimePerPoint = route.EstimatedDuration.TotalMinutes / route.Path.Positions.Count;

            foreach (MapRouteLeg leg in route.Legs)
            {
                TimeSpan legTime = leg.EstimatedDuration;

                List<MapRouteView> result = await LegToSubRoutes(leg, startingMapIndex * 10 + minutes);
                subRoutes.AddRange(result);

                // advance next leg starting time
                minutes += legTime.Minutes;

                // advance the mean
                mean = (((double)numLegs - 1) / numLegs) * mean + (1.0 / numLegs) * currentLegAvgRain;

                ++numLegs;
            }

            TotalRouteAverage = mean;

            return subRoutes;



        }

        public async Task<RoutePredictorResult> routeToPredictions(MapRoute route)
        {
            // this determines the radar map we will be using
            double minutes = 0;

            // vars for calculating the mean
            int numLegs = 1;

            //double meanTimePerPoint = route.EstimatedDuration.TotalMinutes / route.Path.Positions.Count;

            // init result variable
            RoutePredictorResult res = new RoutePredictorResult();
            res.averages = new double[4];

            // speed fix for bikes
            int factor = 1;
            if (this.RouteKind == RouteKind.BIKE)
            {
                var drivingSpeed = (route.LengthInMeters / 1000.0) / route.EstimatedDuration.TotalHours; // km/h
                // 15.5 is the average bike speed
                factor = (int)(drivingSpeed / 15.5);

            }

            for (int k = 0; k <= RadarMapManager.totalOldMaps; ++ k )
            {
                res.averages[k] = 0;
                res.routesPerStartingTime[k] = new List<Color>();
            }


            foreach (MapRouteLeg leg in route.Legs)
            {
                   

                   RoutePredictorResult current = await predictionsForLeg(leg, minutes);

                   // add sub route itself
                   res.routesCollection.AddRange(current.routesCollection);
                   
                    // merge leg result
                   for (int k = 0; k <= RadarMapManager.totalOldMaps; ++k)
                   {
                       // update average
                       res.averages[k] = (((double)numLegs - 1) / numLegs) * res.averages[k] + (1.0 / numLegs) * current.averages[k];
                       // add sub route colors
                       res.routesPerStartingTime[k].AddRange(current.routesPerStartingTime[k]);
                      
                   }


                   TimeSpan legTime = leg.EstimatedDuration;
                    // advance next leg starting time
                    minutes += legTime.TotalMinutes * factor;
       
                    ++numLegs;
            }

            return res;
        }
        

        public double MapRouteToEstimations(MapRoute mapRoute, out List<LegAnnotation> annotations, int startingMapIndex)
        {

            // this determines the radar map we will be using
            double minutes = 0;
            annotations = new List<LegAnnotation>();



            // vars for calculating the mean
            double mean = 0;
            int numLegs = 1;

            foreach (MapRouteLeg leg in mapRoute.Legs)
            {
                TimeSpan legTime = leg.EstimatedDuration;
                int predictionMapIndex = minutesToMapIndex(minutes, legTime.Minutes);

                predictionMapIndex = startingMapIndex + predictionMapIndex > 3 ? 3 : startingMapIndex + predictionMapIndex;

                double legAvgRain = GeopathToAverageRain(leg.Path, this.Maps[predictionMapIndex]);

                // get middle point of leg
                BasicGeoposition position = leg.Path.Positions.ElementAt(leg.Path.Positions.Count / 2);
                Geopoint middlePoint = new Geopoint(position);

                // create the annotation
                annotations.Add(new LegAnnotation(middlePoint, legAvgRain));

                // advance next leg starting time
                minutes += legTime.Minutes;

                // advance the mean
                mean = (((double)numLegs - 1) / numLegs) * mean + (1.0 / numLegs) * legAvgRain;

                ++numLegs;
            }

            return mean;

        }

        private int minutesToMapIndex(double minutes, double estimatedDuration)
        {

            double absStart;
            double absEnd;

            double finishTime = minutes + estimatedDuration;

            if (minutes < 10.0)
            {
                // figure on which side the route lasts more
                // below 10, or after 10 ?

                absStart = Math.Abs(10.0 - minutes);
                absEnd = Math.Abs(10.0 - finishTime);

                if (absEnd > absStart)
                {
                    return 1;
                }
                else
                    return 0;

            }
            if (minutes < 20.0)
            {
                absStart = Math.Abs(20.0 - minutes);
                absEnd = Math.Abs(20.0 - finishTime);

                if (absEnd > absStart)
                {
                    return 2;
                }
                else
                    return 1;
            }
            if (minutes < 30.0)
            {
                absStart = Math.Abs(20.0 - minutes);
                absEnd = Math.Abs(20.0 - finishTime);

                if (absEnd > absStart)
                {
                    return 3;
                }
                else
                    return 2;
            }

            // minutes >= 30.0
            return 3;


        }

    }




    public class LegAnnotation
    {
        // should be the middle point on the route, where to draw the pushpin
        public Geopoint Location { get; set; }

        // holds the average rain over leg in a route
        public double AverageOverLeg { get; set; }

        // map pushpin
        public DependencyObject Pushpin { get; set; }

        public LegAnnotation(Geopoint location, double averageRain)
        {
            this.Location = location;
            this.AverageOverLeg = averageRain;

            // initialize the pushpin
            this.Pushpin = MapUtils.getRouteRainPushPin(averageRain);

        }

        public void addToMapControl(MapControl control)
        {
            MapControl.SetLocation(this.Pushpin, this.Location);
            MapControl.SetNormalizedAnchorPoint(this.Pushpin, new Point(0.0, 1.0));
            control.Children.Add(this.Pushpin);
        }


    }
}
