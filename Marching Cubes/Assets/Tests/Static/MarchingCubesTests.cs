using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests
{
    public class CubeTests
    {
        MarchingCubes.ScalarField constant = v => 1;
        MarchingCubes.ScalarField evenOdd = v => (v.x + v.y + v.z) % 2;

        [Test]
        public void CubePopulatesVertices()
        {
            var cube = new MarchingCubes.Cube(Vector3.zero, evenOdd);
            Assert.AreEqual(0, cube.values[0]);
            Assert.AreEqual(1, cube.values[1]);
            Assert.AreEqual(0, cube.values[2]);
            Assert.AreEqual(1, cube.values[3]);
            Assert.AreEqual(1, cube.values[4]);
            Assert.AreEqual(0, cube.values[5]);
            Assert.AreEqual(1, cube.values[6]);
            Assert.AreEqual(0, cube.values[7]);
        }
        [Test]
        public void CubePopulatesConstantValues()
        {
            var cube = new MarchingCubes.Cube(Vector3.zero, constant);
            foreach (float value in cube.values)
            {
                Assert.AreEqual(1, value);
            }
        }
    }

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
            Assert.AreEqual(0, MarchingCubes.Vertices2Edge(0, 1));
            Assert.AreEqual(0, MarchingCubes.Vertices2Edge(1, 0));
            Assert.AreEqual(1, MarchingCubes.Vertices2Edge(1, 2));
            Assert.AreEqual(1, MarchingCubes.Vertices2Edge(2, 1));
            Assert.AreEqual(2, MarchingCubes.Vertices2Edge(2, 3));
            Assert.AreEqual(2, MarchingCubes.Vertices2Edge(3, 2));
            Assert.AreEqual(3, MarchingCubes.Vertices2Edge(3, 0));
            Assert.AreEqual(3, MarchingCubes.Vertices2Edge(0, 3));
            // Top plane
            Assert.AreEqual(4, MarchingCubes.Vertices2Edge(4, 5));
            Assert.AreEqual(4, MarchingCubes.Vertices2Edge(5, 4));
            Assert.AreEqual(5, MarchingCubes.Vertices2Edge(5, 6));
            Assert.AreEqual(5, MarchingCubes.Vertices2Edge(6, 5));
            Assert.AreEqual(6, MarchingCubes.Vertices2Edge(6, 7));
            Assert.AreEqual(6, MarchingCubes.Vertices2Edge(7, 6));
            Assert.AreEqual(7, MarchingCubes.Vertices2Edge(7, 8));
            Assert.AreEqual(7, MarchingCubes.Vertices2Edge(8, 7));
            // Verticals
            Assert.AreEqual(8, MarchingCubes.Vertices2Edge(0, 4));
            Assert.AreEqual(8, MarchingCubes.Vertices2Edge(4, 0));
            Assert.AreEqual(9, MarchingCubes.Vertices2Edge(1, 5));
            Assert.AreEqual(9, MarchingCubes.Vertices2Edge(5, 1));
            Assert.AreEqual(10, MarchingCubes.Vertices2Edge(2, 6));
            Assert.AreEqual(10, MarchingCubes.Vertices2Edge(6, 2));
            Assert.AreEqual(11, MarchingCubes.Vertices2Edge(3, 7));
            Assert.AreEqual(11, MarchingCubes.Vertices2Edge(7, 3));
        }
        [Test]
        public void MissingEdgeLookup()
        {
            try
            {
                MarchingCubes.Vertices2Edge(0, 5);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains("no edge", e.Message);
            }
        }
    }
}
