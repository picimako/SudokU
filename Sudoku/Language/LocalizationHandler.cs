using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using Sudoku.Configuration;
using static Sudoku.Configuration.ConfigurationKeys;

namespace Sudoku.Language
{
    class LocHandler
    {
        #region Members

        private static LocHandler HANDLER;
        private Dictionary<string, string> localization;
        private string localizationFolder;

        #endregion

        #region InitSingleton

        public static LocHandler get
        {
            get
            {
                if (HANDLER == null)
                {
                    HANDLER = new LocHandler();
                }
                return HANDLER;
            }
        }

        private LocHandler()
        {
            InitLocalization();
        }

        #endregion

        #region Methods

        #region Private

        public string LocalizationFolder
        {
            get { return localizationFolder; }
        }

        private void InitLocalization()
        {
            localization = new Dictionary<string, string>();
        }

        #endregion

        #region Public

        public void ReadLocalization()
        {
            InitLocalization();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                localizationFolder = Environment.CurrentDirectory + "/Resources/Localization/";
                xmlDoc.Load(localizationFolder + ConfigHandler.get.Get(LANGUAGE) + ".xml");
                XmlNode root = xmlDoc.DocumentElement;
                XmlNodeList nodeList = root.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    localization.Add(node.Attributes["id"].Value, node.Attributes["value"].Value);
                }
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Language folder cannot be found.", "Missing folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Selected language file cannot be found.", "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException();
            }
        }

        public string Get(string id)
        {
            return localization[id];
        }

        #endregion

        #endregion
    }
}
