using System;

namespace Minesweeper.WPF
{
    interface IPlate
    {
        // pozycje pól 
        int RowPosition { get; }
        int ColPosition { get; }
    }
}
