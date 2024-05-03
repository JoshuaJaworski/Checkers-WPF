using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class Piece
    {
        public PieceT Type { get; set; }
        public int King { get; set; }

        /// No king = 0. King = 1.
        public Piece(PieceT type)
        {
            this.Type = type;
            King = 0;
        }
    }

    //Red is player 2
    //Black is player 1
    public enum PieceT
    {
        Red,
        Black,
        Empty
    }
}
