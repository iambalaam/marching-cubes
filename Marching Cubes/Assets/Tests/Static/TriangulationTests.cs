using NUnit.Framework;
using System;

namespace Tests
{
    public class TriangulationTests
    {
        [Test]
        public void TriangulationsAreMultiplesOf3()
        {
            foreach(int[] tris in Triangulation.lookupTable)
            {
                Assert.True(tris.Length % 3 == 0, "Lookup entry wasn't a multiple of 3");
            }
        }

        [Test]
        public void AllTriangulations()
        {
            double allPossibleTriangulations = Math.Pow(2, 8);
            Assert.AreEqual(allPossibleTriangulations, Triangulation.lookupTable.Length);
        }

    }
}
