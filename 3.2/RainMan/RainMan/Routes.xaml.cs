using Microsoft.WindowsAzure.MobileServices;
using RainMan.Common;
using RainMan.DataModels;
using RainMan.Navigation;
using RainMan.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    #region
    public class MapPin
    {
        public String imagePath { get; set; }
        public Geopoint Location { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }


        public Point AnchorPoint { get; set; }

        // color either Colors.red or Colors.blue
        public MapPin(Color color, Geopoint location)
        {
            this.Location = location;
            if (color == Colors.Red)
            {
                imagePath = "ms-appx:///Assets/radar/mappin.png";
            }
            else
            {
                imagePath = "ms-appx:///Assets/radar/waypointpin.png";
            }
            // set default size
            this.Height = 35;
            this.Width = 20;
            // default anchor point
            this.AnchorPoint = new Point(0.5, 1);


        }
    }


    public class QuickRouteOptions : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Double overlayVisibile;

        public Geopoint StartPoint { get; set; }
        public Geopoint DestinationPoint { get; set; }

        public Double OverlayVisibile
        {
            get
            {
                return this.overlayVisibile;
            }

            set
            {

                this.overlayVisibile = value;
                NotifyPropertyChanged("OverlayVisibile");

            }
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<MapPin> Points { get; set; }

        public QuickRouteOptions()
        {
            this.Points = new ObservableCollection<MapPin>();
        }

        public void setPointCollection(Geopoint start, Geopoint end)
        {
            StartPoint = start;
            DestinationPoint = end;

            Points.Clear();
            Points.Add(new MapPin(Colors.Red, start));
            Points.Add(new MapPin(Colors.Blue, end));
        }

        public Boolean isMapSet()
        {
            return this.Points.Count > 0;
        }


    }
    #endregion

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Routes : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private IMobileServiceTable<PathGroup> groupsTable = App.mobileClient.GetTable<PathGroup>();


        private QuickRouteOptions quickRouteHubOptions = new QuickRouteOptions();

        // arguments for prediction
        private int maxStallTime = 3;  //30 minutes forward
        private RouteKind routeKind = RouteKind.BIKE;
        private int numTimeSlots = 10;

        public Routes()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
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


            this.defaultViewModel["PathGroups"] = new List<PathGroupWrapper>();

            this.predictAppBarGroup.Visibility = Visibility.Collapsed;
            this.manageGroupAppBar.Visibility = Visibility.Collapsed;
           
         
            managerAppBarVisible = Visibility.Collapsed;
            predictAppBarGroupVisibile = Visibility.Collapsed;

            if(Frame.BackStackDepth > 1)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }

 
            if (!this.defaultViewModel.ContainsKey("QuickRouteOptions"))
            {
                // first time
                quickRouteHubOptions.OverlayVisibile = 1;
                this.defaultViewModel["QuickRouteOptions"] = quickRouteHubOptions;
            }

            if (e.NavigationParameter is GroupBuilderNavigator)
            {

                GroupBuilderNavigator arg = e.NavigationParameter as GroupBuilderNavigator;
                if (!arg.IsUserGroup)
                {
                    // argument for quick routing
                    quickRouteHubOptions.setPointCollection(arg.StartLocation, arg.EndLocation);
                    quickRouteHubOptions.OverlayVisibile = 0;


                    // enable prediction button
                    predictAppBar.Visibility = Visibility.Visible;


                }
            }


            try
            {

                // load user specific groups
                loadingErrorMessage.Text = "Downloading user groups ... ";
                loadingErrorMessage.Visibility = Visibility.Visible;
                loadingListProgress.Visibility = Visibility.Visible;  
                var x = await this.groupsTable.Where(item => item.UserId == App.userId).ToCollectionAsync<PathGroup>();
                this.loadingListProgress.IsActive = false;
                this.loadingListProgress.Visibility = Visibility.Collapsed;

                this.defaultViewModel["PathGroups"] = wrapGroups(x);
                if(x.Count == 0)
                {

                    loadingErrorMessage.Text = "No groups created";
                }
                else
                {
                    loadingErrorMessage.Visibility = Visibility.Collapsed;
                    fadeInList.Begin();
                   
                }
               
            }

            catch
            {
                this.loadingListProgress.Visibility = Visibility.Collapsed;
                loadingErrorMessage.Visibility = Visibility.Visible;
                loadingErrorMessage.Text = "Oops! server is down";

            }
           

        }

        private List<PathGroupWrapper> wrapGroups(Collection<PathGroup> collection)
        {
            List<PathGroupWrapper> res = new List<PathGroupWrapper>();
            foreach(PathGroup pt in collection)
            {
                res.Add(pt.toListViewItem());
            }
            return res;

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


   

        private async Task mapUpdate()
        {
            // get start and end poin
            List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();
            basicPositions.Add(quickRouteHubOptions.Points.ElementAt(0).Location.Position);
            basicPositions.Add(quickRouteHubOptions.Points.ElementAt(1).Location.Position);

            try
            {

                bool result = await map.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(basicPositions), null, MapAnimationKind.None);

                if (!result)
                {
                    MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                    await errorDialog.ShowAsync();
                }
                else
                {
                    map.ZoomLevel -= 0.1;
                }
            }
            catch
            {

                MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                errorDialog.ShowAsync();

            }

        }


      

        private void flyoutGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            grid.Margin = new Thickness(0, this.ActualHeight - 250, 0, 0);
        }

        

        private void walkFlyout_Click(object sender, RoutedEventArgs e)
        {
            FadeOutStory.Begin();
            this.routeKind = RouteKind.WALK;
        }


        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

            FadeOutStory.Begin();
            this.routeKind = RouteKind.BIKE;
        }

        private void carFlyout_Click(object sender, RoutedEventArgs e)
        {
            FadeOutStory.Begin();
            this.routeKind = RouteKind.DRIVE;
        }

        private void MapOverlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // no need to create a name for the group
            GroupBuilderNavigator groupBuilderArg = new GroupBuilderNavigator(false);
            Frame.Navigate(typeof(GroupBuilder), groupBuilderArg);

        }

        private void time0_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 0; // how many predictions maps we're willing to use
            timeButton.Content = String.Format("{0} minutes", 0);
        }

        private void time10_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 1;
            timeButton.Content = String.Format("{0} minutes", 10);
        }

        private void time20_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 2;
            timeButton.Content = String.Format("{0} minutes", 20);
        }

        private void time30_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 3;
            timeButton.Content = String.Format("{0} minutes", 30);
        }


        private void FadeOutStory_Completed(object sender, object e)
        {
            switch(this.routeKind)
            {
                case RouteKind.BIKE:
                    this.transportImagebrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons/bike.png"));
                    break;
                case RouteKind.WALK:
                    this.transportImagebrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons/walking.png"));
                    break;
                case RouteKind.DRIVE:
                    this.transportImagebrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons/car.png"));
                    break;

            }
            FadeInStory.Begin();
        }

        private async void map_Loaded(object sender, RoutedEventArgs e)
        {
            if (quickRouteHubOptions.OverlayVisibile == 1)
            {
                // set center point 
                map.Center = RadarMapManager.center;
                map.ZoomLevel = 6;
            }
            else
            {

                await mapUpdate();

            }
        }

        private void MapOverlay2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GroupBuilderNavigator groupBuilderArg = new GroupBuilderNavigator(false);
            Frame.Navigate(typeof(GroupBuilder), groupBuilderArg);
        }


        // save command bar state
        private Visibility predictAppBarGroupVisibile = Visibility.Collapsed;
       
        private Visibility managerAppBarVisible = Visibility.Collapsed;
        private Visibility predictAppBarVisibile = Visibility.Collapsed;


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int no = pivot.SelectedIndex;
            if(no == 0)
            {
                // save state
                predictAppBarGroupVisibile = predictAppBarGroup.Visibility;
              
                
                managerAppBarVisible = manageGroupAppBar.Visibility;

                // hide

                predictAppBarGroup.Visibility = Visibility.Collapsed;
                addGroup.Visibility = Visibility.Collapsed;
                
                manageGroupAppBar.Visibility = Visibility.Collapsed;

                // restore state
                predictAppBar.Visibility = predictAppBarVisibile;

            }
            else
            {
                // save state
                predictAppBarVisibile = predictAppBar.Visibility;
                predictAppBar.Visibility = Visibility.Collapsed;

                // restore state
                predictAppBarGroup.Visibility = predictAppBarGroupVisibile;
                addGroup.Visibility = Visibility.Visible;
               
                manageGroupAppBar.Visibility = managerAppBarVisible;

            }
        }


        private PathGroup selectedGroup;
        private PathGroupWrapper previousGroup = null;


        private void groupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

        }

        private void groupList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // set command bar  
            manageGroupAppBar.Visibility = Visibility.Visible;
            predictAppBarGroup.Visibility = Visibility.Visible;

            selectedGroup = ((PathGroupWrapper)e.ClickedItem).toPathGroup();
            
            // deselect previous
            if(previousGroup != null)
                previousGroup.Selected = false;

            // select current
            var selectedGroupWrapper = (PathGroupWrapper)e.ClickedItem;
            selectedGroupWrapper.Selected = true;

            // rememebr previous
            previousGroup = selectedGroupWrapper;


        }

        private void addGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupBuilderNavigator groupBuilderArg = new GroupBuilderNavigator(true);
            Frame.Navigate(typeof(GroupBuilder), groupBuilderArg);
        }

        private void predictAppBar_Click(object sender, RoutedEventArgs e)
        {
            List<Geopoint> wayPoints = new List<Geopoint>();
            wayPoints.Add(quickRouteHubOptions.StartPoint);
            wayPoints.Add(quickRouteHubOptions.DestinationPoint);
            List<List<Geopoint>> paths = new List<List<Geopoint>>();
            // only one path in this case
            paths.Add(wayPoints);
            

            RoutePredictionArgs args = new RoutePredictionArgs(quickRouteHubOptions.StartPoint,
                                                               quickRouteHubOptions.DestinationPoint,
                                                               paths, this.routeKind, this.maxStallTime, this.numTimeSlots
                                                               );
            Frame.Navigate(typeof(RoutePredictions), args);

        }

        private void hideSettingsGridStory_Completed(object sender, object e)
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
            this.BottomAppBar.Visibility = Visibility.Visible;
        }

        private void settingsAppbar_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsGrid.Visibility = Visibility.Visible;
            this.BottomAppBar.Visibility = Visibility.Collapsed;
            this.showSettingsGridStory.Begin();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            

            // save logic

            this.hideSettingsGridStory.Begin();

        }

        private void timeSlotSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            numTimeSlots = (int)e.NewValue;
            if(this.sliderValueText != null)
                 this.sliderValueText.Text = numTimeSlots.ToString();
        }

        private void manageGroupAppBar_Click(object sender, RoutedEventArgs e)
        {

            // deselect previous
            if (previousGroup != null)
                previousGroup.Selected = false;

            Frame.Navigate(typeof(ManageGroup), this.selectedGroup);
        }

        private IMobileServiceTable<DataModels.Path> pathTable = App.mobileClient.GetTable<DataModels.Path>();

        private async void predictAppBarGroup_Click(object sender, RoutedEventArgs e)
        {
            // group prediction

            // fetch start and destination points
            Geopoint groupStartPoint = GeopointSerializer.ByteArrayToObject(this.selectedGroup.SourcePoint);
            Geopoint groupDestPoint = GeopointSerializer.ByteArrayToObject(this.selectedGroup.DestinationPoint);

            // fetch routes from cloud

            try
            {

                var paths = await pathTable.Where(item => item.UserId == App.userId).ToCollectionAsync<DataModels.Path>();
                var groupPaths = paths.Where(item => item.groupId == this.selectedGroup.Id).ToList();

                if(groupPaths.Count == 0)
                {
                    MessageDialog diag = new MessageDialog("Selected group is empty! Please add routes to the group first");
                    await diag.ShowAsync();
                    return;
                }

                var decodedPaths = PathGroup.toGeopathList(groupPaths);
                var names = PathGroup.toNameList(groupPaths);
                RoutePredictionArgs args = new RoutePredictionArgs(groupStartPoint,
                                                               groupDestPoint,
                                                               decodedPaths, this.routeKind, this.maxStallTime, this.numTimeSlots, names
                                                               );
                Frame.Navigate(typeof(RoutePredictions), args);

            }

            catch
            {

                // error occured
                MessageDialog diagg = new MessageDialog("Oops, connection with server was lost. Please try again later");
                diagg.ShowAsync();
            }

            

        }

       

    }


    class toColorBrushConverter : IValueConverter
    {

       

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Boolean selected = (bool)value;

            if (selected)
            {
                return new SolidColorBrush(Colors.Blue);
            }
            else
                return new SolidColorBrush(Colors.Black);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    class toTextSizeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Boolean selected = (bool)value;

            if (selected)
            {
                return 30;
            }
            else
                return 20;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 10;
        }
    }

}
