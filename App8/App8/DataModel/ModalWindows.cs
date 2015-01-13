using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace App8.DataModel
{
    public class ModalWindow
    {

        public ContentDialog Dialog { get; set; }

        public ModalWindow()
        {


            //Progress Bar
            ProgressBar bar = new ProgressBar();
            bar.IsIndeterminate = true;



            //Downloading Data text
            TextBlock txt = new TextBlock();
            txt.Text = "Downloading data...";
            txt.FontSize = 17;
            txt.Foreground = new SolidColorBrush(Colors.White);
            txt.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //This could take a few seconds
            TextBlock txt2 = new TextBlock();
            txt2.Text = "This could take a few seconds.";
            txt2.FontSize = 17;
            txt2.Foreground = new SolidColorBrush(Colors.White);
            txt2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //Please do not close the application.
            TextBlock txt3 = new TextBlock();
            txt3.Text = "Please do not close the application.";
            txt3.FontSize = 17;
            txt3.Foreground = new SolidColorBrush(Colors.White);
            txt3.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            StackPanel stk = new StackPanel();
            stk.Children.Add(bar);
            stk.Children.Add(txt);
            stk.Children.Add(txt2);
            stk.Children.Add(txt3);


            ContentDialog dlg = new ContentDialog();
            dlg.Content = stk;
            SolidColorBrush color = new SolidColorBrush(Colors.Black);
            color.Opacity = 0.7;
            dlg.Background = color;
            dlg.Margin = new Thickness(0, 250, 0, 0);

            this.Dialog = dlg;

        }
        public ModalWindow(String firstLine, String secLine, String thirdLine)
        {
            //Progress Bar
            ProgressBar bar = new ProgressBar();
            bar.IsIndeterminate = true;

            //Downloading Data text
            TextBlock txt = new TextBlock();
            txt.Text = firstLine;
            txt.FontSize = 17;
            txt.Foreground = new SolidColorBrush(Colors.White);
            txt.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //This could take a few seconds
            TextBlock txt2 = new TextBlock();
            txt2.Text = secLine;
            txt2.FontSize = 17;
            txt2.Foreground = new SolidColorBrush(Colors.White);
            txt2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //Please do not close the application.
            TextBlock txt3 = new TextBlock();
            txt3.Text = thirdLine;
            txt3.FontSize = 17;
            txt3.Foreground = new SolidColorBrush(Colors.White);
            txt3.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            StackPanel stk = new StackPanel();
            
            stk.Children.Add(txt);
            stk.Children.Add(txt2);
            stk.Children.Add(txt3);
            stk.Children.Add(bar);

            ContentDialog dlg = new ContentDialog();
            dlg.Content = stk;
            SolidColorBrush color = new SolidColorBrush(Colors.Black);
            color.Opacity = 0.7;
            dlg.Background = color;
            dlg.Margin = new Thickness(0, 250, 0, 0);

            this.Dialog = dlg;
        }
    }
}
