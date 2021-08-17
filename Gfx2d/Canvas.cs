using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MyStory.Gfx2d
{
    class Canvas
    {
        public Camera Camera { get; private set; }
        public IList<ILayer> Layers { get; private set; }

        public Canvas(GraphicsDevice device)
        {
            Camera = new Camera();
            Layers = new List<ILayer>();
        }

        public void Draw(Viewport viewport)
        {
            foreach (var layer in Layers)
            {
                layer.Draw(viewport, Camera);
            }
        }
    }
}
