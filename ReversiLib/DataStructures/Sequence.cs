using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.DataStructures
{
    public interface ISequence<out T>
    {
        T this[int index] { get; }

        int Start { get; }

        int End { get; }

        int Length { get; }
    }

    public static class Sequence
    {
        public static Sequence<char> FromString( string str )
        {
            return new Sequence<char>( str.ToCharArray() );
        }
    }

    public static class ISequenceExtensions
    {
        public static bool IsValidIndex<T>( this ISequence<T> xs, int index )
        {
            return xs.Start <= index && index < xs.End;
        }

        public static bool IsEmpty<T>( this ISequence<T> xs )
        {
            return xs.Length == 0;
        }

        public static IEnumerable<int> Indices<T>( this ISequence<T> xs )
        {
            return Enumerable.Range( xs.Start, xs.Length );
        }

        public static IEnumerable<T> ToEnumerable<T>( this ISequence<T> xs )
        {
            return xs.Indices().Select( i => xs[i] );
        }

        public static ISequence<R> Map<T, R>( this ISequence<T> xs, Func<T, R> func )
        {
            return new VirtualSequence<R>( xs.Start, xs.Length, i => func( xs[i] ) );
        }

        public static ISequence<T> Slice<T>( this ISequence<T> xs, int start, int length )
        {
            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( "length" );
            }
            else if ( start + length >= xs.Length )
            {
                throw new ArgumentException( "length" );
            }
            else
            {
                return new VirtualSequence<T>( length, i => xs[start + i] );
            }
        }

        public static ISequence<T> Slice<T>( this ISequence<T> xs, int start )
        {
            return xs.Slice( start, xs.Length - start );
        }

        public static int? FindIndex<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            foreach ( var i in xs.Indices() )
            {
                var item = xs[i];

                if ( predicate( item ) )
                {
                    return i;
                }
            }

            return null;
        }

        public static int? FindIndex<T>( this ISequence<T> xs, T x )
        {
            return xs.FindIndex( y => x == null ? y == null : x.Equals( y ) );
        }

        public static bool EqualItems<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            if ( xs == null || ys == null )
            {
                throw new ArgumentNullException();
            }
            else
            {
                if ( xs.Start != ys.Start || xs.End != ys.End )
                {
                    return false;
                }
                else
                {
                    foreach ( var index in xs.Indices() )
                    {
                        if ( !xs[index].Equals( ys[index] ) )
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        public static int ComputeHashCode<T>( this ISequence<T> xs )
        {
            return xs.ToEnumerable().Select( x => x.GetHashCode() ).Aggregate( 0, ( x, y ) => x ^ y );
        }

        public static string Join( this ISequence<string> xs, string separator )
        {
            return String.Join( separator, xs.ToEnumerable() );
        }

        public static void Each<T>( this ISequence<T> xs, Action<T> action )
        {
            foreach ( var x in xs.ToEnumerable() )
            {
                action( x );
            }
        }
    }

    public abstract class SequenceBase<T> : ISequence<T>
    {
        public abstract int Start { get; }

        public abstract int End { get; }

        public abstract int Length { get; }

        public abstract T this[int index] { get; }

        public override bool Equals( object obj )
        {
            return Equals( obj as ISequence<T> );
        }

        public bool Equals( ISequence<T> xs )
        {
            if ( xs == null )
            {
                return false;
            }
            else
            {
                return this.EqualItems( xs );
            }
        }

        public override int GetHashCode()
        {
            return this.ComputeHashCode();
        }

        public override string ToString()
        {
            return string.Format( "[{0}]", this.Map( x => x.ToString() ).Join( ", " ) );
        }
    }

    public class Sequence<T> : SequenceBase<T>
    {
        private readonly int startIndex;

        private readonly T[] items;

        public Sequence( int startIndex, int length, Func<int, T> initializer )
        {
            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( "length", "Must be positive" );
            }
            else
            {
                this.startIndex = startIndex;
                items = ( from i in Enumerable.Range( 0, length )
                          select initializer( i ) ).ToArray();
            }
        }

        public Sequence( int startIndex, int length, T initialValue = default(T) )
            : this( startIndex, length, i => initialValue )
        {
            // NOP
        }

        public Sequence( int length, Func<int, T> initializer )
            : this( 0, length, initializer )
        {
            // NOP
        }

        public Sequence( int length, T initialValue = default(T) )
            : this( length, i => initialValue )
        {
            // NOP
        }

        public Sequence( params T[] xs )
            : this( xs.Length, i => xs[i] )
        {
            // NOP
        }

        public override T this[int index]
        {
            get
            {
                return items[index - startIndex];
            }
        }

        public override int Start
        {
            get
            {
                return startIndex;
            }
        }

        public override int End
        {
            get
            {
                return startIndex + Length;
            }
        }

        public override int Length
        {
            get
            {
                return items.Length;
            }
        }
    }

    public class VirtualSequence<T> : SequenceBase<T>
    {
        private readonly int startIndex;

        private readonly int length;

        private readonly Func<int, T> fetcher;

        public VirtualSequence( int startIndex, int length, Func<int, T> fetcher )
        {
            this.startIndex = startIndex;
            this.length = length;
            this.fetcher = fetcher;
        }

        public VirtualSequence( int length, Func<int, T> fetcher )
            : this( 0, length, fetcher )
        {
            // NOP
        }

        public override int Length
        {
            get
            {
                return length;
            }
        }

        public override int Start
        {
            get
            {
                return startIndex;
            }
        }

        public override int End
        {
            get
            {
                return startIndex + length;
            }
        }

        public override T this[int index]
        {
            get
            {
                if ( !this.IsValidIndex( index ) )
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    return fetcher( index );
                }
            }
        }
    }
}
