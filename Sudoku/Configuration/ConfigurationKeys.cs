namespace Sudoku.Configuration
{
    /// <summary>
    /// Store the name of XML configuration attributes.
    /// </summary>
    public sealed class ConfigurationKeys
    {
        public static readonly ConfigurationKeys DEFAULT_FILE_PATH = new ConfigurationKeys("DefaultFilePath");
        public static readonly ConfigurationKeys LANGUAGE = new ConfigurationKeys("Language");
        public static readonly ConfigurationKeys CELL_RED_BACKGROUND_ENABLED = new ConfigurationKeys("CellRedBackgroundEnabled");
        public static readonly ConfigurationKeys SHOW_INCORRECT_CELLS_ENABLED = new ConfigurationKeys("ShowIncorrectCellsEnabled");
        public static readonly ConfigurationKeys SHOW_NUMBER_OF_INCORRECT_CELLS_ENABLED = new ConfigurationKeys("ShowNumberOfIncorrectCellsEnabled");
        public static readonly ConfigurationKeys SHOW_WHETHER_SOLUTION_IS_CORRECT_ENABLED = new ConfigurationKeys("ShowWhetherSolutionIsCorrect");
        public static readonly ConfigurationKeys SUM_OF_NUMBERS_BIGGER_IN_CAGE_CHECK_ENABLED = new ConfigurationKeys("SumOfNumbersBiggerInCageCheckEnabled");

        private readonly string name;

        private ConfigurationKeys(string name)
        {
            this.name = name;
        }

        public string Name()
        {
            return name;
        }
    }
}
