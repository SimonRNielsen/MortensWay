using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MortensWay
{
    internal class KeyboardInput
    {

        #region Fields

        public Action CloseGame;

        #endregion
        #region Properties



        #endregion
        #region Constructor

        public KeyboardInput() { }

        #endregion
        #region Methods

        /// <summary>
        /// Handles input
        /// </summary>
        /// <param name="gameTime">GameWorld logic</param>
        public void HandleInput(GameTime gameTime)
        {

            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Keys.Escape))
                CloseGame?.Invoke();

            if (input.IsKeyDown(Keys.Space))
                GameWorld.DebugMode = true;
            else if (GameWorld.DebugMode)
                GameWorld.DebugMode = false;

        }

        #endregion

    }
}
