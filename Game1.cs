using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyStory.Gfx2d;
using MyStory.Gfx2d.Animation;
using SpriteFontPlus;

namespace MyStory
{

    public class Game1 : Game
    {
        private const int TILE_SIZE = 8;
        private GraphicsDeviceManager _graphics;

        private Canvas _canvas;
        private MapLayer _worldMap;
        private SpriteLayer _playerLayer;
        private Sprite _player;
        private SpriteFont _font;

        private MapLayer _clouds;

        AnimationController _animationController;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void GenCloudMap()
        {
            var heightmap = SimplexNoise.Noise.Calc2D(_clouds.Width, _clouds.Height, .02f);


            for (int y = 0; y < _worldMap.Height; y++)
            {
                for (int x = 0; x < _worldMap.Width; x++)
                {
                    var factor = 1;
                    var value = 0;

                    for (int i = 0; i < 7; i++)
                    {
                        if (200 < (heightmap[x, y] * factor))
                        {
                            value = i;
                        }
                    }

                    value = Math.Max(0, Math.Min(7, value));
                    if (value > 0)
                        _clouds.Tiles[_clouds.Width * y + x].TextureCoordinates = new Rectangle(0, (int)(value) * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                }
            }

        }

        private void GenMap()
        {

            SimplexNoise.Noise.Seed = (int)(new System.Random().NextDouble() * int.MaxValue);

            var heightmap = SimplexNoise.Noise.Calc2D(_worldMap.Width, _worldMap.Height, .03f);

            var xbuf = _worldMap.Width / 3f;
            var ybuf = _worldMap.Height / 3f;

            var tilemap = new int[7];

            var floatValue = 120;

            tilemap[0] = (int)(floatValue / 7 * 0);
            tilemap[1] = (int)(floatValue / 7 * .75);
            tilemap[2] = (int)(floatValue / 7 * 1.25);
            tilemap[3] = (int)(floatValue / 7 * 1.7);
            tilemap[4] = (int)(floatValue / 7 * 2.0);
            tilemap[5] = (int)(floatValue / 7 * 9);
            tilemap[6] = (int)(floatValue / 7 * 12);


            for (int y = 0; y < _worldMap.Height; y++)
            {
                for (int x = 0; x < _worldMap.Width; x++)
                {
                    var factor = Math.Min(1, x / xbuf) * Math.Min(1, (_worldMap.Width - x - 1) / xbuf) * Math.Min(1, y / ybuf) * Math.Min(1, (_worldMap.Height - y - 1) / ybuf);
                    var value = 0;

                    for (int i = 0; i < 7; i++)
                    {
                        if (tilemap[i] < (heightmap[x, y] * factor))
                        {
                            value = i;
                        }
                    }

                    value = Math.Max(0, Math.Min(7, value));

                    for (int i = 0; i < (int)8; i++)
                    {
                        _worldMap.Tiles[_worldMap.Width * y + x].TextureCoordinates = new Rectangle(0, (int)(value) * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    }
                }
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            //_graphics.SynchronizeWithVerticalRetrace = false;
            //_graphics.ApplyChanges();
            //IsFixedTimeStep = false;
            //this.TargetElapsedTime = TimeSpan.FromMilliseconds(300);
        }

        protected override void LoadContent()
        {
            _text = new SpriteBatch(GraphicsDevice);

            _canvas = new Canvas(GraphicsDevice);
            _worldMap = new MapLayer(256, 256, TILE_SIZE, new SpriteBatch(GraphicsDevice), new Texture2D(GraphicsDevice, TILE_SIZE, TILE_SIZE * 7));
            
            _canvas.Layers.Add(_worldMap);
            //_canvas.Camera.SetScale(10);

            _playerLayer = new SpriteLayer(new SpriteBatch(GraphicsDevice));
            _player = new Sprite(TILE_SIZE, TILE_SIZE * 2, new Texture2D(GraphicsDevice, TILE_SIZE, TILE_SIZE * 2));
            _playerLayer.Sprites.Add(_player);
            _canvas.Layers.Add(_playerLayer);

            
            _clouds = new MapLayer(256, 256, TILE_SIZE, new SpriteBatch(GraphicsDevice), new Texture2D(GraphicsDevice, TILE_SIZE, TILE_SIZE));
            _canvas.Layers.Add(_clouds);

            _animationController = new AnimationController();
            _animationController.Animations.Add(new SpatialScrollingAnimation(new Vector2(1, 0), TimeSpan.FromMilliseconds(100), _canvas.Camera));
            _animationController.Animations.Add(new SpatialScrollingAnimation(new Vector2(-10, 5), TimeSpan.FromMilliseconds(500), _clouds));
            _animationController.Animations.Add(new SpatialTranslateAnimation(new Vector2(250, 250), TimeSpan.FromSeconds(3), _player));

            GenMap();
            GenCloudMap();

            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.DarkBlue, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 1 * TILE_SIZE, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.Blue, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 2 * TILE_SIZE, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.LightBlue, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 3 * TILE_SIZE, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.Beige, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 4 * TILE_SIZE, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.Green, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 5 * TILE_SIZE, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.SaddleBrown, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);
            _worldMap.TileSet.SetData<Color>(0, 0, new Rectangle(0, 6 * TILE_SIZE, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(Color.Gray, TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);

            _player.TileSet.SetData<Color>(0, 0, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE * 2), Enumerable.Repeat(Color.Red, TILE_SIZE * TILE_SIZE * 2).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE * 2);

            _clouds.TileSet.SetData<Color>(0, 0, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE), Enumerable.Repeat(new Color(Color.WhiteSmoke,.25f), TILE_SIZE * TILE_SIZE).ToArray<Color>(), 0, TILE_SIZE * TILE_SIZE);

            _clouds.BlendState = BlendState.Additive; 

            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"),
                25,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );

            _font = fontBakeResult.CreateSpriteFont(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            _animationController.Update(gameTime.ElapsedGameTime);


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _canvas.Camera.Move(0, -3);
                //_player.Move(0, -1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _canvas.Camera.Move(0, 3);
                //_player.Move(0, 1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _canvas.Camera.Move(-3, 0);
                //_player.Move(-1, 0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _canvas.Camera.Move(3, 0);
                //_player.Move(1, 0);
            }

            this.Window.Title = _canvas.Camera.ToString() + " FPS: " + 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        
            SpriteBatch _text;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _canvas.Draw(GraphicsDevice.Viewport);

            //xd_text.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            
            //_text.DrawString(_font, "HelloWorld", new Vector2(300, 300), new Color(Color.White,.25f));

            //_text.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
