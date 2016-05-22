using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Evacuation_Master_3000;
using NUnit.Framework;

namespace P2TestEnvironment
{
    [TestFixture]
    public class DataTests
    {
        [Test]
        public void DataAllPeopleStartsNull()
        {
            Data data = new Data();
            Assert.AreEqual(0,data.AllPeople.Count);
        }
    }
}
