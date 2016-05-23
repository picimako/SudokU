using System;
using System.Windows.Forms;
using System.IO;
using Sudoku.Dialog;
using Sudoku.Language;
using Sudoku.Log;

namespace Sudoku
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                ConfigHandler.get.ReadConfiguration();
                LocHandler.get.ReadLocalization();
                Logger.Instance.SetLogFilePathForApplicationStartupPath(Application.StartupPath);
                Logger.Instance.Open();
            }
            catch (IOException)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SudokuApp());
        }
    }
}
