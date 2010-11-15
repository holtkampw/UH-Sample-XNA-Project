using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.ScreenManagement;
using UHSampleGame;
using UHSampleGame.CameraManagement;
using System.Threading;
using System.Diagnostics; 

namespace UHSampleGame.ProjectileManagment
{
    public static class ProjectileManager
    {
        #region Class Variables
        static List<Projectile> projectiles;
        static List<List<Upgrade>> upgradeEffect;
        static List<List<Repair>> repairEffect;
       
        const int MAX_PROJECTILES = 900;

        static ParticleSystem explosionParticles;
        static ParticleSystem explosionSmokeParticles;
        static ParticleSystem projectileTrailParticles;
        static ParticleSystem smokePlumeParticles;
        static ParticleSystem fireParticles;
        static ParticleSystem[] upgradeParticles;
        static ParticleSystem[] repairParticles;
        static int projectileCount;
        static int updatedProjectiles;
        static int projectilesToUpdate;
        static int[] upgradeCount;
        static int updatedStars;
        static int starsToUpdate;
        static int[] repairCount;
        static CameraManager cameraManager;
        static Vector3 vel = new Vector3();

        public static Thread particleThread;
        public static EventWaitHandle particleThreadExit;
        static Stopwatch timer;
        static float FrequencyInverse;
        static float elapsedMilliseconds;

        public static object projectileLock = new object();
        public static object upgradeLock = new object();
        public static object repairLock = new object();

        #endregion

        public static void Dispose()
        {
            ScreenManager.Game.Components.Remove(explosionParticles);
            ScreenManager.Game.Components.Remove(explosionSmokeParticles);
            ScreenManager.Game.Components.Remove(projectileTrailParticles);
            ScreenManager.Game.Components.Remove(smokePlumeParticles);
            ScreenManager.Game.Components.Remove(fireParticles);

            explosionParticles.Dispose();
            explosionSmokeParticles.Dispose();
            projectileTrailParticles.Dispose();
            smokePlumeParticles.Dispose();
            fireParticles.Dispose();
            for (int i = 0; i < upgradeParticles.Length; i++)
            {
                if(upgradeParticles[i] != null)
                    upgradeParticles[i].Dispose();
            }

            for (int i = 0; i < repairParticles.Length; i++)
            {
                if (repairParticles[i] != null)
                    repairParticles[i].Dispose();
            }

            explosionParticles = null;
            explosionSmokeParticles = null;
            projectileTrailParticles = null;
            smokePlumeParticles = null;
            fireParticles = null;
            upgradeParticles = null;
            repairParticles = null;

            projectiles = null;
            upgradeEffect = null;
            upgradeCount = null;
            repairEffect = null;
            repairCount = null;
            GC.Collect();
        }

        public static void Initialize()
        {
            // Construct our particle system components.
            explosionParticles = new ExplosionParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            fireParticles = new FireParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            //starParticles = new StarParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            upgradeParticles = new ParticleSystem[5];
            repairParticles = new ParticleSystem[5];
            for (int i = 1; i < 5; i++)
            {
                
                switch(i)
                {
                    case 1:
                        upgradeParticles[i] = new P1Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        repairParticles[i] = new P1Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        break;
                    case 2:
                        upgradeParticles[i] = new P2Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        repairParticles[i] = new P2Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        break;
                    case 3:
                        upgradeParticles[i] = new P3Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        repairParticles[i] = new P3Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        break;
                    case 4:
                        upgradeParticles[i] = new P4Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        repairParticles[i] = new P4Upgrade(ScreenManager.Game, ScreenManager.Game.Content);
                        break;

                }

                upgradeParticles[i].DrawOrder = 600;
                repairParticles[i].DrawOrder = 600;
                ScreenManager.Game.Components.Add(upgradeParticles[i]);
                ScreenManager.Game.Components.Add(repairParticles[i]);
            }


            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;
            //starParticles.DrawOrder = 600;

            // Register the particle system components.
            ScreenManager.Game.Components.Add(explosionParticles);
            ScreenManager.Game.Components.Add(explosionSmokeParticles);
            ScreenManager.Game.Components.Add(projectileTrailParticles);
            ScreenManager.Game.Components.Add(smokePlumeParticles);
            ScreenManager.Game.Components.Add(fireParticles);
            //ScreenManager.Game.Components.Add(starParticles);

            projectiles = new List<Projectile>();
            //stars = new List<Star>();
            
            for (int i = 0; i < MAX_PROJECTILES; i++)
            {
                projectiles.Add(new Projectile(explosionParticles, explosionSmokeParticles, projectileTrailParticles));
                //stars.Add(new Star(starParticles));
            }

            upgradeEffect = new List<List<Upgrade>>();
            upgradeCount = new int[5];
            upgradeEffect.Add(new List<Upgrade>());
            repairEffect = new List<List<Repair>>();
            repairEffect.Add(new List<Repair>());
            repairCount = new int[5];
            for (int p = 1; p < 5; p++)
            {
                upgradeEffect.Add(new List<Upgrade>());
                repairEffect.Add(new List<Repair>());
                for (int i = 0; i < MAX_PROJECTILES; i++)
                {
                    upgradeEffect[p].Add(new Upgrade(upgradeParticles[p]));
                    repairEffect[p].Add(new Repair(repairParticles[p]));
                }
                upgradeCount[p] = 0;
                repairCount[p] = 0;
            }
            projectileCount = 0;
            
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            particleThread = new Thread(ThreadedUpdate);
            particleThreadExit = new ManualResetEvent(false);
            timer = System.Diagnostics.Stopwatch.StartNew();
        }

