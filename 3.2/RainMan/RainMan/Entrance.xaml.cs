using RainMan.Common;
using RainMan.Tasks;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Entrance : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Entrance()
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

            RadarMapManager manager = null;
            int numTries = 2;
            Boolean error = false;
            for (int i = 0; i < numTries; ++i)
            {



                try
                {
                    // just update the radar manager
                    manager = await RadarMapManager.getRadarMapManager();
                    
                    if(manager.error)
                    {
                        error = true;
                    }
                }
                catch
                {

                    error = true;
                }

                if(error && i == 0)
                {
                    // first try, wait a second before trying again
                    await Task.Delay(1000);
                    error = false;
                }
                else if (!error)
                {
                    break;
                }
      
            }

            
            // error on second try aswel
            if (error)
            {
                this.Progress.IsActive = false;

                this.fadeOutText.Begin();

                
            }
            else
            {

                // calculate the prediction icon set aswell
                var screenBounds = Window.Current.Bounds;
                var heightResizeFactor = 130.0 / 666.666;
                var widthResizeFactor = 170.0 / 400;
                try
                {

                    var icons = await PredictionIconDataSource.getData(manager);
                    foreach (var icon in icons.PredictionIcons)
                    {
                        icon.ItemHeight = heightResizeFactor * screenBounds.Height;
                        icon.ItemWidth = widthResizeFactor * screenBounds.Width;
                    }

                    Frame.Navigate(typeof(Main), icons);
                }
                catch
                {
                    error = true;
                }

                if(error)
                {
                    this.Progress.IsActive = false;

                    this.fadeOutText.Begin();
                    return;

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

        private void fadeOutText_Completed(object sender, object e)
        {
            title.Visibility = Visibility.Collapsed;
            this.Error.Text = "It seems our services are temporary unavailable, please try again later. Also, please make sure you are connected to the internet and that your system clock is correct";
            this.fadeInText.Begin();
        }
    }
}
