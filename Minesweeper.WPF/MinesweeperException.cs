using System;

namespace Minesweeper.WPF

    
{
    /// <summary>
    /// Wyjątki przy tworzeniu gry
    /// </summary>
    public class MinesweeperException : ApplicationException
    {
     

        public MinesweeperException(string message)
            : base(message)
        {
        }

        public MinesweeperException(string message, Exception innerExeption)
            : base(message, innerExeption)
        {
        }
    }
}
