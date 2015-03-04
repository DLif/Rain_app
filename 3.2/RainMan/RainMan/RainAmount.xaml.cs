using RainMan.Common;
using RainMan.DataModels;
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
using RainMan.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RainAmount : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
       
        private List<Geopoint> wayPoints = new List<Geopoint>();
        private List<DependencyObject> wayPointsPins = new List<DependencyObject>();
        private List<MapPolyline> lineCollection = new List<MapPolyline>();
        private int numPredictionImages = 3;
        

      




        public RainAmount()
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            map.Center = RadarMapManager.center;
            map.ZoomLevel = 10D;


            this.undoAppBar.Visibility = Visibility.Collapsed;
           // DependencyObject startPin = getBoundPin(this.source);
            //DependencyObject endPin = getBoundPin(this.destination);

            //this.map.Children.Add(startPin);
            //this.map.Children.Add(endPin);


            // update location of pin(s) on map
           // MapControl.SetLocation(startPin, this.source);
           // MapControl.SetNormalizedAnchorPoint(startPin, new Point(0.5, 1));
           // MapControl.SetLocation(endPin, this.destination);
           // MapControl.SetNormalizedAnchorPoint(endPin, new Point(0.5, 1));



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

        private PathGroup currentGroup = null;

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

        private void enableAddress_Checked(object sender, RoutedEventArgs e)
        {
            TipGrid.Visibility = Visibility.Collapsed;
            this.addressGrid.Visibility = Visibility.Visible;
            addressGridFadeIn.Begin();

        }

        private void enableAddress_Unchecked(object sender, RoutedEventArgs e)
        {
            addressGridFadeOut.Begin();
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
            imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/waypointpin.png"));
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

            MapService.ServiceToken = "BaBZ6ETOrg8G3L31STm8dg";

        }

        private void undoAppBar_Click(object sender, RoutedEventArgs e)
        {

        
            if(this.polygon != null)
            {
                // remove the polygon and restore the polylines
                this.map.MapElements.Remove(polygon);
                polygon = null;

                // restore the polylines
                foreach(MapPolyline polyLine in this.lineCollection)
                {
                    this.map.MapElements.Add(polyLine);
                }

                this.enableAddress.Visibility = Visibility.Visible;
                return;
            }


            if (this.wayPoints.Count == 0)
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

            // remove the line associated with it, if any
            if (this.wayPoints.Count > 0)
            {
                this.lineCollection.RemoveAt(lineCollection.Count - 1);
                this.map.MapElements.RemoveAt(map.MapElements.Count - 1);
            }
            else
            {
                this.undoAppBar.Visibility = Visibility.Collapsed;


            }



            if(this.wayPoints.Count < 3)
            {
                this.acceptAppBar.Visibility = Visibility.Collapsed;
            }
        }

        private void map_MapTapped(MapControl sender, MapInputEventArgs args)
        {

            TipGrid.Visibility = Visibility.Collapsed;

            if (this.polygon != null)
                return;

            Geopoint point = args.Location;

            if(this.undoAppBar.Visibility == Visibility.Collapsed)
            {
                this.undoAppBar.Visibility = Visibility.Visible;
            }

            // add the point to the map
            this.addWayPoint(point);

            if(this.wayPoints.Count > 2)
            {
                this.acceptAppBar.Visibility = Visibility.Visible;
            }


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

            MapPolyline polyLine = new MapPolyline();
            
            if(wayPoints.Count > 1)
            {
                List<BasicGeoposition> points = new List<BasicGeoposition>();
                points.Add(this.wayPoints.ElementAt(this.wayPoints.Count - 2).Position);
                points.Add(this.wayPoints.ElementAt(this.wayPoints.Count - 1).Position);
                polyLine.Path = new Geopath(points);
                polyLine.StrokeColor = Colors.Blue;
              
                // add the polyline
                polyLine.StrokeThickness = 3.0;
                map.MapElements.Add(polyLine);

                // store a refrence for the polyline
                this.lineCollection.Add(polyLine);
                
            }
            

            
        }


        

        private MapPolygon polygon = null;

        private void acceptAppBar_Click(object sender, RoutedEventArgs e)
        {

            if (this.polygon == null)
            {

                // create a polygon
                this.polygon = new MapPolygon();

                List<BasicGeoposition> points = new List<BasicGeoposition>();

                foreach (Geopoint pt in this.wayPoints)
                {
                    points.Add(pt.Position);
                }

                // create the path
                this.polygon.Path = new Geopath(points);
                this.polygon.FillColor = Colors.DarkCyan;
                this.polygon.StrokeColor = Colors.Blue;
                this.polygon.StrokeThickness = 4;

                // clear polylines
                this.map.MapElements.Clear();
                this.map.MapElements.Add(this.polygon);

                // hide other buttons
                this.enableAddress.Visibility = Visibility.Collapsed;
            }

            BottomAppBar.Visibility = Visibility.Collapsed;
            // polygon created
            // show date selection

            overlayGrid.Visibility = Visibility.Visible;



        }

     

        private async void addressTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
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

      

        private void addressGridFadeOut_Completed(object sender, object e)
        {
            addressGrid.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            overlayGrid.Visibility = Visibility.Collapsed;
            ResultText.Visibility = Visibility.Collapsed;
            progress.Visibility = Visibility.Collapsed;
            BottomAppBar.Visibility = Visibility.Visible;

        }

        private async void GoBtn_Click(object sender, RoutedEventArgs e)
        {

            if (progress.IsActive == true)
            {
                return;
            }
            DateTime time = date.Date.LocalDateTime;
            TimeSpan diff = DateTime.Now - time;

            if (diff.TotalDays > 20)
            {
                MessageDialog diag = new MessageDialog("Date is too old! Requests are limited to 20 days back");
                await diag.ShowAsync();
                return;
            }

            progress.IsActive = true;
            progress.Visibility = Visibility.Visible;




            // first, get all points
            List<PixelRep> pixels = findAllPixels();

            // build the polygon
            var poly = new CustomPolygon(pixels.Count, pixels);
            List<PixelRep> inside_points = PolygonPixels.getAllPointsInsidePolygon(poly);


            // finally, build the request!
            APIRequest request = new APIRequest(pixels);

            String encodedRequest = RainApiSerializer.SerializeRequest(request);
            var dict = new Dictionary<String, String>();
            dict.Add("places", encodedRequest);
            dict.Add("numDaysString", diff.Days.ToString());

            try
            {
                string result = await App.mobileClient.InvokeApiAsync<string>("RainAmount", System.Net.Http.HttpMethod.Get, dict);



                double res = ((Double.Parse(result) * (1 / 6.0)) / 1000) / (inside_points.Count + pixels.Count); // in liters [Note: this is bullshit ]
                //double res = 0.23;
                if(this.usePredictions.IsOn)
                {
                    // add precitions data
                    // use:
                    var x = this.numPredictionImages;

                    pixels.AddRange(inside_points);
                    res += future_calc(x, pixels);


                }


                this.progress.IsActive = false;
                progress.Visibility = Visibility.Collapsed;

                this.ResultText.Visibility = Visibility.Visible;
                this.ResultText.Text = string.Format("Total: {0:0.000} Meters average", res);


            }
            catch (Exception ex)
            {
                MessageDialog diag = new MessageDialog("Server error while processing request");
                diag.ShowAsync();
                this.progress.IsActive = false;
                progress.Visibility = Visibility.Collapsed;
                
            }

        }

       
        private List<PixelRep> findAllPixels()
        {

            // get all points
            List<PixelRep> shapePixels = new List<PixelRep>();
            foreach(Geopoint point in this.wayPoints)
            {
                int locationPixel = PointTranslation.locationToPixel(point.Position.Latitude, point.Position.Longitude);
                int x_pixel = locationPixel % 512;
                int y_pixel = (locationPixel - x_pixel) / 512;

                shapePixels.Add(new PixelRep(x_pixel, y_pixel));
            }

            //var poly = new CustomPolygon(shapePixels.Count, shapePixels);
            //return PolygonPixels.getAllPointsInsidePolygon(poly);
            return shapePixels;


        }



        private void usePredictions_Toggled(object sender, RoutedEventArgs e)
        {
            if(usePredictions.IsOn)
            {
                this.predictionNumBtn.Visibility = Visibility.Visible;
            }
            else
            {
                this.predictionNumBtn.Visibility = Visibility.Collapsed;
            }
        }

       

        private void prediction30_Click(object sender, RoutedEventArgs e)
        {
            this.numPredictionImages = 3;
            predictionNumBtn.Content = "Minutes: 30";
            
        }

        private void prediction20_Click(object sender, RoutedEventArgs e)
        {
            this.numPredictionImages = 2;
            predictionNumBtn.Content = "Minutes: 20";
        }

        private void prediction10_Click(object sender, RoutedEventArgs e)
        {
            this.numPredictionImages = 1;
            predictionNumBtn.Content = "Minutes: 10";
        }


        private double future_calc(int future_images, List<PixelRep> polygon_points)
        {
            double power = 0.0;
           // var poly = new CustomPolygon(polygon_points.Count, polygon_points);
           // List<PixelRep> inside_points = PolygonPixels.getAllPointsInsidePolygon(poly);

            int image_size_x = 512;
            int image_size_y = 512;

            for (int i = 4; i < 4 + future_images ; i++)
            {
                WriteableBitmap currentMap = RadarMapManager.getRadarMapManager().Maps.ElementAt(i).ReadableImage;
                using (var buffer = currentMap.PixelBuffer.AsStream())
                {

                    foreach (PixelRep j in polygon_points)
                    {

                        Byte[] pixels = new Byte[4 * image_size_x * image_size_y];
                        buffer.Read(pixels, 0, pixels.Length);
                        power += ColorTranslator.power_to_radius(pixels, j.X,j.Y, 0, currentMap.PixelWidth);
                        
                    }
                }

            }
            return power;
        }




    }
}
