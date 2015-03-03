using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RainMan.Tasks
{
    public class PredictionLoadingScreen
    {


        //private ContentDialog Dialog { get; set; }
        private ContentDialog Dialog { get; set; }
        public List<String> pathNames { get; set; }
        private int currentStepNum;

        private Boolean shown = false;

        public Boolean Shown
        {
            get
            {
                return this.shown;
            }
        }

        // current step in calculation
        // steps are defined as follows:
        // 1: Calculating routes from points
        // 2: Calculating predictions for each route
        // 3: drawing results on the map
        public int CurrentStepNum
        {
            get
            {
                return this.currentStepNum;
            } 
            set
            {
                StepTextBlock.Text = string.Format("Step {0}", value);
                currentStepNum = value;
                if(value == 1)
                {
                    // downloading routes
                    this.ProgressBar.Value = 5;
                    this.StepDescription = "Computing routes from given waypoints ...";
                    return;
                }
    
                if(value == 3)
                {
                    // done with calculating prediction
                    this.StepDescription = "Rendering results on the map ...";
                    this.ProgressBar.Value = 80;

                    return;
                    
                }
                if(value == 4)
                {
                    this.StepTextBlock.Visibility = Visibility.Collapsed;
                    this.StepDescription = "Done!";
                    this.ProgressBar.Value = 100;
                }
            }
       
        }

        public void updatePredictionProgress(int donePaths, int totalPaths)
        {
            this.CurrentStepNum = 2;
            double totalValueForStep2 = 60;
            this.ProgressBar.Value = totalValueForStep2 * (donePaths / (double)totalPaths) + 20;
            if (pathNames != null)
                this.StepDescription = string.Format("Calculating prediction for '{0}' [{1}/{2}]...", pathNames.ElementAt(donePaths), donePaths+1, totalPaths);
            else
                this.stepDescription = "Calculating prediction ...";
        }

        // text describing the current step
        private string stepDescription;

        public string StepDescription
        {
            get
            {
                return this.stepDescription;
            }
            set
            {
                this.stepDescription = value;
                this.DescriptionTextBlock.Text = value;
            }
        }

       // private Boolean shown = false;

        public async Task Show()
        {
            if (!shown)
            {
                shown = true;
                await this.Dialog.ShowAsync();
                
            }
            
        }

        public async Task WaitHide()
        {
            this.CurrentStepNum = 4;
            await Task.Delay(TimeSpan.FromSeconds(1.1));
            Hide();
        }

        public void Hide()
        {
            if(shown)
            {
            this.Dialog.Hide();
                shown = false;
            }
        }

        // text block that holds the step number and description
        private TextBlock StepTextBlock { get; set; }

        private TextBlock DescriptionTextBlock { get; set; }

        private ProgressBar ProgressBar { get; set; }

        public Button GoBackButton {get; set;}


        public void ShowError(string errorMessage)
        {
             this.StepTextBlock.Visibility = Visibility.Collapsed;
             this.StepDescription = errorMessage;
             this.ProgressBar.Visibility = Visibility.Collapsed;
             this.GoBackButton.Visibility = Visibility.Visible;

        }

        public PredictionLoadingScreen(List<string> pathNames, Grid layoutGrid)
       
        {

            this.pathNames = pathNames;

            //Progress Bar
            ProgressBar bar = new ProgressBar();
            bar.IsIndeterminate = false;
            bar.Height = 15;

            this.ProgressBar = bar;
            

            //Downloading Data text
            TextBlock txt = new TextBlock();
            txt.Text = "Please wait while we prepare the results";
            txt.FontSize = 17;
            txt.Foreground = new SolidColorBrush(Colors.Black);
            txt.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //This could take a few seconds
            TextBlock txt2 = new TextBlock();
            txt2.Text = "This could take a few seconds.";
            txt2.FontSize = 17;
            txt2.Foreground = new SolidColorBrush(Colors.Black);
            txt2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //Please do not close the application.
            TextBlock txt3 = new TextBlock();
            txt3.Text = "Please do not close the application.";
            txt3.FontSize = 17;
            txt3.Foreground = new SolidColorBrush(Colors.Black);
            txt3.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            


            // create progress grid
            Grid progressGrid = new Grid();
            
           // ColumnDefinition column = new ColumnDefinition();
           // column.Width = new GridLength(1, GridUnitType.Star);
            //progressGrid.ColumnDefinitions.Add(column);

            //column = new ColumnDefinition();
           // column.Width = new GridLength(3, GridUnitType.Star);
            //progressGrid.ColumnDefinitions.Add(column);
            progressGrid.HorizontalAlignment = HorizontalAlignment.Left;
            progressGrid.Margin = new Thickness(20, 25, 0, 10);

            progressGrid.RowDefinitions.Add(new RowDefinition());
            progressGrid.RowDefinitions.Add(new RowDefinition());

            // Grid 'step' content
            // current step textblock
            TextBlock txt4 = new TextBlock();
            txt4.Text = string.Format("Step {0}:", this.CurrentStepNum);
            txt4.FontSize = 18;
            txt4.Foreground = new SolidColorBrush(Colors.Black);
            txt4.FontWeight = FontWeights.Bold;
            txt4.SetValue(Grid.ColumnProperty, 0);

            progressGrid.Children.Add(txt4);
            this.StepTextBlock = txt4;

            // description textblock
            txt4 = new TextBlock();
            txt4.Text = "Doing something !";
            txt4.FontSize = 15;
            txt4.Foreground = new SolidColorBrush(Colors.Black);
            txt4.TextTrimming = TextTrimming.CharacterEllipsis;
            //txt4.SetValue(Grid.ColumnProperty, 1);
            txt4.SetValue(Grid.RowProperty, 1);
            txt4.HorizontalAlignment = HorizontalAlignment.Center;
            txt4.Margin = new Thickness(10, 10, 0, 0);
            progressGrid.Children.Add(txt4);
            this.DescriptionTextBlock = txt4;

            var button = new Button();
            this.GoBackButton = button;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.Margin = new Thickness(0, 20, 0, 0);
            button.Content = "Go back";
            button.Visibility = Visibility.Collapsed;
            button.Foreground = new SolidColorBrush(Colors.Black);
            button.BorderBrush = new SolidColorBrush(Colors.Black);


            StackPanel stk = new StackPanel();

            //stk.Background = new SolidColorBrush(Colors.White) { Opacity = 1 };
            stk.Children.Add(txt);
            stk.Children.Add(progressGrid);
            stk.Children.Add(bar);
            stk.Children.Add(button);
            //stk.Margin = new Thickness(0, 250, 0, 0);
           // stk.SetValue(Grid.RowProperty, 1);
            //stk.Children.Add(txt2);
            //stk.Children.Add(txt3);


           // Grid backGroundGrid = new Grid();
           // backGroundGrid.RowDefinitions.Add(new RowDefinition()); backGroundGrid.RowDefinitions.Add(new RowDefinition()); backGroundGrid.RowDefinitions.Add(new RowDefinition());
           // backGroundGrid.Children.Add(stk);
           // backGroundGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
           // backGroundGrid.VerticalAlignment = VerticalAlignment.Stretch;
           // backGroundGrid.Background = new SolidColorBrush(Colors.Black) { Opacity = 0.8 };
           // backGroundGrid.Visibility = Visibility.Collapsed;

            
            ContentDialog dlg = new ContentDialog();
           
            dlg.Content = stk;
            SolidColorBrush color = new SolidColorBrush(Colors.White);
            color.Opacity = 1;
            dlg.Background = color;
            dlg.Margin = new Thickness(0, 250, 0, 0);

            this.Dialog = dlg;
           // this.Dialog = backGroundGrid;
            this.CurrentStepNum = 1;

           // backGroundGrid.SetValue(Grid.RowProperty, 0);
           // backGroundGrid.SetValue(Grid.ColumnProperty, 0);
           //// backGroundGrid.SetValue(Grid.RowSpanProperty, 2);
            //backGroundGrid.SetValue(Grid.ColumnSpanProperty, 3);
            //layoutGrid.Children.Add(this.Dialog);

        }
    }
}
