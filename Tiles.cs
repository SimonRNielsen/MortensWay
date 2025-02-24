using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortensWay
{
    internal class Tiles : GameObject<Enum>
    {
        public Tiles(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {
            Type = type;
            position = new Vector2(spawnPos.X + sprite.Width/2, spawnPos.Y + sprite.Height/2);

            LayerType(type);
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void LayerType(Enum type)
        {
            if (type is TileTypes.Forest)
            {
                this.layer = 1f;
            }
            else if (type is TileTypes.Grass)
            {
                this.layer = 0f;
            }

        }
    }
}
