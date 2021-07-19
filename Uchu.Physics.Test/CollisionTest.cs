using System;
using System.Numerics;
using NUnit.Framework;

namespace Uchu.Physics.Test
{
    public class CollisionTest
    {
        private PhysicsSimulation _simulation;

        [SetUp]
        public void SetupTest()
        {
            _simulation = new PhysicsSimulation();
        }

        [Test]
        public void SphereSphereCollision()
        {
            // r=10 at (0,0,0)
            var sphereAtZero = SphereBody.Create(_simulation, new Vector3(0, 0, 0),
                10f);

            // r=10 at (100,0,0)
            var sphereFar = SphereBody.Create(_simulation,
                new Vector3(100, 0, 0),
                10f);

            // r=4 at (11,0,0)
            var sphereClose = SphereBody.Create(_simulation,
                new Vector3(11, 0, 0),
                4f);

            Assert.IsFalse(PhysicsSimulation.SphereSphereCollision(sphereAtZero, sphereFar));
            Assert.IsTrue(PhysicsSimulation.SphereSphereCollision(sphereAtZero, sphereClose));
        }

        [Test]
        public void BoxSphereCollision()
        {
            // r=10 at (100,0,0)
            var sphereFar = SphereBody.Create(_simulation,
                new Vector3(100, 0, 0),
                10f);

            // r=4 at (7,0,0)
            var sphereClose = SphereBody.Create(_simulation,
                new Vector3(7, 0, 0),
                4f);

            // r=0.5 at (5,5,0)
            var sphere2 = SphereBody.Create(_simulation,
                new Vector3(5, 5, 0),
                0.5f);

            // r=0.5 at (6,0,0)
            var sphere3 = SphereBody.Create(_simulation,
                new Vector3(6, 0, 0),
                0.5f);

            // 10x10x10 at (0,0,0)
            var boxAtZero = BoxBody.Create(_simulation,
                new Vector3(0, 0, 0),
                Quaternion.Identity,
                Vector3.One * 10);

            // No rotation
            Assert.IsTrue(PhysicsSimulation.BoxSphereCollision(boxAtZero, sphereClose));
            Assert.IsFalse(PhysicsSimulation.BoxSphereCollision(boxAtZero, sphereFar));

            Assert.IsTrue(PhysicsSimulation.BoxSphereCollision(boxAtZero, sphere2));
            Assert.IsFalse(PhysicsSimulation.BoxSphereCollision(boxAtZero, sphere3));

            // Rotate box by 45deg around Z axis. Should now miss small sphere at (5,5,0) and hit small sphere at (6,0,0)
            var boxAtZeroRotated = BoxBody.Create(_simulation,
                new Vector3(0, 0, 0),
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float) Math.Tau / 8),
                Vector3.One * 10);
            Assert.IsFalse(PhysicsSimulation.BoxSphereCollision(boxAtZeroRotated, sphere2));
            Assert.IsTrue(PhysicsSimulation.BoxSphereCollision(boxAtZeroRotated, sphere3));
        }
    }
}
