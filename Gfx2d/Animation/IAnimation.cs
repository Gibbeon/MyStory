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
    interface IAnimation
    {       
        bool IsDone { get; }
     
        public void Update(TimeSpan elapsedTime);
    }
}
