using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Domain;
using Reversi.DataStructures;
using Reversi.Cells;

namespace ReversiLibTests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void CreateNew()
        {
            var game = Game.CreateNew();

            var expectedBoard = Board(
                "........",
                "........",
                "........",
                "...21...",
                "...12...",
                "........",
                "........",
                "........" );

            Assert.AreEqual( 8, game.Board.Width );
            Assert.AreEqual( 8, game.Board.Height );
            AssertEqualGrids( expectedBoard, game.Board.VirtualMap( sqr => sqr.Owner.Value ) );

            Assert.AreSame( Player.ONE, game.CurrentPlayer.Value );
            Assert.AreEqual( 2, game.StoneCount( Player.ONE ).Value );
            Assert.AreEqual( 2, game.StoneCount( Player.TWO ).Value );
            Assert.IsFalse( game.IsGameOver.Value );
            Assert.IsNull( game.PlayerWithMostStones.Value );
        }

        [TestMethod]
        public void CreateInProgress()
        {
            var board = Board(
                ".112",
                "....",
                "....",
                "...." );

            var game = Game.CreateInProgress( board, Player.TWO );

            Assert.AreEqual( 4, game.Board.Width );
            Assert.AreEqual( 4, game.Board.Height );
            AssertEqualGrids( board, game.Board.VirtualMap( sqr => sqr.Owner.Value ) );

            Assert.AreSame( Player.TWO, game.CurrentPlayer.Value );
            Assert.AreEqual( 2, game.StoneCount( Player.ONE ).Value );
            Assert.AreEqual( 1, game.StoneCount( Player.TWO ).Value );
            Assert.IsFalse( game.IsGameOver.Value );
            Assert.AreSame( Player.ONE, game.PlayerWithMostStones.Value );
        }

        [TestMethod]
        public void PlaceStone()
        {
            var initialBoard = Board(
                ".112",
                "....",
                "....",
                "...." );

            var expectedBoard = Board(
                "2222",
                "....",
                "....",
                "...." );

            var game = Game.CreateInProgress( initialBoard, Player.TWO );
            var position = new Vector2D( 0, 0 );

            Assert.IsTrue( game.Board[position].IsValidMove.Value );

            game.Board[position].PlaceStone();

            AssertEqualGrids( expectedBoard, game.Board.VirtualMap( sqr => sqr.Owner.Value ) );

            Assert.IsNull( game.CurrentPlayer.Value );
            Assert.AreEqual( 0, game.StoneCount( Player.ONE ).Value );
            Assert.AreEqual( 4, game.StoneCount( Player.TWO ).Value );
            Assert.IsTrue( game.IsGameOver.Value );
            Assert.AreSame( Player.TWO, game.PlayerWithMostStones.Value );
        }

        [TestMethod]
        public void CurrentPlayerGetsChangeNotification()
        {
            var initialBoard = Board(
                ".112",
                "....",
                "....",
                "122." );

            var expectedBoard = Board(
                "2222",
                "....",
                "....",
                "122." );

            var game = Game.CreateInProgress( initialBoard, Player.TWO );
            var position = new Vector2D( 0, 0 );

            var notified = false;
            game.CurrentPlayer.PropertyChanged += ( cell, args ) => notified = true;

            game.Board[position].PlaceStone();

            Assert.IsTrue( notified );
        }

        [TestMethod]
        public void SquareOwnerGetsChangeNotification()
        {
            var initialBoard = Board(
                ".112",
                "....",
                "....",
                "122." );

            var expectedBoard = Board(
                "2222",
                "....",
                "....",
                "122." );

            var game = Game.CreateInProgress( initialBoard, Player.TWO );
            var position = new Vector2D( 0, 0 );

            IVar<int> notificationCount = new Var<int>( 0 );

            game.Board.Each( square => { ObserveChange( square.Owner, notificationCount ); } );
            
            game.Board[position].PlaceStone();

            Assert.AreEqual( 3, notificationCount.Value );
        }

        private static void ObserveChange<T>( ICell<T> cell, IVar<int> counter )
        {
            cell.PropertyChanged += ( c, args ) => { counter.Value++; };
        }

        private void AssertEqualGrids<T>( IGrid<T> expected, IGrid<T> actual )
        {
            Assert.AreEqual( expected.Width, actual.Width, "Grids should have equal widths" );
            Assert.AreEqual( expected.Height, actual.Height, "Grids should have equal heights" );

            foreach ( var p in expected.AllPositions() )
            {
                var expectedValue = expected[p];
                var actualValue = actual[p];

                Assert.AreEqual( expectedValue, actualValue, string.Format( "Values at position {0} should be equal ({1} != {2})", p, expectedValue, actualValue ) );
            }
        }

        private IGrid<Player> Board( params string[] lines )
        {
            var grid = Grid.CreateCharacterGrid( lines );

            return grid.Map( c =>
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
            } );
        }
    }
}
