using Reversi.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reversi.Domain;
using Reversi.DataStructures;
using System.Windows.Input;
using System.Windows;


namespace ReversiGUI
{
    public class ButtonViewModel
    {
        private Vector2D pos;
        private Game game;
        private ClickCommand click;


        public ICell<Player> Owner
        {
            get;
            set;
        }

        public ISquare Square
        {
            get;
            set;
        }

        public ICommand Click
        {
            get
            {
                return click;
            }
        }

        public ICell<bool> IsValidMove
        {
            get;
            set;
        }

        public ButtonViewModel(int row,int col,Game game)
        {
            
            this.game = game;
            pos = new Vector2D(row-1,col-1);

           
  
           
            Owner = game.Board[pos].Owner;

            IsValidMove = game.Board[pos].IsValidMove; 
            
            Square = game.Board[pos]; 
          
            click=new ClickCommand(this);
            
        }



        public void PerformClick(){
            game.Board[pos].PlaceStone(); 
        }

    }

    class RowViewModel
    {
        IList<ButtonViewModel> buttons;

        public IList<ButtonViewModel> Buttons
        {
            get { return buttons; }
        }

        public RowViewModel(int row,Game game)
        {
            buttons = Enumerable.Range(1, 8).Select(i => new ButtonViewModel(row,i,game)).ToList().AsReadOnly();  
        }
    }

    class ViewModel
    {
        private IList<RowViewModel> rows;
        private Game game;

        public IList<RowViewModel> Rows
        {
            get { return rows; }
        }

        public ICell<int> ScorePlayer1
        {
            get;
            set;
        }

        public ICell<int> ScorePlayer2
        {
            get;
            set;
        }

        public ICell<bool> GameOver
        {
            get;
            set;
        }

        public ICell<Player> PlayerWithMostStones
        {
            get;
            set;
        }

        public ICell<Player> CurrentPlayer
        {
            get;
            set;
        }

        public ViewModel(Game game)
        {
            this.game = game;
            rows = Enumerable.Range(1, 8).Select(i => new RowViewModel(i,this.game)).ToList().AsReadOnly();
            ScorePlayer1=this.game.StoneCount(this.game.CurrentPlayer.Value);
            ScorePlayer2=this.game.StoneCount(this.game.CurrentPlayer.Value.Other);
            CurrentPlayer = this.game.CurrentPlayer;
            this.PlayerWithMostStones = this.game.PlayerWithMostStones;
            GameOver = this.game.IsGameOver;   
        }

        public String[] GetSaveData()
        {
            String[] lines = new String[game.Board.Height+1];
            
            for(int i=0;i<game.Board.Height;i++){
                String str="";
                for (int j = 0; j < game.Board.Height; j++)
                {
                    Player owner = game.Board[new Vector2D(j,i)].Owner.Value;
                    if(owner==null){
                        str += ".";
                    }
                    else if (owner == Player.ONE)
                    {
                        str += "1";
                    }
                    else if (owner == Player.TWO)
                    {
                        str += "2";
                    }
                }
                lines[i] = str;
            }
            if (game.CurrentPlayer.Value == Player.ONE)
            {
                lines[lines.Length - 1] = "1";
            }
            else if (game.CurrentPlayer.Value == Player.TWO)
            {
                lines[lines.Length - 1] = "2";
            }

            return lines;
        }

        public IGrid<Player> ConvertSavedData(params string[] lines)
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
        }

    }

    class ClickCommand:ICommand{

        ButtonViewModel buttonVM;

        public ClickCommand(ButtonViewModel buttonVM)
        {
            this.buttonVM = buttonVM;
            buttonVM.IsValidMove.PropertyChanged += IsValidMove_PropertyChanged;
        }

        void IsValidMove_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(buttonVM, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return buttonVM.IsValidMove.Value;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            buttonVM.PerformClick();
            
        }
    }

}
