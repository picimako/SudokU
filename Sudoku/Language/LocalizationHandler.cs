﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Sudoku;
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
                xmlDoc.Load("Languages/" + ConfigHandler.get.GetConfig("alapNyelv") + ".xml");
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
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Selected language file cannot be found.", "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetLoc(string id)
        {
            return localization[id];
        }

        #endregion

        #endregion
    }
}
