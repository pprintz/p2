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
            Tile newtile = new Tile(Int32.MaxValue, Int32.MinValue);
            Assert.AreEqual(Int32.MaxValue, newtile.X);
            Assert.AreEqual(Int32.MinValue, newtile.Y);
            Assert.AreEqual(Tile.Types.Free, tile.Type);
        }
        [Test]
        public void DiagonalDistanceToIfXDistanceIsSmalerThenYDistanceTest()
        {
            BuildingBlock pointOne = new BuildingBlock(0, 0);
            BuildingBlock pointTwo = new BuildingBlock(2, 1);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(1 + Math.Sqrt(2), distance);
        }
        [Test]
        public void DiagonalDistanceToIfYDistanceIsSmalerThenXDistanceTest()
        {
            BuildingBlock pointOne = new BuildingBlock(1, 2);
            BuildingBlock pointTwo = new BuildingBlock(1, 1);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(1, distance);
        }
        [Test]
        public void DiagonalDistanceToIfYDistanceIsEqualToXDistanceTest()
        {
            BuildingBlock pointOne = new BuildingBlock(10, 10);
            BuildingBlock pointTwo = new BuildingBlock(0, 0);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(10 * Math.Sqrt(2), distance);
        }
        [Test]
        public void DiagonalDistanceIntMax()
        {
            BuildingBlock pointOne = new BuildingBlock(0, 0);
            BuildingBlock pointTwo = new BuildingBlock(Int32.MaxValue, Int32.MaxValue);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual((Int32.MaxValue) * Math.Sqrt(2), distance);
        }
        [Test]
        public void DiagonalDistanceHalfOfIntMax()
        {
            BuildingBlock pointOne = new BuildingBlock(Int32.MaxValue / 2, Int32.MaxValue / 2);
            BuildingBlock pointTwo = new BuildingBlock(0, 0);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual((Int32.MaxValue / 2) * Math.Sqrt(2), distance);
        }
        [Test]
        public void DiagonalDistanceIntMin()
        {
            int min = Int32.MinValue + 1;
            BuildingBlock pointOne = new BuildingBlock(min, min);
            BuildingBlock pointTwo = new BuildingBlock(0, 0);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual((Math.Abs(min) * Math.Sqrt(2)), distance);
        }
        [Test]
        public void DiagonalDistanceHalfOfIntMin()
        {
            int min = (Int32.MinValue + 1) / 2;
            BuildingBlock pointOne = new BuildingBlock(min, min);
            BuildingBlock pointTwo = new BuildingBlock(0, 0);
            var distance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual((Math.Abs(min) * Math.Sqrt(2)), distance);
        }

        [Test]
        public void CompareDiagonalAndEuclid()
        {
            BuildingBlock pointOne = new BuildingBlock(1, 1);
            BuildingBlock pointTwo = new BuildingBlock(0, 0);
            var eDistance = pointOne.DistanceTo(pointTwo);
            var dDistance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(eDistance, dDistance);

            pointOne = new BuildingBlock(1, 2);
            pointTwo = new BuildingBlock(0, 0);
            eDistance = pointOne.DistanceTo(pointTwo);
            dDistance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreNotEqual(eDistance, dDistance);

            pointOne = new BuildingBlock(0, 1);
            pointTwo = new BuildingBlock(0, 0);
            eDistance = pointOne.DistanceTo(pointTwo);
            dDistance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreEqual(eDistance, dDistance);

            pointOne = new BuildingBlock(Int32.MaxValue, Int32.MaxValue - 1);
            pointTwo = new BuildingBlock(0, 0);
            eDistance = pointOne.DistanceTo(pointTwo);
            dDistance = pointOne.DiagonalDistanceTo(pointTwo);
            Assert.AreNotEqual(eDistance, dDistance);
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