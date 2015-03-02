using RainMan.Common;
using RainMan.DataModels;
using RainMan.Navigation;
using RainMan.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Double EstimatedTime { get; set; }

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
        private RouteGroupAnnotator predictor;
          

        // path related information
        private List<Double> estimatedPathLength = new List<Double>();
        private List<GeoboundingBox> boxes = new List<GeoboundingBox>();


        public RoutePredictions()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

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
        private async Task<MapRoute> createRoute(List<Geopoint> path, ModalWindow computationWindow)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
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
               // computationWindow.Dialog.Hide();
                MessageDialog diag = new MessageDialog("Error!");
                await diag.ShowAsync();
      
            }
            sw.Stop();
            TimeSpan TIME = sw.Elapsed;

            this.boxes.Add(routeResult.Route.BoundingBox);
            this.estimatedPathLength.Add(routeResult.Route.LengthInMeters);

            return routeResult.Route;


        }

#endregion



        private async Task updateContentView()
        {

            RadarMapManager manager = RadarMapManager.getRadarMapManager();

            // set path info
            PathInfo pathInfo = new PathInfo();
            pathInfo.PathName = arguments.RouteNames == null ? null : arguments.RouteNames.ElementAt(predictor.CurrentRouteIndex);
            pathInfo.EstimatedTime = this.predictor.routeAnnotations.ElementAt(predictor.CurrentRouteIndex).EstimatedTimeMinutes;
            pathInfo.EstimatedLength = this.estimatedPathLength.ElementAt(predictor.CurrentRouteIndex);
            pathInfo.AvgRain = this.predictor.routeAnnotations.ElementAt(predictor.CurrentRouteIndex).Averages[predictor.CurrentTimeIndex];

            pathInfo.EndTime = manager.Maps.ElementAt(RadarMapManager.totalNumMaps - 2).Time;                 // remember to handle this
            pathInfo.StartTime = manager.Maps.ElementAt(RadarMapManager.totalOldMaps).Time;
            pathInfo.PathTime = manager.Maps.ElementAt(predictor.CurrentTimeIndex + RadarMapManager.totalOldMaps).Time;

            this.defaultViewModel["PathInfo"] = pathInfo;

            // update slider
            this.exitTimeSlider.Value = this.predictor.CurrentTimeIndex * 10 + 5;

            // update route
            await map.TrySetViewBoundsAsync(this.boxes.ElementAt(predictor.CurrentRouteIndex), null, MapAnimationKind.None);

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

            Stopwatch sw = new Stopwatch();
            sw.Start();

            RadarMapManager manager = RadarMapManager.getRadarMapManager();
            MapService.ServiceToken = "BaBZ6ETOrg8G3L31STm8dg";
            
           ModalWindow computationWindow = new ModalWindow("Calculating ...", "", "", 1);    
           //await computationWindow.Dialog.ShowAsync();

            // get arguments passed to page
            arguments = e.NavigationParameter as RoutePredictionArgs;

            handlePathNames();

            // get num time slots per route 
            int numTimeSlots = arguments.NumTimeSlots;
    
            List<MapRoute> routes = new List<MapRoute>();

            // create an actual path for each collection of waypoints in the given group
            for (int i = 0; i < arguments.RouteCollection.Count; ++ i )
            {
                MapRoute res = await createRoute(arguments.RouteCollection.ElementAt(i), computationWindow);
                routes.Add(res);

                // should handle failure here!

            }

            // allocate predictor 
            predictor = new RouteGroupAnnotator(routes, map);

            // init prediction
            await this.predictor.InitRouteGroupsPredictions(arguments.Transportation, numTimeSlots);

            // draw all the pushpins
            annotations = this.predictor.getAllAnnotations();
            //this.defaultViewModel["Pins"] = annotations;

            // start binding data
            SuggestionInfo suggestion = new SuggestionInfo();

            // put time and path name
            string pathName = arguments.RouteNames == null ? null : arguments.RouteNames.ElementAt(predictor.CurrentRouteIndex);
            suggestion.TimeSuggestion = manager.Maps.ElementAt(predictor.CurrentTimeIndex + RadarMapManager.totalOldMaps).Time;
            suggestion.PathNameSuggestion = pathName;

            // put suggestion
            this.defaultViewModel["Suggestion"] = suggestion;
            this.SuggestionGrid.Visibility = Visibility.Visible;

            // update content view
            await updateContentView();

            sw.Stop();
            var x = sw.Elapsed;

            //computationWindow.Dialog.Hide();
        }

        private List<Annotation> annotations; 

        private void handlePathNames()
        {
            List<String> Names = new List<string>();
            if(arguments.RouteNames != null)
            {
                // prediction on a group
                this.routesAppBar.Visibility = Visibility.Visible;
                Names = arguments.RouteNames;

            }
            
            // translate the given names to a presentable form
            List<PathNameDescriptor> pathNames = PathNameDescriptor.toPathNameDescriptors(Names);
            this.defaultViewModel["PathNames"] = pathNames;
        }

        private void setTimeText(int timeIndex)
        {
            //this.timeToLeaveText.Text = String.Format("Leave in {0} minutes", timeIndex * 10);
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

            // clear push pins
            this.map.Children.Clear(); 

            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

     

        private void appInfo_Checked(object sender, RoutedEventArgs e)
        {
            this.informationGrid.Visibility = Visibility.Visible;
               
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
                predictor.changeTimeIndex(newIndex);
                updateContentView();
            }

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


            if (predictor.CurrentRouteIndex == index)
            {
             
                this.fadeOutPaths.Begin(); 
                // same item
                return;
            }


            this.predictor.changeRoute(index);

            // update content view
            await updateContentView();
          
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
