
using Microsoft.Xna.Framework.Graphics;
namespace MyStory.Gfx2d
{
    interface ILayer
    {        
        public SpriteBatch SpriteBatch { get; }
        /*public BlendState? BlendState { get; }
        public SamplerState? SamplerState { get; }
        public DepthStencilState? DepthStencilState { get; }
        public RasterizerState? RasterizerState { get; }
        public Effect? Effect { get; }*/
        void Draw(Viewport viewport, Camera camera);
    }
}
