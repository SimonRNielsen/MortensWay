using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortensWay
{
    internal class Monster : GameObject<Enum>
    {
        public Monster(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {
            position = spawnPos;
            layer = 1;
            scale = 0.5f;
        }

        public override void LoadContent(ContentManager content)
        {
            
        }
    }
}
