using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sudoku.Configuration;
using Sudoku.Log;
using Sudoku.Generate;
using Sudoku.Language;
using static Sudoku.Configuration.ConfigurationKeys;

namespace Sudoku.Dialog
{
    class MenuHandler
    {
        #region Members

        private MenuStrip menu;
        private Dictionary<SudokuCreationType, EventHandler> eventHandlers;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;
        private Logger log = Logger.Instance;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem generateSubMenu;
        private ToolStripMenuItem openSubMenu;

        #endregion

        #region Methods

        #region Constructor

        public MenuHandler()
        {
            eventHandlers = new Dictionary<SudokuCreationType, EventHandler>();
        }

        #endregion

        #region Setters

        public void AddEventHandler(SudokuCreationType type, EventHandler handler)
        {
            this.eventHandlers.Add(type, handler);
        }

        #endregion

        #region MenuCreation

        public MenuStrip BuildMainDialogMenu()
        {
            menu = new MenuStrip();
            CreateFileMenu();
            CreateSettingsMenu();
            CreateAboutMenu();
            return menu;
        }

        private void CreateSettingsMenu()
        {
            menu.Items.Add(
                CreateMenuItem("options_menu", "options_menu", new EventHandler(SettingsMenuItem_Click)));
        }

        private void CreateAboutMenu()
        {
            menu.Items.Add(
                CreateMenuItem("about_menu", "about_menu", (sender, e) => { new AboutBox().ShowDialog(); }));
        }

        private void CreateFileMenu()
        {
            fileMenu = new ToolStripMenuItem(loc.Get("file_menu"));
            fileMenu.Tag = "file_menu";
            menu.Items.Add(fileMenu);
            CreateGenerateSubMenu(fileMenu);
            CreateOpenSubMenu(fileMenu);
            fileMenu.DropDownItems.Add(CreateMenuItem("exit_app", "exit_app", (sender, e) =>
            {
                log.Close("Closing the application.");
                Application.Exit();
            }));
        }

        private void CreateGenerateSubMenu(ToolStripMenuItem fileMenu)
        {
            generateSubMenu = CreateMenuItem("generate", "generate");
            fileMenu.DropDownItems.Add(generateSubMenu);
            generateSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku", "", eventHandlers[SudokuCreationType.GEN_SUD]));
            generateSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku-X", "", eventHandlers[SudokuCreationType.GEN_SUDX]));
            generateSubMenu.DropDownItems.Add(CreateMenuItem("centerdot", "centerdot", eventHandlers[SudokuCreationType.GEN_CENT]));
        }

        private void CreateOpenSubMenu(ToolStripMenuItem fileMenu)
        {
            openSubMenu = CreateMenuItem("open", "open");
            fileMenu.DropDownItems.Add(openSubMenu);
            openSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku", "", eventHandlers[SudokuCreationType.OPEN_SUD]));
            openSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku-X", "", eventHandlers[SudokuCreationType.OPEN_SUDX]));
            openSubMenu.DropDownItems.Add(CreateMenuItem("centerdot", "centerdot", eventHandlers[SudokuCreationType.OPEN_CENT]));
        }

        private ToolStripMenuItem CreateMenuItem(string caption, string locId, params EventHandler[] handler)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(String.IsNullOrEmpty(locId) ? caption : loc.Get(caption));
            item.Tag = locId;
            if (handler.Length > 0)
            {
                item.Click += handler[0];
            }
            return item;
        }

        #endregion

        #region EventHandlers
        
        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            string languageBeforeSettingsDialogOpened = conf.Get(LANGUAGE);
            new SettingsForm().ShowDialog();
            if (conf.Get(LANGUAGE) != languageBeforeSettingsDialogOpened)
                SetLabels();
            //TODO: update labels of MainWindow starting from this point
        }

        #endregion

        #region Labeling

        public void SetLabels()
        {
            foreach (ToolStripMenuItem item in menu.Items)
            {
                SetLabelText(item);
            }
        }

        private void SetMenuItemLabels(ToolStripMenuItem root)
        {
            foreach (ToolStripMenuItem item in root.DropDownItems)
            {
                SetLabelText(item);
            }
        }

        private void SetLabelText(ToolStripMenuItem item)
        {
            if (item.Tag != null && !String.IsNullOrEmpty(item.Tag.ToString()))
            {
                item.Text = loc.Get(item.Tag.ToString());
            }
            if (item.DropDownItems.Count > 0)
            {
                SetMenuItemLabels(item);
            }
        }

        #endregion

        #region MenuItemStatusChange

        public void EnableOpenAndGenerateMenuItems()
        {
            SetOpenAndGenerateMenuItemsStatus(true);
        }

        public void DisableOpenAndGenerateMenuItems()
        {
            SetOpenAndGenerateMenuItemsStatus(false);
        }

        private void SetOpenAndGenerateMenuItemsStatus(bool value)
        {
            foreach (ToolStripMenuItem item in generateSubMenu.DropDownItems)
            {
                item.Enabled = value;
            }
            foreach (ToolStripMenuItem item in openSubMenu.DropDownItems)
            {
                item.Enabled = value;
            }
        }

        #endregion

        #endregion
    }
}
