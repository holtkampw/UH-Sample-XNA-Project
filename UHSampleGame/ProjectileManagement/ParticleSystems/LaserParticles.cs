#region File Description
//-----------------------------------------------------------------------------
// ExplosionParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UHSampleGame.ProjectileManagment
{
    /// <summary>
    /// Custom particle system for creating the fiery part of the explosions.
    /// </summary>
    class LaserParticles : ParticleSystem
    {
        public LaserParticles(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Laser";

            settings.MaxParticles = 30000;

            settings.Duration = TimeSpan.FromSeconds(1);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 30;
            settings.MaxHorizontalVelocity = 30;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 10;

            settings.EndVelocity = 0;

            settings.MinColor = Color.Coral;
            settings.MaxColor = Color.DarkRed;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 3;

            settings.MinStartSize = 100;
            settings.MaxStartSize = 200;

            settings.MinEndSize = 100;
            settings.MaxEndSize = 200;

            // Use additive blending.
            settings.BlendState = BlendState.AlphaBlend;
        }
    }
}
