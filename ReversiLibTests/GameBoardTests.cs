using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.DataStructures;
using System.Collections.Generic;
using Reversi.Domain;

namespace ReversiLibTests
{
    [TestClass]
    public class GameBoardTests
    {
        private static void TestFindDistanceToCaptureTwin( Player player, Vector2D direction, int? expected, params string[] lines )
        {
            var grid = Grid.CreateCharacterGrid( lines );

            var board = GameBoard.Create( grid.Map( c =>
            {
                switch ( c )
                {
                    case '.':
                    case 'x':
                        return null;

                    case '1':
                        return Player.ONE;

                    case '2':
                        return Player.TWO;

                    default:
                        throw new ArgumentException();
                }
            } ) );

            var position = ( from pos in grid.AllPositions()
                             where grid[pos] == 'x'
                             select pos ).First();
            var actual = board.FindDistanceToCaptureTwin( position, direction, player );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void FindDistanceToCaptureTwin()
        {
            TestFindDistanceToCaptureTwin(
                Player.ONE, v( 1, 0 ), 1,
                "x1" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 1,
                "x2" );

            TestFindDistanceToCaptureTwin(
                Player.ONE, v( 1, 0 ), 2,
                "x21" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 2,
                "x12" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 3,
                "x112" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 3,
                "x1122" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 2,
                ".x122" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 3,
                ".x112" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), null,
                ".x111" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( -1, 0 ), null,
                ".x112" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( -1, 0 ), 1,
                "2x112" );

            TestFindDistanceToCaptureTwin(
                Player.ONE, v( -1, 0 ), 2,
                "12x221" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( -1, 0 ), 2,
                "21x112" );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 0 ), 3,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, 1 ), 2,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 0, 1 ), null,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( -1, 1 ), null,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( -1, 0 ), 1,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( -1, -1 ), null,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 0, -1 ), 2,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );

            TestFindDistanceToCaptureTwin(
                Player.TWO, v( 1, -1 ), 2,
                ".2.2.",
                ".11..",
                "2x112",
                "1.1..",
                ".2.2." );
        }

        private static void TestIsValidMove( params string[] lines )
        {
            var grid = Grid.CreateCharacterGrid( lines );

            var board = GameBoard.Create( grid.Map( c =>
            {
                switch ( c )
                {
                    case 'b':
                    case 'B':
                    case 'w':
                    case 'W':
                    case '.':
                        return null;

                    case '1':
                        return Player.ONE;

                    case '2':
                        return Player.TWO;

                    default:
                        throw new ArgumentException();
                }
            } ) );

            var position = ( from pos in grid.AllPositions()
                             where "BbWw".Contains( grid[pos] )
                             select pos ).First();

            var square = grid[position];
            var player = Char.ToLower( square ) == 'b' ? Player.ONE : Player.TWO;
            var actual = board.IsValidMove( position, player );
            var expected = Char.IsUpper( square );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void IsValidMove()
        {
            TestIsValidMove( "B21" );
            TestIsValidMove( "b1" );
            TestIsValidMove( "B221" );
            TestIsValidMove( "B2221" );
            TestIsValidMove( "b1221" );
            TestIsValidMove( "W12" );
            TestIsValidMove( "W112" );
            TestIsValidMove( "w2" );
            TestIsValidMove( "w212" );

            TestIsValidMove(
                ".1...",
                ".2...",
                ".2...",
                ".2...",
                ".B..."
                );

            TestIsValidMove(
                ".1...",
                ".2...",
                ".2.1.",
                ".12..",
                ".B..."
                );

            TestIsValidMove(
                ".1...",
                ".2...",
                ".2.1.",
                ".11..",
                ".b..."
                );

            TestIsValidMove(
                "2111W",
                ".....",
                ".....",
                ".....",
                "....."
                );

            TestIsValidMove(
                "211.w",
                ".....",
                ".....",
                ".....",
                "....."
                );

            TestIsValidMove(
                "....W",
                "...1.",
                "..1..",
                ".2...",
                "1...."
                );
        }

        private static void TestValidMoves( Player player, params string[] lines )
        {
            var grid = Grid.CreateCharacterGrid( lines );

            var board = GameBoard.Create( grid.Map( c =>
                {
                    switch ( c )
                    {
                        case '.':
                        case 'x':
                            return null;

                        case '1':
                            return Player.ONE;

                        case '2':
                            return Player.TWO;

                        default:
                            throw new ArgumentException();
                    }
                } ) );

            var expected = new HashSet<Vector2D>( from pos in grid.AllPositions()
                                                  where grid[pos] == 'x'
                                                  select pos );
            var actual = new HashSet<Vector2D>( board.ValidMoves( player ) );

            AssertEqualElements( expected, actual );
        }

        [TestMethod]
        public void ValidMoves()
        {
            TestValidMoves( Player.ONE, "x21" );
            TestValidMoves( Player.TWO, "x12" );
            TestValidMoves( Player.ONE, "x221" );
            TestValidMoves( Player.TWO, "x112" );
            TestValidMoves( Player.ONE, ".x221." );
            TestValidMoves( Player.TWO, ".x112." );

            TestValidMoves( Player.ONE,
                "....",
                "....",
                "....",
                "...."
                );

            TestValidMoves( Player.ONE,
                "x...",
                "2...",
                "1...",
                "...."
                );

            TestValidMoves( Player.ONE,
                "x...",
                "2...",
                "2...",
                "1..."
                );

            TestValidMoves( Player.ONE,
                ".1..",
                ".2..",
                ".2..",
                ".x.."
                );

            TestValidMoves( Player.ONE,
                "..1.",
                "..2.",
                "..2.",
                "..x."
                );

            TestValidMoves( Player.ONE,
                "x...",
                ".2..",
                "..1.",
                "...."
                );

            TestValidMoves( Player.ONE,
                "1...",
                ".2..",
                "..x.",
                "...."
                );

            TestValidMoves( Player.ONE,
                "...x",
                "..2.",
                ".1..",
                "...."
                );

            TestValidMoves( Player.ONE,
                "...1",
                "..2.",
                ".x..",
                "...."
                );

            TestValidMoves( Player.ONE,
                "x221",
                "22..",
                "2.2.",
                "1..1"
                );

            TestValidMoves( Player.ONE,
                "..1..",
                "..2..",
                "12x21",
                "..2..",
                "..1.."
                );

            TestValidMoves( Player.ONE,
                ".xxx.",
                "xx2xx",
                ".222.",
                "11111"
                );
        }

        private void TestCapturedStones( Player player, params string[] lines )
        {
            var grid = Grid.CreateCharacterGrid( lines );

            var board = GameBoard.Create( grid.Map( c =>
            {
                switch ( c )
                {
                    case '.':
                    case 'x':
                        return null;

                    case 'b':
                    case 'B':
                        return Player.ONE;

                    case 'w':
                    case 'W':
                        return Player.TWO;

                    default:
                        throw new ArgumentException();
                }
            } ) );

            var position = grid.AllPositions().Where( p => grid[p] == 'x' ).First();
            var expected = new HashSet<Vector2D>( from pos in grid.AllPositions()
                                                  where grid[pos] == 'B' || grid[pos] == 'W'
                                                  select pos );
            var actual = new HashSet<Vector2D>( board.CapturedStones( position, player ) );

            AssertEqualElements( expected, actual );
        }

        [TestMethod]
        public void CapturedStones()
        {
            TestCapturedStones( Player.ONE, "xWb" );
            TestCapturedStones( Player.TWO, "xBw" );
            TestCapturedStones( Player.ONE, "xWWb" );
            TestCapturedStones( Player.TWO, "xBBw" );
            TestCapturedStones( Player.ONE, "bWWx" );
            TestCapturedStones( Player.TWO, "wBBx" );

            TestCapturedStones( Player.ONE,
                "..b..",
                "..W..",
                "bWxWb",
                "..W..",
                "..b.."
                );

            TestCapturedStones( Player.ONE,
                "b.b..",
                ".WW..",
                "bWxWb",
                "..W..",
                "..b.."
                );

            TestCapturedStones( Player.ONE,
                "b.b..",
                ".WWw.",
                "bWxWb",
                "..Ww.",
                "..b.."
                );

            TestCapturedStones( Player.ONE,
                "b...b",
                "W..W.",
                "W.W..",
                "WW...",
                "xwww."
                );
        }

        private void TestPlaceStone( Player player, string[] before, string[] after )
        {
            var beforeGrid = Grid.CreateCharacterGrid( before );
            var board = GameBoard.Create( beforeGrid.Map( c =>
            {
                switch ( c )
                {
                    case '.':
                    case 'x':
                        return null;

                    case '1':
                        return Player.ONE;

                    case '2':
                        return Player.TWO;

                    default:
                        throw new ArgumentException();
                }
            } ) );

            var expected = GameBoard.Create( Grid.CreateCharacterGrid( after ).Map( c =>
            {
                switch ( c )
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
            } ) );

            var position = beforeGrid.AllPositions().Where( p => beforeGrid[p] == 'x' ).First();
            board.PlaceStone( position, player );

            AssertEqualBoards( expected, board );
        }

        [TestMethod]
        public void PlaceStone()
        {
            TestPlaceStone( Player.ONE,
                new string[] { "x21" },
                new string[] { "111" } );

            TestPlaceStone( Player.TWO,
                new string[] { "x12" },
                new string[] { "222" } );

            TestPlaceStone( Player.ONE,
                new string[] { "x221" },
                new string[] { "1111" } );

            TestPlaceStone( Player.TWO,
                new string[] { "x112" },
                new string[] { "2222" } );

            TestPlaceStone( Player.ONE,
                new string[] { 
                    "x2221",
                    "22...",
                    "2.2..",
                    "2..1.",
                    "1...."
                },
                new string[] { 
                    "11111",
                    "11...",
                    "1.1..",
                    "1..1.",
                    "1...."
                } );
        }

        private static Vector2D v( int x, int y )
        {
            return new Vector2D( x, y );
        }

        private static GameBoard Board( params string[] rows )
        {
            var rs = new Sequence<string>( rows ).Map( row => Sequence.FromString( row ).Map( ParsePlayer ) );
            var grid = new Grid<Player>( rs[0].Length, rs.Length, v => rs[v.Y][v.X] );

            return GameBoard.Create( grid );
        }

        private static Player ParsePlayer( char c )
        {
            switch ( c )
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
        }

        private static void AssertEqualElements<T>( ISet<T> xs, ISet<T> ys )
        {
            Assert.AreEqual( xs.Count, ys.Count );

            foreach ( var x in xs )
            {
                Assert.IsTrue( ys.Contains( x ), string.Format( "{0} should be in {1}", x, string.Join( ",", ys.Select( y => y.ToString() ) ) ) );
            }

            Assert.IsTrue( xs.IsSupersetOf( ys ) );
        }

        private static void AssertEqualBoards( GameBoard expected, GameBoard actual )
        {
            Assert.AreEqual( expected.Width, actual.Width, "Boards should have equal widths" );
            Assert.AreEqual( expected.Height, actual.Height, "Boards should have equal heights" );

            foreach ( var p in expected.AllPositions )
            {
                var expectedValue = expected[p];
                var actualValue = actual[p];

                Assert.AreEqual( expectedValue, actualValue, string.Format( "Values at position {0} should be equal ({1} != {2})", p, expectedValue, actualValue ) );
            }
        }
    }
}
