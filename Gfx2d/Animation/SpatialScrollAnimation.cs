using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyStory.Gfx2d;
using SpriteFontPlus;

namespace MyStory.Gfx2d.Animation
{
    public class SpatialScrollingAnimation : ISpatialAnimation
    {
        public Vector2 Delta { get; protected set; }
        public Spatial Spatial { get; protected set; }
        public TimeSpan DeltaTime { get; protected set; }
        public bool IsDone { get; protected set; }
        public SpatialScrollingAnimation(Vector2 delta, TimeSpan deltaTime, Spatial spatial)
        {
            Delta = delta;
            Spatial = spatial;
            DeltaTime = deltaTime;
            IsDone = false;
        }

        public void Update(TimeSpan elapsedTime)
        {
            float increments = (float)(elapsedTime.TotalMilliseconds / DeltaTime.TotalMilliseconds);

            Spatial.SetPosition(Spatial.Position + (Delta * increments));
        }
    }
}
