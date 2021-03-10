using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests
{
    public class MarchingCubesTests
    {
        [Test]
        public void TriangulationsAreMultiplesOf3()
        {
            foreach(int[] tris in MarchingCubes.lookupTable)
            {
                Assert.True(tris.Length % 3 == 0, "Lookup entry wasn't a multiple of 3");
            }
        }

        [Test]
        public void AllTriangulations()
        {
            double allPossibleTriangulations = Math.Pow(2, 8);
            Assert.AreEqual(allPossibleTriangulations, MarchingCubes.lookupTable.Length);
        }

        [Test]
        public void VertexLookup()
        {
            /*
             * Representation of cube
             *             (z)
             *             .4------4------5
             *           .' |           .'|
             *         .7   8         .5  |
             *       .'     |       .'    |
             *      7-------+-6---6'      9
             *      |       |     |       |
             *      |       |     10      |
             *      11      |     |       |
             *      |      .0-----+-0-----1 (x)
             *      |    .'       |     .'
             *      |  .3         |   .1
             *      |.'           | .'
             *      3------2------2' 
             *    (y)
             */
            Assert.AreEqual(0, MarchingCubes.lookupVertex(new Vector3Int(0, 0, 0)));
            Assert.AreEqual(1, MarchingCubes.lookupVertex(new Vector3Int(1, 0, 0)));
            Assert.AreEqual(2, MarchingCubes.lookupVertex(new Vector3Int(1, 1, 0)));
            Assert.AreEqual(3, MarchingCubes.lookupVertex(new Vector3Int(0, 1, 0)));
            Assert.AreEqual(4, MarchingCubes.lookupVertex(new Vector3Int(0, 0, 1)));
            Assert.AreEqual(5, MarchingCubes.lookupVertex(new Vector3Int(1, 0, 1)));
            Assert.AreEqual(6, MarchingCubes.lookupVertex(new Vector3Int(1, 1, 1)));
            Assert.AreEqual(7, MarchingCubes.lookupVertex(new Vector3Int(0, 1, 1)));
        }
    }
}
