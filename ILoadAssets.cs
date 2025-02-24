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
            GameWorld.gameFont = null;
        }

        /// <summary>
        /// Loads single sprites
        /// </summary>
        /// <param name="content">GameWorld logic</param>
        /// <param name="sprites">Dictionary containing single sprites with Enum as the key</param>
        private static void LoadSprites(ContentManager content, Dictionary<Enum, Texture2D> sprites)
        {
            //sprites.Add(LogicItems.MousePointer, content.Load<Texture2D>("Sprites\\GameItems\\mousePointer"));
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
