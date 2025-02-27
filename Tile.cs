using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private bool fencePath = false;
        private HashSet<Edge> edges = new HashSet<Edge>();
        private HashSet<Edge> fakeEdges = new HashSet<Edge>();
        private Monster monster;
        private bool discovered = false;
        private Tile parent;
        private Enum originalType;
        private Texture2D originalTexture;

        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;

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
        public bool Discovered { get => discovered; set => discovered = value; }
        public Tile Parent { get => parent; set => parent = value; }
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
        public Tile(Enum type, Vector2 spawnPos, bool fencePath) : base(type, spawnPos)
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

            this.fencePath = fencePath;

            LayerType(type);
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void CreateEdges(HashSet<Tile> list)
        {
            if (walkable)
                foreach (Tile other in list)
                {
                    if (this != other && other.Walkable)
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

            Thread.Sleep(1000);
            monster.IsAlive = true;
            GameWorld.AddObject(monster);
            GameWorld.soundEffects[SoundEffects.GooseHonk].Play();

        }

        /// <summary>
        /// Used for restarting
        /// </summary>
        public void SetOriginalState()
        {

            color = Color.White;
            if (type is TileTypes.FencePath)
            {
                monster.IsAlive = false;
                walkable = true;
                color = Color.Red;
            }
            G = 0;
            H = 0;

        }

        public void LayerType(Enum type)
        {
            if (type is TileTypes.Fence)
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
                this.Color = Color.Red; //Only to test
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
