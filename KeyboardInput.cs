using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MortensWay
{
    internal class KeyboardInput
    {

        #region Fields

        public Action CloseGame;
        public Action Reset;
        public Action Restart;

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

            if (input.IsKeyDown(Keys.B) && !GameWorld.AlgorithmIsChosen)
            {
                GameWorld.TilesMoved = 0; //Reset tile count
                if (GameWorld.Restart)
                    Reset?.Invoke();
                BFS.StartBFS();
            }

            if (input.IsKeyDown(Keys.A) && !GameWorld.AlgorithmIsChosen)
            {
                GameWorld.TilesMoved = 0; //Reset tile count
                if (GameWorld.Restart)
                    Reset?.Invoke();
                AStar.StartAStar();
            }

            if (GameWorld.Restart)
                if (input.IsKeyDown(Keys.R))
                {
                    GameWorld.TilesMoved = 0; //Reset tile count
                    if (GameWorld.ChosenAlgorithm == AlgorithmType.AStat)
                        GameWorld.ChosenAlgorithm = AlgorithmType.BFS;
                    else
                        GameWorld.ChosenAlgorithm = AlgorithmType.AStat;
                    Restart?.Invoke();
                    GameWorld.Restart = false;
                }

            if (input.IsKeyDown(Keys.D) && !GameWorld.AlgorithmIsChosen)
            {
                GameWorld.TilesMoved = 0; //Reset tile count
                if (GameWorld.Restart)
                    Reset?.Invoke();
                DFS.StartDFS();
            }

        }

        #endregion

    }
}
