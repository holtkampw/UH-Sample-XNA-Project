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
    class Upgrade
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

        ParticleSystem starParticles;

        public Vector3 Position;
        public Vector3 velocity;

        static Random random = new Random();
        public bool Active = false;

        Vector3 fromCircle = Vector3.Zero;
        int degrees = 0;
        int altitude = 0;
        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Upgrade(ParticleSystem starParticles)
        {
            this.starParticles = starParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            Position = Vector3.Zero;
            velocity = Vector3.Zero;

           
        }

        public void SetPositionAndVelocity(Vector3 position)
        {
            this.Position = position;
            this.Active = true;
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(float elapsedTime)
        {

            GetNextPointOnCircle();
            starParticles.AddParticle(fromCircle , velocity);
            degrees += 10;
            if(degrees >= 360)
                altitude += 8;

            if (degrees >= 720)
            {
                degrees = 0;
                altitude = 0;
                Active = false;
                return false;
            }
            return true;
        }

        //Vector3 RandomPointOnCircle()
        //{
        //    const float radius = 100;
        //    const float height = 100;

        //    double angle = random.NextDouble() * Math.PI * 2;

        //    float x = (float)Math.Cos(angle);
        //    float y = (float)Math.Sin(angle);

        //    return new Vector3(x * radius, 0, y * radius);
        //}


        void GetNextPointOnCircle()
        {
            fromCircle.X = Position.X + (float)(radius * Math.Cos((Math.PI / 180) * degrees));
            fromCircle.Y = (float)altitude;
            fromCircle.Z = Position.Z + (float)(radius * Math.Sin((Math.PI / 180) * degrees));
        }
    }
}
