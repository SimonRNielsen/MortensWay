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
        public Morten playerMorten;

        public Random random = new Random();
        public static Tile keyOne;
        public static Tile keyTwo;

        //private static List<T> tileObjects = new List<Tile>();


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

            //Adding Morten instants
            playerMorten = new Morten(MortensEnum.Bishop, new Vector2(64, 64 * 13));
            gameObjects.Add(playerMorten);

            //Adding key
            keyOne = new Tile(TileTypes.Key, KeyPlacement(random, grid));
            gameObjects.Add(keyOne);
            keyTwo = new Tile(TileTypes.Key, KeyPlacement(random, grid));
            gameObjects.Add(keyTwo);

            #region gamemap
            //grass
            for (int j = 0; j < 15; j++)
            {
                for (int i = 0; i < 15; i++)
                {
                    TileTypes tile;
                    switch (i)
                    {
                        case 0 when j == 13:
                            tile = TileTypes.Portal;
                            break;
                        case 1 when j == 3:
                            tile = TileTypes.TowerKey;
                            break;
                        case 13 when j == 12:
                            tile = TileTypes.TowerPortion;
                            break;
                        case > 2 when i < 12 && (j == 12 || j == 14):
                            tile = TileTypes.Fence;
                            break;
                        case > 2 when i < 12 && j == 13:
                            tile = TileTypes.FencePath;
                            break;
                        case 1 when j == 13:
                        case 2 when j < 14 && j > 10:
                        case 3 when j == 11:
                        case 4 when j < 12 && j > 3:
                        case 4 when j == 3:
                        case 5 when j == 3:
                        case 6 when j == 3:
                        case 7 when j == 3:
                        case 8 when j == 3:
                        case 9 when j == 3:
                        case 9 when j < 12 && j > 3:
                        case 10 when j == 11:
                        case 11 when j == 11:
                        case 12 when j == 11 || j == 13:
                        case 13 when j == 11 || j == 13:
                            tile = TileTypes.Path;
                            break;
                        case 5 when j < 12 && j > 3:
                        case 6 when j < 12 && j > 3:
                        case 7 when j < 12 && j > 3:
                        case 8 when j < 12 && j > 3:
                            tile = TileTypes.Stone;
                            break;
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

        /// <summary>
        /// Finding a random walkable place for the key to spawn 
        /// </summary>
        /// <param name="random">A variabel from the Random class</param>
        /// <param name="grid">HashSet grid over the tiles</param>
        /// <returns></returns>
        public Vector2 KeyPlacement(Random random, HashSet<Tile> grid)
        {
            bool finding = true;
            Vector2 placement = Vector2.Zero;

            while (finding)
            {
                //Random generation the x and y position
                int rndX = random.Next(0, 16);
                int rndY = random.Next(0, 16);

                //The random generatet placement
                placement = new Vector2(rndX * 64, rndY * 64);

                //Tjecking if the position is walkable
                foreach (Tile item in grid)
                {
                    if (item.Position == placement && item.Walkable == true)
                    {
                        if (!item.Type.Equals(TileTypes.Portal) || !item.Type.Equals(TileTypes.TowerKey) || !item.Type.Equals(TileTypes.TowerPortion))
                        {
                            //If the position of the tile is walkable then change "finding" to false to break the while loop
                            finding = false;
                        }
                    }
                    continue;
                }

                finding = false;
            }

            return placement;

        }


        #endregion
    }
}