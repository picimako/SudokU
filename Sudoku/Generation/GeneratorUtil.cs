using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Controller;

namespace Sudoku.Generate
{
    sealed class GeneratorUtil
    {
        #region Members

        private SudokuExercise se = SudokuExercise.get;
        private int difficulty, killerDifficulty;
        private int[][,] solution;

        //Ebben a szótárban tárolom azokat az Pair listákat, amelyek azon táblákhoz tartoznak melyben a hátramaradt 4 cella négyzet/téglalap alakú
        //kulcsként tárolom a tábla számát
        private Dictionary<int, List<Pair>> rectangularCells;

        //a törölt értékű cellák indexeit és törlés előtti értékét tárolja
        private Dictionary<Pair, int> removedCellsAndValuesBeforeRemoval;

        #endregion

        #region Properties

        public int Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }

        public int KillerDifficulty
        {
            get { return killerDifficulty; }
            set { killerDifficulty = value; }
        }

        public int[][,] Solution
        {
            get { return solution; }
            set { solution = value; }
        }

        public Dictionary<int, List<Pair>> RectangularCells
        {
            get { return rectangularCells; }
            set { rectangularCells = value; }
        }

        public Dictionary<Pair, int> RemovedCellsAndValuesBeforeRemoval
        {
            get { return removedCellsAndValuesBeforeRemoval; }
            set { removedCellsAndValuesBeforeRemoval = value; }
        }

        #endregion

        #region Constructor

        public GeneratorUtil(int difficulty, int killerDifficulty)
        {
            this.difficulty = difficulty;
            this.killerDifficulty = killerDifficulty;
        }

        #endregion

        #region Methods

        public void InitializeDictionaries()
        {
            rectangularCells = new Dictionary<int, List<Pair>>();
            removedCellsAndValuesBeforeRemoval = new Dictionary<Pair, int>();
        }

        public void InitializeSolutionContainer()
        {
            CommonUtil.InitializeArray(out solution);
        }

        /// <summary> Beírja r-t a megfelelő táblákba, és beállítja a foglalt cellákat </summary>
        /// <param name="r"> A beírandó szám </param>
        /// <param name="i"> A kitöltött cella sorindexe </param>
        /// <param name="j"> A kitöltött cella oszlopindexe </param>
        /// <param name="kellSzamtombKitolt"> Megadja, hogy el kell-e végezni a foglalt cellák beállítását tombok[r]-ben </param>
        public void SetValueOfFilledCell(int r, int i, int j, bool kellSzamtombKitolt)
        {
            //A feladat, majd r saját számtömbjébe beírom r-t
            se.Exercise[0][i, j] = se.Exercise[r][i, j] = r;

            //Végigmegyek a számtömbökön
            for (int k = 1; k <= 9; k++)
            {
                //Ha nem tombok[r]-be akarok írni, akkor tombok[k]-ba írok -1-et az előbb kitöltött cella indexeivel megegyező cellába
                if (k != r)
                    se.Exercise[k][i, j] = -1;
            }

            /* Ha el kell végezni, akkor elvégzem tombok[r]-ben a foglalt cellák beállítását
             * Azért adom meg ezt a paramétert, mert a kitöltés során lehet olyan eset, amikor az egész r tömbben már csak egyetlen egy
             * üres cella van, ekkor csak be kell írni a számot, de ebben a tömbben már felesleges a szükséges cellákat -1-re (foglaltra állítani)
             * mert már minden cella r vagy -1 értékű*/
            if (kellSzamtombKitolt)
                se.Ctrl.MakeHousesOccupied(r, i, j);
        }

        #endregion
    }
}
