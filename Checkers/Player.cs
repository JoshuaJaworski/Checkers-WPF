using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Checkers
{
    internal class Player
    {
        // Player information
        public string pName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }


        // Saves the current players information into the txt file.
        public void Save(string fName)
        {
            // Use stream writer to 
            using (StreamWriter writer = new StreamWriter(fName, true))
            {
                writer.WriteLine(pName.Trim());
                writer.WriteLine(Wins);
                writer.WriteLine(Losses);
            }
        }
    }
}
