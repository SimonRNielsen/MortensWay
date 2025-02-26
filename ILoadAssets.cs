using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MortensWay
{

    /// <summary>
    /// Interface to load assets
    /// </summary>
    public interface ILoadAssets
    {

        /// <summary>
        /// Runs all methods in the interface and sets default text font
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        public static void Load(ContentManager content)
        {

            LoadSprites(content, GameWorld.sprites);
            LoadAnimations(content, GameWorld.animations);
            LoadSoundEffects(content, GameWorld.soundEffects);
            LoadMusic(content, GameWorld.music);
            GameWorld.gameFont = content.Load<SpriteFont>("gameFont");

        }

        /// <summary>
        /// Loads single sprites
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        /// <param name="sprites">Dictionary containing single sprites with Enum as the key</param>
        private static void LoadSprites(ContentManager content, Dictionary<Enum, Texture2D> sprites)
        {

            #region LogicItems

            sprites.Add(LogicItems.MousePointer, content.Load<Texture2D>("Sprites\\LogicItems\\mousePointer"));
            sprites.Add(LogicItems.CollisionPixel, content.Load<Texture2D>("Sprites\\LogicItems\\pixel"));

            #endregion
            #region Tiles

            sprites.Add(TileTypes.FencePath, content.Load<Texture2D>("Sprites\\Tiles\\dirtTile"));
            sprites.Add(TileTypes.Path, content.Load<Texture2D>("Sprites\\Tiles\\dirtTile"));
            sprites.Add(TileTypes.Grass, content.Load<Texture2D>("Sprites\\Tiles\\grassTile"));
            sprites.Add(TileTypes.Fence, content.Load<Texture2D>("Sprites\\fence"));
            sprites.Add(TileTypes.Stone, content.Load<Texture2D>("Sprites\\stone"));
            sprites.Add(TileTypes.TowerKey, content.Load<Texture2D>("Sprites\\church-bell_9988486"));
            sprites.Add(TileTypes.TowerPortion, content.Load<Texture2D>("Sprites\\church-bell"));
            sprites.Add(TileTypes.Portal, content.Load<Texture2D>("Sprites\\Portal"));
            sprites.Add(TileTypes.Key, content.Load<Texture2D>("Sprites\\key"));

            #endregion
            #region Avatars

            sprites.Add(Monstre.Goose, content.Load<Texture2D>("Sprites\\aggro0"));
            sprites.Add(MortensEnum.Bishop, content.Load<Texture2D>("Sprites\\bishop"));

            #endregion


        }

        /// <summary>
        /// Loads sprite-arrays
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        /// <param name="animations">Dictionary containing sprite-arrays with Enum as the key</param>
        private static void LoadAnimations(ContentManager content, Dictionary<Enum, Texture2D[]> animations)
        {

            

        }

        /// <summary>
        /// Loads SoundEffects
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        /// <param name="soundEffects">Dictionary containing SoundEffects with Enum as the key</param>
        private static void LoadSoundEffects(ContentManager content, Dictionary<Enum, SoundEffect> soundEffects)
        {
            soundEffects.Add(SoundEffects.GooseHonk, content.Load<SoundEffect>("Sounds\\gooseSound_Short"));
        }

        /// <summary>
        /// Loads Songs
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        /// <param name="music">Dictionary containing Songs with Enum as the key</param>
        private static void LoadMusic(ContentManager content, Dictionary<Enum, Song> music)
        {

        }
    }


}
