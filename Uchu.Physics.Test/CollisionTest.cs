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

        [Test]
        public void BoxBoxCollision()
        {
            // This test does not check cases where the boxes collide but none of
            // the vertices are contained by the other box (edge-edge collisions).

            // 10x10x10 at (0,0,0)
            var boxAtZero = BoxBody.Create(_simulation,
                new Vector3(0, 0, 0),
                Quaternion.Identity,
                Vector3.One * 10);

            // 1x1x1 at (5,5,5)
            var smallBox5 = BoxBody.Create(_simulation,
                new Vector3(5,5,5),
                Quaternion.Identity,
                Vector3.One);

            // 5x5x5 at (-7,0,0)
            var box7 = BoxBody.Create(_simulation,
                new Vector3(-7, 0, 0),
                Quaternion.Identity,
                Vector3.One * 5);

            // 1x10x1 at (0,8,0)
            var box8 = BoxBody.Create(_simulation,
                new Vector3(0, 8, 0),
                Quaternion.Identity,
                new Vector3(1, 10, 1));

            // 1x10x1 at (0,8,0) but rotated 90 deg around Z axis
            var box8Rotated = BoxBody.Create(_simulation,
                new Vector3(0, 8, 0),
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float) (Math.Tau / 4)),
                new Vector3(1, 10, 1));

            // 1x1x1 at (4,8.5,0.5)
            var box9 = BoxBody.Create(_simulation,
                new Vector3(4, 8.5f, 0.5f),
                Quaternion.Identity,
                new Vector3(1, 1, 1));

            Assert.IsTrue(PhysicsSimulation.BoxBoxCollision(boxAtZero, smallBox5));
            Assert.IsTrue(PhysicsSimulation.BoxBoxCollision(boxAtZero, box7));
            Assert.IsTrue(PhysicsSimulation.BoxBoxCollision(boxAtZero, box8));
            Assert.IsFalse(PhysicsSimulation.BoxBoxCollision(boxAtZero, box8Rotated));
            Assert.IsFalse(PhysicsSimulation.BoxBoxCollision(smallBox5, box7));
            Assert.IsTrue(PhysicsSimulation.BoxBoxCollision(box9, box8Rotated));
        }

        [Test]
        public void CapsuleSphereCollision()
        {
            // r=1.5 at (0,8,0)
            var sphere = SphereBody.Create(_simulation,
                new Vector3(0, 8, 0),
                1.5f);

            // pointing straight up, should hit sphere
            var capsule1 = CapsuleBody.Create(this._simulation,
                new Vector3(0, 0, 0),
                Quaternion.Identity,
                1f,
                6f);

            // rotated 45deg, should miss sphere
            var capsule2 = CapsuleBody.Create(this._simulation,
                new Vector3(0, 0, 0),
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float) (0.125 * Math.Tau)),
                1f,
                6f);

            // floating, rotated 90deg, should hit sphere
            var capsule3 = CapsuleBody.Create(this._simulation,
                new Vector3(8, 8, 0),
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float) (0.25 * Math.Tau)),
                1f,
                6f);

            Assert.IsTrue(PhysicsSimulation.CapsuleSphereCollision(capsule1, sphere));
            Assert.IsFalse(PhysicsSimulation.CapsuleSphereCollision(capsule2, sphere));
            Assert.IsTrue(PhysicsSimulation.CapsuleSphereCollision(capsule3, sphere));
        }
    }
}
