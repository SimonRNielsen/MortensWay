using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortensWay
{
    public class AStar
    {
        private static Dictionary<Vector2, Tile> cells;

        public AStar(Dictionary<Vector2, Tile> cells)
        {
            Cells = cells;
        }

        private static HashSet<Tile> openList = new HashSet<Tile>();
        private static HashSet<Tile> closedList = new HashSet<Tile>();

        public static Dictionary<Vector2, Tile> Cells { get => cells; set => cells = value; }

        public static List<Tile> FindPath(Vector2 startPoint, Vector2 endPoint)
        {
            openList.Add(Cells[startPoint]);
            while (openList.Count > 0)
            {
                Tile curCell = openList.First();
                foreach (var t in openList)
                {
                    if (t.F < curCell.F || t.F == curCell.F && t.H < curCell.H)
                    {
                        curCell = t;
                    }
                }
                openList.Remove(curCell);
                closedList.Add(curCell);

                if (curCell.Position.X == endPoint.X && curCell.Position.Y == endPoint.Y)
                {
                    return RetracePath(Cells[startPoint], Cells[endPoint]);
                }

                List<Tile> neighbours = GetNeighbours(curCell);
                foreach (var neighbour in neighbours)
                {
                    if(closedList.Contains(neighbour))
                    continue;
                
                    int newMovementCostToNeighbour = curCell.G + GetDistance(curCell.Position, neighbour.Position);

                    if (newMovementCostToNeighbour < neighbour.G || !openList.Contains(neighbour))
                    {
                        neighbour.G = newMovementCostToNeighbour;
                        //udregner H med manhatten princip
                        neighbour.H = ((Math.Abs((int)neighbour.Position.X - (int)endPoint.X) + Math.Abs((int)endPoint.Y - (int)neighbour.Position.Y)) * 10);
                        neighbour.Parent = curCell;

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            return null;

        }

        private static List<Tile> RetracePath(Tile startPoint, Tile endPoint)
        {
            List<Tile> path = new List<Tile>();
            Tile currentNode = endPoint;

            while (currentNode != startPoint)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Add(startPoint);
            path.Reverse();

            return path;
        }

        private static int GetDistance(Vector2 neighbourPosition, Vector2 endPoint)
        {
            int dstX = Math.Abs((int)neighbourPosition.X - (int)endPoint.X);
            int dstY = Math.Abs((int)neighbourPosition.Y - (int)endPoint.Y);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }

        

        private static List<Tile> GetNeighbours(Tile curCell)
        {
            List<Tile> neighbours = new List<Tile>(8);
            //var wallSprite = TileTypes.Stone;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    Tile curNeighbour;
                    if (Cells.TryGetValue(new Vector2(curCell.Position.X + i, curCell.Position.Y + j), out var cell))
                    {
                        curNeighbour = cell;
                    }
                    else
                    {
                        continue;
                    }

                    if ( curNeighbour.Type.Equals(TileTypes.Stone))
                    {
                        continue;
                    }

                    //hjørner
                    switch (i)
                    {
                        case -1 when j == 1 && (Cells[curCell.Position + new Vector2(i, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j)].Type.Equals(TileTypes.Stone)):
                        case 1 when j == 1 && (Cells[curCell.Position + new Vector2(i, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j)].Type.Equals(TileTypes.Stone)):
                        case -1 when j == -1 && (Cells[curCell.Position + new Vector2(i, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j)].Type.Equals(TileTypes.Stone)):
                        case 1 when j == -1 && (Cells[curCell.Position + new Vector2(i, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j)].Type.Equals(TileTypes.Stone)):
                            continue;
                        default:
                            neighbours.Add(curNeighbour);
                            break;
                    }
                }

            }

            return neighbours;
        }

    }

}
