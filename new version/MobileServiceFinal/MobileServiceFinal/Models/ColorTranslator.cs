using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileServiceFinal.Models
{
    class ColorTranslator
    {
        static int image_size_x = 512;
        static int image_size_y = 512;

        //HUE ranges 0-235,290-360
        //this array holds the intervals given in the site(they are very problematic)
        static double[] lowerPart_intervals = new double[15] { 50.0, 40.0, 30.0, 24.0, 18.0, 13.0, 9.0, 6.0, 4.0, 2.0, 1.2, 0.7, 0.2, 0.1, 0.0 };
        static double[] upperPart_intervals = new double[4] { 250.0, 200.0, 100.0, 50.0 };

        static int[][] fullArrayBarPixels =
            new int[242][] { new int[3] { 220, 23, 225 }, new int[3] { 226, 19, 229 }, new int[3] { 237, 13, 234 }, new int[3] { 245, 7, 240 }, new int[3] { 251, 3, 247 }, new int[3] { 252, 2, 250 }, new int[3] { 249, 3, 252 }, new int[3] { 247, 4, 252 }, new int[3] { 245, 5, 250 }, new int[3] { 247, 5, 249 }, new int[3] { 252, 2, 247 }, new int[3] { 254, 2, 247 }, new int[3] { 254, 2, 247 }, new int[3] { 251, 3, 249 }, new int[3] { 245, 6, 249 }, new int[3] { 244, 7, 245 }, new int[3] { 241, 5, 235 }, new int[3] { 238, 4, 223 }, new int[3] { 234, 2, 210 }, new int[3] { 231, 1, 197 }, new int[3] { 227, 2, 182 }, new int[3] { 223, 2, 169 }, new int[3] { 218, 0, 156 }, new int[3] { 213, 0, 144 }, new int[3] { 208, 1, 133 }, new int[3] { 205, 1, 122 }, new int[3] { 201, 1, 108 }, new int[3] { 195, 0, 94 }, new int[3] { 192, 0, 78 }, new int[3] { 189, 0, 66 }, new int[3] { 188, 1, 56 }, new int[3] { 189, 1, 49 }, new int[3] { 195, 4, 45 }, new int[3] { 198, 4, 41 }, new int[3] { 201, 5, 41 }, new int[3] { 204, 6, 39 }, new int[3] { 210, 7, 36 }, new int[3] { 214, 8, 31 }, new int[3] { 218, 9, 28 }, new int[3] { 221, 9, 23 }, new int[3] { 225, 10, 18 }, new int[3] { 228, 12, 13 }, new int[3] { 231, 14, 9 }, new int[3] { 233, 15, 5 }, new int[3] { 236, 16, 2 }, new int[3] { 238, 19, 0 }, new int[3] { 243, 24, 4 }, new int[3] { 244, 29, 1 }, new int[3] { 242, 33, 0 }, new int[3] { 242, 39, 0 }, new int[3] { 245, 44, 2 }, new int[3] { 246, 50, 2 }, new int[3] { 247, 53, 2 }, new int[3] { 248, 59, 3 }, new int[3] { 249, 65, 1 }, new int[3] { 249, 71, 1 }, new int[3] { 249, 76, 0 }, new int[3] { 248, 81, 0 }, new int[3] { 251, 87, 0 }, new int[3] { 252, 93, 1 }, new int[3] { 251, 97, 1 }, new int[3] { 252, 100, 1 }, new int[3] { 253, 104, 2 }, new int[3] { 255, 108, 5 }, new int[3] { 255, 110, 5 }, new int[3] { 254, 113, 5 }, new int[3] { 255, 117, 6 }, new int[3] { 255, 121, 6 }, new int[3] { 255, 123, 5 }, new int[3] { 255, 126, 6 }, new int[3] { 255, 131, 7 }, new int[3] { 255, 136, 6 }, new int[3] { 255, 140, 8 }, new int[3] { 255, 143, 5 }, new int[3] { 254, 147, 5 }, new int[3] { 255, 151, 7 }, new int[3] { 255, 155, 5 }, new int[3] { 255, 157, 4 }, new int[3] { 255, 158, 5 }, new int[3] { 254, 160, 3 }, new int[3] { 255, 164, 5 }, new int[3] { 255, 166, 4 }, new int[3] { 255, 167, 5 }, new int[3] { 255, 170, 5 }, new int[3] { 255, 171, 6 }, new int[3] { 255, 174, 5 }, new int[3] { 255, 177, 6 }, new int[3] { 255, 179, 5 }, new int[3] { 255, 182, 5 }, new int[3] { 254, 185, 4 }, new int[3] { 255, 186, 3 }, new int[3] { 254, 190, 4 }, new int[3] { 255, 192, 6 }, new int[3] { 255, 194, 5 }, new int[3] { 255, 197, 7 }, new int[3] { 255, 198, 5 }, new int[3] { 254, 205, 6 }, new int[3] { 254, 207, 5 }, new int[3] { 254, 209, 6 }, new int[3] { 255, 211, 6 }, new int[3] { 255, 214, 5 }, new int[3] { 255, 217, 5 }, new int[3] { 255, 220, 4 }, new int[3] { 255, 222, 3 }, new int[3] { 255, 224, 5 }, new int[3] { 255, 227, 4 }, new int[3] { 254, 228, 7 }, new int[3] { 255, 231, 7 }, new int[3] { 255, 234, 11 }, new int[3] { 255, 235, 10 }, new int[3] { 255, 237, 13 }, new int[3] { 255, 238, 14 }, new int[3] { 255, 241, 18 }, new int[3] { 252, 243, 18 }, new int[3] { 254, 245, 20 }, new int[3] { 254, 247, 19 }, new int[3] { 254, 249, 23 }, new int[3] { 254, 251, 24 }, new int[3] { 255, 253, 28 }, new int[3] { 252, 254, 30 }, new int[3] { 249, 255, 33 }, new int[3] { 239, 255, 34 }, new int[3] { 224, 254, 34 }, new int[3] { 210, 254, 35 }, new int[3] { 195, 255, 37 }, new int[3] { 181, 255, 36 }, new int[3] { 166, 253, 34 }, new int[3] { 153, 255, 34 }, new int[3] { 139, 255, 34 }, new int[3] { 125, 255, 33 }, new int[3] { 112, 254, 30 }, new int[3] { 96, 255, 30 }, new int[3] { 81, 255, 33 }, new int[3] { 65, 255, 31 }, new int[3] { 54, 254, 33 }, new int[3] { 43, 253, 32 }, new int[3] { 40, 251, 32 }, new int[3] { 36, 251, 33 }, new int[3] { 34, 249, 31 }, new int[3] { 30, 248, 29 }, new int[3] { 27, 247, 25 }, new int[3] { 24, 246, 23 }, new int[3] { 20, 245, 19 }, new int[3] { 18, 245, 18 }, new int[3] { 15, 239, 18 }, new int[3] { 13, 239, 17 }, new int[3] { 12, 238, 16 }, new int[3] { 10, 236, 14 }, new int[3] { 8, 234, 13 }, new int[3] { 6, 232, 11 }, new int[3] { 5, 230, 12 }, new int[3] { 5, 228, 15 }, new int[3] { 2, 225, 14 }, new int[3] { 3, 222, 16 }, new int[3] { 3, 220, 19 }, new int[3] { 3, 217, 21 }, new int[3] { 3, 214, 23 }, new int[3] { 3, 212, 23 }, new int[3] { 3, 209, 23 }, new int[3] { 3, 208, 27 }, new int[3] { 0, 204, 29 }, new int[3] { 0, 204, 31 }, new int[3] { 0, 202, 32 }, new int[3] { 0, 200, 32 }, new int[3] { 0, 197, 32 }, new int[3] { 0, 195, 35 }, new int[3] { 0, 191, 36 }, new int[3] { 0, 188, 39 }, new int[3] { 0, 185, 40 }, new int[3] { 1, 184, 44 }, new int[3] { 2, 180, 44 }, new int[3] { 0, 177, 45 }, new int[3] { 1, 174, 46 }, new int[3] { 0, 172, 46 }, new int[3] { 0, 170, 47 }, new int[3] { 0, 168, 49 }, new int[3] { 0, 160, 50 }, new int[3] { 1, 158, 51 }, new int[3] { 0, 157, 54 }, new int[3] { 0, 154, 53 }, new int[3] { 0, 152, 54 }, new int[3] { 0, 150, 57 }, new int[3] { 0, 148, 60 }, new int[3] { 0, 148, 64 }, new int[3] { 0, 144, 65 }, new int[3] { 0, 144, 69 }, new int[3] { 1, 142, 73 }, new int[3] { 0, 141, 75 }, new int[3] { 1, 139, 77 }, new int[3] { 0, 138, 78 }, new int[3] { 0, 137, 81 }, new int[3] { 0, 137, 83 }, new int[3] { 1, 134, 89 }, new int[3] { 0, 133, 90 }, new int[3] { 0, 132, 93 }, new int[3] { 0, 132, 95 }, new int[3] { 0, 133, 98 }, new int[3] { 1, 134, 103 }, new int[3] { 2, 136, 109 }, new int[3] { 2, 138, 114 }, new int[3] { 3, 140, 122 }, new int[3] { 2, 143, 127 }, new int[3] { 2, 147, 134 }, new int[3] { 2, 149, 139 }, new int[3] { 0, 151, 142 }, new int[3] { 0, 154, 146 }, new int[3] { 1, 157, 153 }, new int[3] { 1, 160, 156 }, new int[3] { 2, 161, 164 }, new int[3] { 0, 164, 166 }, new int[3] { 1, 165, 167 }, new int[3] { 2, 167, 169 }, new int[3] { 2, 170, 173 }, new int[3] { 3, 172, 175 }, new int[3] { 2, 174, 178 }, new int[3] { 2, 175, 179 }, new int[3] { 4, 179, 184 }, new int[3] { 1, 180, 184 }, new int[3] { 0, 182, 185 }, new int[3] { 1, 185, 187 }, new int[3] { 2, 187, 189 }, new int[3] { 1, 189, 190 }, new int[3] { 2, 190, 191 }, new int[3] { 2, 191, 193 }, new int[3] { 1, 196, 198 }, new int[3] { 3, 198, 204 }, new int[3] { 3, 193, 207 }, new int[3] { 1, 185, 209 }, new int[3] { 0, 176, 213 }, new int[3] { 1, 169, 216 }, new int[3] { 0, 159, 219 }, new int[3] { 0, 149, 220 }, new int[3] { 0, 141, 220 }, new int[3] { 0, 132, 225 }, new int[3] { 0, 121, 228 }, new int[3] { 1, 111, 232 }, new int[3] { 1, 101, 238 }, new int[3] { 1, 92, 241 }, new int[3] { 0, 84, 244 }, new int[3] { 0, 76, 237 }, new int[3] { 4, 70, 219 }, new int[3] { 0, 0, 0 } };
        static double[] fullPowerArray =
            new double[242] { 250, 245.833333333333, 241.666666666667, 237.5, 233.333333333333, 229.166666666667, 225, 220.833333333333, 216.666666666667, 212.5, 208.333333333333, 204.166666666667, 200, 195.833333333333, 191.666666666667, 187.5, 183.333333333333, 179.166666666667, 175, 170.833333333333, 166.666666666667, 162.5, 158.333333333333, 154.166666666667, 150, 145.833333333333, 141.666666666667, 137.5, 133.333333333333, 129.166666666667, 125, 120.833333333333, 116.666666666667, 112.5, 108.333333333333, 104.166666666667, 100, 97.2222222222222, 94.4444444444444, 91.6666666666667, 88.8888888888889, 86.1111111111111, 83.3333333333333, 80.5555555555556, 77.7777777777778, 75, 72.2222222222222, 69.4444444444444, 66.6666666666667, 63.8888888888889, 61.1111111111111, 58.3333333333333, 55.5555555555556, 52.7777777777778, 50, 49.375, 48.75, 48.125, 47.5, 46.875, 46.25, 45.625, 45, 44.375, 43.75, 43.125, 42.5, 41.875, 41.25, 40.625, 40, 39.4444444444444, 38.8888888888889, 38.3333333333333, 37.7777777777778, 37.2222222222222, 36.6666666666667, 36.1111111111111, 35.5555555555556, 35, 34.4444444444444, 33.8888888888889, 33.3333333333333, 32.7777777777778, 32.2222222222222, 31.6666666666667, 31.1111111111111, 30.5555555555556, 30, 29.4, 28.8, 28.2, 27.6, 27, 26.4, 25.8, 25.2, 24.6, 24, 23.7272727272727, 23.4545454545455, 23.1818181818182, 22.9090909090909, 22.6363636363636, 22.3636363636364, 22.0909090909091, 21.8181818181818, 21.5454545454545, 21.2727272727273, 21, 20.7272727272727, 20.4545454545455, 20.1818181818182, 19.9090909090909, 19.6363636363636, 19.3636363636364, 19.0909090909091, 18.8181818181818, 18.5454545454545, 18.2727272727273, 18, 17.7058823529412, 17.4117647058824, 17.1176470588235, 16.8235294117647, 16.5294117647059, 16.2352941176471, 15.9411764705882, 15.6470588235294, 15.3529411764706, 15.0588235294118, 14.7647058823529, 14.4705882352941, 14.1764705882353, 13.8823529411765, 13.5882352941176, 13.2941176470588, 13, 12.6666666666667, 12.3333333333333, 12, 11.6666666666667, 11.3333333333333, 11, 10.6666666666667, 10.3333333333333, 10, 9.66666666666667, 9.33333333333333, 9, 8.66666666666667, 8.33333333333333, 8, 7.66666666666667, 7.33333333333333, 7, 6.66666666666667, 6.33333333333333, 6, 5.81818181818182, 5.63636363636364, 5.45454545454546, 5.27272727272727, 5.09090909090909, 4.90909090909091, 4.72727272727273, 4.54545454545455, 4.36363636363636, 4.18181818181818, 4, 3.77777777777778, 3.55555555555556, 3.33333333333333, 3.11111111111111, 2.88888888888889, 2.66666666666667, 2.44444444444444, 2.22222222222222, 2, 1.94285714285714, 1.88571428571429, 1.82857142857143, 1.77142857142857, 1.71428571428571, 1.65714285714286, 1.6, 1.54285714285714, 1.48571428571429, 1.42857142857143, 1.37142857142857, 1.31428571428571, 1.25714285714286, 1.2, 1.16875, 1.1375, 1.10625, 1.075, 1.04375, 1.0125, 0.98125, 0.95, 0.91875, 0.8875, 0.85625, 0.825, 0.79375, 0.7625, 0.73125, 0.7, 0.666666666666667, 0.633333333333333, 0.6, 0.566666666666667, 0.533333333333333, 0.5, 0.466666666666667, 0.433333333333333, 0.4, 0.366666666666667, 0.333333333333333, 0.3, 0.266666666666667, 0.233333333333333, 0.2, 0.19, 0.18, 0.17, 0.16, 0.15, 0.14, 0.13, 0.12, 0.11, 0.1, 0.0871428571428571, 0.0742857142857143, 0.0614285714285714, 0.0485714285714286, 0.0357142857142857, 0.0228571428571429, 0.01, 0 };

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

        public static double median_power(Byte[] pixels, int x_pixel, int y_pixel, int pixelRadius, int width)
        {
            List<double> powers_list = new List<double>();

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
                    else power = ColorTranslator.RGB_array_power(r, g, b);

                    powers_list.Add(power);
                }
            }
            double median = powers_list.ElementAt((int)Math.Floor((double)powers_list.Count / 2.0));
            return median;
        }

        public static double RGB_array_power(int R, int G, int B)
        {
            int min_index = -1;
            int index = 0;
            double min_distance = Double.MaxValue;

            if (R < 30 && G < 30 && B < 30) //nearly black
            {
                return 0.0;
            }
            foreach (int[] test_point in fullArrayBarPixels)
            {
                double cur_distance = distance_formula(R, G, B, test_point[0], test_point[1], test_point[2]);
                if (cur_distance < min_distance)
                {
                    min_distance = cur_distance;
                    min_index = index;
                }
                index++;
            }
            return fullPowerArray[min_index];
        }

        public static double distance_formula(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            return Math.Sqrt((double)((r2 - r1) * (r2 - r1) + (g2 - g1) * (g2 - g1) + (b2 - b1) * (b2 - b1)));
            //return Math.Abs(RBG_to_HUE(r1,g1,b1) - RBG_to_HUE(r2,g2,b2));
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
    }
}