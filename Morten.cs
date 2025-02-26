using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MortensWay
{
    public class Morten : GameObject<Enum>
    {
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
                position = entry.Position;
                if (entry.FencePath)
                    entry.Walkable = false;
                else if (entry.Type.Equals(TileTypes.Key) && entry == path.Last())
                    entry.ChangeBackFromKey();
                Thread.Sleep(750);
            }

            GameWorld.Arrived = true;

        }
    }
}
