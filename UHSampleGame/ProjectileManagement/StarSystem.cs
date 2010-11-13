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
    class Star
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
        float degrees = 0;
        float altitude = 0;
        #endregion

        #region Fields

        ParticleSystem starParticles;
        ParticleEmitter starTrail;

        public Vector3 Position;
        public Vector3 velocity;
        float age;

        static Random random = new Random();
        public bool Active = false;

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Star(ParticleSystem starParticles)
        {
            this.starParticles = starParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            Position = Vector3.Zero;

            velocity.X = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() + 0.5) * verticalVelocityRange;
            velocity.Z = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;

            this.starTrail = new ParticleEmitter(starParticles,
                                               trailParticlesPerSecond, Position);
        }

        public void SetPositionAndVelocity(Vector3 position)
        {
            this.Position = position;
            this.Active = true;
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Simple projectile physics.
            //Position += velocity * elapsedTime;
            //velocity.Y -= elapsedTime * gravity;
            //age += elapsedTime;

            //Vector3 newPosition = Position + RandomPointOnCircle();
            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.

            //starTrail.Update(gameTime, newPosition);

            starParticles.AddParticle(Position + GetNextPointOnCircle(), Vector3.Zero);
            degrees += 10f;
            if(degrees >= 360)
                altitude += 8f;

            if (degrees >= 720)
            {
                degrees = 0;
                altitude = 0;
                Active = false;
                return false;
            }
            //if (age > projectileLifespan)
            //{
            //    for (int i = 0; i < numExplosionParticles; i++)
            //        starParticles.AddParticle(newPosition, velocity);

            //    Active = false;
            //    return false;
            //}

            return true;
        }

        Vector3 RandomPointOnCircle()
        {
            const float radius = 100;
            const float height = 100;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector3(x * radius, 0, y * radius);
        }


        Vector3 GetNextPointOnCircle()
        {
            return new Vector3((float)(radius * Math.Cos((Math.PI / 180) * degrees)), altitude, (float)(radius * Math.Sin((Math.PI / 180) * degrees)));
        }
    }
}
