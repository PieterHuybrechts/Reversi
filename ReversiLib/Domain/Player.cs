using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Domain
{
    public abstract class Player
    {
        /// <summary>
        /// Player object that represents player one
        /// </summary>
        public static readonly Player ONE = new PlayerOne();

        /// <summary>
        /// Player object that represents player two
        /// </summary>
        public static readonly Player TWO = new PlayerTwo();

        /// <summary>
        /// This player's competitor. For example, ONE.Other == TWO.
        /// </summary>
        public abstract Player Other { get; }

        /// <summary>
        /// Returns the index of this player. Player one has index 0,
        /// player two has index 1.
        /// </summary>
        public abstract int ArrayIndex { get; }

        public override int GetHashCode()
        {
            return ArrayIndex;
        }

        public override bool Equals( object obj )
        {
            return object.ReferenceEquals( this, obj );
        }

        private class PlayerOne : Player
        {
            public override Player Other
            {
                get { return TWO; }
            }

            public override string ToString()
            {
                return "Player 1";
            }

            public override int ArrayIndex
            {
                get { return 0; }
            }
        }

        private class PlayerTwo : Player
        {
            public override Player Other
            {
                get { return ONE; }
            }

            public override string ToString()
            {
                return "Player 2";
            }

            public override int ArrayIndex
            {
                get { return 1; }
            }
        }
    }
}
