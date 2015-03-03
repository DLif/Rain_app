using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RainMan
{
    public sealed partial class LoadingDialog : UserControl
    {

        // list of path names, null in case of quick route
        private List<String> pathNames;

        public List<String> PathNames
        {
            get
            {
                return pathNames;
            }
            set
            {
                this.pathNames = value;
            }
        }

      
        public RoutedEventHandler OnClick
        {
           
            set 
            {
                this.goBackButton.Click += value;
            }
        }

        // current step number
        // current step in calculation
        // steps are defined as follows:
        // 1: Calculating routes from points
        // 2: Calculating predictions for each route
        // 3: drawing results on the map
        private int currentStepNum;

       

        public LoadingDialog()
        {
            this.InitializeComponent();
 
        }
        
       
        

        public void updatePredictionProgress(int donePaths, int totalPaths)
        {
            this.CurrentStepNum = 2;
            double totalValueForStep2 = 60;
            this.progressBar.Value = totalValueForStep2 * (donePaths / (double)totalPaths) + 20;
            if (pathNames != null)
                this.descriptionTextBlock.Text = string.Format("Calculating prediction for '{0}' [{1}/{2}]...", pathNames.ElementAt(donePaths), donePaths + 1, totalPaths);
            else
                this.descriptionTextBlock.Text = "Calculating prediction ...";
        }

        public int CurrentStepNum
        {
            get
            {
                return this.currentStepNum;
            }
            set
            {
                stepTextBlock.Text = string.Format("Step {0}", value);
                currentStepNum = value;
                if (value == 1)
                {
                    // downloading routes
                    this.progressBar.Value = 5;
                    this.descriptionTextBlock.Text = "Computing routes from given waypoints ...";
                    return;
                }

                if (value == 3)
                {
                    // done with calculating prediction
                    this.descriptionTextBlock.Text = "Rendering results on the map ...";
                    this.progressBar.Value = 80;

                    return;

                }
                if (value == 4)
                {
                    this.stepTextBlock.Visibility = Visibility.Collapsed;
                    this.descriptionTextBlock.Text = "Done!";
                    this.progressBar.Value = 100;
                }
            }

        }

        public void Show()
        {
            
                control.Visibility = Visibility.Visible;

         

        }

        // show done message, wait a little and close the window
        public async Task WaitHide()
        {
            this.CurrentStepNum = 4;
            await Task.Delay(TimeSpan.FromSeconds(1.1));
            Hide();
        }

        // simply hide the window
        public void Hide()
        {
            
            
                control.Visibility = Visibility.Collapsed;
               
            
        }

        // set error message and show error panel
        public void ShowError(string errorMessage)
        {
            this.stepTextBlock.Visibility = Visibility.Collapsed;
            this.descriptionTextBlock.Text = errorMessage;
            this.progressBar.Visibility = Visibility.Collapsed;
            this.goBackButton.Visibility = Visibility.Visible;

        }

       

        

    }
}
