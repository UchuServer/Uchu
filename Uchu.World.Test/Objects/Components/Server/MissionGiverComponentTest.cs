using System;
using Moq;
using NUnit.Framework;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Systems.Missions;

namespace Uchu.World.Test.Objects.Components.Server
{
    public class MissionGiverComponentTest
    {
        /// <summary>
        /// Mission giver component used with the tests.
        /// </summary>
        private MissionGiverComponent _missionGiverComponent;

        /// <summary>
        /// Player mission inventory used with the tests.
        /// </summary>
        private MissionInventoryComponent _missionInventoryComponent;

        /// <summary>
        /// Mission NPC component used with the tests.
        /// </summary>
        private MissionNPCComponent _missionNpcComponent;

        /// <summary>
        /// Sets up the tests.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._missionGiverComponent = new MissionGiverComponent();
            this._missionInventoryComponent = new MissionInventoryComponent();
            this._missionNpcComponent = new MissionNPCComponent()
            {
                AcceptsMission = true,
                OffersMission = true,
            };
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with no stored missions.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferEmpty()
        {
            this._missionGiverComponent.Missions = Array.Empty<(Missions, MissionNPCComponent)>();
            Assert.AreEqual(0, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with a stored mission with the default id.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferDefaultMissionId()
        {
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions() { Id = default, }, this._missionNpcComponent)
            };
            Assert.AreEqual(0, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with a stored mission but missions aren't offered.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferDoesntOfferMission()
        {
            this._missionNpcComponent.OffersMission = false;
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions() { Id = 1, }, this._missionNpcComponent)
            };
            Assert.AreEqual(0, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with a mission ready to complete taking priority.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferReadyToCompletePriority()
        {
            var mockMission1 = new Mock<MissionInstance>(1, null);
            mockMission1.SetupGet(mission => mission.State).Returns(MissionState.Available);
            var mockMission2 = new Mock<MissionInstance>(2, null);
            mockMission2.SetupGet(mission => mission.State).Returns(MissionState.ReadyToComplete);
            
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions() { Id = 1, }, this._missionNpcComponent),
                (new Missions() { Id = 2, }, this._missionNpcComponent),
            };
            this._missionInventoryComponent.AddTestMission(mockMission1.Object);
            this._missionInventoryComponent.AddTestMission(mockMission2.Object);
            Assert.AreEqual(2, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with a mission active taking priority over available.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferActivePriority()
        {
            var mockMission1 = new Mock<MissionInstance>(1, null);
            mockMission1.SetupGet(mission => mission.State).Returns(MissionState.Available);
            var mockMission2 = new Mock<MissionInstance>(2, null);
            mockMission2.SetupGet(mission => mission.State).Returns(MissionState.Active);
            var mockMission3 = new Mock<MissionInstance>(1, null);
            mockMission3.SetupGet(mission => mission.State).Returns(MissionState.Available);
            
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions() { Id = 1, }, this._missionNpcComponent),
                (new Missions() { Id = 2, }, this._missionNpcComponent),
                (new Missions() { Id = 3, }, this._missionNpcComponent),
            };
            this._missionInventoryComponent.AddTestMission(mockMission1.Object);
            this._missionInventoryComponent.AddTestMission(mockMission2.Object);
            this._missionInventoryComponent.AddTestMission(mockMission3.Object);
            Assert.AreEqual(2, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with a mission completed active (repeating) taking priority over available.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferCompletedActivePriority()
        {
            var mockMission1 = new Mock<MissionInstance>(1, null);
            mockMission1.SetupGet(mission => mission.State).Returns(MissionState.Available);
            var mockMission2 = new Mock<MissionInstance>(2, null);
            mockMission2.SetupGet(mission => mission.State).Returns(MissionState.CompletedActive);
            var mockMission3 = new Mock<MissionInstance>(1, null);
            mockMission3.SetupGet(mission => mission.State).Returns(MissionState.Available);
            
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions() { Id = 1, }, this._missionNpcComponent),
                (new Missions() { Id = 2, }, this._missionNpcComponent),
                (new Missions() { Id = 3, }, this._missionNpcComponent),
            };
            this._missionInventoryComponent.AddTestMission(mockMission1.Object);
            this._missionInventoryComponent.AddTestMission(mockMission2.Object);
            this._missionInventoryComponent.AddTestMission(mockMission3.Object);
            Assert.AreEqual(2, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with an empty random mission pool.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferEmptyRandomPool()
        {
            var mockMission = new Mock<MissionInstance>(1, null);
            mockMission.SetupGet(mission => mission.State).Returns(MissionState.Available);
            
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions()
                {
                    Id = 1,
                    RandomPool = "",
                }, this._missionNpcComponent),
            };
            this._missionInventoryComponent.AddTestMission(mockMission.Object);
            Assert.AreEqual(1, this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent));
        }
        
        /// <summary>
        /// Tests GetIdMissionToOffer with a random mission pool.
        /// </summary>
        [Test]
        public void TestGetIdMissionToOfferRandomPool()
        {
            var mockMission = new Mock<MissionInstance>(1, null);
            mockMission.SetupGet(mission => mission.State).Returns(MissionState.Available);
            
            this._missionGiverComponent.Missions = new[]
            {
                (new Missions()
                {
                    Id = 1,
                    IsRandom = true,
                    RandomPool = "2,3",
                }, this._missionNpcComponent),
            };
            this._missionInventoryComponent.AddTestMission(mockMission.Object);
            var randomMission = this._missionGiverComponent.GetIdMissionToOffer(this._missionInventoryComponent);
            Assert.IsTrue(randomMission == 2 || randomMission == 3);
        }
    }
}

