using Reversi.Cells;
using Reversi.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Domain
{
    internal class GameState
    {
        private ICell<State> state;

        public static GameState CreateNewGame()
        {
            return new GameState( GameBoard.CreateDefault(), Player.ONE );
        }

        public static GameState CreateInProgress( IGrid<Player> board, Player nextPlayer )
        {
            var gameBoard = GameBoard.Create( board );

            return new GameState( gameBoard, nextPlayer );
        }

        private GameState( GameBoard initialBoard, Player initialPlayer )
            : this( CreateInitialState( initialBoard, initialPlayer ) )
        {
            // NOP
        }

        private GameState( ICell<State> state )
        {
            this.state = state;
        }

        private static ICell<State> CreateInitialState( GameBoard initialBoard, Player initialPlayer )
        {
            if ( !initialBoard.HasValidMove( initialPlayer ) )
            {
                throw new ArgumentException( "No move available for given player" );
            }
            else
            {
                var state = new Cell<State>();
                state.Value = new InProgress( state, initialBoard, initialPlayer );

                return state;
            }
        }

        public IGrid<Player> Board
        {
            get
            {
                return state.Value.Board.View;
            }
        }

        public bool IsValidMove( Vector2D position )
        {
            return state.Value.IsValidMove( position );
        }

        public IEnumerable<Vector2D> Captured( Vector2D position )
        {
            return state.Value.Captured( position );
        }

        public Player CurrentPlayer
        {
            get
            {
                return state.Value.CurrentPlayer;
            }
        }

        public bool IsGameOver
        {
            get
            {
                return state.Value.IsGameOver;
            }
        }

        public Player PlayerWithMostStones
        {
            get
            {
                return state.Value.PlayerWithMostStones;
            }
        }

        public void MakeMove( Vector2D position )
        {
            state.Value.MakeMove( position );
        }

        public GameState Copy()
        {
            var stateCopy = new Cell<State>();

            stateCopy.Value = state.Value.Copy( stateCopy );

            return new GameState( state );
        }

        public IEnumerable<Vector2D> ValidMoves
        {
            get
            {
                return this.state.Value.ValidMoves;
            }
        }

        public int StoneCount( Player player )
        {
            if ( player == null )
            {
                throw new ArgumentNullException();
            }
            else
            {
                return state.Value.Board.CountStones( player );
            }
        }

        private abstract class State
        {
            protected readonly GameBoard board;

            protected readonly ICell<State> state;

            protected State( ICell<State> state, GameBoard board )
            {
                this.state = state;
                this.board = board;
            }

            public GameBoard Board
            {
                get
                {
                    return board;
                }
            }

            public abstract State Copy( ICell<State> cell );

            public abstract void MakeMove( Vector2D position );

            public abstract bool IsValidMove( Vector2D position );

            public abstract Player CurrentPlayer { get; }

            public abstract bool IsGameOver { get; }

            public abstract Player PlayerWithMostStones { get; }

            public abstract IEnumerable<Vector2D> Captured( Vector2D position );

            public abstract IEnumerable<Vector2D> ValidMoves { get; }
        }

        private class InProgress : State
        {
            private Player currentPlayer;

            public InProgress( ICell<State> state, GameBoard board, Player player )
                : base( state, board )
            {
                this.currentPlayer = player;
            }

            public override State Copy( ICell<State> cell )
            {
                return new InProgress( cell, board, currentPlayer );
            }

            public override Player PlayerWithMostStones
            {
                get { return this.board.WinningPlayer; }
            }

            public override bool IsGameOver
            {
                get { return false; }
            }

            public override Player CurrentPlayer
            {
                get { return currentPlayer; }
            }

            public override bool IsValidMove( Vector2D position )
            {
                return board.IsValidMove( position, this.currentPlayer );
            }

            public override void MakeMove( Vector2D position )
            {
                if ( !IsValidMove( position ) )
                {
                    throw new ArgumentException();
                }
                else
                {
                    board.PlaceStone( position, currentPlayer );

                    if ( board.HasValidMove( currentPlayer.Other ) )
                    {
                        currentPlayer = currentPlayer.Other;
                    }
                    else if ( !board.HasValidMove(currentPlayer ) )
                    {
                        state.Value = new GameOver( state, board );
                    }
                }
            }

            public override IEnumerable<Vector2D> Captured( Vector2D position )
            {
                if ( IsValidMove( position ) )
                {
                    return board.CapturedStones( position, currentPlayer );
                }
                else
                {
                    return Enumerable.Empty<Vector2D>();
                }
            }

            public override IEnumerable<Vector2D> ValidMoves
            {
                get
                {
                    return this.board.ValidMoves( this.currentPlayer );
                }
            }
        }

        private class GameOver : State
        {
            public GameOver( ICell<State> state, GameBoard board )
                : base( state, board )
            {

            }

            public override Player CurrentPlayer
            {
                get { throw new InvalidOperationException(); }
            }

            public override bool IsGameOver
            {
                get { return true; }
            }

            public override Player PlayerWithMostStones
            {
                get { return board.WinningPlayer; }
            }

            public override bool IsValidMove( Vector2D position )
            {
                return false;
            }

            public override void MakeMove( Vector2D position )
            {
                throw new InvalidOperationException();
            }

            public override State Copy( ICell<State> state )
            {
                return new GameOver( state, board );
            }

            public override IEnumerable<Vector2D> Captured( Vector2D position )
            {
                throw new InvalidOperationException();
            }

            public override IEnumerable<Vector2D> ValidMoves
            {
                get
                {
                    return Enumerable.Empty<Vector2D>();
                }
            }
        }
    }
}
