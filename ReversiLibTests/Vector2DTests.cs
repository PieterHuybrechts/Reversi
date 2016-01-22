using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.DataStructures;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReversiLibTests
{
    [TestClass]
    public class Vector2DTests
    {
        [DebuggerStepThrough]
        private static Vector2D v( int x, int y )
        {
            return new Vector2D( x, y );
        }

        [TestMethod]
        public void Equality()
        {
            TestEquality( true, v( 0, 0 ), v( 0, 0 ) );
            TestEquality( true, v( 5, 3 ), v( 5, 3 ) );
            TestEquality( false, v( 0, 0 ), v( 1, 0 ) );
            TestEquality( false, v( 0, 0 ), v( 0, 1 ) );
            TestEquality( false, v( 1, 0 ), v( 0, 0 ) );
            TestEquality( false, v( 0, 1 ), v( 0, 0 ) );

            TestEquality( true, null, null );
            TestEquality( false, v( 0, 0 ), null );
            TestEquality( false, null, v( 0, 0 ) );
        }

        private void TestEquality( bool expected, Vector2D u, Vector2D v )
        {
            var actual = u == v;

            Assert.AreEqual( expected, actual, string.Format( "{0} == {1} should be equal to {2}", u, v, expected ) );
        }

        [TestMethod]
        public void Addition()
        {
            TestAddition( v( 0, 0 ), v( 0, 0 ), v( 0, 0 ) );
            TestAddition( v( 1, 0 ), v( 0, 0 ), v( 1, 0 ) );
            TestAddition( v( 1, 0 ), v( 1, 0 ), v( 0, 0 ) );
            TestAddition( v( 2, 0 ), v( 1, 0 ), v( 1, 0 ) );
            TestAddition( v( 0, 1 ), v( 0, 0 ), v( 0, 1 ) );
            TestAddition( v( 0, 1 ), v( 0, 1 ), v( 0, 0 ) );
            TestAddition( v( 0, 2 ), v( 0, 1 ), v( 0, 1 ) );
            TestAddition( v( 2, 3 ), v( 0, 3 ), v( 2, 0 ) );
        }

        private void TestAddition( Vector2D expected, Vector2D u, Vector2D v )
        {
            var actual = u + v;

            Assert.AreEqual( expected, actual, string.Format( "{0} + {1} should be equal to {2}", u, v, expected ) );
        }

        [TestMethod]
        public void Negation()
        {
            TestNegation( v( 0, 0 ), v( 0, 0 ) );
            TestNegation( v( -1, 0 ), v( 1, 0 ) );
            TestNegation( v( 0, -1 ), v( 0, 1 ) );
        }

        private void TestNegation( Vector2D expected, Vector2D v )
        {
            var actual = -v;

            Assert.AreEqual( expected, -v, string.Format( "Negation of {0} should be {1}", v, expected ) );
        }

        [TestMethod]
        public void Subtraction()
        {
            TestSubtraction( v( 0, 0 ), v( 0, 0 ), v( 0, 0 ) );
            TestSubtraction( v( 0, 0 ), v( 1, 0 ), v( 1, 0 ) );
            TestSubtraction( v( 1, 0 ), v( 2, 0 ), v( 1, 0 ) );
            TestSubtraction( v( 0, 1 ), v( 0, 1 ), v( 0, 0 ) );
            TestSubtraction( v( 0, 0 ), v( 0, 1 ), v( 0, 1 ) );
            TestSubtraction( v( 0, 1 ), v( 0, 2 ), v( 0, 1 ) );
            TestSubtraction( v( 3, 2 ), v( 5, 3 ), v( 2, 1 ) );
        }

        private void TestSubtraction( Vector2D expected, Vector2D u, Vector2D v )
        {
            var actual = u - v;

            Assert.AreEqual( expected, actual, string.Format( "{0} - {1} should be equal to {2}", u, v, expected ) );
        }

        [TestMethod]
        public void Multiplication()
        {
            TestMultiplication( v( 0, 0 ), v( 0, 0 ), 0 );
            TestMultiplication( v( 0, 0 ), v( 5, 3 ), 0 );
            TestMultiplication( v( 5, 3 ), v( 5, 3 ), 1 );
            TestMultiplication( v( 4, 14 ), v( 2, 7 ), 2 );
        }

        private void TestMultiplication( Vector2D expected, Vector2D v, int factor )
        {
            var actual = v * factor;

            Assert.AreEqual( expected, actual, string.Format( "{0} * {1} should be equal to {2}", v, factor, expected ) );
        }

        [TestMethod]
        public void Around()
        {
            TestAround( v( 0, 0 ), v( 1, 0 ), v( 1, 1 ), v( 0, 1 ), v( -1, 1 ), v( -1, 0 ), v( -1, -1 ), v( 0, -1 ), v( 1, -1 ) );
            TestAround( v( 3, 2 ), v( 4, 2 ), v( 4, 3 ), v( 3, 3 ), v( 2, 3 ), v( 2, 2 ), v( 2, 1 ), v( 3, 1 ), v( 4, 1 ) );
        }

        private void TestAround( Vector2D v, params Vector2D[] expected )
        {
            var expectedSet = new HashSet<Vector2D>( expected );
            var actualSet = new HashSet<Vector2D>( v.Around );

            AssertEqualElements( expectedSet, actualSet );
        }

        private static void AssertEqualElements<T>(ISet<T> xs, ISet<T> ys)
        {
            Assert.AreEqual( xs.Count, ys.Count );
            Assert.IsTrue( xs.IsSubsetOf( ys ) );
            Assert.IsTrue( xs.IsSupersetOf( ys ) );
        }
    }
}
