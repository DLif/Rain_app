using RainMan.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace RainMan.Tasks
{



    // this class is resposible for rain prediction over a group of routes
    // also provides methods to draw and update a given map control with the routes and annotations

    public class RouteGroupAnnotator
    {

        // annonations/predictions per route in group
        public List<SingleRouteAnnotations> routeAnnotations = new List<SingleRouteAnnotations>();

        public MapControl Map { get; set; }

        public List<MapRoute> Routes { get; set; }

        public int CurrentRouteIndex { get; set; }

        public int CurrentTimeIndex { get; set; }


        public SingleRouteAnnotations getCurrentAnnotation()
        {
            return this.routeAnnotations.ElementAt(CurrentRouteIndex);
        }

        public RouteGroupAnnotator(List<MapRoute> routes, MapControl map)
        {
            this.Routes = routes;
            this.Map = map;
            this.CurrentRouteIndex = -1;
        }

        public async Task InitRouteGroupsPredictions(RouteKind kind, int timeSlots)
        {

            List<RadarMap> Maps = new List<RadarMap>();
            RadarMapManager manager = RadarMapManager.getRadarMapManager();

            // save only the relevant maps
            for (int i = RadarMapManager.totalOldMaps; i < RadarMapManager.totalNumMaps; ++i)
            {
                Maps.Add(manager.Maps.ElementAt(i));
            }


            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Task> tasks = new List<Task>();
            // create an annotation and predictions for each route in the group
            foreach (MapRoute route in Routes)
            {
                SingleRouteAnnotations annotation = new SingleRouteAnnotations();
                // add to collection
                routeAnnotations.Add(annotation);

                // initialize predictions and annotations for current route in group
                await annotation.routeToAnnotations(route, kind, timeSlots, Maps);

               

            }
            //await Task.WhenAll(tasks);
            sw.Stop();
            TimeSpan x = sw.Elapsed;


            // load all the routes
            loadAllPaths(Routes);

            // show the selected path (so far without annotations)
            int bestPath = this.getBestPathIndex();
            showPath(bestPath);

            // annoate the best path from the group with the best possible departure time
            int bestTimeIndex = this.routeAnnotations.ElementAt(bestPath).BestTime;
            // finally, set the annotations
            this.SetRouteAnnotations(bestPath, bestTimeIndex);

        }

        private void showPath(int pathIndex)
        {

            if (this.CurrentRouteIndex == pathIndex)
                return;

            if (this.CurrentRouteIndex >= 0)
            {
                // hide previous route 
                this.Map.Routes.ElementAt(CurrentRouteIndex).OutlineColor = Colors.Transparent;
                this.Map.Routes.ElementAt(CurrentRouteIndex).RouteColor = Colors.Transparent;
            }


            // now, show current
            MapRouteView curr = this.Map.Routes.ElementAt(pathIndex);
            curr.OutlineColor = Colors.Blue;
            curr.RouteColor = Colors.Blue;

        }

        // method loads all the rounds onto the map
        private void loadAllPaths(List<MapRoute> routes)
        {
            foreach (var path in routes)
            {
                MapRouteView view = new MapRouteView(path);
                view.OutlineColor = Colors.Transparent;
                view.RouteColor = Colors.Transparent;
                this.Map.Routes.Add(view);

            }

            // add all annotations
            foreach(var prediction in this.routeAnnotations)
            {
                foreach(LegAnnotation annot in prediction.LegAnnotations)
                {

                    MapControl.SetLocation(annot.Pushpin, annot.Location);
                    MapControl.SetNormalizedAnchorPoint(annot.Pushpin, new Point(0.5, 1));
                    annot.Pushpin.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                    this.Map.Children.Add(annot.Pushpin);
                }
            }

        }
        public void changeTimeIndex(int timeIndex)
        {
            // need only to change the annotations
            this.SetRouteAnnotations(this.CurrentRouteIndex, timeIndex);
        }

        public void changeRoute(int routeIndex)
        {
            if (routeIndex == this.CurrentRouteIndex)
                return;

            // first, show the correct path on the map [and hide previous]
            this.showPath(routeIndex);

            // need to fetch the best time index for selected route
            int timeIndex = this.routeAnnotations.ElementAt(routeIndex).BestTime;

            // second, hide previous annotations and set current
            this.SetRouteAnnotations(routeIndex, timeIndex);

        }

      
     
        // set the annotation object for given path and given departure time
        private void SetRouteAnnotations(int pathIndex, int timeIndex)
        {


            this.CurrentTimeIndex = timeIndex;

            SingleRouteAnnotations result = this.routeAnnotations.ElementAt(pathIndex);

            // update colors according to starting time
            result.setRouteAtTime(timeIndex);

            if (this.CurrentRouteIndex == pathIndex)
                // if so, we already updated the colors
                return;

            if (CurrentRouteIndex >= 0)
            {
                // hide previous pushpins
                SingleRouteAnnotations prev = this.routeAnnotations.ElementAt(CurrentRouteIndex);
                foreach(LegAnnotation annotation in prev.LegAnnotations)
                {
                    annotation.Pushpin.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                }
            }

            // add current pushpins
            foreach(LegAnnotation annotation in result.LegAnnotations)
            {
                annotation.Pushpin.SetValue(Grid.VisibilityProperty, Visibility.Visible);                     
                
            }

            this.CurrentRouteIndex = pathIndex;

        }

        // get the index of the group with the minimal rain damage
        public int getBestPathIndex()
        {
            int minIndex = 0;
            double minAvg = this.routeAnnotations.ElementAt(0).Averages[this.routeAnnotations.ElementAt(0).BestTime];
            for (int i = 1; i < this.routeAnnotations.Count; ++i)
            {
                double currAvg = this.routeAnnotations.ElementAt(i).Averages[this.routeAnnotations.ElementAt(i).BestTime];
                if (currAvg < minAvg)
                {
                    minAvg = currAvg;
                    minIndex = i;
                }

            }
            return minIndex;
        }



    }

    public class SingleRouteAnnotations
    {

        // average per starting point
        public List<Color>[] Colors = new List<Color>[4];

        // total average per starting point
        public double[] Averages = new double[4];

        // list of leg annotations
        public List<LegAnnotation> LegAnnotations = new List<LegAnnotation>();

        private int bestTime;

        // best time to leave (minimal rain average)
        public int BestTime { get { return this.bestTime; } }

        // traversal estimation in MINUTES
        public double EstimatedTimeMinutes { get; set; }

        private int currentTimeIndex;

        public SingleRouteAnnotations()
        {
            // initialize data structures
            for (int i = 0; i < 4; ++i)
            {
                Colors[i] = new List<Color>();
            }

            currentTimeIndex = -1;
        }

      


        // sets the annotations' colors according to the chosen route and the chosen starting time
        public void setRouteAtTime(int timeIndex)
        {

            if (currentTimeIndex == timeIndex)
                return;

            currentTimeIndex = timeIndex;
            List<Color> currentColors = this.Colors[timeIndex];
            int k = 0;
            foreach (LegAnnotation anno in LegAnnotations)
            {
                // update the annotation color
                anno.colorRectangle.Fill = new SolidColorBrush(currentColors.ElementAt(k));
                ++k;
            }

        }

        // how many times the average speed of vehicle is larger than a bikes
        // used to convert time estimations
        private double toBikeFixFactor(double lengthInMeters, double totalHours)
        {
            var avgDrivingSpeed = lengthInMeters / totalHours; // km/h
            // 15.5 is the average bike speed
            return avgDrivingSpeed / 15.5;
        }


        // calculate prediction for the given route and fill the data (colors and averages)
        public async Task routeToAnnotations(MapRoute route, RouteKind routeKind, int numTimeSlots, List<RadarMap> maps)
        {
           
            await predictionsForPath(route, 0, numTimeSlots,routeKind, maps);

            // find best time to leave
            int minIndex = 0;
            for (int i = 0; i < 4; ++i)
            {
                if (this.Averages[i] < this.Averages[minIndex])
                    minIndex = i;
            }

            this.bestTime = minIndex;

        }


        public async Task predictionsForPath(MapRoute path, double startingMinute, int numTimeSlots, RouteKind kind, List<RadarMap> Maps)
        {

            int numPaths = 1;
            int i, k = 0;

            // how many points in a slot
            int slotSize = (path.Path.Positions.Count / numTimeSlots);


            Stopwatch sw = new Stopwatch();
            sw.Start();

            double[][] results = new double[numTimeSlots + 1][];

            
            int numIterations = (int)Math.Floor(path.Path.Positions.Count / (double)(slotSize -1));

            var seq = Enumerable.Range(0, numIterations);

            var tasks = seq.Select(async j =>
                {
                    i = 1 + (slotSize - 1) * j;
                   int startIndex = i - 1;
                   int lastIndex = Math.Min(i + slotSize - 2, path.Path.Positions.Count - 1);
                   Geopoint startPoint = new Geopoint(path.Path.Positions.ElementAt(startIndex));
                   Geopoint endPoint = new Geopoint(path.Path.Positions.ElementAt(lastIndex));

                   results[j] = await LocationsToEstimations.getTimeAndDistance(startPoint, endPoint, kind);
                   // awaitingTask.Wait();
                   // results[j] = awaitingTask.Result;
               });

           await Task.WhenAll(tasks);

           

           // int count = path.Path.Positions.Count;

          //  ParallelOptions options = new ParallelOptions();

          //  options.MaxDegreeOfParallelism = 1;

           // Parallel.For(0, numIterations,options, (j) =>
           //     {
           //         i = 1 + (slotSize - 1) * j;
           //        int startIndex = i - 1;
            //       int lastIndex = Math.Min(i + slotSize - 2, count - 1);
             //       Geopoint startPoint = new Geopoint(path.Path.Positions.ElementAt(startIndex));
            //        Geopoint endPoint = new Geopoint(path.Path.Positions.ElementAt(lastIndex));

                   // Task<double[]> awaitingTask = LocationsToEstimations.getTimeAndDistance(startPoint, endPoint, kind);
                    
                   // results[j] = awaitingTask.Result;

         //    });
//
            sw.Stop();
            var x = sw.Elapsed;
   
            for (i = 1, k = 0; i < path.Path.Positions.Count; i = i + slotSize - 1, ++k)
            {

                int startIndex = i -1;
                int lastIndex = Math.Min(i + slotSize - 2, path.Path.Positions.Count -1);

                LegAnnotation annot = new LegAnnotation(new Geopoint(path.Path.Positions.ElementAt((int)(0.5 * startIndex + 0.5 * lastIndex))));
                this.LegAnnotations.Add(annot);

                
               // Geopoint startPoint = new Geopoint(path.Path.Positions.ElementAt(startIndex));
               // Geopoint endPoint = new Geopoint(path.Path.Positions.ElementAt(lastIndex));

               // double[] estimations  = await LocationsToEstimations.getTimeAndDistance(startPoint, endPoint, kind);

                double[] estimations = results[k];
                
                double estimatedTime = estimations[1];
                double estimatedLength = estimations[0];


                double factor = 1;
                if (kind == RouteKind.BIKE)
                    factor = toBikeFixFactor(estimatedLength, estimatedTime);


                for ( k = 0; k <= RadarMapManager.totalOldMaps; ++k)
                {


                    int predictionMapIndex = minutesToMapIndex(startingMinute + 10 * k, estimatedTime * factor);

                    // get average rain over path
                    double curr = GeopathToAverageRain(path.Path, Maps[predictionMapIndex], i - 1, Math.Min(i + slotSize - 2, path.Path.Positions.Count - 1));

                    // get avg color
                    Color color = ColorTranslator.rainToColor(curr);

                    this.Colors[k].Add(color);
                    this.Averages[k] = (((double)numPaths - 1) / numPaths) * this.Averages[k] + (1.0 / numPaths) * curr;
                }

                

                // advance time
                startingMinute += estimatedTime * factor;
                this.EstimatedTimeMinutes += estimatedTime * factor;

                numPaths++;

            }

           
        }

        private static int minutesToMapIndex(double minutes, double estimatedDuration)
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




        #region
        public static double GeopathToAverageRain(Geopath path, RadarMap predictionMap)
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

        public static double GeopathToAverageRain(Geopath path, RadarMap predictionMap, int startIndex, int endIndex)
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
        #endregion


    }

    #region
    public class Annotation
    {

        // location of the annotation
        public Geopoint Location { get; set; }

        // anchor point
        public Point AnchorPoint { get; set; }

        // the pushpin it self
        public DependencyObject Pushpin { get; set; }


        public Annotation(Geopoint location)
        {
            this.Location = location;
            this.AnchorPoint = new Point(0.0, 1.0);

        }
    }

    public class TimeMark : Annotation
    {

        // time mark of 10, 20 or 30 minutes radar map
        public int Time { get; set; }


        public TimeMark(int timeMark, Geopoint location)
            : base(location)
        {

            this.Time = timeMark;
            // create the map push pin
            this.Pushpin = MapUtils.getTimeAnnotation(timeMark);
        }

    }

    public class LegAnnotation : Annotation
    {

        // map pushpin
        public DependencyObject Pushpin { get; set; }

        // Color rectangle
        public Rectangle colorRectangle { get; set; }


        public LegAnnotation(Geopoint location)
            : base(location)
        {

            Rectangle tempRect;

            // initialize the pushpin (with default colour), fetch the rect that holds the color fill
            this.Pushpin = MapUtils.getRouteRainPushPin(0, out tempRect);

            this.colorRectangle = tempRect;


        }

    }
    #endregion
}
