using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
