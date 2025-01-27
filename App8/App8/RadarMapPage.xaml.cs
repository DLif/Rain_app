﻿using App8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using App8.DataModel;
using Windows.Services.Maps;
using System.Threading.Tasks;  

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RadarMapPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public RadarMapManager mapManager;

        public RadarMapPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;


            /* navigate the map to israel */
            //Israel          34.282   29.000   35.667   33.286 
            //country        longmin   latmin  longmax   latmax
          //  mapManager = new RadarMapManager();
          //  mapManager.updateMaps();

            
         
           

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

            // get the singleton
            this.mapManager = RadarMapManager.getRadarMapManager();

            ModalWindow temporaryDialog = new ModalWindow();
            ContentDialog dialog = temporaryDialog.Dialog;
            dialog.ShowAsync();

            try
            {
                await this.mapManager.updateRadarMaps(true);
               
            }
            catch
            {

                dialog.Hide();
                MessageDialog diag = new MessageDialog("Oops, our servers are down! please try again later", "Oops");
                hideBar.Begin();
                diag.ShowAsync();

                return;
                // TODO:
                // navigate back to main page (?)


            }

            dialog.Hide();

            this.defaultViewModel["RadarMaps"] = mapManager.Maps;


            this.slider_panel.Visibility = Visibility.Visible;
            ShowSlider.Begin();
            

          
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

       


        public static GeoboundingBox GetBounds(/*this*/ MapControl map)
        {
            Geopoint topLeft = null;

            try
            {
                map.GetLocationFromOffset(new Windows.Foundation.Point(0, 0), out topLeft);
            }
            catch
            {
                var topOfMap = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 85,
                    Longitude = 0
                });

                Windows.Foundation.Point topPoint;
                map.GetOffsetFromLocation(topOfMap, out topPoint);
                map.GetLocationFromOffset(new Windows.Foundation.Point(0, topPoint.Y), out topLeft);
            }

            Geopoint bottomRight = null;
            try
            {
                map.GetLocationFromOffset(new Windows.Foundation.Point(map.ActualWidth, map.ActualHeight), out bottomRight);
            }
            catch
            {
                var bottomOfMap = new Geopoint(new BasicGeoposition()
                {
                    Latitude = -85,
                    Longitude = 0
                });

                Windows.Foundation.Point bottomPoint;
                map.GetOffsetFromLocation(bottomOfMap, out bottomPoint);
                map.GetLocationFromOffset(new Windows.Foundation.Point(0, bottomPoint.Y), out bottomRight);
            }

            if (topLeft != null && bottomRight != null)
            {
                return new GeoboundingBox(topLeft.Position, bottomRight.Position);
            }

            return null;
        }

        private void location_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void location_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = "";
        }
        private DependencyObject getMyLocationPin()
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

        private int mapHeight;
        private int mapWidth;
        private double baseZoomLevel;
        private double baseScale;
        private  void map_Loaded(object sender, RoutedEventArgs e)
        {

            // set bounds to israel
            /* navigate the map to israel */
            //Israel          34.282   29.000   35.667   33.286 
            //country        longmin   latmin  longmax   latmax

            //center at beyt dagan
           // var mapCenter = new BasicGeoposition() { Latitude = 31.9994, Longitude = 34.8301 };
            var mapCenter = new BasicGeoposition() { Latitude = 31.9819, Longitude = 34.8287 };
            Geopoint mapCenterPoint = new Geopoint(mapCenter);

            map.Center = mapCenterPoint;
           // map.ZoomLevel = 7.16106397;
            map.ZoomLevel = 6.933;
            this.baseScale = getScale();

            //map.ZoomLevel = 6.85;

         //   List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();
         //   basicPositions.Add(new BasicGeoposition() { Latitude = 33.286, Longitude = 34.282 });
         //   basicPositions.Add(new BasicGeoposition() { Latitude = 29.5148, Longitude = 34.9465 });

            this.mapHeight = (int)Math.Floor(((MapControl)sender).ActualHeight);
            this.mapWidth = (int)Math.Floor(((MapControl)sender).ActualWidth);


         //  await this.map.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(basicPositions), null, MapAnimationKind.Linear);
           baseZoomLevel = this.map.ZoomLevel;

        }

        // current location pin rectangle
        private DependencyObject currentLocationPin = null;


        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {


            

            if(progressBar.Visibility == Visibility.Visible)
            {
                // other action is being done, wait till its over
                return;
            }

            if(this.flyoutOpened)
            {
                // close the flyout
                this.Flyout.Hide();
                this.flyoutOpened = false;

            }


            progressBar.Visibility = Visibility.Visible;
            progressBar.Opacity = 1;

            // locate current location
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            String  error = "Oops, could not locate you! try again later";
            Boolean errorOccured = false;

            try
            {

                var myPosition = await locator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromSeconds(60),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                // navigate to location

                var geopoint = new Geopoint(new BasicGeoposition
                {
                    Latitude = myPosition.Coordinate.Latitude,
                    Longitude = myPosition.Coordinate.Longitude
                });

                PredictionIconDataSource.currentLocation = geopoint;

                // draw a pin on the map
                if (this.currentLocationPin == null)
                {

                   
                    DependencyObject obj = getMyLocationPin();
                    this.map.Children.Add(obj);
                    // update location of pin on map
                    MapControl.SetLocation(obj, geopoint);
                    MapControl.SetNormalizedAnchorPoint(obj, new Point(0.5, 1));

                    this.currentLocationPin = obj;
                    
                    

                }
                else
                {
                    // update location of pin on map
                    MapControl.SetLocation(this.currentLocationPin, geopoint);
                    MapControl.SetNormalizedAnchorPoint(this.currentLocationPin, new Point(0.5, 0.5));
                }

                Boolean success = await map.TrySetViewAsync(geopoint, 11D, 0, 0, MapAnimationKind.None);
                if (!success)
                {
                    errorOccured = true;
                }
 

            }

            catch (System.UnauthorizedAccessException)
            {

                // fatal error
                errorOccured = true;

            }
            catch (TaskCanceledException)
            {
                // Cancelled or timed-out.
                errorOccured = true;

            }

            progressBar.Visibility = Visibility.Collapsed;

            if(errorOccured)
            {
                MessageDialog dialog = new MessageDialog(error);
                dialog.ShowAsync();
            }

        }

    

        private DependencyObject getMapPin()
        {
            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            //  myGrid.RowDefinitions.Add(new RowDefinition());
           myGrid.Background = new SolidColorBrush(Colors.Transparent);


         //  GeoboundingBox currentBox = GetBounds(this.map);

            //Creating a Rectangle
          //  var myRectangle = new Rectangle { Fill = new SolidColorBrush(Colors.Black), Height = this.mapHeight, Width = 40 };
            ImageBrush imgBrush = new ImageBrush();
          //  BitmapImage cloudImage = new BitmapImage(new Uri("ms-appx:///Assets/radar/prediction.jpg"));

           // RadarMapManager.makeTransparentBackground(cloudImage);



          //  imgBrush.ImageSource = mapManager.Maps[0].ImageSrc;

            
            var secRec = new Rectangle { Fill = imgBrush, Height = 512 , Width = 512  };
           // myRect = secRec;

        //    secRec.SetValue(Grid.RowProperty, 0);
         //   secRec.SetValue(Grid.ColumnProperty, 0);

          //  myRectangle.SetValue(Grid.RowProperty, 0);
          //  myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            //myGrid.Children.Add(myRectangle);
            myGrid.Children.Add(secRec);

            //Creating a Polygon
            // var myPolygon = new Polygon()
            // {
            //     Points = new PointCollection() { new Point(2, 0), new Point(22, 0), new Point(2, 40) },
            //     Stroke = new SolidColorBrush(Colors.Black),
            //     Fill = new SolidColorBrush(Colors.Black)
            // };
            //myPolygon.SetValue(Grid.RowProperty, 1);
            //myPolygon.SetValue(Grid.ColumnProperty, 0);

            //Adding the Polygon to the Grid
            // myGrid.Children.Add(myPolygon);
            return myGrid;


        }

       

        private Grid CreateMapPin()
        {
            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            //  myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);

            //Creating a Rectangle filled with an image
           
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/mapping.png"));

            var rec = new Rectangle { Fill = imgBrush, Height = 100, Width = 100 };

            //rec.SetValue(Grid.RowProperty, 0);
           // rec.SetValue(Grid.ColumnProperty, 0);
            myGrid.Children.Add(rec);

            return myGrid;

        }


        private DependencyObject CreatePin()
        {
            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
          //  myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);

            //Creating a Rectangle
            var myRectangle = new Rectangle { Fill = new SolidColorBrush(Colors.Black), Height = 40, Width = 40 };
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rain/rain_strong.png"));

            var secRec = new Rectangle { Fill = imgBrush, Height = 120, Width = 120 };

            myRectangle.SetValue(Grid.RowProperty, 0);
            myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            //myGrid.Children.Add(myRectangle);
            myGrid.Children.Add(secRec);

            //Creating a Polygon
           // var myPolygon = new Polygon()
           // {
           //     Points = new PointCollection() { new Point(2, 0), new Point(22, 0), new Point(2, 40) },
           //     Stroke = new SolidColorBrush(Colors.Black),
           //     Fill = new SolidColorBrush(Colors.Black)
           // };
            //myPolygon.SetValue(Grid.RowProperty, 1);
            //myPolygon.SetValue(Grid.ColumnProperty, 0);

            //Adding the Polygon to the Grid
           // myGrid.Children.Add(myPolygon);
            return myGrid;
        }


        private double getScale()
        {

            // Get the Map control's current zoom level.
             double zoomLevel = map.ZoomLevel;
            // Use the latitude from the center point of the Map control.

             double latitude = map.Center.Position.Latitude;
            // // The following formula for map resolution needs latitude in radians.
              latitude = latitude * (Math.PI / 180);
            // This constant is based on the diameter of the Earth and the
            // equations Microsoft uses to determine the map shown for the
            // Map control's zoom level.
             const double BING_MAP_CONSTANT = 156543.04;
            // Calculate the number of meters represented by a single pixel.
              double MetersPerPixel =
                 BING_MAP_CONSTANT * Math.Cos(latitude) / Math.Pow(2, zoomLevel);
            // Aditional units.
            //  double KilometersPerPixel = MetersPerPixel / 1000;
            //  double FeetPerPixel = MetersPerPixel * 3.28;
            //  double MilesPerPixel = FeetPerPixel / 5280;

              return MetersPerPixel;
        }


        private int currentMapIndex = 0;
        private void timeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {


            // Get the Map control's current zoom level.
           // double zoomLevel = sender.ZoomLevel;
            // Use the latitude from the center point of the Map control.
            
           // double latitude = sender.Center.Position.Latitude;
           // // The following formula for map resolution needs latitude in radians.
          //  latitude = latitude * (Math.PI / 180);
            // This constant is based on the diameter of the Earth and the
            // equations Microsoft uses to determine the map shown for the
            // Map control's zoom level.
          //  const double BING_MAP_CONSTANT = 156543.04;
            // Calculate the number of meters represented by a single pixel.
          //  double MetersPerPixel =
         //     BING_MAP_CONSTANT * Math.Cos(latitude) / Math.Pow(2, zoomLevel);
            // Aditional units.
          //  double KilometersPerPixel = MetersPerPixel / 1000;
          //  double FeetPerPixel = MetersPerPixel * 3.28;
          //  double MilesPerPixel = FeetPerPixel / 5280;
            // Determine the distance represented by the rectangle control.
         //   double scaleDistance = recScale.Width * MilesPerPixel;
          //  tbScale.Text = scaleDistance.ToString() + " miles";



         //   double val = ((Slider)sender).Value;
            
          //  double curr = map.ZoomLevel;

          //  if (this.baseZoomLevel + val > 20)
          //  {
          //      map.ZoomLevel = 20;
          //  }
          //  else
          //      map.ZoomLevel = this.baseZoomLevel + val;

            double val = ((Slider)sender).Value;
            int nextMapIndex = 0 ;
            this.mapManager.Maps.ElementAt(currentMapIndex).setVisible(false);
            if(val <= 5)
            {
                nextMapIndex = 0;
            }
            else if(val  <= 10)
            {
                nextMapIndex = 1;
            }
            else if(val <= 15)
            {
                nextMapIndex = 2;
            }
            else if(val <= 20)
            {
                nextMapIndex = 3;
            }
            this.mapManager.Maps.ElementAt(nextMapIndex).setVisible(true);
            this.currentMapIndex = nextMapIndex;

            // resize the new map
            this.map_ZoomLevelChanged(map, this);

        }

        private void map_ZoomLevelChanged(MapControl sender, object args)
        {

            var maps = this.mapManager.Maps;

            if (maps.Count > 0)
            {
                double currScale = getScale();
                double resizeParam = this.baseScale / currScale;
                var currentMap = maps.ElementAt(currentMapIndex);

                currentMap.Height = 512 * resizeParam;
                currentMap.Width = 512 * resizeParam;
            }
            
        }

        

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
           
            

        }

        private bool flyoutOpened = false;

        private async void goButton_Click(object sender, RoutedEventArgs e)
        {

            // hide previous error message if was shown
            this.errorText.Visibility = Visibility.Collapsed;

            String address = this.location.Text;
            if(address.Length == 0)
            {
                // empty text

                this.errorText.Text = "Please enter an address/location";
                this.errorText.Visibility = Visibility.Visible;
            }

            // show progress bar
            locationFindBar.Visibility = Visibility.Visible;
            locationFindBar.IsIndeterminate = true;
            

            try
            {

                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(address, RadarMapManager.center, 5);


                if (!this.flyoutOpened)
                {

                    // ignore the result
                    return;
                }

                if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
                {

                    MapLocation location = result.Locations.ElementAt(0);
                    Geopoint point = location.Point;

                    PredictionIconDataSource.currentLocation = point;

                    Boolean success = await map.TrySetViewAsync(point, 12D, 0, 0, MapAnimationKind.None);


                    if (!success)
                    {
                        // failed
                        this.errorText.Text = "Oops! failed to navigate to location";
                        this.errorText.Visibility = Visibility.Visible;
                        locationFindBar.Visibility = Visibility.Collapsed;

                    }
                    else
                    {
                        this.Flyout.Hide();
                    }

                }
                else
                {
                    this.errorText.Text = "Location was not found, try again";
                    this.errorText.Visibility = Visibility.Visible;
                    locationFindBar.Visibility = Visibility.Collapsed;
                }
            }

            catch(System.UnauthorizedAccessException)
            {
                // todo: critical error
                // remember to implement this
            }
            catch (TaskCanceledException)
            {
                this.errorText.Text = "Location was not found, try again";
                this.errorText.Visibility = Visibility.Visible;
                locationFindBar.Visibility = Visibility.Collapsed;
            }


           
        }

        private void Flyout_Opening(object sender, object e)
        {
            this.errorText.Visibility = Visibility.Collapsed;
            locationFindBar.Visibility = Visibility.Collapsed;

            flyoutOpened = true;

        }

        private void Flyout_Closed(object sender, object e)
        {
            flyoutOpened = false;
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {

            
            if(this.flyoutOpened)
            {
                flyoutOpened = false;
                this.Flyout.Hide();
            }
         

        }

        private Boolean flag = false;
        private async void map_MapTapped(MapControl sender, MapInputEventArgs args)
        {

            if(flag )
            {
                    this.mapManager.Maps.ElementAt(currentMapIndex).setVisible(true);
                    return;
                
            }

            PredictionIconDataSource.currentLocation = args.Location;
            int locationPixel = PointTranslation.locationToPixel(args.Location.Position.Latitude, args.Location.Position.Longitude);
            int x_pixel = locationPixel % 512;
            int y_pixel = (locationPixel - x_pixel) / 512;
            MessageDialog diaga = new MessageDialog(String.Format("[ {0} , {1} ]", x_pixel, y_pixel));
            await diaga.ShowAsync();
            flag = true;

        }

        private void map_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
            if (this.mapManager.Maps.ElementAt(currentMapIndex).Visibile == Visibility.Visible)
            {
                this.mapManager.Maps.ElementAt(currentMapIndex).setVisible(false);
                flag = false;
                return;
            }
        }

        
    }
}
