using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Sudoku;
using Sudoku.Generate;
using Sudoku.Language;

namespace Sudoku.Dialogusok
{
    class MenuHandler
    {
        #region Members

        private MenuStrip menu;
        private EventHandler[] eventHandlers;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem generateSubMenu;
        private ToolStripMenuItem openSubMenu;

        #endregion

        #region Methods

        #region Setters

        public void SetEventHandlers(params EventHandler[] handlers)
        {
            this.eventHandlers = handlers;
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
                CreateMenuItem("about_menu", "about_menu", delegate(object sender, EventArgs e) { new AboutBox().ShowDialog(); }));
        }

        private void CreateFileMenu()
        {
            fileMenu = new ToolStripMenuItem(loc.GetLoc("file_menu"));
            menu.Items.Add(fileMenu);
            CreateGenerateSubMenu(fileMenu);
            CreateOpenSubMenu(fileMenu);
            //Adding exit menu
            fileMenu.DropDownItems.Add(CreateMenuItem("exit_app", "exit_app", delegate(object sender, EventArgs e) { Application.Exit(); }));
        }

        private void CreateGenerateSubMenu(ToolStripMenuItem fileMenu)
        {
            generateSubMenu = CreateMenuItem("generate", "generate");
            fileMenu.DropDownItems.Add(generateSubMenu);
            //Sudoku
            generateSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku", "", eventHandlers[0]));
            //Sudoku-X
            generateSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku-X", "", eventHandlers[1]));
            //Center Dot Sudoku
            generateSubMenu.DropDownItems.Add(CreateMenuItem("centerdot", "centerdot", eventHandlers[2]));
        }

        private void CreateOpenSubMenu(ToolStripMenuItem fileMenu)
        {
            openSubMenu = CreateMenuItem("open", "open");
            fileMenu.DropDownItems.Add(openSubMenu);
            //Sudoku
            openSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku", "", eventHandlers[3]));
            //Sudoku-X
            openSubMenu.DropDownItems.Add(CreateMenuItem("Sudoku-X", "", eventHandlers[4]));
            //Center Dot Sudoku
            openSubMenu.DropDownItems.Add(CreateMenuItem("centerdot", "centerdot", eventHandlers[5]));
        }

        private ToolStripMenuItem CreateMenuItem(string caption, string locId, params EventHandler[] handler)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(String.IsNullOrEmpty(locId) ? caption : loc.GetLoc(caption));
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
            string languageBeforeSettingsDialogOpened = conf.GetConfig("alapNyelv");
            new SettingsForm().ShowDialog();
            if (conf.GetConfig("alapNyelv") != languageBeforeSettingsDialogOpened)
                SetLabels();
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
            if (item.Tag != null && !"".Equals(item.Tag.ToString()))
            {
                item.Text = loc.GetLoc(item.Tag.ToString());
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
            SetOpenAndGenerateMenuItems(true);
        }

        public void DisableOpenAndGenerateMenuItems()
        {
            SetOpenAndGenerateMenuItems(false);
        }

        private void SetOpenAndGenerateMenuItems(bool value)
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
