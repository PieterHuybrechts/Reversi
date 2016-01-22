using Reversi.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace ReversiGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel vM;

        public MainWindow()
        {
            InitializeComponent();

            vM = new ViewModel(Game.CreateNew());
            this.DataContext = vM;

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Restart_Button_Click(object sender, RoutedEventArgs e)
        {
            vM = new ViewModel(Game.CreateNew());   
            this.DataContext = vM;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            String[] lines=vM.GetSaveData();

            String Path="../../_Resources/Save.txt";

            using (StreamWriter sw = File.CreateText(Path))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    sw.WriteLine(lines[i]);
                }
            }

        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            String Path = "../../_Resources/Save.txt";
            Player Playr=null;

            ArrayList Lines = new ArrayList();

            using (StreamReader sr = File.OpenText(Path))
            {
                String str = "";

                while ((str = sr.ReadLine()) != null)
                {
                    Lines.Add(str);
                }
            }

            String[] Lns = new String[Lines.Count-1];

            int i = 0;

            foreach(object L in Lines){
                String Line = (String) L;
                if (i < Lns.Length)
                {
                    Lns[i] = Line;
                }
                else
                {
                    if (Line.Equals("1"))
                    {
                        Playr = Player.ONE;
                    }
                    else if (Line.Equals("2"))
                    {
                        Playr = Player.TWO;   
                    }
                }
                
                i++;
            }

           
            var Board=vM.ConvertSavedData(Lns);

            vM = new ViewModel(Game.CreateInProgress(Board, Playr));
            this.DataContext = vM;

        }

        /*private IGrid<Player> Board(params string[] lines)
        {
            var grid = Grid.CreateCharacterGrid(lines);

            return grid.Map(c =>
            {
                switch (c)
                {
                    case '.':
                        return null;

                    case '1':
                        return Player.ONE;

                    case '2':
                        return Player.TWO;

                    default:
                        throw new ArgumentException();
                }
            });
        }*/

    }
}
