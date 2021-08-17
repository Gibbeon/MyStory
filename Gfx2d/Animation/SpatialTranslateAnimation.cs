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
    public class SpatialTranslateAnimation : ISpatialAnimation
    {
        public Vector2 Initial { get; protected set; }
        public Vector2 Target { get; protected set; }
        public TimeSpan TotalTime { get; protected set; }
        public TimeSpan ElapsedTime { get; protected set; }
        public Spatial Spatial { get; protected set; }
        public bool IsDone { get; protected set; }
        public SpatialTranslateAnimation(Vector2 target, TimeSpan totalTime, Spatial spatial)
        {
            Initial = spatial.Position;
            Target = target;
            TotalTime = totalTime;
            Spatial = spatial;
        }

        public void Update(TimeSpan elapsedTime)
        {
            ElapsedTime += elapsedTime;

            Spatial.SetPosition(Vector2.Lerp(Initial, Target, Math.Min(1.0f, (float)(ElapsedTime.TotalMilliseconds / TotalTime.TotalMilliseconds))));

            IsDone = ElapsedTime.TotalMilliseconds >= TotalTime.TotalMilliseconds;
        }
    }

}
