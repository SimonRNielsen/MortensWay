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
    public class Tile : GameObject<Enum>
    {
        private bool walkable = true;
        private HashSet<Edge> edges = new HashSet<Edge>();

        public HashSet<Edge> Edges { get => edges; }
        public bool Walkable
        {
            get => walkable;
            set
            {
                if (value == false)
                {
                    walkable = value;

                    Thread t = new Thread(SpawnMonster);
                    t.IsBackground = true;
                    t.Start();
                    edges = new HashSet<Edge>(); //Evt. fjerne reference til denne edge fra andre via metode?
                }
            }
        }

        public Tile(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {
            switch (type)
            {
                case TileTypes.Forest:
                case TileTypes.Fence:
                    walkable = false;
                    break;
                case TileTypes.FencePath:
                    sprite = GameWorld.sprites[TileTypes.Path];
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
                if (walkable && this != other)
                {
                    float distance = Vector2.Distance(position, other.Position);
                    if (distance < 91)
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


        private void SpawnMonster()
        {

            Thread.Sleep(3000);
            GameWorld.AddObject(new Monster(Monstre.Goose, position));

        }

    }
}
