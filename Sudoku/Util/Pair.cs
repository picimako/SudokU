using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public class Pair
    {
        #region Members

        private int _i;
        private int _j;

        #endregion

        #region Properties

        //For table indexing
        public int i { get { return _i; } set { _i = value; } }
        public int j { get { return _j; } set { _j = value; } }

        public int CageIndex { get { return _i; } set { _i = value; } }

        #endregion

        #region Constructor

        public Pair() { }

        public Pair(int i, int j)
        {
            this._i = i;
            this._j = j;
        }

        #endregion
    }
}
