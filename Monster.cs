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

        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }
    }
}
