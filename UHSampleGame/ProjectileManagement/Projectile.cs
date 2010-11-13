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
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    class Projectile
    {
        #region Constants

        const float trailParticlesPerSecond = 200;
        const int numExplosionParticles = 40;
        const int numExplosionSmokeParticles = 30;
        const float projectileLifespan = 1.5f;
        const float sidewaysVelocityRange = 100;
        const float verticalVelocityRange = 10;
        const float gravity = 2;

        #endregion

        #region Fields

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleEmitter trailEmitter;

        public Vector3 Position;
        public Vector3 velocity;
        float age;

        static Random random = new Random();
        public bool Active = false;

        float elapsedTime;

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(ParticleSystem explosionParticles,
                          ParticleSystem explosionSmokeParticles,
                          ParticleSystem projectileTrailParticles)
        {
            this.explosionParticles = explosionParticles;
            this.explosionSmokeParticles = explosionSmokeParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            Position = Vector3.Zero;

            velocity.X = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() + 0.5) * verticalVelocityRange;
            velocity.Z = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;

            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(projectileTrailParticles,
                                               trailParticlesPerSecond, Position);
        }

        public void SetPositionAndVelocity(Vector3 position, Vector3 velocity)
        {
            this.Position = position;
            this.velocity.X = velocity.X * sidewaysVelocityRange;
            this.velocity.Y = velocity.Y * verticalVelocityRange;
            this.velocity.Z = velocity.Z * sidewaysVelocityRange;
            Active = true;
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(float elapsedTime)
        {
            elapsedTime = elapsedTime / 1000.0f;

            // Simple projectile physics.
            Position += velocity * elapsedTime;
            velocity.Y -= elapsedTime * gravity;
            age += elapsedTime;

            // Update the particle emitter, which will create our particle trail.
            trailEmitter.Update(elapsedTime, Position);

            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.
            if (age > projectileLifespan)
            {
                for (int i = 0; i < numExplosionParticles; i++)
                    explosionParticles.AddParticle(Position, velocity);

                for (int i = 0; i < numExplosionSmokeParticles; i++)
                    explosionSmokeParticles.AddParticle(Position, velocity);

                Active = false;
                return false;
            }
                
            return true;
        }
    }
}