        public static void AddParticle(Vector3 startPosition, Vector3 endPosition)
        {
            lock (projectileLock)
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
        }

        public static void Upgrade(Vector3 position, int playerNum)
        {
            if (playerNum > 4 || playerNum < 1)
                return;

            lock (upgradeLock)
            {

                for (int i = 0; i < MAX_PROJECTILES; i++)
                {
                    if (!upgradeEffect[playerNum][i].Active)
                    {
                        upgradeEffect[playerNum][i].SetPositionAndVelocity(position);
                        upgradeCount[playerNum]++;
                        upgradeEffect[playerNum][i].Active = true;
                        break;
                    }
                }
            }
        }

        public static void Repair(Vector3 position, int playerNum)
        {
            if (playerNum > 4 || playerNum < 1)
                return;

            lock (repairLock)
            {
                for (int i = 0; i < MAX_PROJECTILES; i++)
                {
                    if (!repairEffect[playerNum][i].Active)
                    {
                        repairEffect[playerNum][i].SetPositionAndVelocity(position);
                        repairCount[playerNum]++;
                        repairEffect[playerNum][i].Active = true;
                        break;
                    }
                }
            }
        }

        public static void ThreadedUpdate()
        {
#if XBOX
            int[] cpus = new int[1];
            cpus[0] = 5;
            Thread.CurrentThread.SetProcessorAffinity(cpus);
#endif

            timer.Start();
            while (!particleThreadExit.WaitOne(8)) //thread goes on forever!
            {
                FrequencyInverse = (float)(1.0 / (double)Stopwatch.Frequency);
                elapsedMilliseconds = timer.ElapsedTicks * FrequencyInverse;
                Update(elapsedMilliseconds);
                Draw(elapsedMilliseconds);
            }
            timer.Stop();
        }


        public static void Update(float elapsedTime)
        {
            updatedProjectiles = 0;
            projectilesToUpdate = projectileCount;

            if (projectileCount == 0 && 
                upgradeCount[1] == 0 && upgradeCount[2] == 0 && upgradeCount[3] == 0 && upgradeCount[4] == 0 &&
                repairCount[1] == 0 && repairCount[2] == 0 && repairCount[3] == 0 && repairCount[4] == 0)
                return;

            lock (projectileLock)
            {
                for (int i = 0; (i < MAX_PROJECTILES) && (updatedProjectiles < projectilesToUpdate); i++)
                {
                    if (projectiles[i].Active)
                    {
                        if (!projectiles[i].Update(elapsedTime))
                        {
                            // Remove projectiles at the end of their life.
                            projectiles[i].Active = false;
                            projectileCount--;
                            updatedProjectiles++;
                            if (updatedProjectiles >= projectilesToUpdate)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            for (int p = 1; p < 5; p++)
            {
                updatedStars = 0;
                lock (upgradeLock)
                {
                    starsToUpdate = upgradeCount[p];
                    for (int i = 0; (i < MAX_PROJECTILES) && (updatedStars < starsToUpdate); i++)
                    {
                        if (upgradeEffect[p][i].Active)
                        {
                            if (!upgradeEffect[p][i].Update(elapsedTime))
                            {
                                upgradeEffect[p][i].Active = false;
                                upgradeCount[p]--;
                                updatedStars++;

                                if (updatedStars >= starsToUpdate)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int p = 1; p < 5; p++)
            {
                updatedStars = 0;
                lock (repairLock)
                {
                    starsToUpdate = repairCount[p];
                    for (int i = 0; (i < MAX_PROJECTILES) && (updatedStars < starsToUpdate); i++)
                    {
                        if (repairEffect[p][i].Active)
                        {
                            if (!repairEffect[p][i].Update(elapsedTime))
                            {
                                repairEffect[p][i].Active = false;
                                repairCount[p]--;
                                updatedStars++;

                                if (updatedStars >= starsToUpdate)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

        public static void Draw(float elapsedTime)
        {

            explosionParticles.SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);
            explosionSmokeParticles.SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);
            projectileTrailParticles.SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);
            smokePlumeParticles.SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);
            fireParticles.SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);

            for (int p = 1; p < 5; p++)
            {
                upgradeParticles[p].SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);
                repairParticles[p].SetCamera(ref cameraManager.ViewMatrix, ref cameraManager.ProjectionMatrix);
            }
        }
    }
}
