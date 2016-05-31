using System;
using System.Windows.Forms;
using System.IO;
using Sudoku.Configuration;
using Sudoku.Dialog;
using Sudoku.Language;
using Sudoku.Log;

namespace Sudoku
{
    //TODO: Handle AccessViolationException
    //http://stackoverflow.com/questions/3469368/how-to-handle-accessviolationexception
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Logger.Instance.SetLogFilePathForApplicationStartupPath(Application.StartupPath);
                Logger.Instance.Open();
                Environment.CurrentDirectory = Environment.GetEnvironmentVariable("SUDOKU_PATH", EnvironmentVariableTarget.Machine);
                ConfigHandler.get.ReadConfiguration();
                LocHandler.get.ReadLocalization();
            }
            catch (IOException)
            {
                Logger.Instance.Close("Error happened during loading resources.");
                return;
            }
            catch (ArgumentNullException)
            {
                Logger.Instance.Close("The requested system environment variable (SUDOKU_PATH) is not set.");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SudokuApp());
        }
    }
}
