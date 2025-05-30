using System;
using System.Collections;
using _Scripts.ChunkingSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// ReSharper disable all
namespace EditModeTests
{
    public class LODManagerTests
    {
        [Test]
        public void Can_Instantiate_A_LODManager()
        {
            var lodManagerUnderTest = new LODManager(12);
            Assert.IsNotNull(lodManagerUnderTest);
            Assert.That(lodManagerUnderTest.GetMaxLODLevel(), Is.EqualTo(5));
            Assert.That(lodManagerUnderTest.GetLODs(), Is.EqualTo(new int[] { 12, 6, 4, 3, 2, 1 }));
        }

        [Test]
        public void Can_Set_Min_Chunk_Size()
        { // should probably also test correct errors for input validation
            var lodManagerUnderTest = new LODManager(12);
            int minChunkSize = 120; //{1,2,3,4,5,6,8,10,12,15,20,24,30,40,60,120}
            lodManagerUnderTest.SetMinChunkSize(minChunkSize);
            int receivedMinChunkSize = lodManagerUnderTest.GetMinChunkSize();
            
            Assert.That(receivedMinChunkSize, Is.EqualTo(minChunkSize));
            Assert.That(lodManagerUnderTest.GetMaxLODLevel(), Is.EqualTo(15));
            Assert.That(lodManagerUnderTest.GetLODs(), Is.EqualTo(new int[] {120,60,40,30,24,20,15,12,10,8,6,5,4,3,2,1}));
                                                                                                            
        }

        [Test]
        public void Has_Default_Chunk_Size_Of_120()
        {
            var lodManagerUnderTest = new LODManager(120);
            int expectedMinChunkSize = 120;
            int receivedMinChunkSize =  lodManagerUnderTest.GetMinChunkSize();
            Assert.That(receivedMinChunkSize, Is.EqualTo(expectedMinChunkSize));
        }
        
        [Test]
        public void Can_Generate_Correct_LODs_From_Min_Chunk_Size()
        {
            const int testMinChunkSize = 12;
            var expectedLods = new int[] { 12, 6, 4, 3, 2, 1 };
            var lods = LODManager.ComputeLODsFromMinChunkSizeDescending(testMinChunkSize);
            Assert.That(lods, Is.EqualTo(expectedLods));
        }
        
        [Test]
        public void Stores_A_Max_LOD_Level()
        {
            var lodManagerUnderTest = new LODManager(120);
            int maxLODLevel = 5;
            lodManagerUnderTest.SetNumLODLevels(maxLODLevel);
            int level = lodManagerUnderTest.GetMaxLODLevel();
            Assert.That(level, Is.EqualTo(maxLODLevel));
        }

        [TestCase(0, 5,5,1)]
        [TestCase(5,5,5,12)]
        [TestCase(5, 5,4, 12)]
        [TestCase(0, 3, 5, 3)]
        [TestCase(1, 5,5, 2)]
        public void Calculates_Correct_LOD(int nodeLevel, int treeHeight, int maxLODLevel, int expected)
        {
            var lodManagerUnderTest = new LODManager(12); //{12,6,4,3,2,1} - 6 members
            lodManagerUnderTest.SetNumLODLevels(maxLODLevel);
            
            Assert.That(lodManagerUnderTest.ComputeLOD(nodeLevel, treeHeight), Is.EqualTo(expected));
            
        }
        
        [Test]
        public void Throws_When_Max_LOD_Too_High()
        {
            int maxLODLevel = 10;
            var lodManagerUnderTest = new LODManager(12);
            Assert.Throws<ArgumentException>(() => lodManagerUnderTest.SetNumLODLevels(maxLODLevel));
        }
        
    }
    
}
