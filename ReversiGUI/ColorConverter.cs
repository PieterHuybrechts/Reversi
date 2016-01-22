using Reversi.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ReversiGUI
{
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Player owner = (Player)value;



            if (owner == Player.ONE)
            {
                return Brushes.White;
            }
            else if (owner == Player.TWO)
            {
                SolidColorBrush brush=new SolidColorBrush();
                brush.Color = Color.FromRgb(10, 10, 10);
                return brush;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
