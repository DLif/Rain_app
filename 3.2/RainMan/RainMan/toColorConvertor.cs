using RainMan.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace RainMan
{
    class toColorConvertor : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            double avg = (double)value;

            Color result = ColorTranslator.rainToColor(avg);
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    class toLocationConvertor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((string)value == "current")
            {
                return "AT YOUR CURRENT LOCATION";
            }
            else
            {
                return string.Format("AT '{0}'", ((string)value).ToUpper());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
