using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Sudoku.Language
{
    class LocHandler
    {
        #region Members

        private static LocHandler HANDLER;
        private Dictionary<string, string> localization;

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
                xmlDoc.Load(Environment.CurrentDirectory + "/Resources/Localization/" + ConfigHandler.get.GetConfig("alapNyelv") + ".xml");
                XmlNode root = xmlDoc.DocumentElement;
                XmlNodeList nodeList = root.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    localization.Add(node.Attributes["id"].Value, node.Attributes["value"].Value);
                }
            }
            catch (DirectoryNotFoundException)
            {
                //TODO: megoldani valahogy, hogy lokalizált hibaüzenetet írjak ki
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
