using System.Windows.Forms;
using Sudoku.Configuration;
using Sudoku.Language;
using static Sudoku.Configuration.ConfigurationKeys;

namespace Sudoku.Dialog
{
    class SelectExerciseDialogFactory
    {
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;

        public OpenFileDialog CreateDialog()
        {
            OpenFileDialog selectExerciseDialog = new OpenFileDialog();
            selectExerciseDialog.InitialDirectory = conf.Get(DEFAULT_FILE_PATH);
            selectExerciseDialog.Title = loc.Get("select_file");
            selectExerciseDialog.Filter = loc.Get("text_files") + "(*.txt)|*.txt";
            return selectExerciseDialog;
        }
    }
}
