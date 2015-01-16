using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * ASSUMPTION - north part of the glob, east side
 */
namespace App8.DataModel
{
    /* TESTS
     * 31.147623, 35.372394(south east) => pre_x = 59.404 , pre_y = 67.07 , (plus,plus)
     * 32.777489, 35.528949(north east) => pre_x = 62.043 , pre_y = 34.67 ,(minus,plus)
     * 32.799424, 33.949664(north west) => pre_x = 35.422 , pre_y = 34.234 ,(minus,minus)
     * 31.216941, 34.294361(south west) => pre_x = 41.232 , pre_y = 65.692 ,(plus,minus)
     * */
    class PointTranslation
    {
        static double maxDistFromCenter_X = 280.0;
        static double maxDistFromCenter_Y = 280.0;

        static double center_lat = 32.006340;
        static double center_long = 34.814471;

        static int pixel_X_num = 512;
        static int pixel_Y_num = 512;

        public static int locationToPixel(double input_lat, double input_long)
        {

            //clac distance W-E - same latitude(X is represented by longitude)
            double earthDistance_X = DistanceOfEarthPoints.DistanceBetweenPlaces(input_long, center_lat, center_long, center_lat);
            //clac distance N-S - same longitude(X is represented by latitude)
            double earthDistance_Y = DistanceOfEarthPoints.DistanceBetweenPlaces(center_long, input_lat, center_long, center_lat);

            //calc percentage from center - as preperation to percentage from left corner side
            double percentage_X = (earthDistance_X / maxDistFromCenter_X) * 100;
            double percentage_Y = (earthDistance_Y / maxDistFromCenter_Y) * 100;

            //check that the point is inside the rectangle
            if (percentage_X > 100 || percentage_Y > 100)
            {
                return -1;
            }

            //calc derections compared to center
            Tuple<derections, derections> from_center = clac_derection(input_lat, input_long);

            //calc percentage from left corner side (easier to think that way)
            percentage_Y = center_to_topLeftCorner_Percentage(percentage_Y, from_center.Item1);
            percentage_X = center_to_topLeftCorner_Percentage(percentage_X, from_center.Item2);

            return getPixel(percentage_X, percentage_Y);
        }

        private static double center_to_topLeftCorner_Percentage(double center_percentage, derections direction_from_center)
        {
            if (direction_from_center != derections.MINUS) return (center_percentage / 2) + 50;
            else return 50 - (center_percentage / 2);
        }

        //calc pixel num from percentage_X and percentage_Y
        private static int getPixel(double percentage_X, double percentage_Y)
        {
            int x = (int)(pixel_X_num * (percentage_X / 100));//number of pixels right
            int y = (int)(pixel_Y_num * (percentage_Y / 100));//number of pixels down
            return x + pixel_X_num * y;
        }

        //distance is absolute - we need to know the points direction compared to the senter -north west?,east?
        //This function return a tuple - the first one for x(West will be MINUS,East PLUS) the second for y(North will be MINUS,South PLUS)
        private static Tuple<derections, derections> clac_derection(double input_lat, double input_long)
        {
            derections NS = derections.CENTER;//North will be MINUS,South PLUS
            derections EW = derections.CENTER;//West will be MINUS,East PLUS

            if (input_lat > center_lat) NS = derections.MINUS;
            else if (input_lat < center_lat) NS = derections.PLUS;//else ==

            if (input_long < center_long) EW = derections.MINUS;
            else if (input_long > center_long) EW = derections.PLUS;//else ==

            return new Tuple<derections, derections>(NS, EW);
        }
    }

    enum derections
    {
        CENTER, MINUS, PLUS
    };

}
