using Reversi.Cells;
using Reversi.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Domain
{
    internal class GameBoard
    {
        private readonly IGrid<IVar<Player>> grid;

        private readonly IGrid<Player> view;

        public static GameBoard CreateDefault()
        {
            return new GameBoard( CreateDefaultInitialGrid() );
        }

        public static GameBoard Create( IGrid<Player> grid )
        {
            return new GameBoard( grid.Map( p => new Var<Player>( p ) ) );
        }

        private GameBoard( IGrid<IVar<Player>> grid )
        {
            this.grid = grid;
            view = grid.VirtualMap( cell => cell.Value );
        }

        private static IGrid<IVar<Player>> CreateDefaultInitialGrid()
        {
            return new Grid<Cell<Player>>( 8, 8, p => new Cell<Player>( StoneAtPositionInDefaultInitialGrid( p ) ) );
        }

        private static Player StoneAtPositionInDefaultInitialGrid( Vector2D position )
        {
            if ( position == new Vector2D( 3, 4 ) || position == new Vector2D( 4, 3 ) )
            {
                return Player.ONE;
            }
            else if ( position == new Vector2D( 3, 3 ) || position == new Vector2D( 4, 4 ) )
            {
                return Player.TWO;
            }
            else
            {
                return null;
            }
        }

        public int? FindDistanceToCaptureTwin( Vector2D position, Vector2D direction, Player player )
        {
            var slice = grid.Slice( position, direction );

            for ( var i = 1; i < slice.End; ++i )
            {
                if ( slice[i].Value == null )
                {
                    return null;
                }
                else if ( slice[i].Value == player )
                {
                    return i;
                }
            }

            return null;
        }

        public bool IsValidMove( Vector2D position, Player player )
        {
            if ( position == null )
            {
                throw new ArgumentNullException( "position" );
            }
            else if ( player == null )
            {
                throw new ArgumentNullException( "player" );
            }
            else if ( grid[position].Value == null )
            {
                foreach ( var direction in Vector2D.Directions )
                {
                    var distance = FindDistanceToCaptureTwin( position, direction, player );

                    if ( distance.HasValue && distance.Value > 1 )
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private IEnumerable<Vector2D> CapturedStones( Vector2D position, Vector2D direction, Player player )
        {
            var distanceToTwin = FindDistanceToCaptureTwin( position, direction, player );

            if ( distanceToTwin.HasValue )
            {
                for ( var i = 1; i != distanceToTwin; ++i )
                {
                    var p = position + i * direction;

                    yield return p;
                }
            }
        }

        public IEnumerable<Vector2D> CapturedStones( Vector2D position, Player player )
        {
            foreach ( var direction in Vector2D.Directions )
            {
                foreach ( var p in CapturedStones( position, direction, player ) )
                {
                    yield return p;
                }
            }
        }

        private void CaptureStones( Vector2D position, Player player )
        {
            foreach ( var p in CapturedStones( position, player ) )
            {
                grid[p].Value = player;
            }
        }

        public void PlaceStone( Vector2D position, Player player )
        {
            if ( !IsValidMove( position, player ) )
            {
                throw new ArgumentException( string.Format( "{0} cannot place stone on position {1}", player.ToString(), position.ToString() ) );
            }
            else
            {
                this.grid[position].Value = player;
                CaptureStones( position, player );
            }
        }

        public IEnumerable<Vector2D> ValidMoves( Player player )
        {
            return this.grid.AllPositions().Where( p => IsValidMove( p, player ) );
        }

        public bool HasValidMove( Player player )
        {
            return this.grid.AllPositions().Any( p => IsValidMove( p, player ) );
        }

        public IGrid<Player> View
        {
            get
            {
                return this.view;
            }
        }

        public int Width
        {
            get
            {
                return this.grid.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.grid.Height;
            }
        }

        public IEnumerable<Vector2D> AllPositions
        {
            get
            {
                return this.grid.AllPositions();
            }
        }

        public Player this[Vector2D position]
        {
            get
            {
                return this.grid[position].Value;
            }
        }

        public GameBoard Copy()
        {
            var grid = new Grid<ICell<Player>>( this.grid.Width, this.grid.Height, p => new Cell<Player>( this.grid[p].Value ) );

            return new GameBoard( grid );
        }

        public int CountStones( Player player )
        {
            return grid.Count( p => p.Value == player );
        }

        public Player WinningPlayer
        {
            get
            {
                var c1 = CountStones( Player.ONE );
                var c2 = CountStones( Player.TWO );

                if ( c1 > c2 )
                {
                    return Player.ONE;
                }
                else if ( c1 < c2 )
                {
                    return Player.TWO;
                }
                else
                {
                    return null;
                }
            }
        }

        public override int GetHashCode()
        {
            return this.grid.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as GameBoard );
        }

        public bool Equals( GameBoard gameBoard )
        {
            return this.grid.Equals( gameBoard.grid );
        }
    }
}
