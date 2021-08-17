using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MyStory.Gfx2d
{
    class SpriteLayer : Spatial, ILayer
    {
        public SpriteBatch SpriteBatch { get; protected set; }
        public IList<Sprite> Sprites { get; private set; }

        public SpriteLayer(SpriteBatch spriteBatch)
        {
            Sprites = new List<Sprite>();
            SpriteBatch = spriteBatch;
        }

        public void Draw(Viewport viewport, Camera camera)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null,  this.GetLocalMatrix() * camera.GetLocalMatrix());

            foreach (var sprite in Sprites)
            {
                sprite.Draw(viewport, camera, SpriteBatch);
            }

            SpriteBatch.End();
        }
    }
}