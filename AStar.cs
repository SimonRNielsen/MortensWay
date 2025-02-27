using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public static List<Tile> FindPath(Vector2 startVector, Vector2 endVector)
        {

            //Ryder tidligere data
            openList.Clear();
            closedList.Clear();

            //Using tile as cell
            Tile startTile = Cells[startVector];
            Tile endTile = Cells[endVector];
            Debug.WriteLine($"{startVector}");
            openList.Add(cells[startVector]);

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

                if (curCell.Position.X == endVector.X && curCell.Position.Y == endVector.Y)
                {
                    return RetracePath(Cells[startVector], Cells[endVector]);
                }

                List<Tile> neighbours = GetNeighbours(curCell);
                foreach (var neighbour in neighbours)
                {
                    if (closedList.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = curCell.G + GetDistance(curCell.Position, neighbour.Position);

                    if (newMovementCostToNeighbour < neighbour.G || !openList.Contains(neighbour))
                    {
                        neighbour.G = newMovementCostToNeighbour;

                        //udregner H med manhatten princip
                        neighbour.H = (((int)Math.Abs(neighbour.Position.X - endVector.X) + (int)Math.Abs(endVector.Y - neighbour.Position.Y)) * 10);
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

        private static List<Tile> RetracePath(Tile startVector, Tile endVector)
        {
            List<Tile> path = new List<Tile>();
            Tile currentNode = endVector;

            while (currentNode != startVector)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Add(startVector);
            path.Reverse();

            return path;
        }

        private static int GetDistance(Vector2 neighbourPosition, Vector2 endVector)
        {
            int dstX = Math.Abs((int)neighbourPosition.X - (int)endVector.X);
            int dstY = Math.Abs((int)neighbourPosition.Y - (int)endVector.Y);

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
                    if (Cells.TryGetValue(new Vector2((int)curCell.Position.X + (i * 64), (int)curCell.Position.Y + (j * 64)), out var cell))
                    {
                        curNeighbour = cell;
                    }
                    else
                    {
                        continue;
                    }

                    if (curNeighbour.Type.Equals(TileTypes.Stone) || curNeighbour.Type.Equals(TileTypes.Fence))
                    {
                        continue;
                    }

                    if (!curNeighbour.Walkable)  // Hvis en tile ikke længere er walkable
                    {
                        continue; // Spring den over
                    }

                    //hjørner
                    switch (i)
                    {
                        case -1 when j == 1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Stone)):
                        case 1 when j == 1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Stone)):
                        case -1 when j == -1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Stone)):
                        case 1 when j == -1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Stone) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Stone)):
                        case -1 when j == 1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Fence) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Fence)):
                        case 1 when j == 1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Fence) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Fence)):
                        case -1 when j == -1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Fence) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Fence)):
                        case 1 when j == -1 && (Cells[curCell.Position + new Vector2(i * 64, 0)].Type.Equals(TileTypes.Fence) || Cells[curCell.Position + new Vector2(0, j * 64)].Type.Equals(TileTypes.Fence)):
                            continue;
                        default:
                            neighbours.Add(curNeighbour);
                            break;
                    }
                }

            }

            return neighbours;
        }
        public static void StartAStar()
        {
            GameWorld.AlgorithmIsChosen = true;
            GameWorld.ChosenAlgorithm = AlgorithmType.AStat;
        }

    }

}
