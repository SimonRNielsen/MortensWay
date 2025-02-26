using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortensWay
{
    public class Edge
    {

        #region Fields

        private int weight;
        private Tile from;
        private Tile to;

        #endregion
        #region Properties

        public int Weight { get => weight; private set => weight = value; }
        public Tile From { get => from; }
        //public Tile To { get => to; }
        public Tile To
        {
            get
            {
                if (to.Walkable)
                    return to;
                else
                    return null;
            }
        }

        #endregion
        #region Constructor

        public Edge(int weight, Tile from, Tile to)
        {
            this.weight = weight;
            this.from = from;
            this.to = to;
        }

        #endregion

    }
}
