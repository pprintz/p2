using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evacuation_Master_3000;
using NUnit.Framework;

namespace P2TestEnvironment
{
    [TestFixture]
    class BuildingBlockTests
    {
        [Test]
        public void BuildingBlockStart()
        {
            BuildingBlock buildingBlock = new BuildingBlock(0, 0);
            Assert.AreEqual(0, buildingBlock.X);
            Assert.AreEqual(0, buildingBlock.Y);
            Assert.AreEqual(0, buildingBlock.HeatmapCounter);
            Assert.AreEqual(false, buildingBlock.IsChecked);
            Assert.AreEqual(double.MaxValue, buildingBlock.LengthFromSource);
            Assert.AreEqual(0, buildingBlock.LengthToDestination);
            Assert.AreEqual(null, buildingBlock.Parent);
            Assert.AreEqual(new HashSet<Tile>(), buildingBlock.Neighbours);
            Assert.AreEqual(Tile.Types.Free, buildingBlock.Type);
        }
    }
}
