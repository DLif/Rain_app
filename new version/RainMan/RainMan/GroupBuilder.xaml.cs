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
using RainMan.Navigation;
using RainMan.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupBuilder : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Boolean shouldSaveName = false;
        private IMobileServiceTable<PathGroup> groupsTable = App.mobileClient.GetTable<PathGroup>();

        public GroupBuilder()
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

        private GroupBuilderNavigator givenArgument = null;

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
            
            this.givenArgument = (GroupBuilderNavigator)e.Parameter;

            // request a path name only if the user wants to create a new a group and save it
            this.shouldSaveName = givenArgument.IsUserGroup;

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void acceptAppBar_Click(object sender, RoutedEventArgs e)
        {
            givenArgument.StartLocation = this.startingPoint;
            givenArgument.EndLocation = this.endingPoint;
            if (!this.shouldSaveName)
            {
                // navigate back
                this.Frame.Navigate(typeof(Routes), givenArgument);

            }

        }



        private void map_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private DependencyObject getWayPointPin(Geopoint point)
        {

            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);
            ImageBrush imgBrush = new ImageBrush();
            if (this.startingPoint == null)
            {
                imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/mappin.png"));
            }
            else
            {
                imgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/radar/waypointpin.png"));
            }

            //Creating a Rectangle
            var myRectangle = new Rectangle { Fill = imgBrush, Height = 35, Width = 20 };
            myRectangle.SetValue(Grid.RowProperty, 0);
            myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            myGrid.Children.Add(myRectangle);

            return myGrid;

        }

        private void enableAddress_Checked(object sender, RoutedEventArgs e)
        {
            this.addressGrid.Visibility = Visibility.Visible;
            this.addressFadeIn.Begin();
        }

        private void enableAddress_Unchecked(object sender, RoutedEventArgs e)
        {
            this.addressFadeOut.Begin();
        }


        private Geopoint startingPoint = null;
        private DependencyObject startPin = null;
        private Geopoint endingPoint = null;
        private DependencyObject endingPin = null;

        private void addWayPoint(Geopoint point)
        {
            // this.wayPoints.Add(point);
            DependencyObject wayPointPin = getWayPointPin(point);
            // this.wayPointsPins.Add(wayPointPin);

            // add pin to map
            this.map.Children.Add(wayPointPin);

            // MapControl.SetLocation(wayPointPin, point);
            // MapControl.SetNormalizedAnchorPoint(wayPointPin, new Point(0.5, 1));
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

            // trying to locate
            this.locateAddressModalWindow.Dialog.ShowAsync();

            String address = addressTextBox.Text;
            String errorText = "";
            Geopoint location = null;
            try
            {

                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(address, RadarMapManager.center, 5);



                if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
                {

                    location = result.Locations[0].Point;
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
                errorText = "Map service is disabled!";
            }
            catch (TaskCanceledException)
            {
                errorText = "Location was not found, try again";

            }

            if (errorText == "")
            {
                this.locateAddressModalWindow.Dialog.Hide();
                this.handleNewPoint(location);
            }

            this.locateAddressModalWindow.Dialog.Hide();
            if (errorText != "")
            {
                MessageDialog diag = new MessageDialog(errorText);
                diag.ShowAsync();
            }

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.addressTextBox.Text = "";
        }

        private void map_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            handleNewPoint(args.Location);
        }


        private void handleNewPoint(Geopoint location)
        {
            if (this.startingPoint != null && this.endingPoint != null)
                return;

            // show undo button
            if (this.undoAppBar.Visibility == Visibility.Collapsed)
                this.undoAppBar.Visibility = Visibility.Visible;

            DependencyObject wayPointPin = getWayPointPin(location);
            if (this.startingPoint == null)
            {

                // starting point not set
                // get the pin
                this.startingPoint = location;
                this.startPin = wayPointPin;
                this.messageText.Text = "Now, Select the target destination";

            }
            else
            {
                // ending point not set
                this.endingPoint = location;
                this.endingPin = wayPointPin;
                this.messageText.Text = "Ready to go!";
                this.acceptAppBar.Visibility = Visibility.Visible;

                this.enableAddress.IsChecked = false;
                this.enableAddress.Visibility = Visibility.Collapsed;
                this.addressGrid.Visibility = Visibility.Collapsed;

                this.locateMe.Visibility = Visibility.Collapsed;
            }


            // add pin to map
            this.map.Children.Add(wayPointPin);
            MapControl.SetLocation(wayPointPin, location);
            MapControl.SetNormalizedAnchorPoint(wayPointPin, new Point(0.5, 1));
        }

        private void undoAppBar_Click(object sender, RoutedEventArgs e)
        {
            if (this.endingPoint != null)
            {
                // remove
                this.map.Children.Remove(this.endingPin);
                this.endingPin = null;
                this.endingPoint = null;
                this.messageText.Text = "Select the target destination";
                this.acceptAppBar.Visibility = Visibility.Collapsed;

                this.enableAddress.Visibility = Visibility.Visible;
                this.locateMe.Visibility = Visibility.Visible;


                return;
            }

            if (this.startingPoint != null)
            {
                this.map.Children.Remove(this.startPin);
                this.startPin = null;
                this.startingPoint = null;
                // hide undo bar
                this.undoAppBar.Visibility = Visibility.Collapsed;
                this.messageText.Text = "Select the starting position";
                return;
            }
        }


        private ModalWindow locateMeModalWindow = new ModalWindow("Locating your position", "This may take a few seconds ... ", "");
        private ModalWindow locateAddressModalWindow = new ModalWindow("Locating address", "This may take a few seconds ... ", "");

        private async void locateMe_Click(object sender, RoutedEventArgs e)
        {

            if (this.enableAddress.IsChecked == true)
            {
                this.enableAddress.IsChecked = false;
                this.addressGrid.Visibility = Visibility.Collapsed;
            }

            locateMeModalWindow.Dialog.ShowAsync();

            // locate current location
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            String error = "Oops, could not locate you! try again later";
            Boolean errorOccured = false;

            Geopoint location = null;

            try
            {

                var myPosition = await locator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromSeconds(60),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                // navigate to location

                location = new Geopoint(new BasicGeoposition
                {
                    Latitude = myPosition.Coordinate.Latitude,
                    Longitude = myPosition.Coordinate.Longitude
                });



                Boolean success = await map.TrySetViewAsync(location, 11D, 0, 0, MapAnimationKind.None);
                if (!success)
                {
                    errorOccured = true;
                }
            }
            catch
            {

                errorOccured = true;

            }
            if (!errorOccured)
            {
                // add the pin, handle events
                this.handleNewPoint(location);
            }

            locateMeModalWindow.Dialog.Hide();
            if (errorOccured)
            {
                MessageDialog dialog = new MessageDialog(error);
                dialog.ShowAsync();
            }






        }

        private async void goButton_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text == "")
            {
                name.Text = "Please enter a name!";
                return;

            }

            if (startDescription.Text == "")
            {
                startDescription.Text = "Please enter description!";
                return;
            }
            if (endDescription.Text == "")
            {
                endDescription.Text = "Please enter description!";
                return;

            }


            this.Flyout.Hide();
            this.addressGrid.Visibility = Visibility.Collapsed;
            this.addressGrid.Opacity = 0;

            givenArgument.StartLocation = this.startingPoint;
            givenArgument.EndLocation = this.endingPoint;
            givenArgument.Name = name.Text;
            givenArgument.IsUserGroup = true;
            givenArgument.StartName = startDescription.Text;
            givenArgument.EndName = endDescription.Text;

            ModalWindow modalWindow = new ModalWindow("Saving group " + givenArgument.Name, "Please wait...", "");
            modalWindow.Dialog.ShowAsync();
            PathGroup newGroup = givenArgument.toPathGroup();

            // store group in cloud
            newGroup.UserId = App.userId;
            await this.groupsTable.InsertAsync(newGroup);
            modalWindow.Dialog.Hide();



            this.Frame.Navigate(typeof(Routes), givenArgument);
        }

        private void name_GotFocus(object sender, RoutedEventArgs e)
        {
            this.name.Text = "";
        }

        private void Flyout_Opening(object sender, object e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.shouldSaveName)
                this.acceptAppBar.Flyout = null;
        }

        private void startDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            startDescription.Text = "";
        }

        private void endDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            endDescription.Text = "";
        }

        private void CommandBar_Opened(object sender, object e)
        {

        }

        private void addressFadeOut_Completed(object sender, object e)
        {
            this.addressGrid.Visibility = Visibility.Collapsed;
        }
    }
}
