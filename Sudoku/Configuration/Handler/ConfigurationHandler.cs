using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Sudoku.Configuration
{
    /// <summary>
    /// Class for reading, storing and updating the XML configuration.
    /// </summary>
    public class ConfigHandler
    {
        #region Members

        private static ConfigHandler HANDLER;
        private string CONFIG_FILE_RELATIVE_PATH = "/Resources/Config/config.xml";
        private Dictionary<string, string> configuration;
        private XmlDocument xmlConfig;
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

        /// <summary>
        /// Reads the configuration from the predefined configuration file.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when the configuration file doesn't exist.</exception>
        public void ReadConfiguration()
        {
            xmlConfig = new XmlDocument();
            try
            {
                xmlConfig.Load(Environment.CurrentDirectory + CONFIG_FILE_RELATIVE_PATH);
                XmlNode root = xmlConfig.DocumentElement;
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

        /// <summary>
        /// Returns the value of the given config attribute.
        /// </summary>
        /// <param name="config">The config to return the value of.</param>
        /// <returns>The value of the config attribute specified.</returns>
        /// <remarks>No handling of non-existent config name for now.</remarks>
        public string Get(ConfigurationKeys config)
        {
            return configuration[config.Name()];
        }

        /// <summary>
        /// Returns the value of the given config attribute as boolean.
        /// </summary>
        /// <param name="config">The config to return the value of.</param>
        /// <returns>The true or false value of the config attribute specified.</returns>
        /// <remarks>No handling of error if parsing cannot happen to boolean.</remarks>
        public bool GetAsBool(ConfigurationKeys config)
        {
            return Boolean.Parse(Get(config));
        }

        /// <summary>
        /// Sets the given value to the given config attribute.
        /// </summary>
        /// <param name="config">The config attribute to change.</param>
        /// <param name="value">The new value of the config.</param>
        public void SetAttributeValue(ConfigurationKeys config, string value)
        {
            configuration[config.Name()] = value;
            XmlNode node = xmlConfig.SelectSingleNode("/Configuration/Item[@name='" + config.Name() + "']");
            node.Attributes["value"].Value = value;
        }

        /// <summary>
        /// Writes the current state of the configuration into the config file.
        /// </summary>
        public void SaveConfiguration()
        {
            xmlConfig.Save(Environment.CurrentDirectory + CONFIG_FILE_RELATIVE_PATH);
        }

        #endregion

        #endregion
    }
}
