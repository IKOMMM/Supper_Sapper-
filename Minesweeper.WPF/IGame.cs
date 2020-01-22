using System;

namespace Minesweeper.WPF
{
    public interface IGame
    {
        //events associated with the Game
        event EventHandler CounterChanged;
        event EventHandler TimerThresholdReached;
        event EventHandler<PlateEventArgs> ClickPlate;

        void Run(); // gra musi działać!
    }
}
