using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.DataStructures
{
    public interface IGrid<out T>
    {
        T this[Vector2D position] { get; }

        int Width { get; }

        int Height { get; }
    }

    public static class IGridExtensions
    {
        public static bool IsValidPosition<T>( this IGrid<T> grid, Vector2D position )
        {
            return 0 <= position.X && position.X < grid.Width && 0 <= position.Y && position.Y < grid.Height;
        }

        public static IEnumerable<Vector2D> AllPositions<T>( this IGrid<T> grid )
        {
            return from y in Enumerable.Range( 0, grid.Height )
                   from x in Enumerable.Range( 0, grid.Width )
                   select new Vector2D( x, y );
        }

        public static ISequence<T> Slice<T>( this IGrid<T> grid, Vector2D start, Vector2D direction )
        {
            if ( !direction.IsDirection )
            {
                throw new ArgumentException( "Should be a direction", "direction" );
            }
            else
            {
                var hmin = -MaximumSteps( start.X, -direction.X, grid.Width );
                var hmax = MaximumSteps( start.X, direction.X, grid.Width );
                var vmin = -MaximumSteps( start.Y, -direction.Y, grid.Height );
                var vmax = MaximumSteps( start.Y, direction.Y, grid.Height );

                var startIndex = Math.Max( hmin, vmin );
                var end = Math.Min( hmax, vmax ) + 1;
                var length = end - startIndex;

                return new VirtualSequence<T>( startIndex, length, i => grid[start + i * direction] );
            }
        }

        public static ISequence<T> Row<T>( this IGrid<T> grid, int row )
        {
            return grid.Slice( new Vector2D( 0, row ), new Vector2D( 1, 0 ) );
        }

        public static IEnumerable<int> RowIndices<T>( this IGrid<T> grid )
        {
            return Enumerable.Range( 0, grid.Height );
        }

        public static IEnumerable<ISequence<T>> Rows<T>( this IGrid<T> grid )
        {
            return grid.RowIndices().Select( i => grid.Row( i ) );
        }

        public static ISequence<T> Column<T>( this IGrid<T> grid, int column )
        {
            return grid.Slice( new Vector2D( column, 0 ), new Vector2D( 0, 1 ) );
        }

        public static IEnumerable<int> ColumnIndices<T>( this IGrid<T> grid )
        {
            return Enumerable.Range( 0, grid.Width );
        }

        public static IEnumerable<ISequence<T>> Columns<T>( this IGrid<T> grid )
        {
            return grid.ColumnIndices().Select( i => grid.Column( i ) );
        }

        private static int MaximumSteps( int x, int dx, int max )
        {
            if ( dx == -1 )
            {
                return x;
            }
            else if ( dx == 0 )
            {
                return int.MaxValue;
            }
            else if ( dx == 1 )
            {
                return max - x - 1;
            }
            else
            {
                throw new ArgumentOutOfRangeException( "dx" );
            }
        }

        public static IGrid<R> Map<T, R>( this IGrid<T> grid, Func<T, R> function )
        {
            return new Grid<R>( grid.Width, grid.Height, p => function( grid[p] ) );
        }

        public static IGrid<R> VirtualMap<T, R>( this IGrid<T> grid, Func<T, R> function )
        {
            return new VirtualGrid<R>( grid.Width, grid.Height, p => function( grid[p] ) );
        }

        public static void EachPosition<T>( this IGrid<T> grid, Action<Vector2D> action )
        {
            foreach ( var position in grid.AllPositions() )
            {
                action( position );
            }
        }

        public static void Each<T>( this IGrid<T> grid, Action<T> action )
        {
            grid.EachPosition( p => action( grid[p] ) );
        }

        public static IGrid<T> FlipVertically<T>( this IGrid<T> grid )
        {
            return new VirtualGrid<T>( grid.Width, grid.Height, p => grid[new Vector2D( p.X, grid.Height - p.Y - 1 )] );
        }

        public static IGrid<T> FlipHorizontally<T>( this IGrid<T> grid )
        {
            return new VirtualGrid<T>( grid.Width, grid.Height, p => grid[new Vector2D( grid.Width - p.X - 1, p.Y )] );
        }

        public static int Count<T>( this IGrid<T> grid, Func<T, bool> function )
        {
            var count = 0;

            grid.Each( x => count += ( function( x ) ? 1 : 0 ) );

            return count;
        }
    }

    public static class Grid
    {
        public static IGrid<char> CreateCharacterGrid( params string[] strings )
        {
            var height = strings.Length;
            var width = strings[0].Length;

            if ( !strings.All( s => s.Length == width ) )
            {
                throw new ArgumentException( "All strings must have equal length" );
            }
            else
            {
                return new Grid<char>( width, height, p => strings[p.Y][p.X] );
            }
        }

        internal static bool EqualItems<T>( IGrid<T> xss, IGrid<T> yss )
        {
            if ( xss == null )
            {
                throw new ArgumentNullException( "xss" );
            }
            else if ( yss == null )
            {
                throw new ArgumentNullException( "yss" );
            }
            else
            {
                if ( xss.Width == yss.Width && xss.Height == yss.Height )
                {                    
                    return xss.AllPositions().All( p => xss[p] == null ? yss[p] == null : xss[p].Equals( yss[p] ) );
                }
                else
                {
                    return false;
                }
            }
        }

        internal static int HashCode<T>( IGrid<T> grid )
        {
            int result = 0;

            grid.Each( x => result ^= x.GetHashCode() );

            return result;
        }
    }

    public abstract class GridBase<T> : IGrid<T>
    {
        public override bool Equals( object obj )
        {
            return Equals( obj as IGrid<T> );
        }

        public bool Equals( IGrid<T> grid )
        {
            if ( grid == null )
            {
                return false;
            }
            else
            {
                return Grid.EqualItems( this, grid );
            }
        }

        public override int GetHashCode()
        {
            return Grid.HashCode( this );
        }

        public abstract T this[Vector2D position] { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }
    }

    public class Grid<T> : GridBase<T>
    {
        private readonly T[,] items;

        public Grid( int width, int height, Func<Vector2D, T> initializer )
        {
            items = new T[width, height];

            foreach ( var x in Enumerable.Range( 0, width ) )
            {
                foreach ( var y in Enumerable.Range( 0, height ) )
                {
                    var position = new Vector2D( x, y );

                    items[x, y] = initializer( position );
                }
            }
        }

        public Grid( int width, int height, T initialValue = default(T) )
            : this( width, height, p => initialValue )
        {
            // NOP
        }

        public override int Width
        {
            get
            {
                return items.GetLength( 0 );
            }
        }

        public override int Height
        {
            get
            {
                return items.GetLength( 1 );
            }
        }

        public override T this[Vector2D position]
        {
            get
            {
                return items[position.X, position.Y];
            }
        }
    }

    public class VirtualGrid<T> : GridBase<T>
    {
        private readonly Func<Vector2D, T> function;
        private readonly int width;
        private readonly int height;

        public VirtualGrid( int width, int height, Func<Vector2D, T> function )
        {
            this.function = function;
            this.width = width;
            this.height = height;
        }

        public override int Width
        {
            get
            {
                return width;
            }
        }

        public override int Height
        {
            get
            {
                return height;
            }
        }

        public override T this[Vector2D position]
        {
            get
            {
                if ( !this.IsValidPosition( position ) )
                {
                    throw new ArgumentOutOfRangeException( "position" );
                }
                else
                {
                    return function( position );
                }
            }
        }
    }
}
