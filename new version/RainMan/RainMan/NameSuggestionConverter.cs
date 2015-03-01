using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RainMan
{
    public class NameSuggestionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                return "Leave at";
            }
            return "Take '" + (string)value + "'";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    
    public class PathNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                return "";
            }
            return "Path name: " + (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EstimatedTimeConverter : IValueConverter
    {
        

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double minutes = (double)value;

            TimeSpan span = TimeSpan.FromMinutes(minutes);

            return string.Format("Estimated time: {0}:{1} hours", span.Hours, span.Minutes == 0 ? "00" : span.Minutes.ToString() );

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EstimatedLengthConverter : IValueConverter
    {



        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double meters = (double)value;

            return meters.ToString() + " meters";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AvgRainConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double avgRain = (double)value;

            return String.Format("{0:0.00}",avgRain) + " MM\\Hour";

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
