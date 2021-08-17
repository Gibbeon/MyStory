using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyStory.Gfx2d
{
    class Sprite : Spatial
    {
        public Texture2D TileSet { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Sprite(int width, int height, Texture2D tileSet)
        {
            Width = width;
            Height = height;

            TileSet = tileSet;
        }
        public void Move(int x, int y)
        {
            
        }
        public void Draw(Viewport viewport, Camera camera, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                TileSet, //texture
                Position,
                Color.White);
        }
    }
}
