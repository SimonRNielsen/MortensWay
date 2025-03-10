﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace MortensWay
{
    public class Morten : GameObject<Enum>
    {
        private int keyCount;
        private int potionCount;

        public int KeyCount { get => keyCount; set => keyCount = value; }
        public int PotionCount { get => potionCount; set => potionCount = value; }


        public Morten(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {
            this.layer = 1f;

        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void FollowPath(List<Tile> path)
        {

            foreach (Tile entry in path)
            {
                GameWorld.TilesMoved++;
                position = entry.Position;
                if (entry.FencePath)
                    entry.Walkable = false;
                else if (entry.Type.Equals(TileTypes.Key) && entry == path.Last())
                { entry.ChangeBackFromKey();
                    keyCount++;
                }
                else if (entry.Type.Equals(TileTypes.TowerKey) && KeyCount>0 && entry == path.Last())
                {
                    keyCount--;
                    potionCount--;
                }
                else if (entry.Type.Equals(TileTypes.TowerPortion) && KeyCount > 0 && entry == path.Last())
                {
                    keyCount--;
                    potionCount++;
                }
                Thread.Sleep(300);
            }

            GameWorld.Arrived = true;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Arrived && position == GameWorld.Destinations[5].Position) { }
            else
                base.Draw(spriteBatch);
        }
    }
}
