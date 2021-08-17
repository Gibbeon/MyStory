/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace MyStory
{
    class Camera {

        public Camera() {
            Scale = 1.0f;
            Wrap = true;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Scale { get; set; }
        public bool Wrap { get; set; }

        public void Move(int x, int y) {
            X += x;
            Y += y;

            if(!Wrap) {
                X = Math.Min(Width, Math.Max(0, X));
                Y = Math.Min(Height, Math.Max(0, Y));
            } else {
                X %= Width;
                Y %= Height;

                if(X < 0) X = Width + X;
                if(Y < 0) Y = Height + Y;
            }
        }

        public void Zoom(float f) {
            Scale += f;
            
                Scale = Math.Min(100f, Math.Max(0.10f, Scale));
        }

        
        public override string ToString()
        {
            return string.Format("Camera: {0}, {1} @ {2}", X, Y, Scale);
        }
    }

    class Canvas {

        public SpriteBatch SpriteBatch { get; private set; }
        public Camera Camera { get; private set; }        
        public IList<ILayer> Layers { get; private set; }        

        public Canvas(GraphicsDevice device) {
            SpriteBatch = new SpriteBatch(device);
            Camera = new Camera();
            Layers = new List<ILayer>();

            Camera.Height = Camera.Width = 8 * 256;
        }

        public void Draw(Viewport viewport) {
            
            SpriteBatch.Begin();
   
            foreach(var layer in Layers) {
                layer.Draw(viewport, Camera, SpriteBatch);
            }
            
            SpriteBatch.End();
            
        }
    }
    
    class Sprite {
        public Texture2D TileSet { get; set; }

        public int X { get; set; }
        public int Y { get; set; }        
        public int Width { get; set; }
        public int Height { get; set; }
        public float Scale { get; set; }

        public Sprite(int width, int height, Texture2D tileSet) {
            Scale = 1.0f;

            Width = width;
            Height = height;

            TileSet = tileSet;
        }

        
        public void Move(int x, int y) {
            X += x;
            Y += y;
        }

        public void Draw(Viewport viewport, Camera camera, SpriteBatch spriteBatch) {
            var adjScale = camera.Scale * Scale;
            
            var drawLocation = new Vector2(
                (-camera.X + X) * adjScale, 
                (-camera.Y + Y) * adjScale);

                spriteBatch.Draw(
                    TileSet, //texture
                    drawLocation,
                    Color.White);

            }
    }

    interface ILayer {
        void Draw(Viewport viewport, Camera camera, SpriteBatch spriteBatch);
    }

    class SpriteLayer : ILayer {
        public IList<Sprite> Sprites { get; private set; }

        public SpriteLayer() {
            Sprites = new List<Sprite>();
        }

        public void Draw(Viewport viewport, Camera camera, SpriteBatch spriteBatch) {
            foreach(var sprite in Sprites) {
                sprite.Draw(viewport, camera, spriteBatch);
            }
        }
    }

    class MapLayer : ILayer {        
        public int Width { get; set; }
        public int Height { get; set; }
        
        public float Scale { get; set; }
        public int TileSize { get; set; }
        public IList<MapTile> Tiles { get; private set; }

        public bool Wrap { get; set; }

        public Texture2D TileSet { get; set; }

        public MapLayer(int width, int height, int tileSize, Texture2D tileSet) {
            Scale = 1.0f;
            Tiles = new List<MapTile>(height*width);
            Width = width;
            Height = height;
            TileSet = tileSet;
            TileSize = tileSize;
            Wrap = true;

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x ++) {
                    Tiles.Add(new MapTile());
                }
            }
            
        }

        public void Draw(Viewport viewport, Camera camera, SpriteBatch spriteBatch) {
            var adjScale = camera.Scale * Scale;
            var adjTileSize = Math.Max(1, (int)(TileSize * adjScale));

            // this covers the map
            for(int y = 0; y <= viewport.Height; y += adjTileSize) {
                for(int x = 0; x <= viewport.Width; x += adjTileSize) {
                    // find the tile which is upper/left based on camera coordinates
                    int xCamera = (int)(x + camera.X * adjTileSize) % (Wrap ? (Width * adjTileSize) : int.MaxValue);
                    int yCamera = (int)(y + camera.Y * adjTileSize) % (Wrap ? (Height * adjTileSize) : int.MaxValue);

                    var xIndex = xCamera / adjTileSize;
                    var yIndex = yCamera / adjTileSize;

                    if((!Wrap) && (Tiles.Count <= (yIndex * Width + xIndex)))
                        continue;

                    var tile = Tiles[yIndex * Width + xIndex];
                    
                    var drawLocation = new Rectangle(
                        x - xCamera % adjTileSize, 
                        y - yCamera % adjTileSize,
                        adjTileSize+1, 
                        adjTileSize+1);

                    spriteBatch.Draw(
                        TileSet, //texture
                        drawLocation,
                        tile.TextureCoordinates,
                        Color.White);
                }
            }      
        }
    }

    class MapTile {
        public Rectangle TextureCoordinates;

    }

    public class Game1 : Game
    {
        private const int TILE_SIZE = 8;
        private GraphicsDeviceManager _graphics;

        private Canvas _canvas;
        private MapLayer _worldMap;

        private SpriteLayer _playerLayer;
        private Sprite _player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void GenMap() {
            
            SimplexNoise.Noise.Seed = (int)(new System.Random().NextDouble() * int.MaxValue);

            var heightmap = SimplexNoise.Noise.Calc2D(_worldMap.Width,_worldMap.Height, .03f);

            var xbuf = _worldMap.Width/6f;
            var ybuf = _worldMap.Height/6f;

            var tilemap = new int[7];

            tilemap[0] = (int)(150 / 7 * 0);
            tilemap[1] = (int)(150 / 7 * .75);
            tilemap[2] = (int)(150 / 7 * 1.25);
            tilemap[3] = (int)(150 / 7 * 1.7);
            tilemap[4] = (int)(150 / 7 * 2.0);
            tilemap[5] = (int)(150 / 7 * 9);
            tilemap[6] = (int)(150 / 7 * 12);


            for(int y = 0; y < _worldMap.Height; y++) { 
                for(int x = 0; x < _worldMap.Width; x++) {
                    var factor = Math.Min(1, x / xbuf) * Math.Min(1, (_worldMap.Width-x-1) / xbuf) * Math.Min(1, y / ybuf) * Math.Min(1, (_worldMap.Height-y-1) / ybuf);
                    var value = 0;
                    
                    for(int i = 0; i < 7; i++) {
                        if(tilemap[i] < (heightmap[x, y] * factor)) {
                            value = i;
                        }
                    }

                    value = Math.Max(0, Math.Min(7, value));

                    for(int i = 0; i < (int)8; i++) {
                        _worldMap.Tiles[_worldMap.Width * y + x].TextureCoordinates = new Rectangle(0, (int)(value) * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    }
                }
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {            
            _canvas = new Canvas(GraphicsDevice);
            _worldMap = new MapLayer(256,256,TILE_SIZE,new Texture2D(GraphicsDevice, TILE_SIZE, TILE_SIZE * 7));
            _canvas.Layers.Add(_worldMap);

            _playerLayer = new SpriteLayer();
            _player = new Sprite(TILE_SIZE, TILE_SIZE*2, new Texture2D(GraphicsDevice, TILE_SIZE, TILE_SIZE*2));
            _playerLayer.Sprites.Add(_player);
            _canvas.Layers.Add(_playerLayer);

            GenMap();

            //_canvas.Camera.Zoom(-.5f);
            //_canvas.Camera.Move(-1, 0);
            
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 0,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.DarkBlue, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 1*TILE_SIZE,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.Blue, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 2*TILE_SIZE,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.LightBlue, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 3*TILE_SIZE,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.Beige, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 4*TILE_SIZE,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.Green, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 5*TILE_SIZE,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.SaddleBrown, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 6*TILE_SIZE,TILE_SIZE,TILE_SIZE), Enumerable.Repeat(Color.Gray, TILE_SIZE*TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE );

            _player.TileSet.SetData<Color>(0, 0, new Rectangle(0, 0,TILE_SIZE,TILE_SIZE*2), Enumerable.Repeat(Color.Red, TILE_SIZE*TILE_SIZE*2).ToArray<Color>(), 0, TILE_SIZE*TILE_SIZE*2);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                _canvas.Camera.Move(0, -1);
                _player.Move(0, -1);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
                _canvas.Camera.Move(0, 1);
                _player.Move(0, 1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                //_canvas.Camera.Move(-1, 0);
                _player.Move(-1, 0);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                //_canvas.Camera.Move(1, 0);
                _player.Move(1, 0);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
                _canvas.Camera.Zoom(.1f);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.RightShift)) {
                _canvas.Camera.Zoom(-.1f);
            }

            this.Window.Title = _canvas.Camera.ToString();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _canvas.Draw( GraphicsDevice.Viewport);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
*/