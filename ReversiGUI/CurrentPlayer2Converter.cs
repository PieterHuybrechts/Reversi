﻿using Reversi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ReversiGUI
{
    class CurrentPlayer2Converter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Player player = (Player)value;

            if (player == Player.ONE)
            {
                return Brushes.Transparent;
            }
            else if(player ==Player.TWO)
            {
                return Brushes.White;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
