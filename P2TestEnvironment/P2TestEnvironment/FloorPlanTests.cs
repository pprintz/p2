using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evacuation_Master_3000;
using NUnit.Framework.Internal;
using NUnit.Framework;
using static Evacuation_Master_3000.ImportExportSettings;

namespace P2TestEnvironment
{
    [TestFixture]
    class FloorPlanTests
    {
        /// <summary>
        /// Tests whether the floorplan initializes to flooramount  = 0 and with nul description.
        /// </summary>
        [Test]
        public void FloorPlanConstructorTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            Assert.AreEqual(0, floorPlan.FloorAmount);
            Assert.AreEqual(null, floorPlan.Description);
        }

        /// <summary>
        /// Tests whether the constructor workes correctly for the floorplan.
        /// </summary>
        [Test]
        public void FloorPlanCreateTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(10, 10, 1, "test", new[] { "1", "2" });
            Assert.AreEqual(10, floorPlan.Width);
            Assert.AreEqual(10, floorPlan.Height);
            Assert.AreEqual(1, floorPlan.FloorAmount);
            Assert.AreEqual("test", floorPlan.Description);
            Assert.AreEqual(new[] { "1", "2" }, floorPlan.Headers);
        }

        /// <summary>
        /// Tests whether or not the tiles has the correct neighbours
        /// </summary>
        //[Test]
        public void FloorPlanTileNeighbourTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(3, 3, 1, "test", new[] { "1" });
            floorPlan.Initiate();
            Assert.AreEqual(9, floorPlan.BuildingBlocks.Values.Count);
            Assert.AreEqual(9, floorPlan.Tiles.Count);
            Tile tile;
            floorPlan.Tiles.TryGetValue(Coordinate(1, 1, 0), out tile);
            Assert.AreNotEqual(null, tile);
            Assert.AreEqual(8, tile.Neighbours.Count); //Fejl, tile naboer bliver ikke initialiseret

            Tile otherTile;
            int x = 0, y = 0;
            foreach (Tile t in tile.Neighbours)
            {
                if (x == 1 && y == 1)
                {
                    x++;
                }
                floorPlan.Tiles.TryGetValue(Coordinate(x, y, 0), out otherTile);
                Assert.AreNotEqual(null, otherTile);
                Assert.AreEqual(otherTile, t);
                x++;
                if (x == 3)
                {
                    x = 0;
                    y++;
                }
            }
            Assert.AreEqual(0, x);
            Assert.AreEqual(3, y);
        }

        /// <summary>
        /// Tests whether or not the building blocks has the correct neighbours
        /// </summary>
        [Test]
        public void FloorPlanBuildingBlockNeighbourTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(3, 3, 1, "test", new[] { "1" });
            floorPlan.Initiate();
            Assert.AreEqual(9, floorPlan.BuildingBlocks.Count);
            Assert.AreEqual(9, floorPlan.Tiles.Count);
            BuildingBlock block;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 1, 0), out block);
            Assert.AreNotEqual(null, block);
            Assert.AreEqual(8, block.BuildingBlockNeighbours.Count);

            BuildingBlock otherBlock;
            int x = 0, y = 0;
            foreach (BuildingBlock buildingBlock in block.BuildingBlockNeighbours)
            {
                if (x == 1 && y == 1)
                {
                    x++;
                }
                floorPlan.BuildingBlocks.TryGetValue(Coordinate(x, y, 0), out otherBlock);
                Assert.AreNotEqual(null, otherBlock);
                Assert.AreEqual(otherBlock, buildingBlock);
                x++;
                if (x == 3)
                {
                    x = 0;
                    y++;
                }
            }
            Assert.AreEqual(0, x);
            Assert.AreEqual(3, y);

            floorPlan.BuildingBlocks.TryGetValue(Coordinate(0, 0, 0), out block);
            Assert.AreNotEqual(null, block);
            Assert.AreEqual(3, block.BuildingBlockNeighbours.Count);

            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 0, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(0, 1, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 1, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
        }

        /// <summary>
        /// Tests whether a wall is included as a neighbour in an open space next it.      
        /// </summary>
        [Test]
        public void FloorPlanBuildingBlockWallHorizontalVerticalNeighbourTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(3, 3, 1, "test", new[] { "1" });

            BuildingBlock block;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 0, 0), out block);
            block.Type = Tile.Types.Wall;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(0, 1, 0), out block);
            block.Type = Tile.Types.Wall;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 2, 0), out block);
            block.Type = Tile.Types.Wall;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(2, 1, 0), out block);
            block.Type = Tile.Types.Wall;

            floorPlan.Initiate();
            Assert.AreEqual(9, floorPlan.BuildingBlocks.Count);
            Assert.AreEqual(9, floorPlan.Tiles.Count);

            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 1, 0), out block);
            Assert.AreEqual(0, block.BuildingBlockNeighbours.Count);
        }
        
        /// <summary>
        /// Tests whether the function CheckForConnectionsThroughDiagonalUnwalkableElements works as intended.
        /// </summary>
        [Test]
        public void FloorPlanBuildingBlockWallDiagonalNeighbourTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(3, 3, 1, "test", new[] { "1" });

            BuildingBlock block;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(0, 0, 0), out block);
            block.Type = Tile.Types.Wall;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(0, 2, 0), out block);
            block.Type = Tile.Types.Wall;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(2, 0, 0), out block);
            block.Type = Tile.Types.Wall;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(2, 2, 0), out block);
            block.Type = Tile.Types.Wall;

            floorPlan.Initiate();
            Assert.AreEqual(9, floorPlan.BuildingBlocks.Count);
            Assert.AreEqual(9, floorPlan.Tiles.Count);

            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 1, 0), out block);
            Assert.AreEqual(4, block.BuildingBlockNeighbours.Count);

            BuildingBlock otherBlock;
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 0, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(0, 1, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(1, 2, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
            floorPlan.BuildingBlocks.TryGetValue(Coordinate(2, 1, 0), out otherBlock);
            Assert.IsTrue(block.BuildingBlockNeighbours.Any(b => Equals(b, otherBlock)));
        }

        /// <summary>
        /// Tests the initialization of stairs, wether or not the neighbours get set properly
        /// </summary>
        [Test]
        public void FloorPlanBuildingBlockStairNeighbourTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(3, 3, 3, "test", null);
            Assert.AreEqual(27, floorPlan.BuildingBlocks.Count);
            BuildingBlock overBlock = floorPlan.BuildingBlocks[Coordinate(1, 1, 2)];
            BuildingBlock block = floorPlan.BuildingBlocks[Coordinate(1, 1, 1)];
            BuildingBlock underBlock = floorPlan.BuildingBlocks[Coordinate(1, 1, 0)];
            Assert.AreNotEqual(null, overBlock);
            Assert.AreNotEqual(null, block);
            Assert.AreNotEqual(null, underBlock);
            overBlock.Type = Tile.Types.Stair;
            block.Type = Tile.Types.Stair;
            underBlock.Type = Tile.Types.Stair;
            floorPlan.Initiate();
            Assert.AreEqual(9, overBlock.BuildingBlockNeighbours.Count);
            Assert.AreEqual(10, block.BuildingBlockNeighbours.Count);
            Assert.AreEqual(9, underBlock.BuildingBlockNeighbours.Count);
            Assert.True(overBlock.BuildingBlockNeighbours.Contains(block));
            Assert.True(block.BuildingBlockNeighbours.Contains(overBlock));
            Assert.True(block.BuildingBlockNeighbours.Contains(underBlock));
            Assert.True(underBlock.BuildingBlockNeighbours.Contains(block));
        }

        /// <summary>
        /// Tests the priority of the building blocks, wether they get initialized correctly.
        /// </summary>
        [Test]
        public void FloorPlanPriorityTest()
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.CreateFloorPlan(10, 1, 3, "asd", null);
            floorPlan.BuildingBlocks[Coordinate(9, 0, 0)].Type = Tile.Types.Exit;
            floorPlan.BuildingBlocks[Coordinate(5, 0, 0)].Type = Tile.Types.Door;
            floorPlan.BuildingBlocks[Coordinate(1, 0, 0)].Type = Tile.Types.Stair;
            floorPlan.BuildingBlocks[Coordinate(1, 0, 1)].Type = Tile.Types.Stair;
            floorPlan.BuildingBlocks[Coordinate(5, 0, 1)].Type = Tile.Types.Door;
            floorPlan.BuildingBlocks[Coordinate(9, 0, 1)].Type = Tile.Types.Stair;
            floorPlan.BuildingBlocks[Coordinate(9, 0, 2)].Type = Tile.Types.Stair;
            floorPlan.Initiate();

            int testVal = 0;
            TestBlock(9, 0, 0, floorPlan, ref testVal);
            TestBlock(8, 0, 0, floorPlan, ref testVal);
            TestBlock(7, 0, 0, floorPlan, ref testVal);
            TestBlock(6, 0, 0, floorPlan, ref testVal);
            TestBlock(5, 0, 0, floorPlan, ref testVal);
            TestBlock(3, 0, 0, floorPlan, ref testVal);
            //            TestBlock(1, 0, 0, floorPlan, ref testVal);
            TestBlock(1, 0, 1, floorPlan, ref testVal);
            TestBlock(3, 0, 1, floorPlan, ref testVal);
            TestBlock(5, 0, 1, floorPlan, ref testVal);
            //            TestBlock(9, 0, 1, floorPlan, ref testVal);
            TestBlock(9, 0, 2, floorPlan, ref testVal);
        }

        private void TestBlock(int x, int y, int z, FloorPlan plan, ref int testVal)
        {
            Assert.GreaterOrEqual(plan.BuildingBlocks[Coordinate(x, y, z)].Priority, testVal);
            testVal = plan.BuildingBlocks[Coordinate(x, y, z)].Priority;
        }
    }
}
