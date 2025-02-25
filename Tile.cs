using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SharpDX.Direct2D1.Effects;

namespace MortensWay
{
    public class Tile : GameObject<Enum>
    {
        private bool walkable = true;
        private bool fencePath = false;
        private HashSet<Edge> edges = new HashSet<Edge>();
        private HashSet<Edge> fakeEdges = new HashSet<Edge>();
        private HashSet<Edge> realEdges;
        private Monster monster;
        private Enum originalType;
        private Texture2D originalTexture;

        public HashSet<Edge> Edges
        {
            get
            {
                if (walkable)
                    return edges;
                else 
                    return fakeEdges;
            }
        }
        public bool Walkable
        {
            get => walkable;
            set
            {
                if (value == false && walkable == true && type.Equals(TileTypes.FencePath))
                {
                    walkable = value;

                    Thread t = new Thread(SpawnMonster);
                    t.IsBackground = true;
                    t.Start();
                    HashSet<Edge> removeEdges = new HashSet<Edge>(); //Hashset for edges that should be removed
                    foreach (Edge e in edges) //Looks at all edges going from this tile
                    {
                        foreach (Edge f in e.To.edges) //Looks at all the edges from the Tile that this tile leads to
                        {
                            if (f.To == this) //If the edge leads to this tile, the edge is added to a remove list
                            {
                                removeEdges.Add(f);
                            }
                        }
                        if (removeEdges.Count > 0)
                        {
                            foreach (Edge remove in removeEdges) //All removeEdges are removed from the tile he Tile that leads to this tile
                            {
                                e.To.edges.Remove(remove);
                            }
                        }
                    }
                    edges = fakeEdges; //Evt. fjerne reference til denne edge fra andre via metode? -> Se ovenfor
                }
            }
        }
        public bool FencePath { get => fencePath; }

        public Tile(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {
            switch (type)
            {
                case TileTypes.Stone:
                case TileTypes.Fence:
                    walkable = false;
                    break;
                case TileTypes.FencePath:
                    sprite = GameWorld.sprites[TileTypes.Path];
                    monster = new Monster(Monstre.Goose, spawnPos);
                    fencePath = true;
                    break;
                default:
                    break;
            }

            LayerType(type);
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void CreateEdges(HashSet<Tile> list)
        {

            foreach (Tile other in list)
            {
                if (walkable && this != other && other.Walkable)
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

            realEdges = edges; //Måske udkommenteres?

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (type.Equals(TileTypes.Portal))
                spriteBatch.Draw(GameWorld.sprites[TileTypes.Grass], position, null, color, 0, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, spriteEffects[spriteEffectIndex], layer - 0.1f);
            else if (type.Equals(TileTypes.TowerKey))
                spriteBatch.Draw(GameWorld.sprites[TileTypes.Grass], position, null, color, 0, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, spriteEffects[spriteEffectIndex], layer - 0.1f);
            else if (type.Equals(TileTypes.TowerPortion))
                spriteBatch.Draw(GameWorld.sprites[TileTypes.Path], position, null, color, 0, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, spriteEffects[spriteEffectIndex], layer - 0.1f);
            else if (type.Equals(TileTypes.Key))
                spriteBatch.Draw(originalTexture, position, null, color, 0, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, spriteEffects[spriteEffectIndex], layer - 0.1f);
        }

        /// <summary>
        /// Self-explanatory
        /// </summary>
        private void SpawnMonster()
        {

            Thread.Sleep(3000);
            monster.IsAlive = true;
            GameWorld.AddObject(monster);

        }

        /// <summary>
        /// Used for restarting
        /// </summary>
        public void SetOriginalState()
        {

            if (type is TileTypes.FencePath)
            {
                monster.IsAlive = false;
                walkable = true;
                edges = realEdges;
            }

        }

        public void LayerType(Enum type)
        {
            if (type is TileTypes.Fence || type is TileTypes.TowerPortion || type is TileTypes.TowerKey)
            {
                this.layer = 0.90f;
            }
            else if (type is TileTypes.Grass)
            {
                this.layer = 0.1f;
            }
#if DEBUG
            else if (type is TileTypes.FencePath)
            {
                this.color = Color.Red; //Only to test
            }
#endif
            else if (type is TileTypes.Key)
            {
                this.layer = 1f;
            }

        }

        public void ChangeToKey()
        {
            originalTexture = sprite;
            originalType = type;
            type = TileTypes.Key;
            sprite = GameWorld.sprites[type];
        }

        public void ChangeBackFromKey()
        {
            sprite = originalTexture;
            type = originalType;
        }

    }
}
