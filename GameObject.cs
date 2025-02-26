using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Content;

namespace MortensWay
{
    public abstract class GameObject<T>
    {
        #region Fields

        private readonly object animationLock = new object();
        protected T type;
        protected Texture2D sprite;
        protected Texture2D[] sprites;
        protected Color color = Color.White;
        protected Vector2 position;
        protected Vector2 velocity;
        protected SpriteEffects[] spriteEffects = new SpriteEffects[3] { SpriteEffects.None, SpriteEffects.FlipHorizontally, SpriteEffects.FlipVertically };
        protected float fps = 20;
        protected float layer = 0.3f;
        protected float scale = 1;
        protected float timeElapsed;
        protected float speed;
        protected int spriteEffectIndex;
        protected int health;
        protected const int maxHealth = 100;
        private float rotation;
        private int currentIndex;
        private bool isAlive = true;

        #endregion

        #region Properties

        /// <summary>
        /// Returns a CollisionBox dependant on if there's a sprite or not, and if yes, the rotation thereof
        /// </summary>
        public virtual Rectangle CollisionBox
        {
            get
            {
                if (sprite != null)
                    switch (rotation)
                    {
                        case MathHelper.Pi / 2:
                        case (MathHelper.Pi / 2) * 3:
                            return new Rectangle((int)(position.Y - (sprite.Height / 2) * scale), (int)(position.X - (sprite.Width / 2) * scale), (int)(sprite.Height * scale), (int)(sprite.Width * scale));
                        default:
                            return new Rectangle((int)(position.X - (sprite.Width / 2) * scale), (int)(position.Y - (sprite.Height / 2) * scale), (int)(sprite.Width * scale), (int)(sprite.Height * scale));
                    }
                else
                    return new Rectangle();
            }
        }

        /// <summary>
        /// Enables remote monitor and setting of animation array, syncs with Animate method to avoid out-of-bounds exception
        /// </summary>
        public virtual Texture2D[] Sprites
        {
            get => sprites;
            set
            {
                lock (animationLock)
                {
                    sprites = value;
                    currentIndex = 0;
                    timeElapsed = 0;
                }
            }
        }

        /// <summary>
        /// Property for remote designation for removal of object from update loop
        /// </summary>
        public virtual bool IsAlive
        {
            get => isAlive;
            set => isAlive = value;
        }

        /// <summary>
        /// Property for remote setting of health and removal of object from update loop if health reaches 0 or below
        /// </summary>
        public virtual int Health
        {
            get => health;
            set
            {
                health = value;
                switch (health)
                {
                    case <= 0:
                        isAlive = false;
                        break;
                    case > maxHealth:
                        health = maxHealth;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Property for remote reading of current sprite
        /// </summary>
        public virtual Texture2D Sprite { get => sprite; set => sprite = value; }

        /// <summary>
        /// Property for getting/setting the Enum designating what the object identifies as
        /// </summary>
        public virtual T Type { get => type; set => type = value; }

        /// <summary>
        /// Property for getting/setting position of the object
        /// </summary>
        public virtual Vector2 Position { get => position; set => position = value; }

        /// <summary>
        /// Property for getting/setting which SpriteEffect the object is drawn with
        /// </summary>
        public virtual int SpriteEffectIndex { get => spriteEffectIndex; set => spriteEffectIndex = value; }

        /// <summary>
        /// Property for getting/setting rotation of the object
        /// </summary>
        public virtual float Rotation { get => rotation; set => rotation = value; }

        /// <summary>
        /// Property to compare layers
        /// </summary>
        public virtual float Layer { get => layer; }
        public Color Color { get => color; set => color = value; }

        #endregion

        #region Constructor

        /// <summary>
        /// Template for GameObjects-subclasses to follow
        /// </summary>
        /// <param name="type">Enum that defines the object</param>
        /// <param name="spawnPos">Starting position of the object</param>
        public GameObject(T type, Vector2 spawnPos)
        {

            Type = type;
            try
            {
                sprite = GameWorld.sprites[(Type as Enum)];
            }
            catch { }
            //try
            //{
            //    sprites = GameWorld.animations[(Type as Enum)];
            //    if (sprites != null)
            //        sprite = sprites[0];
            //}
            //catch { }
            if (sprite != null)
                position = new Vector2(spawnPos.X + sprite.Width / 2, spawnPos.Y + sprite.Height / 2);
            else
                position = spawnPos;

        }

        #endregion
        #region Methods

        /// <summary>
        /// Method for enabling further loading of content or setting of custom parameters
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        public abstract void LoadContent(ContentManager content);

        /// <summary>
        /// Method for Updating the objects parameters, runs Animate by default if sprites-array is not null
        /// </summary>
        /// <param name="gameTime">GameWorld logic</param>
        public virtual void Update(GameTime gameTime)
        {

            if (sprites != null)
                Animate(gameTime);

        }

        /// <summary>
        /// Method for running animation effects, is synced with setting of sprites-array
        /// </summary>
        /// <param name="gameTime">GameWorld logic</param>
        protected virtual void Animate(GameTime gameTime)
        {

            lock (animationLock)
            {
                if (currentIndex >= sprites.Length - 1)
                {
                    timeElapsed = 0;
                    currentIndex = 0;
                }
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                currentIndex = (int)(timeElapsed * fps);
                sprite = sprites[currentIndex];
            }

        }

        /// <summary>
        /// Method for drawing object if it's sprite is not null
        /// </summary>
        /// <param name="spriteBatch">GameWorld logic</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

            if (sprite != null)
                spriteBatch.Draw(sprite, position, null, Color, rotation, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, spriteEffects[spriteEffectIndex], layer);

        }

        #endregion

    }
}

