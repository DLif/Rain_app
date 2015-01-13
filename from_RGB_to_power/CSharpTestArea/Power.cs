using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTestArea
{
    class Power
    {
        static int Main(string[] args)
        {
            double power = RBG_to_power(233, 0, 208);
            return 0;
        }

        //main clac function
        public static double RBG_to_power(int R, int G, int B)
        {
            return get_hue_power(RBG_to_HUE(R, G, B));
        }

        //HUE ranges 0-230,290-360
        //this array holds the intervals given in the site(they are very problematic)
        static double[] lowerPart_intervals = new double[15] { 50.0, 40.0, 30.0, 24.0, 18.0, 13.0, 9.0, 6.0, 4.0, 2.0, 1.2, 0.7, 0.2, 0.1, 0.0 };
        static double[] upperPart_intervals = new double[4] { 250.0, 200.0, 100.0, 50.0 };

        //assume linear between intervals
        public static double get_hue_power(float point)
        {
            
            //special cases-edges
            if (point == 0) return lowerPart_intervals[14];
            else if (point == 300) return lowerPart_intervals[3];
            //this is the normal cases calculation

            //lower part
            else if (point <= 240)
            {
                int subpart_of_point = get_point_part(point, 0, 230, 14);
                float stratched_point = point * 14;//instad of / num_parts 
                float fraction = (stratched_point - subpart_of_point * 230) / 230;//equals : (point - subpart_of_point * (240 / 14)) / ((240 / 14));
                return lowerPart_intervals[subpart_of_point+1] + (1-fraction) * (lowerPart_intervals[subpart_of_point] - lowerPart_intervals[subpart_of_point + 1]);
            }

            //upper part
            else
            {
                int subpart_of_point = get_point_part(point - 290, 290 - 290, 360 - 290, 3);//swift by 290.this does not change 
                float stratched_point = point * 3;//instad of / num_parts 
                float fraction = (stratched_point - 3 * 290 - subpart_of_point * (360 - 290)) / (360 - 290);//equals : (point - 290 - subpart_of_point * (70 / 3)) / ((70 / 3));
                return upperPart_intervals[subpart_of_point + 1] + (1 - fraction) * (upperPart_intervals[subpart_of_point] - upperPart_intervals[subpart_of_point + 1]);
            }
        }

        //we devide the range between begining and ending to num_parts parts, and we wish to find from which of this parts comes point
        //we do this in order to find between which two intervals lies owr point 
        private static int get_point_part(float point, int begining, int ending, int num_parts)
        {
            int subpart = num_parts - 1;
            float subpart_size = (ending - begining);//instad of / num_parts 
            float point_in_extended_parts = point * (num_parts);

            for (subpart = num_parts - 1; subpart >= 0; subpart--)
            {
                if ((point_in_extended_parts > begining + subpart * subpart_size) && (point_in_extended_parts <= begining + (subpart + 1) * subpart_size))
                {
                    return subpart;
                }
            }
            //should never get here
            return -1;
        }

        //R,B,G sould be between 0-255
        //return hue between 0-360
        public static float RBG_to_HUE(int R, int G, int B)
        {
            float r = R / (float)255.0;
            float g = G / (float)255.0;
            float b = B / (float)255.0;

            //calc max
            float max = Max(r, g, b);
            //calc min
            float min = Min(r, g, b);

            float base_hue;

            if (max == r) base_hue= ((g - b) / (max - min));
            else if (max == g) base_hue = (float)2.0 + ((b - r) / (max - min));
            else base_hue = (float)4.0 + ((r - g) / (max - min)); //max == b

            //convert for hue circle(range 0 to 360)
            base_hue = base_hue*60;
            //in circle (of degrees) x=x+360
            if (base_hue < 0) base_hue = base_hue + 360;
            return base_hue;
        }

        private static float Max(float r, float g, float b)
        {
            float max = r;
            if (g > max) max = g;
            if (b > max) max = b;
            return max;
        }

        private static float Min(float r, float g, float b)
        {
            float min = r;
            if (g < min) min = g;
            if (b < min) min = b;
            return min;
        }
    }
}
