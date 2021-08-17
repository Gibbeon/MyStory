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
    class AnimationController {
        public IList<IAnimation> Animations { get; protected set; }
        
        public AnimationController() {
            Animations = new List<IAnimation>();
        }
        public void Update(TimeSpan elapsedTime) {
            for(var index = 0; index < Animations.Count; index++) {
                Animations[index].Update(elapsedTime);
                if(Animations[index].IsDone) {
                    Animations.RemoveAt(index);
                    index--;
                }
            }
        }
    }
}
