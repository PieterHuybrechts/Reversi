using Reversi.Cells;
using Reversi.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Domain
{
    public class Game
    {
        private readonly GameState gameState;

        private readonly IDerived<Player> currentPlayer;

        private readonly IGrid<Square> board;

        private readonly IDerived<int>[] stoneCounts;

        private readonly IDerived<bool> isGameOver;

        private readonly IDerived<Player> playerWithMostStones;

        /// <summary>
        /// Creates a new game.
        /// </summary>
        /// <returns>
        /// A new game.
        /// </returns>
        public static Game CreateNew()
        {
            return new Game( GameState.CreateNewGame() );
        }

        /// <summary>
        /// Creates a game from a given board and next player.
        /// </summary>
        /// <param name="board">Board</param>
        /// <param name="nextPlayer">Next player</param>
        /// <returns>A game object</returns>
        public static Game CreateInProgress(IGrid<Player> board, Player nextPlayer)
        {
            return new Game( GameState.CreateInProgress( board, nextPlayer ) );
        }

        private Game( GameState gameState )
        {
            this.gameState = gameState;

            currentPlayer = Cell.Derived( DeriveCurrentPlayer );
            board = new Grid<Square>( gameState.Board.Width, gameState.Board.Height, p => new Square( this, p ) );
            stoneCounts = new IDerived<int>[] { Cell.Derived( () => gameState.StoneCount( Player.ONE ) ), Cell.Derived( () => gameState.StoneCount( Player.TWO ) ) };
            isGameOver = Cell.Derived( () => gameState.IsGameOver );
            playerWithMostStones = Cell.Derived( () => gameState.PlayerWithMostStones );
        }

        private Player DeriveCurrentPlayer()
        {
            if ( gameState.IsGameOver )
            {
                return null;
            }
            else
            {
                return gameState.CurrentPlayer;
            }
        }

        private void Refresh()
        {
            currentPlayer.Refresh();
            isGameOver.Refresh();
            board.Each( square => square.Refresh() );

            foreach ( var stoneCount in stoneCounts )
            {
                stoneCount.Refresh();
            }

            playerWithMostStones.Refresh();
        }

        private Player GetOwnerAt( Vector2D position )
        {
            return gameState.Board[position];
        }

        private bool IsValidMove( Vector2D position )
        {
            return gameState.IsValidMove( position );
        }

        private void PlaceStone( Vector2D position )
        {
            gameState.MakeMove( position );

            Refresh();
        }

        /// <summary>
        /// Computes which positions would be captured if the current player
        /// were to put his next stone at the specified position.
        /// </summary>
        /// <param name="position">Position at which the current player intends to put his stone.</param>
        /// <returns>A sequence of all stones that would be captured by the specified move.</returns>
        private IEnumerable<Vector2D> CapturedBy( Vector2D position )
        {
            return gameState.Captured( position );
        }

        /// <summary>
        /// Current player. Contains null if the game is over.
        /// </summary>
        public ICell<Player> CurrentPlayer { get { return currentPlayer; } }

        /// <summary>
        /// True if game is over, false otherwise.
        /// </summary>
        public ICell<bool> IsGameOver { get { return isGameOver; } }

        /// <summary>
        /// Player with most stones. Null if there's a tie.
        /// </summary>
        public ICell<Player> PlayerWithMostStones { get { return playerWithMostStones; } }

        /// <summary>
        /// Returns the number of stones the specified player has.
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>Number of stones</returns>
        public ICell<int> StoneCount( Player player )
        {
            if ( player == null )
            {
                throw new ArgumentNullException( "player" );
            }
            else
            {
                return stoneCounts[player.ArrayIndex];
            }
        }

        public IGrid<ISquare> Board { get { return board; } }

        private class Square : ISquare
        {
            private readonly Game game;

            private readonly Vector2D position;

            private readonly IDerived<Player> owner;

            private readonly IDerived<bool> isValidMove;

            public Square( Game game, Vector2D position )
            {
                this.game = game;
                this.position = position;
                this.owner = Cell.Derived( () => game.GetOwnerAt( position ) );
                this.isValidMove = Cell.Derived( () => game.IsValidMove( position ) );
            }

            public Vector2D Position
            {
                get
                {
                    return position;
                }
            }

            public ICell<Player> Owner
            {
                get { return owner; }
            }

            public ICell<bool> IsValidMove
            {
                get { return isValidMove; }
            }

            public void PlaceStone()
            {
                game.PlaceStone( position );
            }

            public void Refresh()
            {
                owner.Refresh();
                isValidMove.Refresh();
            }

            public IEnumerable<Vector2D> CapturedBy()
            {
                return game.gameState.Captured( position );
            }
        }
    }

    public interface ISquare
    {
        /// <summary>
        /// Position of the square in the grid.
        /// </summary>
        Vector2D Position { get; }

        /// <summary>
        /// Owner of the square. If the square is empty, Owner is null.
        /// </summary>
        ICell<Player> Owner { get; }

        /// <summary>
        /// True if the current player can place his next stone on this square.
        /// </summary>
        ICell<bool> IsValidMove { get; }

        /// <summary>
        /// Returns a list of positions which would be captured if the next player
        /// were to but his next stone on this square.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Vector2D> CapturedBy();

        /// <summary>
        /// Puts the next player's stone here.
        /// </summary>
        void PlaceStone();
    }
}
