using App8.Common;
using App8.Data;
using App8.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace App8
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

        private Visibility mapVisibility;

        public Visibility MapVisibility
        {
            get
            {
                return this.mapVisibility;
            }


            set
            {

                this.mapVisibility = value;
                NotifyPropertyChanged("MapVisibility");

            }
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string travelMethod;
        
        public String TravelMethod
        {
            get
            {
                return this.travelMethod;
            }
            set
            {
                this.travelMethod = value;
                NotifyPropertyChanged("TravelMethod");

            }
        }

        public ObservableCollection<MapPin> Points { get; set; }

        public QuickRouteOptions()
        {
            this.Points = new ObservableCollection<MapPin>();
        }

        public void setPointCollection(Geopoint start, Geopoint end)
        {
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
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private RadarMapManager mapManager;
       
        private QuickRouteOptions quickRouteHubOptions = new QuickRouteOptions();
        private int maxStallTime = 30;
        private IMobileServiceTable<PathGroup> groupsTable = App.mobileClient.GetTable<PathGroup>();


        public HubPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

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
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;

            if (App.user == null)
            {
                this.defaultViewModel["PathGroups"] = new List<PathGroup>();
            }
            else
            {
                this.defaultViewModel["PathGroups"] = await this.groupsTable.ToCollectionAsync<PathGroup>();
            }

            this.mapManager = RadarMapManager.getRadarMapManager();
            if (this.mapManager.NeedToUpdate())
            {

                // download new radar maps, update prediction icons

                ModalWindow temporaryDialog = new ModalWindow();
     
                ContentDialog diag = temporaryDialog.Dialog;
                diag.ShowAsync();

                try
                {
                    await mapManager.updateRadarMaps(false);
                }
                catch
                {

                    diag.Hide();
                    MessageDialog errorDialog = new MessageDialog("could not connect to server, try again later", "Oops");
                    errorDialog.ShowAsync();
                    return;
                }


                var icons = await PredictionIconDataSource.getData(this.mapManager);

                //when done, close the dialog
                diag.Hide();
                this.defaultViewModel["IconCollection"] = icons;

            }

  
            quickRouteHubOptions.MapVisibility = Visibility.Collapsed;
            quickRouteHubOptions.TravelMethod = "Not set";
               
            this.defaultViewModel["QuickRouteOptions"] = quickRouteHubOptions;
           
            if (e.NavigationParameter is GroupBuilderNavigator)
            {
                GroupBuilderNavigator arg = e.NavigationParameter as GroupBuilderNavigator;
                if (!arg.IsUserGroup)
                {
                    // argument for quick routing
                    quickRouteHubOptions.setPointCollection(arg.StartLocation, arg.EndLocation);
                    quickRouteHubOptions.MapVisibility = Visibility.Visible;

                    // enable prediction button
                    appRoutePrediction.Visibility = Visibility.Visible;

                  
                }
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
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
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
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
  
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

            if (Hub.SectionsInView.ElementAt(0) == predictionHub)
            {
                Frame.Navigate(typeof(RadarMapPage));
            }
            else if(Hub.SectionsInView.ElementAt(0) == HubQuickRoute)
            {
                // no need to create a name for the group
                GroupBuilderNavigator groupBuilderArg = new GroupBuilderNavigator(false);
                Frame.Navigate(typeof(GroupBuilder), groupBuilderArg);
            }
        }


        private MapControl quickRouteMapControl = null;

        private async void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            quickRouteMapControl = (MapControl)sender;

            quickRouteMapControl.Center = RadarMapManager.center;
            quickRouteMapControl.ZoomLevel = 10D;

            if(quickRouteHubOptions.isMapSet())
            {
               
               // get start and end poin
             List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();
             basicPositions.Add(quickRouteHubOptions.Points.ElementAt(0).Location.Position);
             basicPositions.Add(quickRouteHubOptions.Points.ElementAt(1).Location.Position);
                

                try
                {

                    bool result = await quickRouteMapControl.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(basicPositions), null, MapAnimationKind.None);



                    if (!result)
                    {
                        MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                        errorDialog.ShowAsync();
                    }
                    else
                    {
                        quickRouteMapControl.ZoomLevel -= 0.5;
                    }
                }
                catch
                {

                    MessageDialog errorDialog = new MessageDialog("Sorry, something went wrong with the mapping service!", "Oops");
                    errorDialog.ShowAsync();

                }
   
            }
        

        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            await AuthenticateAsync();

            if(App.user != null)
            {

                ModalWindow loggedIn = new ModalWindow("Successfully logged in!", "Loading your data from the cloud ...", "");
                loggedIn.Dialog.ShowAsync();

                // user has logged in
                loginButton.Visibility = Visibility.Collapsed;
                var userGroups = await groupsTable.ToCollectionAsync<PathGroup>();
                this.defaultViewModel["PathGroups"] = userGroups;
                this.addGroup.Visibility = Visibility.Visible;

                loggedIn.Dialog.Hide();

            }


        }

        private async System.Threading.Tasks.Task AuthenticateAsync()
        {
            while (App.user == null)
            {
                string message = "";
                try
                {
                    App.user = await App.mobileClient
                        .LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                    
                       
                }
                catch (InvalidOperationException)
                {
                    message = "Login failed, try again later";
                }
                catch
                {
                    message = "Something went wrong";
                }

                if (message != "") { 
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
                 }
            }
        }

        private async void Hub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
           if(Hub.SectionsInView[0] == HubQuickRoute)
           {
               locationChangeButton.Visibility = Visibility.Collapsed;
               mapAppButton.Visibility = Visibility.Visible;
               mapAppButton.Label = "Map View";
               if (this.quickRouteMapControl.Visibility == Visibility.Visible)
               {
                   appRoutePrediction.Visibility = Visibility.Visible;
               }
               addGroup.Visibility = Visibility.Collapsed;


               return;
           }

           if(Hub.SectionsInView[0] == predictionHub)
           {
               locationChangeButton.Visibility = Visibility.Visible;
               mapAppButton.Visibility = Visibility.Visible;
               mapAppButton.Label = "Radar map";
               appRoutePrediction.Visibility = Visibility.Collapsed;
               addGroup.Visibility = Visibility.Collapsed;
               return;
           }

            if(Hub.SectionsInView[0] == Groups)
            {
                locationChangeButton.Visibility = Visibility.Collapsed;
                mapAppButton.Visibility = Visibility.Collapsed;
                appRoutePrediction.Visibility = Visibility.Collapsed;
                addGroup.Visibility = Visibility.Collapsed;

                if (App.user == null && firstMessage)
                {
                    MessageDialog msg = new MessageDialog("You must sign in to see this page", "Sign in");
                    
                    await msg.ShowAsync();
                    firstMessage = false;
                }
                else
                {
                    addGroup.Visibility = Visibility.Visible; 
                }



            }
           
            
        }


        private Boolean firstMessage = true;
        private void transportImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void flyoutGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            grid.Margin = new Thickness(0, this.ActualHeight - 250, 0, 0);
        }

        private void walkFlyout_Click(object sender, RoutedEventArgs e)

        {
            
            this.transportImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons/walking.png"));
        }

        private ImageBrush transportImageBrush = null;

        private void ImageBrush_ImageOpened(object sender, RoutedEventArgs e)
        {
            this.transportImageBrush = (ImageBrush)sender;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            

            this.transportImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons/bike.png"));
        }

        private void carFlyout_Click(object sender, RoutedEventArgs e)
        {
            
            this.transportImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons/car.png"));
        }

        private void MapOverlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            
        }

        private Button transportButton = null;
        private void transportButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            transportButton = (Button)sender;
        }

        private void time0_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 0;
            timeButton.Content = String.Format("Maximum Stall: {0} minutes", 0);
        }

        private void time10_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 10;
            timeButton.Content = String.Format("Maximum Stall: {0} minutes", 10);
        }

        private void time20_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 20;
            timeButton.Content = String.Format("Maximum Stall: {0} minutes", 20);
        }

        private void time30_Click(object sender, RoutedEventArgs e)
        {
            this.maxStallTime = 30;
            timeButton.Content = String.Format("Maximum Stall: {0} minutes", 30);
        }

        private Button timeButton;
        private void timeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            timeButton = (Button)sender;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void addGroup_Click(object sender, RoutedEventArgs e)
        {
             
            GroupBuilderNavigator groupBuilderArg = new GroupBuilderNavigator(true);
            Frame.Navigate(typeof(GroupBuilder), groupBuilderArg);
            
        }

       

        

       

        
       




       
    }
}
