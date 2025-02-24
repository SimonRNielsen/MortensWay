using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;
using System;

namespace MortensWay
{
    internal class MousePointer<T>
    {
        #region Fields

        private T type;                                                         //Generic for defining object
        private Thread inputThread;                                             //Thread for running mouse input non-stop
        private Texture2D sprite;                                               //Custom mouse cursor texture
        private List<GameObject<T>> tempObjects = new List<GameObject<T>>();    //For handling more than one item at a time
        private List<GameObject<T>> gameObjects;                                //Reference-list
        private Vector2 firstPos;
        private Vector2 secondPos;
        private Vector2 position;
        private bool dragging;
        private bool enableDragSelection = false;
        private bool leftClick;
        private bool rightClick;
        private bool ranLeftClick = false;                                      //Blocks more than one run of event
        private bool ranRightClick = false;                                     //Blocks more than one run of event

        /// <summary>
        /// Left click eventhandler
        /// </summary>
        public Action LeftClickEventHandler;

        /// <summary>
        /// Right click eventhandler
        /// </summary>
        public Action RightClickEventHandler;

        #endregion
        #region Properties

        /// <summary>
        /// CollisionBox for registering mouse/objects overlap
        /// </summary>
        public Rectangle CollisionBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, 1, 1); }
        }

        /// <summary>
        /// Drag box for selecting multiple objects
        /// </summary>
        public Rectangle DragBox
        {
            get
            {
                if (dragging)
                    return new Rectangle((int)firstPos.X, (int)firstPos.Y, (int)(secondPos.X - firstPos.X), (int)(secondPos.Y - firstPos.Y));
                else
                    return new Rectangle((int)firstPos.X, (int)firstPos.Y, (int)(position.X - firstPos.X), (int)(position.Y - firstPos.Y));
            }
        }

        /// <summary>
        /// Used externally for mouse position referrencing
        /// </summary>
        public Vector2 Position { get => position; }

        /// <summary>
        /// Handles mouse left-click event and external detection thereof
        /// </summary>
        public bool LeftClick
        {
            get => leftClick;
            private set
            {
                leftClick = value;
                if (leftClick && !ranLeftClick)
                {
                    LeftClickEventHandler?.Invoke();
                    ranLeftClick = true;
                    if (enableDragSelection)
                        firstPos = position;
                }
                else if (!leftClick && ranLeftClick)
                {
                    ranLeftClick = false;
                    if (enableDragSelection)
                        secondPos = position;
                }
            }
        }

        /// <summary>
        /// Handles mouse right-click event and external detection thereof
        /// </summary>
        public bool RightClick
        {
            get => rightClick;
            private set
            {
                rightClick = value;
                if (rightClick && !ranRightClick)
                {
                    RightClickEventHandler?.Invoke();
                    ranRightClick = true;
                }
                else if (!rightClick && ranRightClick)
                    ranRightClick = false;
            }
        }

        #endregion
        #region Constructor

        /// <summary>
        /// Creates an instance for custom mouse handling
        /// </summary>
        /// <param name="type">Enum to define mouse and set sprite</param>
        public MousePointer(T type)
        {

            this.type = type;
            try
            {
                sprite = GameWorld.sprites[type as Enum];
            }
            catch { }
            inputThread = new Thread(HandleInput);
            inputThread.IsBackground = true;
            inputThread.Start();
            LeftClickEventHandler += LeftClickAction;
            RightClickEventHandler += RightClickAction;

        }

        /// <summary>
        /// Creates an instance for custom mouse handling
        /// </summary>
        /// <param name="type">Enum to define mouse and set sprite</param>
        /// <param name="list">Reference list</param>
        public MousePointer(T type, ref List<GameObject<T>> list)
        {

            this.type = type;
            gameObjects = list;
            try
            {
                sprite = GameWorld.sprites[type as Enum];
            }
            catch { }
            inputThread = new Thread(HandleInput);
            inputThread.IsBackground = true;
            inputThread.Start();
            LeftClickEventHandler += LeftClickAction;
            RightClickEventHandler += RightClickAction;

        }

        /// <summary>
        /// Creates an instance for custom mouse handling
        /// </summary>
        /// <param name="type">Enum to define mouse and set sprite</param>
        /// <param name="list">Reference list</param>
        /// <param name="enableDragBox">Enable drag-selection if true</param>
        public MousePointer(T type, ref List<GameObject<T>> list, bool enableDragBox)
        {

            this.type = type;
            gameObjects = list;
            enableDragSelection = enableDragBox;
            try
            {
                sprite = GameWorld.sprites[type as Enum];
            }
            catch { }
            inputThread = new Thread(HandleInput);
            inputThread.IsBackground = true;
            inputThread.Start();
            LeftClickEventHandler += LeftClickAction;
            RightClickEventHandler += RightClickAction;

        }

        #endregion
        #region Methods

        /// <summary>
        /// Draws a custom mousecursor at the location its detected to be in and
        /// </summary>
        /// <param name="spriteBatch">GameWorld logic</param>
        public void Draw(SpriteBatch spriteBatch)
        {

            if (sprite != null)
                spriteBatch.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            if (dragging && GameWorld.sprites.ContainsKey(LogicItems.CollisionPixel))
                DrawCollisionBox(spriteBatch);
            if (tempObjects.Count > 0 && GameWorld.sprites.ContainsKey(LogicItems.SelectionBox))
                foreach (GameObject<T> obj in tempObjects)
                {
                    float selectionBoxScale = SetSelectionBoxSize(obj);
                    spriteBatch.Draw(GameWorld.sprites[LogicItems.SelectionBox], obj.Position, null, Color.White, 0f, new Vector2((obj.Sprite.Width / 2 / selectionBoxScale) + 5, (obj.Sprite.Height / 2 / selectionBoxScale) + 5), selectionBoxScale, SpriteEffects.None, obj.Layer + 0.00001f);
                }
        }

        /// <summary>
        /// Method to run when left mouse is clicked
        /// </summary>
        private void LeftClickAction()
        {



        }

        /// <summary>
        /// Method to run when right mouse is clicked
        /// </summary>
        private void RightClickAction()
        {



        }

        /// <summary>
        /// Thread function to continuously loop HandleInput which translates player input
        /// </summary>
        private void HandleInput()
        {

            while (GameWorld.GameRunning)
            {
                var mouseState = Mouse.GetState();
                position = mouseState.Position.ToVector2();
                LeftClick = mouseState.LeftButton == ButtonState.Pressed;
                RightClick = mouseState.RightButton == ButtonState.Pressed;
                Update();
            }

        }

        /// <summary>
        /// Checks if mouse is on top of GameObject
        /// </summary>
        /// <param name="other">GameObject to be checked with</param>
        /// <returns>true if yes</returns>
        private bool CheckCollision(Rectangle collisionBox, Rectangle other)
        {

            if (collisionBox.Intersects(other))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Method to interact with "collided" object
        /// </summary>
        /// <param name="other">GameObject to manipulate</param>
        private void OnCollision(GameObject<T> other)
        {



        }


        public void DragBoxSelection()
        {

            tempObjects.Clear();

            lock (GameWorld.syncGameObjects)
            {
                foreach (GameObject<T> entry in gameObjects)
                    if (entry is ISelectable<T> && CheckCollision(DragBox, entry.CollisionBox))
                    {
                        tempObjects.Add(entry);
                    }
            }

        }


        private void Update()
        {

            if (enableDragSelection)
            {
                if (firstPos != Vector2.Zero)
                    dragging = true;
                else
                    dragging = false;

                if (secondPos != Vector2.Zero)
                {
                    DragBoxSelection();
                    firstPos = Vector2.Zero;
                    secondPos = Vector2.Zero;
                }
            }

        }

        /// <summary>
        /// Draws drag-selection
        /// </summary>
        /// <param name="spriteBatch">GameWorld logic</param>
        private void DrawCollisionBox(SpriteBatch spriteBatch)
        {

            Color color = Color.Green;
            Rectangle collisionBox = DragBox;
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);


            spriteBatch.Draw(GameWorld.sprites[LogicItems.CollisionPixel], topLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(GameWorld.sprites[LogicItems.CollisionPixel], bottomLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(GameWorld.sprites[LogicItems.CollisionPixel], rightLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(GameWorld.sprites[LogicItems.CollisionPixel], leftLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1f);

        }

        /// <summary>
        /// Sets the size and layer of the selection box
        /// </summary>
        private float SetSelectionBoxSize(GameObject<T> obj)
        {

            float selectionBoxScale = Math.Max(obj.Sprite.Width / (float)GameWorld.sprites[LogicItems.SelectionBox].Width, obj.Sprite.Height / (float)GameWorld.sprites[LogicItems.SelectionBox].Height);

            return selectionBoxScale;

        }

        #endregion
    }
}

