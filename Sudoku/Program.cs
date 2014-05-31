using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Sudoku.Dialogusok;
using Sudoku.Language;

namespace Sudoku
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ConfigHandler.get.ReadConfiguration();
            LocHandler.get.ReadLocalization();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Program indítása
            Application.Run(new SudokuApp());
        }
    }
}
