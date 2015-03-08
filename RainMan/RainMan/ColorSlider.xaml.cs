using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ColorSlider : UserControl
    {
        public ColorSlider()
        {
            this.InitializeComponent();

            // adjust height and width
           // double actualImageHeight = this.imageGrid.Height;
           // double actualImageWidth = this.imageGrid.Width;

           // double ratio = 8 / baseWidth;
            //this.Slider.Width = ratio * actualImageWidth;
           // ratio = 28 / baseHeight;
           // this.Slider.Height = ratio * actualImageHeight;

            // set vertical offset
            //ratio = 9 / baseHeight;
            this.Slider.Margin = new Thickness() { Top = 9, Bottom = 0, Right = 0 , Left = 0};

        }

        private double baseWidth = 271;
        private double baseHeight = 28;


        public double RainAvg
        {

            set
            {
                double newWidth = getUpdatedWidth(value);
                double currentWidth = this.mover.Width;

                moveSliderAnimation.From = currentWidth;
                moveSliderAnimation.To = newWidth;

                if (!playing)
                {
                    // play storyboard
                    playing = true;
                    moveSlider.Begin();
                } 
                else
                {
                    moveSlider.Stop();
                    moveSlider.Begin();
                }
            }

        }

        private Boolean playing = false;

        public double Heightt
        {

            set { this.imageGrid.Height = value; }

        }

        public double Widthh
        {
            set { this.imageGrid.Width = value; }
        }

        private double getUpdatedWidth(double average)
        {

            double actualImageHeight = this.imageGrid.Height;
            double actualImageWidth = this.imageGrid.Width;
        
            double ratio = 0;

            if (average <= 0.1) //248
            {
                ratio = 248 / baseWidth;
                
            }
            else if (average <= 0.2) // 236
            {
                ratio = 236 / baseWidth;
            }
            else if (average <= 0.7) // 222
            {
                ratio = 228 / baseWidth;
            }
            else if (average <= 0.9) // 212
            {
                ratio = 212 / baseWidth;
            }
            else if (average <= 1.2) // 203
            {
                ratio = 203 / baseWidth;
            }
            else if (average <= 2.0)  // 191
            {
                ratio = 191 / baseWidth;
            }
            else if (average <= 4.0) // 179
            {
                ratio = 179 / baseWidth;
            }
            else if (average <= 6.0)// 169
            {
                ratio = 169 / baseWidth;
            }
            else if (average <= 9.0) // 161
            {
                ratio = 161 / baseWidth;
            }
            else if (average <= 13) // 147
            {
                ratio = 147 / baseWidth;
            }
            else if (average <= 22) // 123
            {
                ratio = 123 / baseWidth;
            }
            else if (average <= 26) // 107
            {
                ratio = 107 / baseWidth;
            }
            else if (average <= 30) // 97
            {
                ratio = 97 / baseWidth;
            }
            else if (average <= 40) // 82
            {
                ratio = 82 / baseWidth;
            }
            else if (average <= 60) // 61
            {
                ratio = 61 / baseWidth;
            }
            else if (average <= 100) // 44
            {
               ratio = 44 / baseWidth;
            }
            else if (average <= 150) // 33
            {
                ratio = 33 / baseWidth;
            }
            else
            {
                // 16
                ratio = 16 / baseWidth;
            }

            return ratio * actualImageWidth;
  
        }

        private void moveSlider_Completed(object sender, object e)
        {
            playing = false;
        }
    }
}
