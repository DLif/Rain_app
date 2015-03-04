using RainMan.Common;
using RainMan.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RainMan.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageGroup : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private PathGroup group = null;
        private IMobileServiceTable<PathGroup> groupsTable = App.mobileClient.GetTable<PathGroup>();
        private IMobileServiceTable<DataModels.Path> pathTable = App.mobileClient.GetTable<DataModels.Path>();

        public ManageGroup()
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

            if(Frame.BackStack.ElementAt(Frame.BackStackDepth-1).GetType() == typeof(RouteBuilder))
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }


            this.appBarDeletePath.Visibility = Visibility.Collapsed;
            group = e.NavigationParameter as PathGroup;
            this.GeneralInformationGrid.Visibility = Visibility.Collapsed;
            this.defaultViewModel["Group"] = group;
            this.defaultViewModel["GroupName"] = group.GroupName;
            await refreshPaths();


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

        private void appBarDetails_Click(object sender, RoutedEventArgs e)
        {
            this.BottomAppBar.Visibility = Visibility.Collapsed;
            this.fadeInStory.Begin();
        }

        

     
        private async void appBarDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow win = new ModalWindow(String.Format("Removing {0} ...", this.selectedPath.PathName), "Please wait ... ", "");
            win.Dialog.ShowAsync();

            Boolean error = false;

            try
            {

                await this.pathTable.DeleteAsync(selectedPath);
                this.selectedPath = null;
                this.appBarDeletePath.Visibility = Visibility.Collapsed;
            }
            catch
            {
                error = true;
            }

            if(error)
            {
                win.Dialog.Hide();
                MessageDialog diag = new MessageDialog("Oops, check your internet connection!");
                await diag.ShowAsync();
            }
            else
            {
                await this.refreshPaths();
                win.Dialog.Hide();
            }

                
            
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.groupName.Text == "" || this.startName.Text == "" || this.endName.Text == "")
            {
                MessageDialog diag = new MessageDialog("One of the fields is empty, please fill the information");
                await diag.ShowAsync();
                return;
            }

            group.GroupName = this.groupName.Text;
            group.StartName = this.startName.Text;
            group.FinishName = this.endName.Text;

            string message = null;

            try
            {
                await groupsTable.UpdateAsync(group);
                message = "Changes were succesfully saved!"; 
          
            }
            catch
            {
                message = "Oops, server is down! try again later";
                
            }

            this.fadeOutStory.Begin();
            
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();

        }


        private async Task refreshPaths()
        {

            errorText.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Visible;
            errorText.Text = "Downloading paths ...";

            try
            {

                var paths = await pathTable.Where(item => item.UserId == App.userId).ToCollectionAsync<DataModels.Path>();
                var groupPaths = paths.Where(item => item.groupId == group.Id).ToList();

                // set my paths
                this.defaultViewModel["Paths"] = groupPaths;

                if(groupPaths.Count == 0)
                {
                    errorText.Text = "Group is empty";
                }
                else
                {
                    errorText.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                errorText.Text = "Oops, could not retrieve paths. Please check your internet connection";
            }

            progress.Visibility = Visibility.Collapsed;
           

        }

        private void addPathButton_Click(object sender, RoutedEventArgs e)
        {

            this.GeneralInformationGrid.Visibility = Visibility.Collapsed;

            Frame.Navigate(typeof(RouteBuilder), this.group);

        }

        private void pathList_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private DataModels.Path selectedPath = null;

        private async void pathList_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.selectedPath = e.ClickedItem as DataModels.Path;
            this.appBarDeletePath.Visibility = Visibility.Visible;

            List<Geopoint> wayPoints = PathSerializer.ByteArrayToObject(selectedPath.PathClass);
            wayPoints.RemoveAt(0);
            wayPoints.RemoveAt(wayPoints.Count - 1);

            // remove start and destination
            for(int i = 2; i < this.map.Children.Count; ++i)
            {
                this.map.Children.RemoveAt(i);
            }
            List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();
            int counter = 1;
            foreach(Geopoint wayPoint in wayPoints)
            {
                DependencyObject pin = MapUtils.getMapPin(wayPoint, Colors.Blue, counter.ToString());
                MapUtils.addPinToMap(map, pin, wayPoint);
                basicPositions.Add(wayPoint.Position);
                ++counter;
            }

            
            basicPositions.Add(startPoint.Position);
            basicPositions.Add(endPoint.Position);

            // try to set view
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

        private async void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
                ModalWindow modalWindow = new ModalWindow("Group is being deleted", "Please wait ... ", "");
                modalWindow.Dialog.ShowAsync();
                Boolean error = false;
                try
                {
                    await this.groupsTable.DeleteAsync(this.group);

                    for(int i = 2; i < this.map.Children.Count; ++i)
                    {
                        map.Children.RemoveAt(i);
                    }


                }

                catch
                {
                    error = true;
                }



                modalWindow.Dialog.Hide();

                if(error)
                {
                    MessageDialog diag = new MessageDialog("Sorry, your internet connection seems to be down, try again later");
                    await diag.ShowAsync();
                }
                else
                    Frame.GoBack();
        }

        private void fadeOutStory_Completed(object sender, object e)
        {
            GeneralInformationGrid.Visibility = Visibility.Collapsed;
            this.BottomAppBar.Visibility = Visibility.Visible;
        }

        private DependencyObject startPin;
        private Geopoint startPoint;
        private Geopoint endPoint;
        private DependencyObject endPin;
       

        private async void map_Loaded(object sender, RoutedEventArgs e)
        {
            // get start and end points
            List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();
            startPoint = GeopointSerializer.ByteArrayToObject(this.group.SourcePoint);
            endPoint = GeopointSerializer.ByteArrayToObject(this.group.DestinationPoint);
            basicPositions.Add(startPoint.Position);
            basicPositions.Add(endPoint.Position);

            try
            {

                // add the map icons
               startPin = MapUtils.getMapPin(startPoint, Colors.Red, "Start");
               endPin = MapUtils.getMapPin(endPoint, Colors.Red, "End");
                MapUtils.addPinToMap(map, startPin, startPoint);
                MapUtils.addPinToMap(map, endPin, endPoint);

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

    
    }
}
