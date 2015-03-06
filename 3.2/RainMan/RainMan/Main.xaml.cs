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


//0.1           0,93, 243 ----> 0.07
//0.2           12, 190, 190 --> 0.72
//0.7           1, 171, 171 --> 0.72
//1.2           0, 134, 92 --> 1.35
//2             3, 155, 54 --> 2.94
//4             12, 177, 47 --> 3.84
//6             12, 202, 34 --> 4.54
//9             27, 217, 33 --> 5.16
//13            55, 234, 55 --> 5.39
//18            240, 244, 89 --> 19.52
//24            246, 218, 56 --> 23.31
//30            235, 182, 52 --> 26.43
//40            230, 143, 40 --> 30.2
           

            //var x = ColorTranslator.RBG_to_power(0, 93, 243); 

            //x = ColorTranslator.RBG_to_power(12, 190, 190); 

            //x = ColorTranslator.RBG_to_power(1, 171, 171); 

            //x = ColorTranslator.RBG_to_power(0, 134, 92); 

            //x = ColorTranslator.RBG_to_power(3, 155, 54); 

            //x = ColorTranslator.RBG_to_power(12, 177, 47); 

            //x = ColorTranslator.RBG_to_power(12, 202, 34); 

            //x = ColorTranslator.RBG_to_power(27, 217, 33); 

            //x = ColorTranslator.RBG_to_power(55, 234, 55); 

            //x = ColorTranslator.RBG_to_power(240, 244, 89); 

            //x = ColorTranslator.RBG_to_power(246, 218, 56); 

            //x = ColorTranslator.RBG_to_power(235, 182, 52); 

            //x = ColorTranslator.RBG_to_power(230, 143, 40); 

            ////  x = ColorTranslator.RBG_to_power(255, 220, 2); // should be 24 really is 23.9

            //// x = ColorTranslator.RBG_to_power(255, 182, 2); // should be 30 really is 27.09

            if(Frame.BackStackDepth > 0)
            {
                Frame.BackStack.Clear();
            }

            

            ModalWindow temporaryDialog = new ModalWindow();
            ContentDialog diag = temporaryDialog.Dialog;
            Boolean error = false;

            diag.ShowAsync();

           

           try
           {
                    // create radar map manager
                    this.mapManager = await RadarMapManager.getRadarMapManager();
            }

           catch
           {

                error = true;
                   
           }

           if(error)
           {
                diag.Hide();
                    // handle error


            }

            if(e.NavigationParameter != null)
            {
                var icons = e.NavigationParameter as PredictionCollection;
                this.defaultViewModel["IconCollection"] = icons;
                this.defaultViewModel["Selection"] = icons.PredictionIcons.ElementAt(0);
                this.waterRec.Height = RainToHeight.rainToHeight(icons.PredictionIcons.ElementAt(0).Avg);
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
