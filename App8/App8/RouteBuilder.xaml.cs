using App8.Common;
using App8.DataModel;
using Microsoft.WindowsAzure.MobileServices;
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
using Windows.System;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteBuilder : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Geopoint source;
        private Geopoint destination;

        private List<Geopoint> wayPoints = new List<Geopoint>();
        private List<DependencyObject> wayPointsPins = new List<DependencyObject>();

        // last item we added to list of way points
        private int currentIndex = 0;


       

        public RouteBuilder()
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

            map.Center = RadarMapManager.center;
            map.ZoomLevel = 10D;

            DependencyObject startPin = getBoundPin(this.source);
            DependencyObject endPin = getBoundPin(this.destination);

            this.map.Children.Add(startPin);
            this.map.Children.Add(endPin);


            // update location of pin(s) on map
            MapControl.SetLocation(startPin, this.source);
            MapControl.SetNormalizedAnchorPoint(startPin, new Point(0.5, 1));
            MapControl.SetLocation(endPin, this.destination);
            MapControl.SetNormalizedAnchorPoint(endPin, new Point(0.5, 1));

          

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
            this.source = ((PathBuilderNavigator)e.Parameter).Source;
            this.destination = ((PathBuilderNavigator)e.Parameter).Dest;
            this.navigationHelper.OnNavigatedTo(e);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void enableAddress_Checked(object sender, RoutedEventArgs e)
        {
           
             this.addressGrid.Visibility = Visibility.Visible;

        }

        private void enableAddress_Unchecked(object sender, RoutedEventArgs e)
        {
            this.addressGrid.Visibility = Visibility.Collapsed;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = "";
        }


        private DependencyObject getBoundPin(Geopoint point)
        {

            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/mappin.png"));
            //Creating a Rectangle
            var myRectangle = new Rectangle { Fill = imgBrush, Height = 35, Width = 20 };
            myRectangle.SetValue(Grid.RowProperty, 0);
            myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            myGrid.Children.Add(myRectangle);

            return myGrid;

        }

        private DependencyObject getWayPointPin(Geopoint point)
        {

            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/waypointpin.png"));

            //Creating a Rectangle
            var myRectangle = new Rectangle { Fill = imgBrush, Height = 35, Width = 20 };
            myRectangle.SetValue(Grid.RowProperty, 0);
            myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            myGrid.Children.Add(myRectangle);

            return myGrid;

        }

        private async void map_Loaded(object sender, RoutedEventArgs e)
        {

            
            MapService.ServiceToken = "P4TC89D";

            List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();
            basicPositions.Add(source.Position);
            basicPositions.Add(RadarMapManager.center.Position);
            basicPositions.Add(destination.Position);


            try
            {

                bool result = await map.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(basicPositions), null, MapAnimationKind.Linear);

                if (!result)
                {
                    MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                    errorDialog.ShowAsync();
                }
            }
            catch
            {

                MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                errorDialog.ShowAsync();

            }
        }

        private void undoAppBar_Click(object sender, RoutedEventArgs e)
        {

            if(this.pathGenerated)
            {

                map.Routes.Clear();
                return;
            }

            if(this.wayPoints.Count == 0)
            {
                // no items in stack
                return;
            }

            int lastIndex = this.wayPoints.Count - 1;

            // remove the way point
            this.wayPoints.RemoveAt(lastIndex);

            // remove the pin from the map
            this.map.Children.Remove(this.wayPointsPins.ElementAt(lastIndex));

            // remove the pin from the list
            this.wayPointsPins.RemoveAt(lastIndex);

        }

        private void map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Geopoint point = args.Location;

            // add the point to the map
            this.addWayPoint(point);


        }

        private void addWayPoint(Geopoint point)
        {
            this.wayPoints.Add(point);
            DependencyObject wayPointPin = getWayPointPin(point);
            this.wayPointsPins.Add(wayPointPin);

            // add pin to map
            this.map.Children.Add(wayPointPin);

            MapControl.SetLocation(wayPointPin, point);
            MapControl.SetNormalizedAnchorPoint(wayPointPin, new Point(0.5, 1));
        }


        private Boolean pathGenerated = false;

        private async void acceptAppBar_Click(object sender, RoutedEventArgs e)
        {
          //  List<Geopoint> actualWayPoints = new List<Geopoint>();
          //  actualWayPoints.Add(this.source);
         //   actualWayPoints.AddRange(this.wayPoints);
         //   actualWayPoints.Add(this.destination);

            
            try
            {
             //   MapRouteFinderResult routeResult =
              //      await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(
              //          actualWayPoints,
               //         MapRouteOptimization.Distance
  //
             //       );
              //  MapRouteView routeView = new MapRouteView(routeResult.Route);
             //   map.Routes.Add(routeView);
              //  pathGenerated = true;

                // Start at Microsoft in Redmond, Washington.
                BasicGeoposition startLocation = new BasicGeoposition();
                startLocation.Latitude = 47.643;
                startLocation.Longitude = -122.131;
                Geopoint startPoint = new Geopoint(startLocation);

                // End at the city of Seattle, Washington.
                BasicGeoposition endLocation = new BasicGeoposition();
                endLocation.Latitude = 47.604;
                endLocation.Longitude = -122.329;
                Geopoint endPoint = new Geopoint(endLocation);

                MapService.ServiceToken = "P4TC89D";
                map.MapServiceToken = "P4TC89D";

                // Get the route between the points.
                MapRouteFinderResult routeResult =
                    await MapRouteFinder.GetDrivingRouteAsync(
                    startPoint,
                    endPoint,
                    MapRouteOptimization.Time,
                    MapRouteRestrictions.None);
               

               if( routeResult.Status == MapRouteFinderStatus.Success)
               {
                   MapRouteView routeView = new MapRouteView(routeResult.Route);
                      map.Routes.Add(routeView);
                      pathGenerated = true;
               }

            }
            catch
            {
                MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                errorDialog.ShowAsync();
           }

        }

       

        private async void addressTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
           if( e.Key == VirtualKey.Enter)
           {
               this.Focus(FocusState.Programmatic);

           }
           else
           {
               return;
           }

           if (addressTextBox.Text == "")
           {
               return;
           }

           String address = addressTextBox.Text;
           String errorText = "";
           try
           {

               MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(address, RadarMapManager.center, 5);



               if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
               {

                   addWayPoint(result.Locations[0].Point);
                   await map.TrySetViewAsync(result.Locations[0].Point);

               }
               else
               {
                   errorText = "Location was not found, try again";

               }
           }

           catch (System.UnauthorizedAccessException)
           {
               // todo: critical error
               // remember to implement this
           }
           catch (TaskCanceledException)
           {
               errorText = "Location was not found, try again";

           }
           if (errorText != "")
           {
               MessageDialog diag = new MessageDialog(errorText);
               diag.ShowAsync();
           }




        }

      
    }
}
