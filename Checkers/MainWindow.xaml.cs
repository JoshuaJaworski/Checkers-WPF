using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Player objects
        private Player p1;
        private Player p2;
        /// 2D array to hold board (game state)
        /// private List<Button> boardList;
        private Tile[,] board;
        private Tile prevTile;
        // Player 1 = true. Player 2 = false;
        bool player = true;
        int winner;


        public MainWindow()
        {
            InitializeComponent();
            SetBoard();
            Player1Stats_Button.IsEnabled = false;
            Player2Stats_Button.IsEnabled = false;
        }

        private void ViewPlayer1Stats(object sender, RoutedEventArgs e)
        {
            // Get player stats dictionary

            // Current Directory
            string currentDir = Environment.CurrentDirectory;
            string fName = currentDir + "\\player.txt";

            var playerStats = new Dictionary<string, int[]>();

            // Add players to player.txt
            using (StreamReader sr = new StreamReader(fName))
            {
                while (!sr.EndOfStream)
                {
                    string playerName = sr.ReadLine();
                    int playerWins = int.Parse(sr.ReadLine());
                    int playerLosses = int.Parse(sr.ReadLine());

                    playerStats.Add(playerName, new int[] { playerWins, playerLosses });
                }
            }

            try
            {
                MessageBox.Show($"{p1.pName}\nWins: {playerStats[p1.pName][0]}\nLosses: {playerStats[p1.pName][1]}", "Player 1 Stats");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{p1.pName} stats do not exist.");
            }
            
        }

        private void ViewPlayer2Stats(object sender, RoutedEventArgs e)
        {
            // Get player stats dictionary

            // Current Directory
            string currentDir = Environment.CurrentDirectory;
            string fName = currentDir + "\\player.txt";

            var playerStats = new Dictionary<string, int[]>();

            // Add players to player.txt
            using (StreamReader sr = new StreamReader(fName))
            {
                while (!sr.EndOfStream)
                {
                    string playerName = sr.ReadLine();
                    int playerWins = int.Parse(sr.ReadLine());
                    int playerLosses = int.Parse(sr.ReadLine());

                    playerStats.Add(playerName, new int[] { playerWins, playerLosses });
                }
            }

            try
            {
                MessageBox.Show($"{p2.pName}\nWins: {playerStats[p2.pName][0]}\nLosses: {playerStats[p2.pName][1]}", "Player 2 Stats");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{p2.pName} stats do not exist.");
            }
        }

        // Initialize the board using tiles class. Ensure even tiles are white and odd tiles are black
        // 12 red pieces on top of board (player2). 12 black pieces on bottom of board (player1)
        private void SetBoard()
        {
            board = new Tile[8, 8];

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Need to set the tile color for visual board, will also be useful for move checking.
                    // Even = White. Odd = Black.
                    SolidColorBrush tileC = ((row + col) % 2 == 0) ? Brushes.White : Brushes.Black;
                    Tile tile = new Tile(tileC, row, col);
                    tile.Click += Tile_Click;

                    // Only use black tiles for pieces and play.
                    if (tileC == Brushes.Black)
                    {
                        // Player 2 Red (top of the board)
                        if (row < 3)
                        {
                            tile.Item = new Piece(PieceT.Red);
                        }
                        // Player 1 Black (bottom of the board)
                        else if (row > 4)
                        {
                            tile.Item = new Piece(PieceT.Black);
                        }
                    }

                    // Set piece color and button content.
                    if (tile.Item.Type == PieceT.Red)
                    {
                        tile.Content = "⦿";
                        tile.Foreground = Brushes.Red;
                    }
                    else if (tile.Item.Type == PieceT.Black)
                    {
                        tile.Content = "⦿";
                        tile.Foreground = Brushes.DeepSkyBlue;
                    }

                    // Update array.
                    board[row, col] = tile;
                    Board.Children.Add(tile);
                }
            }
            /*
            /// Update buttons with pieces.
            /// Works but want to try a different method with a uniform board rather than seperate buttons.
            /// Much easier to keep track of information using Tile button class.
            int i = 0;
            boardList = Board.Children.Cast<Button>().ToList();
            boardList.ForEach(button =>
            {
                if (i < 12)
                {
                    button.Content = "●";
                    button.Foreground = Brushes.Red;
                    i++;
                }
                else if (i < 32 && i >= 20)
                {
                    button.Content = "●";
                    button.Foreground = Brushes.Blue;
                    i++;
                }
                else
                {
                    button.Content = string.Empty;
                    i++;
                }
            }
            );*/
        }


        // Function when a tile is clicked
        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile tileClicked = sender as Tile;

            // Unselecting if you click the same tile again
            if (tileClicked == prevTile)
            {
                TileUnselect();
                return;
            }

            // Make sure to check if null before checking the type!!!
            if (prevTile == null || prevTile.Item.Type == PieceT.Empty)
            {
                TileSelect(tileClicked);
                return;
            }

            // Player 1 Turn
            if (!player && prevTile.Item.Type == PieceT.Black)
            {
                //if (CheckMove(prevTile, tileClicked) == 0)
                TileUnselect();
                return;
            }
            // Player 2 Turn
            else if (player && prevTile.Item.Type == PieceT.Red)
            {
                TileUnselect();
                return;
            }

            // Checking to ensure the move is legal
            if (CheckMove(prevTile, tileClicked) == 1)
            {
                int rowDiff = Math.Abs(prevTile.Row - tileClicked.Row);
                Reposition(prevTile, tileClicked);
                TileUnselect();

                // Checking if there is a winner yet
                winner = checkWin(board);
                if (winner == 1)
                {
                    MessageBoxResult result = MessageBox.Show("Player 1 Won!!");

                    // Current Directory
                    string currentDir = Environment.CurrentDirectory;
                    string fName = currentDir + "\\player.txt";

                    var playerStats = new Dictionary<string, int[]>();

                    // Add players to player.txt
                    using (StreamReader sr = new StreamReader(fName))
                    {
                        while (!sr.EndOfStream)
                        {
                            string playerName = sr.ReadLine();
                            int playerWins = int.Parse(sr.ReadLine());
                            int playerLosses = int.Parse(sr.ReadLine());
                            
                            playerStats.Add(playerName, new int[] { playerWins, playerLosses });
                        }
                    }

                    // Add 1 win to player1, 1 loss to player2
                    playerStats[p1.pName][0] += 1;
                    playerStats[p2.pName][1] += 1;

                    // Rewrite player.txt
                    using (StreamWriter writer = new StreamWriter(fName))
                    {
                        foreach (KeyValuePair<string, int[]> kvp in playerStats)
                        {
                            writer.WriteLine(kvp.Key.Trim());
                            writer.WriteLine(kvp.Value[0]);
                            writer.WriteLine(kvp.Value[1]);
                        }
                    }

                    return;

                }
                else if (winner == 2)
                {
                    MessageBoxResult result = MessageBox.Show("Player 2 Won!!");

                    // Current Directory
                    string currentDir = Environment.CurrentDirectory;
                    string fName = currentDir + "\\player.txt";

                    var playerStats = new Dictionary<string, int[]>();

                    // Add players to player.txt
                    using (StreamReader sr = new StreamReader(fName))
                    {
                        while (!sr.EndOfStream)
                        {
                            string playerName = sr.ReadLine();
                            int playerWins = int.Parse(sr.ReadLine());
                            int playerLosses = int.Parse(sr.ReadLine());

                            playerStats.Add(playerName, new int[] { playerWins, playerLosses });
                        }
                    }

                    // Add 1 win to player1, 1 loss to player2
                    playerStats[p2.pName][0] += 1;
                    playerStats[p1.pName][1] += 1;

                    // Rewrite player.txt
                    using (StreamWriter writer = new StreamWriter(fName))
                    {
                        foreach (KeyValuePair<string, int[]> kvp in playerStats)
                        {
                            writer.WriteLine(kvp.Key.Trim());
                            writer.WriteLine(kvp.Value[0]);
                            writer.WriteLine(kvp.Value[1]);
                        }
                    }

                    return;
                }

                // Rechecking for any extra jumps
                // Could not get it working with check method so doing manual check after a capture
                // Checking after capture and making sure tile has no king on it
                if (rowDiff == 2 && tileClicked.Item.King == 0 )
                {
                    if (player && tileClicked.Row - 2 >= 0 && tileClicked.Col - 2 >= 0 && (board[tileClicked.Row - 2, tileClicked.Col - 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row - 1, tileClicked.Col - 1].Item.Type == PieceT.Red))
                    {
                        return;
                    }
                    else if (player && tileClicked.Row - 2 >= 0 && tileClicked.Col + 2 <= 7 && (board[tileClicked.Row - 2, tileClicked.Col + 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row - 1, tileClicked.Col + 1].Item.Type == PieceT.Red))
                    {
                        return;
                    }
                    else if (!player && tileClicked.Row + 2 <= 7 && tileClicked.Col - 2 >= 0 && (board[tileClicked.Row + 2, tileClicked.Col - 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row + 1, tileClicked.Col - 1].Item.Type == PieceT.Black))
                    {
                        //player = !player;
                        return;
                    }
                    // else if (!player && tileClicked.Row + 2 >= 0 && tileClicked.Col + 2 >= 0 && (CheckMove(tileClicked, board[tileClicked.Row + 2, tileClicked.Col + 2]) == 0))
                    else if (!player && tileClicked.Row + 2 <= 7 && tileClicked.Col + 2 <= 7 && (board[tileClicked.Row + 2, tileClicked.Col + 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row + 1, tileClicked.Col + 1].Item.Type == PieceT.Black))
                    {
                        return;
                    }
                    else
                    {
                        player = !player;
                        return;
                    }
                }
                // Checking after capture if tile has a king on it
                else if (rowDiff == 2 && tileClicked.Item.King == 1)
                {
                    if (player && tileClicked.Row - 2 >= 0 && tileClicked.Col - 2 >= 0 && (board[tileClicked.Row - 2, tileClicked.Col - 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row - 1, tileClicked.Col - 1].Item.Type == PieceT.Red))
                    {
                        return;
                    }
                    else if (player && tileClicked.Row - 2 >= 0 && tileClicked.Col + 2 <= 7 && (board[tileClicked.Row - 2, tileClicked.Col + 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row - 1, tileClicked.Col + 1].Item.Type == PieceT.Red))
                    {
                        return;
                    }
                    else if (player && tileClicked.Row + 2 <= 7 && tileClicked.Col - 2 >= 0 && (board[tileClicked.Row + 2, tileClicked.Col - 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row + 1, tileClicked.Col - 1].Item.Type == PieceT.Red))
                    {
                        //player = !player;
                        return;
                    }
                    // else if (!player && tileClicked.Row + 2 >= 0 && tileClicked.Col + 2 >= 0 && (CheckMove(tileClicked, board[tileClicked.Row + 2, tileClicked.Col + 2]) == 0))
                    else if (player && tileClicked.Row + 2 <= 7 && tileClicked.Col + 2 <= 7 && (board[tileClicked.Row + 2, tileClicked.Col + 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row + 1, tileClicked.Col + 1].Item.Type == PieceT.Red))
                    {
                        return;
                    }
                    else if (!player && tileClicked.Row - 2 >= 0 && tileClicked.Col - 2 >= 0 && (board[tileClicked.Row - 2, tileClicked.Col - 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row - 1, tileClicked.Col - 1].Item.Type == PieceT.Black))
                    {
                        return;
                    }
                    else if (!player && tileClicked.Row - 2 >= 0 && tileClicked.Col + 2 <= 7 && (board[tileClicked.Row - 2, tileClicked.Col + 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row - 1, tileClicked.Col + 1].Item.Type == PieceT.Black))
                    {
                        return;
                    }
                    else if (!player && tileClicked.Row + 2 <= 7 && tileClicked.Col - 2 >= 0 && (board[tileClicked.Row + 2, tileClicked.Col - 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row + 1, tileClicked.Col - 1].Item.Type == PieceT.Black))
                    {
                        //player = !player;
                        return;
                    }
                    // else if (!player && tileClicked.Row + 2 >= 0 && tileClicked.Col + 2 >= 0 && (CheckMove(tileClicked, board[tileClicked.Row + 2, tileClicked.Col + 2]) == 0))
                    else if (!player && tileClicked.Row + 2 <= 7 && tileClicked.Col + 2 <= 7 && (board[tileClicked.Row + 2, tileClicked.Col + 2].Item.Type == PieceT.Empty) && (board[tileClicked.Row + 1, tileClicked.Col + 1].Item.Type == PieceT.Black))
                    {
                        return;
                    }
                    else
                    {
                        player = !player;
                        return;
                    }
                }
                // If there was no capture switch player
                else
                {
                    player = !player;
                }
                

            }
            // If the move is illegal
            else
            {
                TileUnselect();
                TileSelect(tileClicked);
            }
            
        }


        // Sets a new tile to prevTile to store it
        private void TileSelect(Tile tile)
        {
            if (tile.Item.Type == PieceT.Empty)
            {
                return;
            }
            prevTile = tile;
            tile.BorderBrush = Brushes.Yellow;
        }

        // Unsets the previous tile
        private void TileUnselect()
        {
            if (prevTile != null)
            {
                prevTile.BorderBrush = prevTile.Color;
                prevTile = null;
            }
        }


        // Need to ensure that the move trying to be made is legal or not 
        // Returns 0 on illegal. Returns 1 on legal.
        private int CheckMove(Tile iTile, Tile fTile)
        {
            // Must be positive
            int rowDiff = Math.Abs(iTile.Row - fTile.Row);
            int columnDiff = Math.Abs(iTile.Col - fTile.Col);

            // Ensure that the final tile does not have a piece in it
            if (fTile.Item.Type != PieceT.Empty)
            {
                return 0;
            }

            // Ensure that the final tile is diagonal from the initial. 
            // 
            if (columnDiff != rowDiff)
            {
                return 0;
            }

            // Need to check if player is trying to do a normal move or take a piece
            // Can determine based on the distance between initial and final tile
            if (rowDiff == 1)
            {
                // Checking the direction that the piece is attempting to move it.
                if (iTile.Item.King == 1 || 
                    (iTile.Item.Type == PieceT.Black && iTile.Row > fTile.Row) || 
                    (iTile.Item.Type == PieceT.Red && iTile.Row < fTile.Row))
                {
                    return 1;
                }
            }
            // Trying to take piece
            // Need to fix bug with king I am guessing but do not know yet.
            else if (rowDiff == 2)
            {
                // How to calculate taken tile between intial and final position. Think of graph
                Tile takenTile = board[(iTile.Row + fTile.Row) / 2, (iTile.Col + fTile.Col) / 2];
                // Ensure that the piece trying to be taken is not the same type, and that the tile is not empty
                // Also need to make sure that the piece you're trying to take is in the correct direction, unless it is a king piece.
                // Need to check for king and same piece. Have to seperate from the other if statement to function properly
                if (iTile.Item.King == 1 && takenTile.Item.Type != PieceT.Empty && takenTile.Item.Type != iTile.Item.Type)
                {
                    return 1;
                }
                // Checking for same piece and direction.
                else if (takenTile.Item.Type != PieceT.Empty && takenTile.Item.Type != iTile.Item.Type &&
                    ((iTile.Item.Type == PieceT.Black && iTile.Row > fTile.Row) ||
                    (iTile.Item.Type == PieceT.Red && iTile.Row < fTile.Row)))
                {
                    return 1;
                }
            }
            return 0;
        }


        // Function to move the piece. Dependent on tile position and if a piece is being taken or not.
        private void Reposition(Tile iTile, Tile fTile)
        {
            // Just need to know the row for capture or not
            int rowDiff = Math.Abs(iTile.Row - fTile.Row);
            // int columnDiff = Math.Abs(iTile.Col - fTile.Col);

            // Handling taking a piece out
            if (rowDiff == 2)
            {
                Tile takenTile = board[(iTile.Row + fTile.Row) / 2, (iTile.Col + fTile.Col) / 2];
                // Take the piece out
                takenTile.Item = new Piece(PieceT.Empty);
                takenTile.Content = null;
            }

            // Updating the final tile to the initial tile
            fTile.Item = iTile.Item;
            fTile.Content = iTile.Content;
            fTile.Foreground = iTile.Foreground;
            iTile.Item = new Piece(PieceT.Empty);
            iTile.Content = null;

            /* Don't need to change color. Can just update foreground with other content.
            // Checking for color
            if (fTile.Item.Type == PieceT.Red)
            {
                /// fTile.Content = "●";
                fTile.Foreground = Brushes.Red;
            }
            else if (fTile.Item.Type == PieceT.Black)
            {
                /// fTile.Content = "●";
                fTile.Foreground = Brushes.Blue;
            }
            */

            // Making a piece king if it gets to the opposite end of the board. Updates privileges
            if (fTile.Item.Type == PieceT.Black && fTile.Row == 0)
            {
                fTile.Content = "♛";
                fTile.Item.King = 1;
                fTile.Foreground = Brushes.DeepSkyBlue;
            }
            else if (fTile.Item.Type == PieceT.Red && fTile.Row == 7)
            {
                fTile.Content = "♛";
                fTile.Item.King = 1;
                fTile.Foreground = Brushes.Red;
            }
        }


        // Need to calculate win
        // Return 0 for no winner
        // Return 1 for player 1 win
        // Return 2 for player 2 win
        private int checkWin(Tile[,] board)
        {
            int p1_pieces = 0;
            int p2_pieces = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Item.Type == PieceT.Black)
                    {
                        p1_pieces++;
                    }
                    else if (board[i, j].Item.Type == PieceT.Red)
                    {
                        p2_pieces++;
                    }
                }
            }
            // P2 win
            if (p1_pieces == 0)
            {
                return 2;
            }
            // P1 win
            else if (p2_pieces == 0){
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // Update the file with player information
        private void Update_file(object sender, RoutedEventArgs e)
        {
            p1 = new Player();
            p2 = new Player();

            p1.pName = Play1_Text.Text;
            p1.Wins = 0;
            p1.Losses = 0;
            p2.pName = Play2_Text.Text;
            p2.Wins = 0;
            p2.Losses = 0;

            string currentDir = Environment.CurrentDirectory;
            string fName = currentDir + "\\player.txt";
            bool p1Found = false, p2Found = false;

            // Add players to player.txt
            using (StreamReader sr = new StreamReader(fName))
            {
                string line;
                int count = 0;
                
                while ((line = sr.ReadLine()) != null) 
                { 
                    if (count % 3 == 0)
                    {
                        if (line == p1.pName) { p1Found = true; }
                        else if (line == p2.pName) {  p2Found = true; }
                    }
                }
            }

            if (p1Found == false) { p1.Save(fName); }
            if (p2Found == false) { p2.Save(fName); }
            
            Player1Stats_Button.IsEnabled = true;
            Player2Stats_Button.IsEnabled = true;
        }
        /*
        private void playerPrompt()
        {
            Player p1 = new Player();
            Player p2 = new Player();

            p1.pName = Play1_Text.Text;
            p1.Wins = 0;
            p1.Losses = 0;
            p2.pName = Play2_Text.Text;
            p2.Wins = 0;
            p2.Losses = 0;

            p1.Save("player.txt");
            p2.Save("player.txt");


        }*/
    }
}
