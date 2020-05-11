using System;

namespace Minesweeper.WPF
    
{
    /// <summary>
    /// Interfejs gry i działania w nim zawarte
    /// </summary>
    public interface IGame
    {              
        //działania zawarte w rozgrywce! 
        event EventHandler CounterChanged;
        event EventHandler TimerThresholdReached;
        event EventHandler<PlateEventArgs> ClickPlate;

        void Run(); //wymaganie działania gry!
    }
}
