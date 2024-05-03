# Checkers-WPFCheckers
Implemented with C# and WPF  
## Game  
 ○ This works how a simple game of checkers is typically played.  
 ○ Player1 uses 12 blue pieces  
 ○ Player2 uses 12 red pieces  
 ○ Pieces can only move forward diagonally by one space  
 ○ If there is an enemy piece diagonal to yours, and an open spot behind it, you can capture your opponent’s piece by “hopping” over theirs  
 ○ If there are possible captures after the first capture, then they must be taken  
 ○ If a piece gets to the opposite end of the board, it is kinged and may move one space diagonally in any direction. All other rules still apply  
 ○ The winner is decided once a player loses all 12 of their pieces  
 ○ This program allows only valid moves  
 ○ Players must take turns moving their pieces, starting with player 1  
 ○ If a subsequent capture is available, then the capturing player must move until no more are possible  
 ○ To move pieces, click on the tile with the piece you’d like to move, then the destination tile  
 ○ If possible, the program will update the board with the new location  
## Save Player  
 ○ This program also saves player data by using the inputted player name and updating wins and losses in a text file as they are executed.  
 ○ The text file is saved to your system’s current directory, but can be viewed with the view player buttons for either player 1 or 2.  
 ○ Input the name of the player in the text box to save or view their profile.  
 ○ Make sure to save before playing or it will default to player 1 and 2.  
