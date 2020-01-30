using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Minesweeper.WPF
{

    /// <summary>
    /// Zarządzanie minami
    /// </summary>
    public class MinesGrid : IGame
    {
        //eventy powiązoane z delegatami EventHandler
        public event EventHandler CounterChanged;
        public event EventHandler TimerThresholdReached;
        public event EventHandler<PlateEventArgs> ClickPlate;
        
        //pola/właściwości
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Mines { get; private set; }
        public int TimeElapsed { get; private set; }
        private Plate[,] plates;
        private int correctFlags;
        private int wrongFlags;
        public int FlaggedMines { get { return (this.correctFlags + this.wrongFlags); } }
        private DispatcherTimer gameTimer;
        public static int lc = 0;

        //konstruktor
        public MinesGrid(int width, int height, int mines)
        {
            this.Width = width;
            this.Height = height;
            this.Mines = mines;
        }
        //metoda sprawdzająca czy obecna pozycja zawiera się w grid'zie
        public bool IsInGrid(int rowPosition, int colPosition)
        {
            return ((rowPosition >= 0) && (rowPosition < this.Width) && (colPosition >= 0) && (colPosition < this.Height));
        }

        //metoda sprawdzająca czy pole zawiera bombe
        public bool IsBomb(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                return this.plates[rowPosition, colPosition].IsMined;
            }
            return false;
        }

        //metoda sprawdza czy obecna pozycja zawiera flage
        public bool IsFlagged(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                return this.plates[rowPosition, colPosition].IsFlagged;
            }
            return false;
        }
        /// <summary>
        /// 



        ///metoda definiująca obecny status pola
        ///wymaga od Plate.Check() określenia czy pole jest zaminowane oraz ile min jest wokoło
        /// </summary>
        public int RevealPlate(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                int result = this.plates[rowPosition, colPosition].Check(); //sprawdza numer min do okoła
                CheckFinish(); //sprawdza czy koniec gry
                return result;
            }
            throw new MinesweeperException("Invalid MinesGrid reference call [row, column] on reveal");
        }
        /// <summary>
        /// metoda do stawiania i usuwania flag po zaznaczeniu
        /// </summary>

        //
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

            currPlate.IsFlagged = !currPlate.IsFlagged; //wartości flag
    
            CheckFinish(); // sprawdza za końcem gry

            //zmiana w wyniku
            this.OnCounterChanged(new EventArgs());
        }
        ///<summary>
        ///metoda do otwierania pojedynczej komórki
        ///</summary>
        
        public void OpenPlate(int rowPosition, int colPosition)
        {
            //sprawdza czy ta komórka nie zostałą już "otwarta"
            if (this.IsInGrid(rowPosition, colPosition) && !this.plates[rowPosition, colPosition].IsRevealed)
            {
                //po czym wywołuje ClickPlate z informacją o komórce
                this.OnClickPlate(new PlateEventArgs(rowPosition, colPosition));
            }
        }
        /// <summary>
        /// metoda sprawdzająca czy cała plansza jest już rozwiązana
        /// </summary>
        //
        private void CheckFinish()
        {
            bool hasFinished = false; //sprawdza czy nie jest jeszcze skończona
            if (this.wrongFlags == 0 && this.FlaggedMines == this.Mines) //nie mamy flag do położenia
            {
                hasFinished = true; //sprawdza czy wszystkie są odkryte
                foreach (Plate item in this.plates)
                {
                    if (!item.IsRevealed && !item.IsMined)
                    {
                        hasFinished = false; //jeżeli wszystkie komórki są nie otwarte gra nie jest skończona
                        break;
                    }
                }
            }

            if (hasFinished)
            {
                gameTimer.Stop(); //gdy gra jest skończona timer stop
                NoMines(true);
            }
           }

        //metoda do stworzenia gry

        /// <summary>
        /// Rozpoczęcie gry
        /// </summary>
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

        //metoda do zatrzymania gry

        /// <summary>
        /// Zatrzymanie zegara
        /// </summary>
        public async Task Stop()
        {


            gameTimer.Stop();
            NoMines(false);

        }

        //Zmiana w zakresie użytyuch flag

        /// <summary>
        /// Licznik Flag
        /// </summary>
        protected virtual void OnCounterChanged(EventArgs e)
        {
            EventHandler handler = CounterChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        //Zmiana licznika czasu
        protected virtual void OnTimeElapsed(object sender, EventArgs e)
        {
            this.TimeElapsed++;
            EventHandler handler = TimerThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        ///<summary>
        ///Kliknięcie do otwarcia pojedynczej komórki - automatyczne by otworzyć wszystkie puste komórki w okolicy tej pojedynczej
        ///</summary>
        
        protected virtual void OnClickPlate(PlateEventArgs e)
        {
            EventHandler<PlateEventArgs> handler = ClickPlate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Metoda sprawdzająca czy wygraliśmy czy przegraliśmy
        /// </summary>
        public async Task NoMines(bool winner)
        {
            
            string win = "You win";

            string lose = "You lose";
           
            if (winner == false) {
                if (lc == 0) {
                    await Task.Run(() => lc++);
                    MessageBox.Show(lose); }
                ;
            }
            else if (winner == true)
            {
                MessageBox.Show(win);
            }
        }
    }
}
