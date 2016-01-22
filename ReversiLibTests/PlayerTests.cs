using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Domain;

namespace ReversiLibTests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Other()
        {
            Assert.AreSame( Player.ONE, Player.TWO.Other );
            Assert.AreSame( Player.TWO, Player.ONE.Other );
        }
    }
}
