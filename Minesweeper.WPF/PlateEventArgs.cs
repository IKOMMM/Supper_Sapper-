using System;

namespace Minesweeper.WPF
{
    /// <summary>
    /// Eventy planszy
    /// </summary>
    public class PlateEventArgs : EventArgs
    {
       
        public int PlateRow { get; set; }
        public int PlateColumn { get; set; }

        public PlateEventArgs(int row, int col)
        {
            this.PlateRow = row;
            this.PlateColumn = col;
        }
    }
}
