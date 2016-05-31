﻿using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Sudoku.Configuration
{
    public class ConfigHandler
    {
        #region Members

        private static ConfigHandler HANDLER;
        private string CONFIG_FILE_RELATIVE_PATH = "/Resources/Config/config.xml";
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
                xmlDoc.Load(Environment.CurrentDirectory + CONFIG_FILE_RELATIVE_PATH);
                XmlNode root = xmlDoc.DocumentElement;
                XmlNodeList nodeList = root.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    configuration.Add(node.Attributes["name"].Value, node.Attributes["value"].Value);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Configuration file cannot be found.", "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException();
            }
        }

        public string Get(ConfigurationKeys config)
        {
            return configuration[config.Name()];
        }

        public void SetAttributeValue(ConfigurationKeys config, string value)
        {
            configuration[config.Name()] = value;
            XmlNode node = xmlDoc.SelectSingleNode("/Configuration/Item[@name='" + config.Name() + "']");
            node.Attributes["value"].Value = value;
        }

        public void SaveConfiguration()
        {
            xmlDoc.Save(Environment.CurrentDirectory + CONFIG_FILE_RELATIVE_PATH);
        }

        #endregion

        #endregion
    }
}
