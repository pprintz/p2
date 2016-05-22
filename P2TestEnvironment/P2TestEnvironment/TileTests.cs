using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Evacuation_Master_3000;
using NUnit.Framework;

namespace P2TestEnvironment
{
    [TestFixture]
    class TileTests
    {
        [Test]
        public void TileConstructorTest()
        {
            Tile tile = new Tile(0, 0);
            Assert.AreEqual(0, tile.X);
            Assert.AreEqual(0, tile.Y);
            Assert.AreEqual(0, tile.Z);
            Assert.AreEqual(Tile.Types.Free, tile.Type);
        }

        [Test]
        public void DiagonalDistanceToIfXDistanceIsSmalerThenYDistanceTest()
        {
            BuildingBlock pointOne = new BuildingBlock(0, 0);
            BuildingBlock pointTwo = new BuildingBlock(2, 1);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(24, distance);
        }

        [Test]
        public void DiagonalDistanceToIfYDistanceIsSmalerThenXDistanceTest()
        {
            BuildingBlock pointOne = new BuildingBlock(1, 2);
            BuildingBlock pointTwo = new BuildingBlock(1, 1);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(10, distance);
        }

        [Test]
        public void DistanceTo()
        {
            BuildingBlock pointOne = new BuildingBlock(1, 1, 1);
            BuildingBlock pointTwo = new BuildingBlock(1, 1, 0);
            var distance = pointOne.DistanceTo(pointTwo);
            Assert.AreEqual(15, distance);
        }
    }
}