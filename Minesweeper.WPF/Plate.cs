using System;

namespace Minesweeper.WPF
{
    public class Plate : IPlate
    {

        //automatic properties (fields)
        public MinesGrid GameGrid { get; private set; }
        public int RowPosition { get; private set; }
        public int ColPosition { get; private set; }
        public bool IsFlagged { get; set; }
        public bool IsMined { get; set; }
        public bool IsRevealed { get; private set; }

        //constructor
        public Plate(MinesGrid grid, int rowPosition, int colPosition)
        {
            this.GameGrid = grid;
            this.RowPosition = rowPosition;
            this.ColPosition = colPosition;
        }

        //method to count the mines around the current cell and to put number on it depending ot the cell count
        //if there're no mines around it redirect to MinesGrid.RevealPlate method to check all cells around for mines around them
        public int Check()
        {
            int counter = 0;

            if (!IsRevealed && !IsFlagged)
            {
                IsRevealed = true;

                for (int i = 0; i < 9; i++) // check all neighbours for bombs 
                {
                    if (i == 4) continue; // don't check itself
                    if (GameGrid.IsBomb(RowPosition + i / 3 - 1, ColPosition + i % 3 - 1)) counter++; // if there is a bomb, counts it
                }

                if (counter == 0)
                {
                    for (int i = 0; i < 9; i++) // check all neighbours for bombs 
                    {
                        if (i == 4) continue; // don't check itself
                        GameGrid.OpenPlate(RowPosition + i / 3 - 1, ColPosition + i % 3 - 1); // reveal all neighbours
                    }
                }
            }

            return counter;
        }
    }
}
