using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Diagnostics;

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
        private static bool arrived = true;

        public Random random = new Random();
        public static Tile keyOne;
        public static Tile keyTwo;

        //private static List<T> tileObjects = new List<Tile>();


        //Irene tester Astar
        private static GameWorld instance;

        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        #endregion
        #endregion
        #region Properties

        /// <summary>
        /// Handles secure closing of seperate threads
        /// </summary>
        public static bool GameRunning { get => gameRunning; }

        public static bool Arrived { get => arrived; set => arrived = value; }

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

            ///////////////////                     ///////////////////                     ///////////////////                     ///////////////////                     ///////////////////                     
            //Dictionary<Vector2, Tile> cells = new Dictionary<Vector2, Tile>();

            //// Eksempel: Tilføj tiles til cells
            //for (int x = 0; x < 10; x++)
            //{
            //    for (int y = 0; y < 10; y++)
            //    {
            //        Vector2 position = new Vector2(x, y);
            //        Tile tile = new Tile(TileTypes.Grass, position);
            //        cells.Add(position, tile);
            //    }
            //}

            //AStar astar = new AStar(cells);
            //List<Tile> path = astar.FindPath(new Vector2(0, 0), new Vector2(5, 5));

            //if (path != null)
            //{
            //    Console.WriteLine("Path found");
            //    foreach (Tile tile in path)
            //    {
            //        Console.WriteLine($"({tile.Position.X}, {tile.Position.Y})");
            //    }
            //}
            //else 
            //{
            //    Console.WriteLine("No path found.");
            //}
            ///////////////////                     ///////////////////                     ///////////////////                     ///////////////////                     ///////////////////                     ///////////////////                     

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

                grid.Add(new Tile(TileTypes.Fence, new Vector2(64 * i, 64 * 12)));
                grid.Add(new Tile(TileTypes.Fence, new Vector2(64 * i, 64 * 14)));
            }

            //Fence path
            for (int i = 3; i < 12; i++)
            {
                grid.Add(new Tile(TileTypes.FencePath, new Vector2(64 * i, 64 * 13)));

            }

            //Dirt & stone
            for (int i = 5; i < 9; i++)
            {
                for (int j = 4; j < 12; j++)
                {
                    if (j < 11)
                    {
                        grid.Add(new Tile(TileTypes.Path, new Vector2(64 * 4, 64 * j)));
                        grid.Add(new Tile(TileTypes.Path, new Vector2(64 * 9, 64 * j)));
                    }
                    grid.Add(new Tile(TileTypes.Stone, new Vector2(64 * i, 64 * j)));
                }
            }
            for (int i = 4; i < 10; i++)
            {
                grid.Add(new Tile(TileTypes.Path, new Vector2(64 * i, 64 * 3)));
            }
            for (int i = 1; i < 5; i++)
            {
                if (i < 3)
                {
                    grid.Add(new Tile(TileTypes.Path, new Vector2(64 * (i + 1), 64 * 10)));
                }
                grid.Add(new Tile(TileTypes.Path, new Vector2(64 * (i + 9), 64 * 10)));
            }
            for (int i = 0; i < 3; i++)
            {
                grid.Add(new Tile(TileTypes.Path, new Vector2(64 * 2, 64 * (i + 11))));
                grid.Add(new Tile(TileTypes.Path, new Vector2(64 * 13, 64 * (i + 11))));
            }
            grid.Add(new Tile(TileTypes.Path, new Vector2(64, 64 * 13)));
            grid.Add(new Tile(TileTypes.Path, new Vector2(64 * 12, 64 * 13)));

            //Towers & Portal
            grid.Add(new Tile(TileTypes.Portal, new Vector2(64 * 0, 64 * 13)));
            grid.Add(new Tile(TileTypes.TowerKey, new Vector2(64 * 1, 64 * 3)));
            grid.Add(new Tile(TileTypes.TowerPortion, new Vector2(64 * 13, 64 * 12)));


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
                        case 2 when j > 10 && j < 14:
                        case 3 when j == 11:
                        case 4 when j > 2 && j < 12:
                        case 5 when j == 3:
                        case 6 when j == 3:
                        case 7 when j == 3:
                        case 8 when j == 3:
                        case 9 when j > 2 && j < 12:
                        case 10 when j == 11:
                        case 11 when j == 11:
                        case 12 when j == 11 || j == 13:
                        case 13 when j == 11 || j == 13:
                            tile = TileTypes.Path;
                            break;
                        case 5 when j > 3 && j < 12:
                        case 6 when j > 3 && j < 12:
                        case 7 when j > 3 && j < 12:
                        case 8 when j > 3 && j < 12:
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
            //Adding key
            //keyOne = new Tile(TileTypes.Key, KeyPlacement(random, grid));
            //gameObjects.Add(keyOne);
            //keyTwo = new Tile(TileTypes.Key, KeyPlacement(random, grid));
            //gameObjects.Add(keyTwo);
            keyOne = ChangeToKey();
            keyTwo = ChangeToKey();
            foreach (Tile entry in grid)
            {
                entry.CreateEdges(grid);
            }

            #endregion

            keyboard.CloseGame += ExitGame;
            
            //Test of BFS: 
            Tile startNode = (Tile)(gameObjects.Find(x => x.Position == playerMorten.Position && x != playerMorten));
            Tile endNode = (Tile)(gameObjects.Find(x => (TileTypes)x.Type == (TileTypes)TileTypes.Key));
            BFS.BFSMethod(startNode, endNode);
            List<Tile> pathTest = BFS.FindPath(endNode, startNode);
            foreach (Tile t in pathTest)
            {
                t.Color = Color.LightBlue;

            }

            ////Test af Astar
            //Tile startPoint = (Tile)(gameObjects.Find(x => (TileTypes)x.Type == TileTypes.Portal));
            //Tile endPoint = (Tile)(gameObjects.Find(x => (TileTypes)x.Type == (TileTypes)TileTypes.TowerKey));
            //AStar.FindPath(startPoint.Position, endPoint.Position);
            //List<Tile> pathAstarTest = AStar.FindPath(startPoint, endPoint);
            //foreach (Tile q in pathAstarTest)
            //{
            //    q.Color = Color.Violet;

            //}
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

