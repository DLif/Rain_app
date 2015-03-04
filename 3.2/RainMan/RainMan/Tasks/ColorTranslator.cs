﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace RainMan.Tasks
{
    class ColorTranslator
    {


        static int image_size_x = 512;
        static int image_size_y = 512;

        //HUE ranges 0-235,290-360
        //this array holds the intervals given in the site(they are very problematic)
        static double[] lowerPart_intervals = new double[15] { 50.0, 40.0, 30.0, 24.0, 18.0, 13.0, 9.0, 6.0, 4.0, 2.0, 1.2, 0.7, 0.2, 0.1, 0.0 };
        static double[] upperPart_intervals = new double[4] { 250.0, 200.0, 100.0, 50.0 };



        static int[][] pointsArray = new int[21][] { 
            new int[3] { 0, 0, 0 }, new int[3] { 6, 82, 244 },new int[3] { 0, 88, 243 }, new int[3] { 3, 195, 202 },new int[3] { 0, 181, 186 }, new int[3] { 1, 169, 175 }
            ,new int[3] { 0, 142, 128 }, new int[3] { 1, 134, 84 },new int[3] { 0, 154, 53 }, new int[3] { 0, 189, 39 },new int[3] { 2, 207, 25 }, new int[3] { 6, 231, 11 }
            ,new int[3] { 34, 249, 30 },new int[3] { 138, 255, 32 }, new int[3] { 230, 252, 33 },new int[3] { 255, 220, 2 } ,new int[3] {255, 182, 2} ,new int[3] {255, 131, 5} 
            ,new int[3] {246, 53,0} ,new int[3] { 194, 5, 42 }, new int[3] { 244, 6, 249 }
        };
        static double[] pointsPower = new double[21] { 0, 0.01, 0.1, 0.2, 0.45, 0.7, 0.95, 1.2, 2, 4, 6, 9, 13, 15.5, 18, 24, 30, 40, 50, 100, 200 };





      

        //This function calculates power sum for the given "picture arry" to the given x and y and radius
        //The array pixels shold be of the size 4 * width * height
        //The array pixels should be fed "ReadableImage.PixelBuffer.AsStream()" before using this function

        /*example use:
         * static int image_size_x = 512;
         * static int image_size_y = 512;
         * public double getAverageRain(Geopoint location, int pixelRadius)
         * {
         *  double power = 0;
         *  int locationPixel = PointTranslation.locationToPixel(location.Position.Latitude, location.Position.Longitude);
         *  int x_pixel = locationPixel % image_size_x;
         *  int y_pixel = (locationPixel - x_pixel) / image_size_x;
         *  
         *  using (var buffer = this.ReadableImage.PixelBuffer.AsStream())
         *  {
         *       Byte[] pixels = new Byte[4 * width * height];
         *       buffer.Read(pixels, 0, pixels.Length);
         *       power = ColorTranslator.power_to_radius(pixels,x_pixel,y_pixel,pixelRadius,this.ReadableImage.PixelWidth);
         *  }
         *  return power;
         * }
        */
        public static double power_to_radius(Byte[] pixels, int x_pixel, int y_pixel, int pixelRadius, int width)
        {
            double power_sum = 0;
            int num_pixels_in_radius = 0;
            //itterate on radius-run on x
            for (int x = x_pixel - pixelRadius; x <= x_pixel + pixelRadius; x++)
            {
                //if this pixels location out of bounds-don't try to add it to calculations
                if (x > image_size_x || x < 0) continue;

                //itterate on radius-run on y
                for (int y = y_pixel - pixelRadius; y <= y_pixel + pixelRadius; y++)
                {
                    //if this pixels location out of bounds-don't try to add it to calculations
                    if (y > image_size_y || y < 0) continue;

                    //index is the pixel's trio(RBG) location in the array
                    int index = ((y * width) + x) * 4;

                    Byte b = pixels[index + 0];
                    Byte g = pixels[index + 1];
                    Byte r = pixels[index + 2];

                    double power;
                    //for black pixel- give 
                    if (r == 0 && g == 0 && b == 0) power = 0;
                    //the case in whitch r==b==g is problematic for hue calculation
                    /*else if (r == b && b == g)
                    {
                        if (pixelRadius < 3) return power_to_radius(pixels, x_pixel, y_pixel, 3, width);
                        else continue;//if radius large enough, ignore point 
                    }*/
                    else power = ColorTranslator.RGB_array_power(r, g, b);

                    num_pixels_in_radius++;
                    power_sum += power;
                }
            }
            return power_sum / num_pixels_in_radius;
        }

        public static double RGB_array_power(int R, int G, int B)
        {
            int min_index = -1;
            int index = 0;
            double min_distance = Double.MaxValue;


            foreach (int[] test_point in pointsArray)
            {
                double cur_distance = distance_formula(R, G, B, test_point[0], test_point[1], test_point[2]);
                if (cur_distance < min_distance)
                {
                    min_distance = cur_distance;
                    min_index = index;
                }
                index++;
            }
            return pointsPower[min_index];
        }

        public static double distance_formula(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            return Math.Sqrt((double)((r2 - r1) * (r2 - r1) + (g2 - g1) * (g2 - g1) + (b2 - b1) * (b2 - b1)));
        }

        //main clac function
        public static double RBG_to_power(int R, int G, int B)
        {
            //init array in function, since c# doesn't allways initiallize it,even if this is global.
            lowerPart_intervals = new double[15] { 50.0, 40.0, 30.0, 24.0, 18.0, 13.0, 9.0, 6.0, 4.0, 2.0, 1.2, 0.7, 0.2, 0.1, 0.0 };
            upperPart_intervals = new double[4] { 250.0, 200.0, 100.0, 50.0 };
            double hue = RBG_to_HUE(R, G, B);
            if (hue > 235 && hue < 290)
            {//this hue should not exsist in the bar. fake nearest one
                if (hue <= 265) hue = 235;
                else hue = 290;
            }
            return get_hue_power(hue);
        }

        //assume linear between intervals
        public static double get_hue_power(double point)
        {

            //special cases-edges
            if (point == 0) return lowerPart_intervals[14];
            else if (point == 290) return upperPart_intervals[3];
            //this is the normal cases calculation

            //lower part
            else if (point <= 235)
            {
                int subpart_of_point = get_point_part(point, 0, 235, 14);
                double stratched_point = point * 14;//instad of / num_parts 
                double fraction = (stratched_point - subpart_of_point * 235) / 235;//equals : (point - subpart_of_point * (235 / 14)) / ((235 / 14));
                return lowerPart_intervals[subpart_of_point + 1] + (1 - fraction) * (lowerPart_intervals[subpart_of_point] - lowerPart_intervals[subpart_of_point + 1]);
            }

            //upper part
            else
            {
                int subpart_of_point = get_point_part(point - 290, 290 - 290, 360 - 290, 3);//swift by 290.this does not change 
                double stratched_point = point * 3;//instad of / num_parts 
                double fraction = (stratched_point - 3 * 290 - subpart_of_point * (360 - 290)) / (360 - 290);//equals : (point - 290 - subpart_of_point * (70 / 3)) / ((70 / 3));
                return upperPart_intervals[subpart_of_point + 1] + (1 - fraction) * (upperPart_intervals[subpart_of_point] - upperPart_intervals[subpart_of_point + 1]);
            }
        }

        //we devide the range between begining and ending to num_parts parts, and we wish to find from which of this parts comes point
        //we do this in order to find between which two intervals lies owr point 
        private static int get_point_part(double point, int begining, int ending, int num_parts)
        {
            int subpart = num_parts - 1;
            double subpart_size = (ending - begining);//instad of / num_parts 
            double point_in_extended_parts = point * (num_parts);

            for (subpart = num_parts - 1; subpart >= 0; subpart--)
            {
                if ((point_in_extended_parts > begining + subpart * subpart_size) && (point_in_extended_parts <= begining + (subpart + 1) * subpart_size))
                {
                    return subpart;
                }
            }
            if (point == ending) return 0;
            else if ((point > ending + (num_parts - 1 + 1) * subpart_size) && (point <= begining)) return num_parts - 1;
            //should never get here
            return -1;
        }

        //R,B,G sould be between 0-255
        //return hue between 0-360
        public static double RBG_to_HUE(int R, int G, int B)
        {
            if (R == G && G == B)
            {
                return (float)0.0;//this is a shade of gray and it's hue is 0;
            }

            double r = R / 255.0;
            double g = G / 255.0;
            double b = B / 255.0;

            //calc max
            double max = Max(r, g, b);
            //calc min
            double min = Min(r, g, b);

            double base_hue;

            if (r == g && g == b) return 0.0;
            else if (max == r) base_hue = ((g - b) / (max - min));
            else if (max == g) base_hue = 2.0 + ((b - r) / (max - min));
            else base_hue = 4.0 + ((r - g) / (max - min)); //max == b

            //convert for hue circle(range 0 to 360)
            base_hue = base_hue * 60;
            //in circle (of degrees) x=x+360
            if (base_hue < 0) base_hue = base_hue + 360;
            return base_hue;
        }

        private static double Max(double r, double g, double b)
        {
            double max = r;
            if (g > max) max = g;
            if (b > max) max = b;
            return max;
        }

        private static double Min(double r, double g, double b)
        {
            double min = r;
            if (g < min) min = g;
            if (b < min) min = b;
            return min;
        }
    

        // dummy implementation
        // needs a more carefull implementation
        public static Color rainToColor(double average)
        {

            if (average <= 0.1) //248
            {
                return Color.FromArgb(255, 9, 92, 245);
            }
            if (average <= 0.2) // 236
            {
                return Color.FromArgb(255, 0, 194, 193);
            }
            if (average <= 0.7) // 222
            {
                return Color.FromArgb(255, 2, 167, 171);
            }
            if (average <= 0.9) // 212
            {
                return Color.FromArgb(255, 2, 143, 127);
            }
            if (average <= 1.2) // 203
            {
                return Color.FromArgb(255, 1, 132, 88);
            }
            if (average <= 2.0)  // 191
            {
                return Color.FromArgb(255, 1, 153, 53);
            }
            if (average <= 4.0) // 179
            {
                return Color.FromArgb(255, 0, 185, 42);
            }
            if (average <= 6.0)// 169
            {
                return Color.FromArgb(255, 0, 204, 29);
            }
            if (average <= 9.0) // 161
            {
                return Color.FromArgb(255, 5, 230, 14);
            }
            if (average <= 13) // 147
            {
                return Color.FromArgb(255, 32, 247, 29);
            }
            if (average <= 22) // 123
            {
                return Color.FromArgb(255, 255, 241, 18);
            }
            if (average <= 26) // 107
            {
                return Color.FromArgb(255, 255, 204, 7);
            }
            if (average <= 30) // 97
            {
                return Color.FromArgb(255, 255, 183, 3);
            }
            if (average <= 40) // 82
            {
                return Color.FromArgb(255, 255, 142, 2);
            }
            if (average <= 60) // 61
            {
                return Color.FromArgb(255, 246, 43, 2);
            }
            if (average <= 100) // 44
            {
                return Color.FromArgb(255, 221, 9, 21);
            }
            if (average <= 150) // 33
            {
                return Color.FromArgb(255, 219, 0, 144);
            }

			// 16
            return Color.FromArgb(255, 255, 0, 252);


			// y offset 9
			// slider details: width 8, height 28
			
			// image details: 271x80

        }
    }
}