using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Sudoku
{
    public class ConfigHandler
    {
        #region Members

        private static ConfigHandler HANDLER;
        private Dictionary<string, string> configuration;
        private XmlDocument xmlDoc;
        private bool exerciseInProgress;

        #endregion

        #region Properties

        public bool ExerciseInProgress
        {
            get { return this.exerciseInProgress; }
            set { this.exerciseInProgress = value; }
        }

        #endregion

        #region InitSingleton

        public static ConfigHandler get
        {
            get
            {
                if (HANDLER == null)
                {
                    HANDLER = new ConfigHandler();
                }
                return HANDLER;
            }
        }

        private ConfigHandler()
        {
            configuration = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        #region Public

        public void ReadConfiguration()
        {
            xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load("config.xml");
                XmlNode root = xmlDoc.DocumentElement;
                XmlNodeList nodeList = root.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    configuration.Add(node.Attributes["name"].Value, node.Attributes["value"].Value);
                }
            }
            catch (FileNotFoundException)
            {
                //TODO: localize error messages
                MessageBox.Show("Configuration file cannot be found.", "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetConfig(string name)
        {
            return configuration[name];
        }

        public void SetAttributeValue(string name, string value)
        {
            configuration[name] = value;
            XmlNode node = xmlDoc.SelectSingleNode("/Configuration/Item[@name='" + name + "']");
            node.Attributes["value"].Value = value;
        }

        public void SaveConfiguration()
        {
            xmlDoc.Save("config.xml");
        }

        #endregion

        #endregion
    }
}
