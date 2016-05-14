using System;
using System.Windows.Forms;
using System.Drawing;
using Sudoku.Controller;

namespace Sudoku.Dialog
{
    public class KillerTextBox : TextBox
    {
        #region Members

        private String sumOfNumbersInCage;
        private bool isBorderNeeded;
        private Font fontForSumOfNumbers;
        private SudokuExercise se = SudokuExercise.get;

        #endregion

        #region Constructor

        /// <param name="sumOfNumbersInCage">A ketrecben levő számok összege</param>
        /// <param name="isBorderNeeded">Ha a feladat Középpont Sudoku, akkor a blokkok középső celláját kerettel jelölöm</param>
        public KillerTextBox(String sumOfNumbersInCage, bool isBorderNeeded)
        {
            this.sumOfNumbersInCage = sumOfNumbersInCage;
            this.isBorderNeeded = isBorderNeeded;
            fontForSumOfNumbers = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));

            //A szöveg igazítása a TextBox-on belül vízszintesen középre
            this.TextAlign = HorizontalAlignment.Center;

            //Stílus beállítása
            this.SetStyle(ControlStyles.UserPaint, true);

            BindEventHandlers();
        }

        #endregion

        #region Methods

        #region Private

        private void BindEventHandlers()
        {
            this.TextChanged += delegate(object sender, EventArgs e) { OnPaint(new PaintEventArgs(CreateGraphics(), ClientRectangle)); };
            this.KeyDown += delegate(object sender, KeyEventArgs e) { OnPaint(new PaintEventArgs(CreateGraphics(), ClientRectangle)); };
        }

        #endregion

        #region Protected

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(new SolidBrush(BackColor), e.ClipRectangle);

            StringFormat sf = new StringFormat();
            sf.Alignment = se.IsExerciseKiller ? StringAlignment.Far : StringAlignment.Center;
            sf.LineAlignment = se.IsExerciseKiller ? StringAlignment.Near : StringAlignment.Center;

            e.Graphics.DrawString(this.Text, this.Font, Brushes.Black, new RectangleF(0, 0, ClientRectangle.Width, ClientRectangle.Height), sf);
            
            if (se.IsExerciseKiller)
            {
                //Kiírom a ketrecösszeget
                e.Graphics.DrawString(sumOfNumbersInCage, fontForSumOfNumbers, Brushes.Black, new PointF(0, 0));
                //Ha kell keretet rajzolni, akkor rajzolok
                if (isBorderNeeded)
                {
                    Pen p = new Pen(Brushes.Black);

                    //Felső
                    e.Graphics.DrawLine(p, new Point(0, 0), new Point(30, 0));
                    //Bal szélső
                    e.Graphics.DrawLine(p, new Point(0, 0), new Point(0, 30));
                }
            }
        }

        #endregion
        #endregion
    }
}
