using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Checkers
{
    internal class Tile : Button
    {
        public SolidColorBrush Color { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public Piece Item { get; set; }


        // Sets the tile as black and includes all information
        public Tile(SolidColorBrush color, int row, int col)
        {
            Color = color;
            Row = row;
            Col = col;
            Item = new Piece(PieceT.Empty);
            Background = color;
            BorderBrush = Brushes.Black;
            FontSize = 40;
            
        }


    }
}
