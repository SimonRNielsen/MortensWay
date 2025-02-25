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
        Dictionary<Point, Cell> cells;

        public AStar()
        {
        }

        private HashSet<Cell> openList = new HashSet<Cell>();
    }
}
