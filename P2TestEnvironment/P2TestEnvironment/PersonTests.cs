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
    class PersonTests
    {
        [Test]
        public void PersonStart()
        {
            BuildingBlock block = new BuildingBlock(0,0);
            Person person = new Person(block);
            Assert.AreEqual(0,person.ID);
            Assert.AreEqual(false,person.Evacuated);
            Assert.AreEqual(new List<BuildingBlock>(), person.PathList);
            Tile tile = person.Position;
            Assert.AreEqual(0, tile.X);
            Assert.AreEqual(0, tile.Y);
        }

        //[Test]
        //public void PersonConditionalMove()
        //{
        //    FloorPlan plan = new FloorPlan();
        //    plan.CreateFloorPlan(10,10,1,"",null);
        //    BuildingBlock block = new BuildingBlock(0,0);
        //    plan.Tiles.Add("test",block);
        //    Person person = new Person(block,0 );
        //    person.ConditionalMove();
        //    Assert.AreEqual(true, person.Evacuated);
        //}
    }
}
