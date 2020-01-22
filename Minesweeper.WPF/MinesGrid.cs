using System;
using System.Windows.Threading;

namespace Minesweeper.WPF
{
    public class MinesGrid : IGame
    {
        //events associated with an EventHandler delegates
        public event EventHandler CounterChanged;
        public event EventHandler TimerThresholdReached;
        public event EventHandler<PlateEventArgs> ClickPlate;
        
        //fields/properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Mines { get; private set; }
        public int TimeElapsed { get; private set; }
        private Plate[,] plates;
        private int correctFlags;
        private int wrongFlags;
        public int FlaggedMines { get { return (this.correctFlags + this.wrongFlags); } }
        private DispatcherTimer gameTimer; 

        //constructor
        public MinesGrid(int width, int height, int mines)
        {
            this.Width = width;
            this.Height = height;
            this.Mines = mines;
        }
        //method to check if the current position is inside the grid
        public bool IsInGrid(int rowPosition, int colPosition)
        {
            return ((rowPosition >= 0) && (rowPosition < this.Width) && (colPosition >= 0) && (colPosition < this.Height));
        }

        //method to check if the current position is mined
        public bool IsBomb(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                return this.plates[rowPosition, colPosition].IsMined;
            }
            return false;
        }

        //method to check if the current position is mined
        public bool IsFlagged(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                return this.plates[rowPosition, colPosition].IsFlagged;
            }
            return false;
        }
        //method to determine the current cell's status
        //it redirect to Plate.Check() to determine if the cell is mined, or how many mines are around it
        public int RevealPlate(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                int result = this.plates[rowPosition, colPosition].Check(); // checks for number of surrounding mines
                CheckFinish(); // checks for end of game
                return result;
            }
            throw new MinesweeperException("Invalid MinesGrid reference call [row, column] on reveal");
        }

        //method to put or remove flag if some cell is selected
        public void FlagMine(int rowPosition, int colPosition)
        {
            if (!this.IsInGrid(rowPosition, colPosition))
            {
                throw new MinesweeperException("Invalid MinesGrid reference call [row, column] on flag");
            }

            Plate currPlate = this.plates[rowPosition, colPosition];
            if (!currPlate.IsFlagged)
            {
                if (currPlate.IsMined)
                {
                    this.correctFlags++;
                }
                else
                {
                    this.wrongFlags++;
                }
            }
            else
            {
                if (currPlate.IsMined)
                {
                    this.correctFlags--;
                }
                else
                {
                    this.wrongFlags--;
                }
            }

            currPlate.IsFlagged = !currPlate.IsFlagged; // updates the flagged value
            CheckFinish(); // checks for end of game
            // Raises CounterChanged event 
            this.OnCounterChanged(new EventArgs());
        }

        //method to open an exact single plate
        public void OpenPlate(int rowPosition, int colPosition)
        {
            // Checks if the plate is not revealed already
            if (this.IsInGrid(rowPosition, colPosition) && !this.plates[rowPosition, colPosition].IsRevealed)
            {
                // then Raises ClickPlate event with plate position data  
                this.OnClickPlate(new PlateEventArgs(rowPosition, colPosition));
            }
        }

        //method to check if the board is fully resolved
        private void CheckFinish()
        {
            bool hasFinished = false; // assumes that the game is not finished
            if (this.wrongFlags == 0 && this.FlaggedMines == this.Mines) // we have zero more flags to put
            {
                hasFinished = true; // assumes that all plates are revealed
                foreach (Plate item in this.plates)
                {
                    if (!item.IsRevealed && !item.IsMined)
                    {
                        hasFinished = false; // if a plate is not revealed than the game is not finished
                        break;
                    }
                }
            }

            if (hasFinished) gameTimer.Stop(); // when the game has finished the timer must be stopped immediately
        }

        //method to create a game
        public void Run()
        {
            this.correctFlags = 0;
            this.wrongFlags = 0;
            this.TimeElapsed = 0;

            this.plates = new Plate[Width, Height];

            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
                {
                    Plate cell = new Plate(this, row, col);
                    this.plates[row, col] = cell;
                }
            }

            int minesCounter = 0;
            Random minesPosition = new Random();

            while (minesCounter < Mines)
            {
                int row = minesPosition.Next(Width);
                int col = minesPosition.Next(Height);

                Plate cell = this.plates[row, col];

                if (!cell.IsMined)
                {
                    cell.IsMined = true;
                    minesCounter++;
                }
            }

            gameTimer = new DispatcherTimer();
            gameTimer.Tick += new EventHandler(OnTimeElapsed);
            gameTimer.Interval = new TimeSpan(0, 0, 1);
            gameTimer.Start();            
        }

        // method to stop the game
        public void Stop()
        {
            gameTimer.Stop();
        }

        // "Flag Counter Changed" Event Raiser
        protected virtual void OnCounterChanged(EventArgs e)
        {
            EventHandler handler = CounterChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        // "Time Counter Changed" Event Raiser
        protected virtual void OnTimeElapsed(object sender, EventArgs e)
        {
            this.TimeElapsed++;
            EventHandler handler = TimerThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        // "Click to Reveal Plate" Event Raiser - used to automatically open all empty plates in a region
        protected virtual void OnClickPlate(PlateEventArgs e)
        {
            EventHandler<PlateEventArgs> handler = ClickPlate;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
