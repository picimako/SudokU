using System;
using System.Windows.Forms;
using System.IO;
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
                Logger.Instance.Info("Error happened during loading resources.");
                Logger.Instance.Close();
                return;
            }
            catch (ArgumentNullException)
            {
                Logger.Instance.Info("The requested system environment variable (SUDOKU_PATH) is not set.");
                Logger.Instance.Close();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SudokuApp());
        }
    }
}
