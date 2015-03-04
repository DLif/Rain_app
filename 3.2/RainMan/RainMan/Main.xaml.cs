using RainMan.Common;
using RainMan.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Main()
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
        /// 

        private RadarMapManager mapManager;
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            if(Frame.BackStackDepth > 0)
            {
                Frame.BackStack.Clear();
            }

            // create radar map manager
            this.mapManager = RadarMapManager.getRadarMapManager();

            ModalWindow temporaryDialog = new ModalWindow();
            ContentDialog diag = temporaryDialog.Dialog;
            Boolean dialogShown = false;


            if (this.mapManager.NeedToUpdate())
            {

                // download new radar maps, update prediction icons
                diag.ShowAsync();
                dialogShown = true;

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

            }

            if (PredictionIconDataSource.NeedToUpdate)
            {
                //if(!dialogShown)
                //{
                //    diag.ShowAsync();
                //    dialogShown = true;
                //}

                var screenBounds = Window.Current.Bounds;
                var heightResizeFactor = 130.0 / 666.666;
                var widthResizeFactor = 170.0 / 400;

                var icons = await PredictionIconDataSource.getData(this.mapManager);
                foreach(var icon in icons.PredictionIcons)
                {
                    icon.ItemHeight = heightResizeFactor * screenBounds.Height;
                    icon.ItemWidth = widthResizeFactor * screenBounds.Width;
                }
                this.defaultViewModel["IconCollection"] = icons;

               
                //this.defaultViewModel["FirstItem"] = icons.PredictionIcons.ElementAt(0);
                //this.defaultViewModel["SecondItem"] = icons.PredictionIcons.ElementAt(0);
                //this.defaultViewModel["ThirdItem"] = icons.PredictionIcons.ElementAt(0);
                //this.defaultViewModel["FourthItem"] = icons.PredictionIcons.ElementAt(0);

                this.defaultViewModel["Selection"] = icons.PredictionIcons.ElementAt(0);
                this.waterRec.Height = RainToHeight.rainToHeight(icons.PredictionIcons.ElementAt(0).Avg);
                
            }

            var x = (PredictionCollection)this.defaultViewModel["IconCollection"];
            
         
            if (dialogShown)
                diag.Hide();



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


        private PredictionIcon nextSelection;
       

       

        private void CurrentGridFadeOut_Completed(object sender, object e)
        {

            waterRec.Height = 0;
            // update view
            this.defaultViewModel["Selection"] = nextSelection;
            // fire fade in animation
            CurrentGridShow.Begin();
        }

        private void CurrentGridShow_Completed(object sender, object e)
        {
            // fill water level
            FillWaterAnimation.To = RainToHeight.rainToHeight(nextSelection.Avg);
            FillWaterStory.Begin();    
            
        }

        private void predictionGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem;
            PredictionIcon selected = (PredictionIcon)item;
            this.nextSelection = selected;
            CurrentGridFadeOut.Begin();
        }

        
        private void mapAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RadarPage));
        }

        private void routesAppBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Routes));
        }

        private void rainAreas_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RainAmount));
        }

        private void commandBar_Opened(object sender, object e)
        {

        }

       
      
    }
}
