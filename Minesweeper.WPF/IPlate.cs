using System;

namespace Minesweeper.WPF
{
    interface IPlate
    {
        // gettery (muszą posiadać swoje gettery) 

        /// <summary>
        /// Gettery i settery do planszy
        /// </summary>
        int RowPosition { get; }
        int ColPosition { get; }
    }
}
