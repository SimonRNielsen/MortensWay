using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Threading;
using System;

namespace MortensWay
{
    public class GameWorld : Game, ILoadAssets
    {
        #region Fields

        private static bool gameRunning = true;
        private static bool debugMode = false;

        #region Collections, Assets, Objects & Eventhandlers

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static ContentManager AddContent;
        internal static MousePointer<Enum> mousePointer;
        internal static KeyboardInput keyboard = new KeyboardInput();
        private static List<GameObject<Enum>> gameObjects = new List<GameObject<Enum>>();
        private static List<GameObject<Enum>> newGameObjects = new List<GameObject<Enum>>();
        public static HashSet<Tile> grid = new HashSet<Tile>();
        public static Dictionary<Enum, Texture2D> sprites = new Dictionary<Enum, Texture2D>();
        public static Dictionary<Enum, Texture2D[]> animations = new Dictionary<Enum, Texture2D[]>();
        public static Dictionary<Enum, SoundEffect> soundEffects = new Dictionary<Enum, SoundEffect>();
        public static Dictionary<Enum, Song> music = new Dictionary<Enum, Song>();
        public static SpriteFont gameFont;
        public static readonly object syncGameObjects = new object();
        public static Morten playerMorten = new Morten(MortensEnum.Bishop, new Vector2(64, 64 * 13));

        #endregion
        #endregion
        #region Properties

        /// <summary>
        /// Handles secure closing of seperate threads
        /// </summary>
        public static bool GameRunning { get => gameRunning; }

        /// <summary>
        /// Enables/Disables collision-textures
        /// </summary>
        public static bool DebugMode { get => debugMode; set => debugMode = value; }

        #endregion
        #region Constructor

        public GameWorld()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

        }

        #endregion
        #region Methods

        protected override void Initialize()
        {

            //Sets window resolution
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 960;
            _graphics.ApplyChanges();
            
            base.Initialize();

            //Instantiates mousePointer and makes "Content" static
            AddContent = Content;
            mousePointer = new MousePointer<Enum>(LogicItems.MousePointer, ref gameObjects, false);

            gameObjects.Add(playerMorten);

            #region gamemap
            //grass
            for (int j = 0; j < 15; j++)
            {
                for (int i = 0; i < 15; i++)
                {
                    TileTypes tile;
                    switch (i)
                    {
                        //case 1 when j > 5: //Test
                        //    tile = TileTypes.Fence;
                        //    break;
                        default:
                            tile = TileTypes.Grass;
                            break;
                    }
                    Tile t = new Tile(tile, new Vector2(64 * i, 64 * j));
                    gameObjects.Add(t);
                    grid.Add(t);
                }
            }
            foreach (Tile entry in grid)
            {
                entry.CreateEdges(grid);
            }

            //Fence
            for (int i = 3; i < 12; i++)
            {
                gameObjects.Add(new Tile(TileTypes.Fence, new Vector2(64 * i, 64 * 12)));
                gameObjects.Add(new Tile(TileTypes.Fence, new Vector2(64 * i, 64 * 14)));
            }

            //Fence path
            for (int i = 3; i < 12; i++)
            {
                gameObjects.Add(new Tile(TileTypes.FencePath, new Vector2(64 * i, 64 * 13)));

            }

            //Dirt & stone
            for (int i = 5; i < 9; i++)
            {
                for (int j = 4; j < 12; j++)
                {
                    if (j < 11)
                    {
                        gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * 4, 64 * j)));
                        gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * 9, 64 * j)));
                    }
                    gameObjects.Add(new Tile(TileTypes.Stone, new Vector2(64 * i, 64 * j)));
                }
            }
            for (int i = 4; i < 10; i++)
            {
                gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * i, 64 * 3)));
            }
            for (int i = 1; i < 5; i++)
            {
                if (i < 3)
                {
                    gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * (i + 1), 64 * 10)));
                }
                gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * (i + 9), 64 * 10)));
            }
            for (int i = 0; i < 3; i++)
            {
                gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * 2, 64 * (i + 11))));
                gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * 13, 64 * (i + 11))));
            }
            gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64, 64 * 13)));
            gameObjects.Add(new Tile(TileTypes.Path, new Vector2(64 * 12, 64 * 13)));


            //Towers & Portal
            gameObjects.Add(new Tile(TileTypes.Portal, new Vector2(64 * 0, 64 * 13)));
            gameObjects.Add(new Tile(TileTypes.TowerKey, new Vector2(64 * 1, 64 * 3)));
            gameObjects.Add(new Tile(TileTypes.TowerPortion, new Vector2(64 * 13, 64 * 12)));

            #endregion


            keyboard.CloseGame += ExitGame;


        }


        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ILoadAssets.Load(Content);


        }


        protected override void Update(GameTime gameTime)
        {
            //Registers keyboard input
            keyboard.HandleInput(gameTime);

            //Update loop (if any objects present)
            if (gameObjects.Count > 0)
                foreach (GameObject<Enum> gameObject in gameObjects)
                {
                    gameObject.Update(gameTime);
                }

            //Handles adding and removing objects from update-loop, is sync'ed with other method using same list via "syncGameObjects" lock object
            lock (syncGameObjects)
            {
                gameObjects.RemoveAll(obj => obj.IsAlive == false);
                gameObjects.AddRange(newGameObjects);
            }
            newGameObjects.Clear();

            base.Update(gameTime);

        }


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Handles sorting from highest value front to lowest value in the back
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

            //Draws custom mousepointer
            mousePointer.Draw(_spriteBatch);

            //Syncs with other method iterating gameObjects via "syncGameObjects" lock object
            lock (syncGameObjects)
            {
                //Iterates list only if any objects present
                if (gameObjects.Count > 0)
                    foreach (GameObject<Enum> gameObject in gameObjects)
                    {
                        gameObject.Draw(_spriteBatch);
                        DrawCollisionBox(gameObject);
                    }
            }

            _spriteBatch.End();

            base.Draw(gameTime);

        }

        /// <summary>
        /// Method to add one or more objects and run its "LoadContent" method
        /// </summary>
        /// <param name="obj">Either single object, list or array</param>
        public static void AddObject(GameObject<Enum> obj)
        {

            obj.LoadContent(AddContent);
            newGameObjects.Add(obj);

        }
        public static void AddObject(GameObject<Enum>[] objs)
        {

            foreach (GameObject<Enum> obj in objs)
            {
                obj.LoadContent(AddContent);
                newGameObjects.Add(obj);
            }

        }
        public static void AddObject(List<GameObject<Enum>> objs)
        {

            foreach (GameObject<Enum> obj in objs)
            {
                obj.LoadContent(AddContent);
                newGameObjects.Add(obj);
            }

        }

#if DEBUG

        private void DrawCollisionBox<T>(GameObject<T> gameObject)
        {

            if (debugMode)
            {

                Color color = Color.Red;
                Rectangle collisionBox = gameObject.CollisionBox;
                Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
                Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
                Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
                Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

                _spriteBatch.Draw(sprites[LogicItems.CollisionPixel], topLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);
                _spriteBatch.Draw(sprites[LogicItems.CollisionPixel], bottomLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);
                _spriteBatch.Draw(sprites[LogicItems.CollisionPixel], rightLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);
                _spriteBatch.Draw(sprites[LogicItems.CollisionPixel], leftLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);

            }

        }

#endif

        private void ExitGame()
        {
            gameRunning = false;
            Thread.Sleep(20);
            Exit();
        }


        #endregion
    }
}