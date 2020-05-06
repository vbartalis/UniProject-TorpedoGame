using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using torpedo2.data;

namespace torpedo2.Tests
{
    [TestClass]
    public class TorpedoTests
    {
        [TestMethod]
        public void AiTest()
        {
            AI ai = new AI();
            int gameSize = 10;

            GameMap gameMap = new GameMap();
            gameMap.InitializePozitions();
            gameMap.Positions[5, 5].Hit = true;
            gameMap.Positions[5, 5].Ship = 2;

            Position position = ai.SearchForTarget(gameSize, gameMap.Positions);

            Assert.AreEqual(gameMap.Positions[4, 5], position);

        }
    }
}