#if DEBUG

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                foreach (GameObject<Enum> entry in gameObjects)
                {
                    if (entry is Tile && (entry as Tile).FencePath)
                        (entry as Tile).Walkable = false;
                }

#endif

            if (arrived)
            {

                Tile startNode = (Tile)(gameObjects.Find(x => (TileTypes)x.Type == TileTypes.Portal));
                Tile endNode = (Tile)(gameObjects.Find(x => (TileTypes)x.Type == (TileTypes)TileTypes.TowerKey));
                BFS.BFSMethod(startNode, endNode);
                List<Tile> pathTest = BFS.FindPath(endNode, startNode);
                foreach (Tile tile in pathTest)
                {
                    tile.Color = Color.LightBlue;

                }
                Thread t = new Thread(() => playerMorten.FollowPath(pathTest));
                t.IsBackground = true;
                t.Start();
                arrived = false;

            }

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
            TnEDebug();
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
                int rndX = random.Next(0, 29);
                int rndY = random.Next(0, 29);

                //The random generatet placement
                placement = new Vector2(rndX * 32, rndY * 32);

                //Tjecking if the position is walkable
                foreach (Tile item in grid)
                {
                    if (item.Position == placement)
                    {
                        if (item.Walkable == false || item.Type.Equals(TileTypes.FencePath))
                        {
                            break;
                        }
                        else if (item.Type.Equals(TileTypes.Portal) || item.Type.Equals(TileTypes.TowerKey) || item.Type.Equals(TileTypes.TowerPortion) || item.Type.Equals(TileTypes.Key))
                        {
                            break;
                        }
                        else
                        {
                            Debug.WriteLine($"{item.Type} {item.Position} {item.Walkable}");
                            //If the position of the tile is walkable then change "finding" to false to break the while loop
                            finding = false;
                            return placement;
                        }
                    }
                }

            }

            return placement;

        }

        private Tile ChangeToKey()
        {

            bool pickRandom = true;
            Tile tile = new Tile(TileTypes.Key, Vector2.Zero);
            while (pickRandom)
            {
                int something = random.Next(0, gameObjects.Count);
                if (gameObjects[something] is Tile)
                {
                    tile = gameObjects[something] as Tile;
                    if (tile.Walkable && !tile.FencePath && (tile.Type.Equals(TileTypes.Grass) || tile.Type.Equals(TileTypes.Path)))
                    {
                        tile.ChangeToKey();
                        pickRandom = false;
                    }
                }
            }
            return tile;

        }
        private void TnEDebug()
        {
            int tiles = 0;
            int edges = 0;
            int edgeweight = 0;
            foreach (Tile tile in grid)
            {
                tiles++;
                foreach (Edge edge in tile.Edges)
                {
                    edges++;
                    edgeweight += edge.Weight;
                }
            }
            float averageWeight = edgeweight / (float)edges;
            Debug.WriteLine("Tiles: " + tiles);
            Debug.WriteLine("Edges: " + edges);
            Debug.WriteLine("Edge weight total: " + edgeweight);
            Debug.WriteLine("Average edge weight: " + averageWeight);
        }

        #endregion
    }
}
