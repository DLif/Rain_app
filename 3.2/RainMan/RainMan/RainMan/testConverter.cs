using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RainMan
{

    public class RainToHeight
    {
        public static double rainToHeight(double avg)
        {
            double start = 10;
            double average = avg;

            if (average <= 0.1)
            {
                return start;
            }
            if (average <= 0.2)
            {
                return start + 10;
            }
            if (average <= 0.7)
            {
                return start + 20;
            }
            if (average <= 0.9)
            {
                return start + 30;
            }
            if (average <= 1.2)
            {
                return start + 40;
            }
            if (average <= 2.0)
            {
                return start + 50;
            }
            if (average <= 4.0)
            {
                return start + 60;
            }
            if (average <= 6.0)
            {
                return start + 70;
            }
            if (average <= 9.0)
            {
                return start + 80;
            }
            if (average <= 13)
            {
                return start + 90;
            }
            if (average <= 22)
            {
                return start + 100;
            }
            if (average <= 26)
            {
                return start + 110;
            }
            if (average <= 30)
            {
                return start + 110;
            }
            if (average <= 40)
            {
                return start + 110;
            }
            if (average <= 60)
            {
                return start + 110;
            }
            if (average <= 100)
            {
                return start + 110;
            }
            if (average <= 150)
            {
                return start + 110;
            }


            return start + 110;
        }
    }

    public class testConverter :IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {

            return RainToHeight.rainToHeight((double)value);

        }
        

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

       
    }
}
