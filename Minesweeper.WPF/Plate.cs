using System;

namespace Minesweeper.WPF
{
    public class Plate : IPlate
    {

        //automatyczne właściwości pól
        public MinesGrid GameGrid { get; private set; }
        public int RowPosition { get; private set; }
        public int ColPosition { get; private set; }
        public bool IsFlagged { get; set; }
        public bool IsMined { get; set; }
        public bool IsRevealed { get; private set; }

        //konstruktor
        public Plate(MinesGrid grid, int rowPosition, int colPosition)
        {
            this.GameGrid = grid;
            this.RowPosition = rowPosition;
            this.ColPosition = colPosition;
        }

        //metoda do liczenia min do okoła pojedynczego pola, wyświetla również numer bomb do okoła
        //w przypadku jak nie ma min do okoła to metoda MinesGrid.RevealPlate sprawdzi gdzie są najbliższe        
        public int Check()
        {
            int counter = 0;

            if (!IsRevealed && !IsFlagged)
            {
                IsRevealed = true;

                for (int i = 0; i < 9; i++) //sprawdź sąsiednie pola
                {
                    if (i == 4) continue; // pomija samą siebie
                    if (GameGrid.IsBomb(RowPosition + i / 3 - 1, ColPosition + i % 3 - 1)) counter++; //gdy znajdują się bomby do okoła, przelicz
                }

                if (counter == 0)
                {
                    for (int i = 0; i < 9; i++) //sprawdź sąsiednie pola
                    {
                        if (i == 4) continue; //pomija samą siebie
                        GameGrid.OpenPlate(RowPosition + i / 3 - 1, ColPosition + i % 3 - 1); //odsłoń sąsiednie pola
                    }
                }
            }

            return counter;
        }
    }
}
