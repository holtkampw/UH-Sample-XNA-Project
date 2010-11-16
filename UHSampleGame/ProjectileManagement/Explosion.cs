#region File Description
//-----------------------------------------------------------------------------
// Projectile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace UHSampleGame.ProjectileManagment
{
    class Explosion
    {
        #region Constants

        const float trailParticlesPerSecond = 200;
        const int numExplosionParticles = 40;
        const int numExplosionSmokeParticles = 30;
        const float projectileLifespan = 1.5f;
        const float sidewaysVelocityRange = 100;
        const float verticalVelocityRange = 10;
        const float gravity = 2;

        const float radius = 60;

        #endregion

        #region Fields

        ParticleSystem explosionParticles;
        ParticleSystem laserParticles;

        public Vector3 Position;
        public Vector3 velocity;
        public Vector3 Destination;
        public Vector3 DestinationNearby;

        static Random random = new Random();
        public bool Active = false;

        Vector3 fromCircle = Vector3.Zero;
        int degrees = 0;
        int altitude = 0;
        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Explosion(ParticleSystem explosionParticles, ParticleSystem laserParticles)
        {
            this.explosionParticles = explosionParticles;
            this.laserParticles = laserParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            Position = Vector3.Zero;
            velocity = Vector3.Zero;
            Destination = Vector3.Zero;
            DestinationNearby = Vector3.Zero;
        }

        public void SetPositionAndVelocity(Vector3 position, Vector3 destination)
        {
            this.Position = position;
            this.Destination = destination;
            velocity.X = (destination.X - position.X) * sidewaysVelocityRange;
            velocity.Y = (destination.Y - position.Y) * verticalVelocityRange;
            velocity.Z = (destination.Z - position.Z) * sidewaysVelocityRange;
            velocity.Normalize();
            this.Active = true;
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(float elapsedTime)
        {
            Position += velocity;
            for (int i = 0; i < 5; i++ )
                laserParticles.AddParticle(Position, Vector3.Zero);

            if (PositionNearby())
            {
                for (int i = 0; i < 50; i++)
                {
                    FindDestinationNearby();
                    explosionParticles.AddParticle(DestinationNearby, Vector3.Zero);
                }

                return false;
            }
            return true;
            //GetNextPointOnCircle();
            //explosionParticles.AddParticle(fromCircle, velocity);
            //degrees += 10;
            //if (degrees >= 360)
            //    altitude += 8;

            //if (degrees >= 720)
            //{
            //    degrees = 0;
            //    altitude = 0;
            //    Active = false;
            //    return false;
            //}
            //return true;
        }

        void FindDestinationNearby()
        {
            DestinationNearby.X = Destination.X + random.Next(-10, 10);
            DestinationNearby.Z = Destination.Z + random.Next(-10, 10);
        }

        bool PositionNearby()
        {
            if (Position.X - Destination.X <= 10 || Position.X - Destination.X <= -10)
            {
                if (Position.Z - Destination.Z <= 10 || Position.Z - Destination.Z <= -10)
                    return true;
            }
            return false;
        }


        void GetNextPointOnCircle()
        {
            fromCircle.X = Position.X + (float)(radius * Math.Cos((Math.PI / 180) * degrees));
            fromCircle.Y = (float)altitude;
            fromCircle.Z = Position.Z + (float)(radius * Math.Sin((Math.PI / 180) * degrees));
        }
    }
}
