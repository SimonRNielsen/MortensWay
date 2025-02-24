using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortensWay
{
    public class Tile : GameObject<Enum>
    {
        private bool walkable = true;
        private HashSet<Edge> edges = new HashSet<Edge>();

        public HashSet<Edge> Edges { get => edges; }

        public Tile(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {
            switch (type)
            {
                case TileTypes.Forest:
                case TileTypes.Fence:
                    walkable = false;
                    break;
                default:
                    break;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void CreateEdges(HashSet<Tile> list)
        {

            foreach (Tile other in list)
            {
                float distance = Vector2.Distance(position, other.Position);
                if (this != other && walkable && distance < 91)
                {
                    int weight;
                    if (distance < 65)
                        weight = 10;
                    else
                        weight = 14;
                    Edges.Add(new Edge(weight, this, other));
                }
            }

        }
    }
}
