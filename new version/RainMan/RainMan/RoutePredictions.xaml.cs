using RainMan.Common;
using RainMan.DataModels;
using RainMan.Navigation;
using RainMan.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace RainMan
{
    #region
    public class SuggestionInfo
    {
        public DateTime TimeSuggestion
        {
            get;
            set;
        }
        public String PathNameSuggestion
        {
            get;
            set;
        }

        public SuggestionInfo()
        {
            this.PathNameSuggestion = null;
        }
    }

    public class PathInfo
    {
        public String PathName
        {
            get;
            set;
        }
        public Double AvgRain { get; set; }

        public TimeSpan EstimatedTime { get; set; }

        public Double EstimatedLength { get; set; }

        public PathInfo()
        {
            PathName = null;
        }

        public DateTime PathTime { get; set; }

        public DateTime StartTime { get; set; } // path independent

        public DateTime EndTime { get; set; }   // path independent
    }


    public class PathNameDescriptor
    {
        // path name
        public String Name { get; set; }


        public int Index { get; set; }
    
        public PathNameDescriptor(String name, int index)
        {
            this.Name = name;
            this.Index = index;
        }

        public static List<PathNameDescriptor> toPathNameDescriptors(List<String> pathNames)
        {
            var res = new List<PathNameDescriptor>();
            int index = 0;
            foreach(string name in pathNames)
            {
                res.Add(new PathNameDescriptor(name, index));
                index++;
            }
            return res;

        }
    }


    #endregion

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoutePredictions : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private RoutePredictionArgs arguments = null;
        private RadarMapManager manager = null;
        private RouteRainPredictor predictor = null;

        // stores computation per ROUTE in group
        private List<RoutePredictorResult> results = new List<RoutePredictorResult>();

        // stores the best starting point for each route in group (in a form of an index of a radar map)
        private List<int> minIndices = new List<int>();

        // stores the best average per group
        private List<double> bestAverages = new List<Double>();

        // number of computation slots per route (to how many subroutes we devide a route into)
        private int NumTimeSlots;

        // computation modal window
        private ModalWindow computationWindow;

        // the following contain the current path
        private int currentPathIndex;

        private int currentTimeIndex;

        // path related information
        private List<TimeSpan> estimatedPathDuration = new List<TimeSpan>();
        private List<Double> estimatedPathLength = new List<Double>();
        private List<GeoboundingBox> boxes = new List<GeoboundingBox>();


        public RoutePredictions()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


#region
        private async Task processRoute(List<Geopoint> path)
        {

            //---------------- PART 1 : Use BING API to complete the route from given waypoints -------------- //

            MapRouteFinderResult routeResult = null;
            if (arguments.Transportation == RouteKind.DRIVE)
            {
                // no restirctions
                routeResult =
                    await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(path,
                    MapRouteOptimization.Time,
                    MapRouteRestrictions.None);
            }

            else if (arguments.Transportation == RouteKind.BIKE)
            {

                // avoid highways
                routeResult =
                    await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(
                    path,
                    MapRouteOptimization.Time,
                    MapRouteRestrictions.Highways);

            }
            else if (arguments.Transportation == RouteKind.WALK)
            {
                // walking 
                routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(path);
            }

            // get the route
            if (routeResult.Status != MapRouteFinderStatus.Success)
            {
                // return error
                computationWindow.Dialog.Hide();
                MessageDialog diag = new MessageDialog("Error!");
                await diag.ShowAsync();
                return;
            }

            // ------------------- PART 2 Route computation ---------------------------------- //

            MapRoute route = routeResult.Route;

            estimatedPathDuration.Add(route.EstimatedDuration);
            this.estimatedPathLength.Add(route.LengthInMeters);
            boxes.Add(route.BoundingBox);

            if (this.arguments.Transportation == RouteKind.BIKE )
            {
                // factor for bike time was not set
                // calcualte it

                  var drivingSpeed = (route.LengthInMeters / 1000.0) / route.EstimatedDuration.TotalHours; // km/h
                  // 15.5 is the average bike speed
                  int factor = (int)(drivingSpeed / 15.5);
                  EstimatedTimeConverter.factor = factor;

            }


            // minimum index [ of starting radar map ] for given path
            int minIndex = 0;

            // get prediction(s) for given route
            var res = await predictor.routeToPredictions(route);

            double[] currentAverrageArray = res.averages;

            // find minimum
            for (int k = 0; k < this.avg.Length; ++k)
            {
                if (currentAverrageArray[k] < currentAverrageArray[minIndex])
                {
                    minIndex = k;
                }
            }

            // save the minimum average and index
            this.minIndices.Add(minIndex);
            this.bestAverages.Add(currentAverrageArray[minIndex]);

            // save the result itself
            this.results.Add(res);

        }

#endregion


        private int previousPathIndex = -1;

        private void showPath()
        {

            List<MapRouteView> viewList;
            if (previousPathIndex >= 0)
            {

                viewList = this.results.ElementAt(this.previousPathIndex).routesCollection;
                // first, hide previous
                foreach (MapRouteView mapRouteView in viewList)
                {
                    mapRouteView.OutlineColor = Colors.Transparent;
                    mapRouteView.RouteColor = Colors.Transparent;
                }
            }
            viewList = this.results.ElementAt(currentPathIndex).routesCollection;

            // now, show current
            for(int i = 0; i < viewList.Count; ++i)
            {
                MapRouteView curr = viewList.ElementAt(i);
                Color currColor = this.results.ElementAt(currentPathIndex).routesPerStartingTime[currentTimeIndex].ElementAt(i);
                curr.OutlineColor = currColor;
                curr.RouteColor = currColor;
            }

            // update current variables
            this.previousPathIndex = this.currentPathIndex;
           


        }


        private void updateContentView()
        {


            // draw current route
            showPath();

            // set path info

            PathInfo pathInfo = new PathInfo();

            pathInfo.PathName = arguments.RouteNames == null ? null : arguments.RouteNames.ElementAt(currentPathIndex);
            pathInfo.EstimatedTime = this.estimatedPathDuration.ElementAt(currentPathIndex);
            pathInfo.EstimatedLength = this.estimatedPathLength.ElementAt(currentPathIndex);
            pathInfo.AvgRain = this.results.ElementAt(currentPathIndex).averages[currentTimeIndex];           // mm/hour
            pathInfo.EndTime = manager.Maps.ElementAt(RadarMapManager.totalNumMaps - 2).Time;                 // remember to handle this
            pathInfo.StartTime = manager.Maps.ElementAt(RadarMapManager.totalOldMaps).Time;
            pathInfo.PathTime = manager.Maps.ElementAt(currentTimeIndex + RadarMapManager.totalOldMaps).Time;

            this.defaultViewModel["PathInfo"] = pathInfo;

        }


        private void loadAllPaths()
        {
            foreach(var path in this.results)
            {
                foreach(MapRouteView view in path.routesCollection)
                {
                    view.OutlineColor = Colors.Transparent;
                    view.RouteColor = Colors.Transparent;
                    this.map.Routes.Add(view);
                }
            }
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {


            MapService.ServiceToken = "BaBZ6ETOrg8G3L31STm8dg";
            manager = RadarMapManager.getRadarMapManager();
            computationWindow = new ModalWindow("Calculating ...", "", "", 1);    
            computationWindow.Dialog.ShowAsync();

            // get arguments
            arguments = e.NavigationParameter as RoutePredictionArgs;

            List<String> Names = new List<string>();
            if(arguments.RouteNames != null)
            {
                this.routesAppBar.Visibility = Visibility.Visible;
                Names = arguments.RouteNames;

            }
            List<PathNameDescriptor> pathNames = PathNameDescriptor.toPathNameDescriptors(Names);
            this.defaultViewModel["PathNames"] = pathNames;

            // get num time slots per route 
            this.NumTimeSlots = arguments.NumTimeSlots;

            // init predictor 
            predictor = new RouteRainPredictor(manager, NumTimeSlots, arguments.Transportation);

            // process each route
            for (int i = 0; i < arguments.RouteCollection.Count; ++ i )
            {

                await processRoute(arguments.RouteCollection.ElementAt(i));

            }


            int minPathIndex = 0;
            // find minimum group and 
            for (int k = 0; k < this.bestAverages.Count; ++k)
            {
                if (this.bestAverages[k] < this.bestAverages[minPathIndex])
                 {
                     minPathIndex = k;
                 }
            }

            // set current path
            this.currentPathIndex = minPathIndex;
            this.currentTimeIndex = minIndices.ElementAt(minPathIndex);

            // load all paths onto map control
            loadAllPaths();


            // start binding data
            SuggestionInfo suggestion = new SuggestionInfo();

            
            // put time and path name
            string pathName = arguments.RouteNames == null ? null : arguments.RouteNames.ElementAt(currentPathIndex);
            suggestion.TimeSuggestion = manager.Maps.ElementAt(currentTimeIndex + RadarMapManager.totalOldMaps).Time;
            suggestion.PathNameSuggestion = pathName;
            // put suggestion
            this.defaultViewModel["Suggestion"] = suggestion;
            this.SuggestionGrid.Visibility = Visibility.Visible;

            // update content view
            updateContentView();

            await map.TrySetViewBoundsAsync(this.boxes.ElementAt(currentPathIndex), null, MapAnimationKind.None);

            this.computationWindow.Dialog.Hide();
        }

        private MapRoute currentRoute;

        private List<Color>[] colorsPerStartingTime = new List<Color>[4];
        private double[] avg = new double[4];
        private List<MapRouteView> routeViews;

        private void setTimeText(int timeIndex)
        {
            //this.timeToLeaveText.Text = String.Format("Leave in {0} minutes", timeIndex * 10);
        }

        public async Task setPredictions(MapRoute route, int numTimeSlots)
        {

            // calculate prediction over route
            RouteRainPredictor predictor = new RouteRainPredictor(manager, numTimeSlots, arguments.Transportation);
            var res = await predictor.routeToPredictions(route);
            colorsPerStartingTime = res.routesPerStartingTime;
            routeViews = res.routesCollection;
            avg = res.averages;
        }


        private void addRouteAnnotations(List<LegAnnotation> legs)
        {
            map.Children.Clear();
            foreach (LegAnnotation leg in legs)
            {
                leg.addToMapControl(map);
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void appTimeSlider_Checked(object sender, RoutedEventArgs e)
        {
            if (this.exitTimeSliderGrid != null)
                this.exitTimeSliderGrid.Visibility = Visibility.Visible;
        }

        private void appTimeSlider_Unchecked(object sender, RoutedEventArgs e)
        {
            this.exitTimeSliderGrid.Visibility = Visibility.Collapsed;
        }

        private void appInfo_Checked(object sender, RoutedEventArgs e)
        {
            this.informationGrid.Visibility = Visibility.Visible;
           // double height = RainToHeight.rainToHeight(this.results.ElementAt(currentPathIndex).averages.ElementAt(currentTimeIndex));
          //  this.fillBarAnimation.To = height;
          //  this.rainRec.Height = height;
           // this.rainRec.Fill = new SolidColorBrush(ColorTranslator.rainToColor(this.results.ElementAt(currentPathIndex).averages.ElementAt(currentTimeIndex)));
          //  this.fillBar.Begin();
            
        }

        private void appInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            this.informationGrid.Visibility = Visibility.Collapsed;
        }

        private void exitTimeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            int oldIndex = ((int)e.OldValue) / 10;
            int newIndex = ((int)e.NewValue) / 10;
            if(oldIndex != newIndex)
            {
                this.currentTimeIndex = newIndex;
                updateContentView();
            }

        }

        private async void updateMapWithRoute(int index)
        {
            
            for (int i = 0; i < this.routeViews.Count; ++ i )
            {
                Color c = this.colorsPerStartingTime[index].ElementAt(i);
                this.routeViews.ElementAt(i).RouteColor = c;
                this.routeViews.ElementAt(i).OutlineColor = c;
            }

     
            await map.TrySetViewBoundsAsync(currentRoute.BoundingBox, null, MapAnimationKind.None);
        }

        private void CommandBar_Opened(object sender, object e)
        {

        }

        private void SuggestionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            fadeOutStoryBoardSuggestion.Begin();

        }

        private void fadeOutStoryBoardSuggestion_Completed(object sender, object e)
        {
            SuggestionGrid.Visibility = Visibility.Collapsed;
            CurrentTimeGrid.Visibility = Visibility.Visible;
            fadeInTime.Begin();
        }

        private void routesAppBar_Click(object sender, RoutedEventArgs e)
        {
            // hide app bar
            this.BottomAppBar.Visibility = Visibility.Collapsed;
            this.PathNamesGrid.Visibility = Visibility.Visible;
            fadeInPaths.Begin();

            // hide everything else
            SuggestionGrid.Visibility = Visibility.Collapsed;
            this.CurrentTimeGrid.Visibility = Visibility.Collapsed;
            this.appInfo.IsChecked = false;
            this.informationGrid.Visibility = Visibility.Collapsed;
        }

        private async void listPathNames_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            var item = (PathNameDescriptor)e.ClickedItem;
            int index = item.Index;


            if (this.currentPathIndex == index)
            {
             
                this.fadeOutPaths.Begin(); 
                // same item
                return;
            }
            this.currentPathIndex = index;

            // fetch the best time index
            this.currentTimeIndex = this.minIndices.ElementAt(currentPathIndex);

            // update content view
            updateContentView();
            
            // update slider
            this.exitTimeSlider.Value = currentTimeIndex * 10 + 5;

            // update route
            await map.TrySetViewBoundsAsync(this.boxes.ElementAt(currentPathIndex), null, MapAnimationKind.None);

            
            this.fadeOutPaths.Begin();

        }

        private void fadeOutPaths_Completed(object sender, object e)
        {
            this.PathNamesGrid.Visibility = Visibility.Collapsed;
            this.BottomAppBar.Visibility = Visibility.Visible;
            this.CurrentTimeGrid.Visibility = Visibility.Visible;
            if(this.CurrentTimeGrid.Opacity == 0)
            {
                this.fadeInTime.Begin();
            }
        }
    }
}
