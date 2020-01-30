using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper.WPF
{
    /// <summary>
    /// Metoda logiczna dla MainWindow.xaml, definiujemy kliki i ich logike
    /// </summary>
    public partial class MainWindow : Window
    {
        private int nrMines;
        public MinesGrid Mines { get; private set; }
        private bool gameStarted;
        private Color[] mineText;


        public MainWindow()
        {
            InitializeComponent();
            gameStarted = false;
            this.nrMines = 15;
            mineText = new Color[] {Colors.White /* first doesn't matter */, 
                                    Colors.Blue, Colors.DarkGreen, Colors.Red, Colors.DarkBlue, 
                                    Colors.DarkViolet, Colors.DarkCyan, Colors.Brown, Colors.Black };
            GameSetup();
        }

   
        private void MenuItem_Click_New(object sender, RoutedEventArgs e)
        {
            GameSetup();
            MinesGrid.lc = 0;
        }



        /// <summary>
        /// Ustawienia gry
        /// </summary>
        private void GameSetup()
        {
            Mines = new MinesGrid(10, 10, nrMines);
            foreach (Button btn in ButtonsGrid.Children)
            {
                btn.Content = ""; // clears flag or bomb image (if any)
                btn.IsEnabled = true; // button gets clickable
            }
            // Attaches Mines Indicator Event
            Mines.CounterChanged += OnCounterChanged;
            MinesIndicator.Text = nrMines.ToString();

            // Attaches Button Click, invoked by a plate
            Mines.ClickPlate += OnClickPlate;

            // Attaches Time Threshold Elapsed Event
            Mines.TimerThresholdReached += OnTimeChanged;            
            TimeIndicator.Text = "0";

            Mines.Run();
            gameStarted = true;
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // closes application
        }

        /// <summary>
        /// Określa co sie dzieje z polem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender; // gets clicked button reference
            int row = ParseButtonRow(btn);
            int col = ParseButtonColumn(btn);
            if (!Mines.IsInGrid(row, col)) throw new MinesweeperException("Invalid Button to MinesGrid reference on reveal"); // the button points to an invalid plate

            if (Mines.IsFlagged(row, col)) return; // flagged plate cannot be revealed

            btn.IsEnabled = false; // disables the button
            if (Mines.IsBomb(row, col)) //a bomb was revealed !!! 
            {
                // attaches bomb image to the button
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                Image bombImage = new Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(@"..\..\Bomb.png", UriKind.Relative);
                bi.EndInit();
                bombImage.Source = bi;
                sp.Children.Add(bombImage);
                btn.Content = sp;
                Mines.Stop();
                
                // finishes the game and opens all plates
                if (gameStarted)
                {
                    gameStarted = false;
                    foreach (Button butn in ButtonsGrid.Children)
                    {
                        if (butn.IsEnabled) this.Button_Click(butn, e); // calls all other unrevealed buttons
                    }
                }
            }
            else // an empty space was revealed
            {
                int count = Mines.RevealPlate(row, col); // opens the plate     and checks for surrounding bombs
                if (count > 0) // put a corresponding label on the current button
                {
                    btn.Foreground = new SolidColorBrush(mineText[count]);
                    btn.FontWeight = FontWeights.Bold;
                    btn.Content = count.ToString();
                }
            }
        }
        /// <summary>
        /// Prawy klick na polu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Right_Button_Click(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender; // gets clicked button reference
            int row = ParseButtonRow(btn);
            int col = ParseButtonColumn(btn);
            if (!Mines.IsInGrid(row, col)) throw new MinesweeperException("Invalid Button to MinesGrid reference on flag"); // the button points to an invalid plate

            if (Mines.IsFlagged(row, col)) // the button has flag image child
            {
                btn.Content = ""; // clears flag image
            }
            else
            {
                // attaches flag image to the button
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                Image flagImage = new Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(@"..\..\Flag.png", UriKind.Relative);
                bi.EndInit();
                flagImage.Source = bi;
                sp.Children.Add(flagImage);
                btn.Content = sp;
            }

            Mines.FlagMine(row, col);
        }

        private int ParseButtonRow(Button btn)
        {
            // Button Name format must be "ButtonXY" or "ButtonXXYY", where X and Y are numberical indices of the mine cell
            if (btn.Name.IndexOf("Button") != 0) throw new MinesweeperException("Wrong button name in UI module"); // the button is wrong
            return int.Parse(btn.Name.Substring(6, (btn.Name.Length - 6) / 2));
        }

        private int ParseButtonColumn(Button btn)
        {
            // Button Name format must be "ButtonXY" or "ButtonXXYY", where X and Y are numberical indices of the mine cell
            if (btn.Name.IndexOf("Button") != 0) throw new MinesweeperException("Wrong button name in UI module"); // the button is wrong
            return int.Parse(btn.Name.Substring(6 + (btn.Name.Length - 6) / 2, (btn.Name.Length - 6) / 2));
        }

        private void OnCounterChanged(object sender, EventArgs e)
        {
            // Updates MineIndicator field in the UI
            MinesIndicator.Text = (this.nrMines - Mines.FlaggedMines).ToString();
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            // Updates MineIndicator field in the UI
            TimeIndicator.Text = Mines.TimeElapsed.ToString();
        }

        private void OnClickPlate(object sender, PlateEventArgs e)
        {
            // Opens requested plate through simulating Button Click
            string btnName = "Button";
            if (Mines.Width <= 10 && Mines.Height <= 10) btnName += String.Format("{0:D1}{1:D1}", e.PlateRow, e.PlateColumn); // one digit coordinates
            else btnName += String.Format("{0:D2}{1:D2}", e.PlateRow, e.PlateColumn); // two digits coordinates

            Button senderButton = (ButtonsGrid.FindName(btnName) as Button);
            if (senderButton == null) throw new MinesweeperException("Invalid Button to MinesGrid reference on multiple reveal"); // the plate refers to an invalid button

            // calls respecive "Button Click" event handler 
            this.Button_Click(senderButton, new RoutedEventArgs());
        }

        private void MinesIndicator_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
