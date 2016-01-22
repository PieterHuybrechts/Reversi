using Reversi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReversiGUI
{
    class PlayerWithMostStonesConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Player player = (Player) value;
            if (player == Player.ONE)
            {
                return "Player 1 wins!";
            }
            else if(player == Player.TWO)
            {
                return "Player 2 wins!";
            }
            else
            {
                return "Draw!";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
