using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyStory.Gfx2d
{
    class MapLayer : Spatial, ILayer
    {
        public SpriteBatch SpriteBatch { get; protected set; }
        public BlendState? BlendState { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public IList<MapTile> Tiles { get; private set; }
        public Texture2D TileSet { get; set; }
        public bool Visible { get; set; }

        public bool Wrap { get; set; }
        public MapLayer(int width, int height, int tileSize, SpriteBatch spriteBatch, Texture2D tileSet)
        {

            SpriteBatch = spriteBatch;
            Tiles = new List<MapTile>(height * width);
            Width = width;
            Height = height;
            TileSet = tileSet;
            TileSize = tileSize;
            Visible = true;
            Wrap = true;

            for (int y = 0; y < height * width; y++)
            {
                Tiles.Add(new MapTile());
            }
        }

        private void DoDraw(int x, int y, int tileCount, Rectangle? textureCoordinates)
        {
            if(textureCoordinates.HasValue) {
                SpriteBatch.Draw(
                    TileSet, //texture
                    new Rectangle(x * TileSize, y * TileSize, tileCount * TileSize, TileSize),
                    textureCoordinates,
                    Color.White);
            }
        }

        public void Draw(Viewport viewport, Camera camera)
        {
            if(!Visible) return;

            var scaledTileSize = (this.Scale.X * camera.Scale.X * TileSize);

            Camera wrappedCamera = Wrap ? Camera.Wrap(camera, this.Position.X, this.Position.Y, (Width * scaledTileSize), (Height * scaledTileSize)) : camera;

            int xTiles = (int)(viewport.Width / Math.Max(1, scaledTileSize));
            int yTiles = (int)(viewport.Height / Math.Max(1, scaledTileSize));

            int xOffset = (int)((wrappedCamera.Position.X)  / Math.Max(1, scaledTileSize));
            int yOffset = (int)((wrappedCamera.Position.Y) / Math.Max(1, scaledTileSize));

            var xMaxTiles = xOffset+xTiles+1;

            if(!Wrap) {
                xTiles = Math.Min(xTiles, Width);
                yTiles = Math.Min(yTiles, Height);
                xOffset = Math.Min(xOffset, Width);
                yOffset = Math.Min(yOffset, Height);
                xMaxTiles = Math.Min(Width, (xOffset+xTiles+1));
            }

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState.PointWrap, null, null, null, wrappedCamera.GetLocalMatrix());

            var coordinates = Tiles[0].TextureCoordinates;
            var xCount = 0;

            for (var y = yOffset; y < (yOffset+yTiles+1); y++)
            {
                for (var x = xOffset; x < (xOffset+xTiles+1); x++)
                {
                    if(!Wrap && ((y >= Height || x >= Width) || (y < 0 || x < 0))) 
                        continue;
                    
                    if (Tiles[(y % Height) * Width + (x % Width)].TextureCoordinates == coordinates)
                    {
                        xCount++;
                        continue;
                    }

                    DoDraw(x - xCount, y, xCount, coordinates);
                    
                    coordinates = Tiles[(y % Height) * Width + (x % Width)].TextureCoordinates;
                    xCount = 1;
                }

                if(!Wrap && (y >= Height || y < 0)) 
                    continue;
            
                DoDraw(xMaxTiles - xCount, y, xCount, coordinates);

                coordinates = Tiles[(y % Height) * Width].TextureCoordinates;
                xCount = 0;
            }

            DoDraw(xMaxTiles - xCount, (yTiles - 1), xCount, coordinates);

            SpriteBatch.End();
        }
    }

    class MapTile
    {
        public Rectangle? TextureCoordinates;
    }
}
