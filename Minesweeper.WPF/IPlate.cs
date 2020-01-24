using System;

namespace Minesweeper.WPF
{
    interface IPlate
    {
        // gettery pól (muszą posiadać swoje gettery)
        int RowPosition { get; }
        int ColPosition { get; }
    }
}
