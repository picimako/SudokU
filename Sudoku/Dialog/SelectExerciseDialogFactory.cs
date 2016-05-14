using System.Windows.Forms;
using Sudoku.Language;

namespace Sudoku.Dialog
{
    class SelectExerciseDialogFactory
    {
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;

        public OpenFileDialog CreateDialog()
        {
            OpenFileDialog selectExerciseDialog = new OpenFileDialog();
            selectExerciseDialog.InitialDirectory = conf.GetConfig("alapFajlUtvonal");
            selectExerciseDialog.Title = loc.Get("select_file");
            selectExerciseDialog.Filter = loc.Get("text_files") + "(*.txt)|*.txt";
            return selectExerciseDialog;
        }
    }
}
