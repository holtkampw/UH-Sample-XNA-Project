using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.ScreenManagement;
using UHSampleGame;
using UHSampleGame.CameraManagement;

namespace UHSampleGame.ProjectileManagment
{
    public static class ProjectileManager
    {
        #region Class Variables
        static List<Projectile> projectiles;
        const int MAX_PROJECTILES = 5000;

        static ParticleSystem explosionParticles;
        static ParticleSystem explosionSmokeParticles;
        static ParticleSystem projectileTrailParticles;
        static ParticleSystem smokePlumeParticles;
        static ParticleSystem fireParticles;
        static int projectileCount;
        static int updatedProjectiles;
        static CameraManager cameraManager;
        static Vector3 vel = new Vector3();
        #endregion

        public static void Initialize()
        {
            // Construct our particle system components.
            explosionParticles = new ExplosionParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            fireParticles = new FireParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system components.
            ScreenManager.Game.Components.Add(explosionParticles);
            ScreenManager.Game.Components.Add(explosionSmokeParticles);
            ScreenManager.Game.Components.Add(projectileTrailParticles);
            ScreenManager.Game.Components.Add(smokePlumeParticles);
            ScreenManager.Game.Components.Add(fireParticles);

            projectiles = new List<Projectile>();
            for (int i = 0; i < MAX_PROJECTILES; i++)
            {
                projectiles.Add(new Projectile(explosionParticles, explosionSmokeParticles, projectileTrailParticles));
            }
            projectileCount = 0;

            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
        }

        public static void AddParticle(Vector3 startPosition, Vector3 endPosition)
        {

            for (int i = 0; i < MAX_PROJECTILES; i++)
            {
                if (!projectiles[i].Active)
                {       
                    vel.X = (endPosition.X - startPosition.X);
                    vel.Y = (endPosition.Y - startPosition.Y);
                    vel.Z = (endPosition.Z - startPosition.Z);
                    vel.Normalize();
                    projectiles[i].SetPositionAndVelocity(startPosition, vel);
                    projectileCount++;
                    projectiles[i].Active = true;
                    break;
                }
            }
        }

        public static void Update(GameTime gameTime)
        {
            updatedProjectiles = 0;
            if (projectileCount == 0)
                return;
            for (int i = 0; i < MAX_PROJECTILES; i++)
            {
                if (projectiles[i].Active)
                {
                    if (!projectiles[i].Update(gameTime))
                    {
                        // Remove projectiles at the end of their life.
                        projectiles[i].Active = false;
                        projectileCount--;
                        updatedProjectiles++;
                        if (updatedProjectiles >= projectileCount)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static void Draw(GameTime gameTime)
        {
            explosionParticles.SetCamera(cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
            explosionSmokeParticles.SetCamera(cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
            projectileTrailParticles.SetCamera(cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
            smokePlumeParticles.SetCamera(cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
            fireParticles.SetCamera(cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
        }
    }
}
