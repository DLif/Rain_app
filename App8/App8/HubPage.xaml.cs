using App8.Common;
using App8.Data;
using App8.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace App8
{




    public class QuickRouteOptions
    {

        public Visibility LoggedIn { get; set; }

        public Visibility MapVisibility { get; set; }
        

        public String TravelMethod { get; set; }
    }

    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private RadarMapManager mapManager; 


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


            ModalWindow temporaryDialog = new ModalWindow();
            this.mapManager = RadarMapManager.getRadarMapManager();

            // get icons, will be awaited
  
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


            QuickRouteOptions quickRouteHubOptions = new QuickRouteOptions();
            quickRouteHubOptions.LoggedIn = App.user != null ? Visibility.Visible : Visibility.Collapsed;
            quickRouteHubOptions.MapVisibility = Visibility.Collapsed ;
            quickRouteHubOptions.TravelMethod = "Not set";

            this.defaultViewModel["QuickRouteOptions"] = quickRouteHubOptions;

           

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
            Frame.Navigate(typeof(RadarMapPage));
        }


        private MapControl quickRouteMapControl = null;

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            quickRouteMapControl = (MapControl)sender;
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            await AuthenticateAsync();

            if(App.user != null)
            {
                // user has logged in
                loginButton.Visibility = Visibility.Collapsed;
            }


        }

        private async System.Threading.Tasks.Task AuthenticateAsync()
        {
            while (App.user == null)
            {
                string message;
                try
                {
                    App.user = await App.mobileClient
                        .LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                    message =
                        string.Format("You are now logged in - {0}", App.user.UserId);
                }
                catch (InvalidOperationException)
                {
                    message = "Login failed, try again later";
                }

                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }

        private void Hub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
           if(Hub.SectionsInView[0] == HubQuickRoute)
           {
               locationChangeButton.Visibility = Visibility.Collapsed;
               mapAppButton.Visibility = Visibility.Visible;
               mapAppButton.Label = "Map View";
               return;
           }

           if(Hub.SectionsInView[0] == predictionHub)
           {
               locationChangeButton.Visibility = Visibility.Visible;
               mapAppButton.Visibility = Visibility.Visible;
               mapAppButton.Label = "Radar map";
               return;
           }
           
            
        }

        /* first hub item icons */




       
    }
}
