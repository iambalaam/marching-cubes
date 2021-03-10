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
            foreach (int[] tris in MarchingCubes.lookupTable)
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

        [Test]
        public void EdgeLookup()
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
            // Bottom plane
            Assert.AreEqual(0, MarchingCubes.lookupEdge(0, 1));
            Assert.AreEqual(0, MarchingCubes.lookupEdge(1, 0));
            Assert.AreEqual(1, MarchingCubes.lookupEdge(1, 2));
            Assert.AreEqual(1, MarchingCubes.lookupEdge(2, 1));
            Assert.AreEqual(2, MarchingCubes.lookupEdge(2, 3));
            Assert.AreEqual(2, MarchingCubes.lookupEdge(3, 2));
            Assert.AreEqual(3, MarchingCubes.lookupEdge(3, 0));
            Assert.AreEqual(3, MarchingCubes.lookupEdge(0, 3));
            // Top plane
            Assert.AreEqual(4, MarchingCubes.lookupEdge(4, 5));
            Assert.AreEqual(4, MarchingCubes.lookupEdge(5, 4));
            Assert.AreEqual(5, MarchingCubes.lookupEdge(5, 6));
            Assert.AreEqual(5, MarchingCubes.lookupEdge(6, 5));
            Assert.AreEqual(6, MarchingCubes.lookupEdge(6, 7));
            Assert.AreEqual(6, MarchingCubes.lookupEdge(7, 6));
            Assert.AreEqual(7, MarchingCubes.lookupEdge(7, 8));
            Assert.AreEqual(7, MarchingCubes.lookupEdge(8, 7));
            // Verticals
            Assert.AreEqual(8, MarchingCubes.lookupEdge(0, 4));
            Assert.AreEqual(8, MarchingCubes.lookupEdge(4, 0));
            Assert.AreEqual(9, MarchingCubes.lookupEdge(1, 5));
            Assert.AreEqual(9, MarchingCubes.lookupEdge(5, 1));
            Assert.AreEqual(10, MarchingCubes.lookupEdge(2, 6));
            Assert.AreEqual(10, MarchingCubes.lookupEdge(6, 2));
            Assert.AreEqual(11, MarchingCubes.lookupEdge(3, 7));
            Assert.AreEqual(11, MarchingCubes.lookupEdge(7, 3));
        }
        [Test]
        public void MissingEdgeLookup()
        {
            try
            {
                MarchingCubes.lookupEdge(0, 5);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains("no edge", e.Message);
            }
        }
    }
}
