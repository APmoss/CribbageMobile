// Ultimate Cribbage
// Config Assembly

// Copyright (C) 2003 - Keith Westley <keithsw1111@hotmail.com>

// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Xml;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows.Forms;

namespace ApplicationConfig
{
	/// <summary>
	/// This class provides the ability to save and retrieve user specific data
	/// 
	/// Features include
	/// 
	///   - Data is saved in a file specific to the logged in user
	///   - Data is saved in a file for the launching executable
	///   - Data can be automatically saved in assembly specific directories
	///   - All data types supported
	///   - Default values can be specified
	///   
	///   As data is stored using the ToString() Method. Objects that do not save all their state may lose
	///   Data by being saved here.
	///   
	/// </summary>
	public class Config
	{
      #region Static Variables
      static string __configDataFile = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + "_config.XML"; // the file that all data is saved to
      static Config __theConfig = new Config(); // singleton instance
      #endregion

      #region Member Variables
      Hashtable _configSections = new Hashtable(); // collection of sections
      bool _isDirty = false; // indicates if data is dirty
      #endregion

      #region Private Classes
      /// <summary>
      /// This class represents a section within a config file
      /// </summary>
      class ConfigSection
      {
         #region Member Variables
         Hashtable _values = new Hashtable(); // values in this section
         string _name; // section name
         #endregion

         #region Constructors

         /// <summary>
         /// Create a section from a saved file
         /// </summary>
         /// <param name="sectionNode">Section node from the file</param>
         public ConfigSection(XmlNode sectionNode)
         {
            // save the section name
            _name = sectionNode.Name;

            // now grab each of the values
            foreach(XmlNode node in sectionNode.ChildNodes)
            {
               // work out the type of the value
               Type type = Type.GetType(node.Attributes["Type"].Value);
               
               // restore it
               object Value = Convert.ChangeType(node.Attributes["Value"].Value, type);

               // save it
               _values.Add(node.Name, Value);
            }
         }

         /// <summary>
         /// Create a new empty section
         /// </summary>
         /// <param name="Name">Name of the new section</param>
         public ConfigSection(string Name)
         {
            // save the name
            _name = Name;
         }
         #endregion

         #region Public Member Functions

         /// <summary>
         /// Gets a named value but returns a default if not found
         /// </summary>
         /// <param name="Name">Name of value to get</param>
         /// <param name="Default">Value to return if value not known</param>
         /// <returns></returns>
         public object GetValue(string Name, object Default)
         {
            // if we have the value
            if (_values.ContainsKey(Name))
            {
               // return it
               return _values[Name];
            }
            else
            {
               // return the default
               return Default;
            }
         }

         /// <summary>
         /// Set the value of the item
         /// </summary>
         /// <param name="Name">Name of the value</param>
         /// <param name="Value">Value of the value</param>
         public void SetValue(string Name, object Value)
         {
            // save the value
            _values[Name] = Value;
         }

         /// <summary>
         /// Save the section using the provided XML writer
         /// </summary>
         /// <param name="xw"></param>
         public void Save(XmlTextWriter xw)
         {
            // Go through each value
            foreach(DictionaryEntry de in _values)
            {
               // Write the value name as the element
               xw.WriteStartElement(de.Key.ToString());

               // Save the data type
               xw.WriteAttributeString("Type", de.Value.GetType().ToString());
               xw.WriteAttributeString("Value", de.Value.ToString());

               // close the element
               xw.WriteEndElement();
            }
         }

         /// <summary>
         /// Get the section name
         /// </summary>
         public string Name
         {
            get
            {
               return _name;
            }
         }
         #endregion
      }
      #endregion

      #region Singleton Accessor
      /// <summary>
      /// Provides access to the singleton instance
      /// </summary>
      public static Config TheConfig
      {
         get
         {
            return __theConfig;
         }
      }
      #endregion

      #region Constructor
      /// <summary>
      /// Constructs the singleton instance
      /// </summary>
      Config()
      {
         // load the file
         Reload();
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// Returns a flag indicating if the data is dirty
      /// </summary>
      public bool IsDirty
      {
         get
         {
            return _isDirty;
         }
      }

      /// <summary>
      /// Load the config file
      /// </summary>
      public void Reload()
      {
         
         // clear any previous data
         _configSections.Clear();

         // data is not dirty
         _isDirty = false;

         // get the file
         IsolatedStorageFileStream store = null;
         try
         {
            try
            {
               store = new IsolatedStorageFileStream(__configDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
               // create an empty file
               store = new IsolatedStorageFileStream(__configDataFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
               System.IO.StreamWriter sw = new StreamWriter(store, System.Text.Encoding.UTF8);
               string sEmpty = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Config>\n</Config>";
               sw.Write(sEmpty);
               sw.Close();
               store = new IsolatedStorageFileStream(__configDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }

            // load the data document
            XmlDataDocument xmlConfigData = new XmlDataDocument();
            xmlConfigData.Load(store);

            // find the root
            XmlNode root = xmlConfigData.SelectSingleNode("/Config");

            if (root == null)
            {
               throw new ApplicationException("Config file corrupt");
            }
            else
            {
               // load each section
               foreach(XmlNode node in root.ChildNodes)
               {
                  _configSections.Add(node.Name, new ConfigSection(node));
               }
            }
         }
         catch (Exception ex)
         {
            throw ex;
         }
         finally
         {
            // close the file
            if (store != null)
            {
               store.Close();
            }
         }
      }

      #region Get
      /// <summary>
      /// Get a named value
      /// </summary>
      /// <param name="Section">Section to get it from</param>
      /// <param name="Name">Name of the value</param>
      /// <returns>null if not present</returns>
      public object GetValue(string Section, string Name)
      {
         return GetValue(Section, Name, null);
      }

      /// <summary>
      /// Get a named value but return default if not found
      /// </summary>
      /// <param name="Section">Section to get it from</param>
      /// <param name="Name">Name of the value</param>
      /// <param name="Default">Default to return if not found</param>
      /// <returns></returns>
      public object GetValue(string Section, string Name, object Default)
      {
         // If the section does not exist
         if (!_configSections.ContainsKey(Section))
         {
            // return the default
            return Default;
         }
         else
         {
            // ask the section for the value
            return ((ConfigSection)_configSections[Section]).GetValue(Name, Default);
         }
      }

      /// <summary>
      /// Get a named value from the calling assemblies section
      /// </summary>
      /// <param name="Name">Name of the value</param>
      /// <returns>null if not found</returns>
      public object GetValue(string Name)
      {
         // make the request using the calling assembly as the section name
         return GetValue(Path.GetFileNameWithoutExtension(Assembly.GetCallingAssembly().Location), Name, null);
      }

      /// <summary>
      /// Get a named value from the calling assemblies section
      /// </summary>
      /// <param name="Name">Name of the value</param>
      /// <param name="Default">Default to return if not found</param>
      /// <returns></returns>
      public object GetValue(string Name, object Default)
      {
         // make the request using the calling assembly as the section name
         return GetValue(Path.GetFileNameWithoutExtension(Assembly.GetCallingAssembly().Location), Name, Default);
      }
      #endregion

      #region Set
      /// <summary>
      /// Save a value
      /// </summary>
      /// <param name="Section">Section to save it in</param>
      /// <param name="Name">Name to save it under</param>
      /// <param name="Value">Value to save</param>
      public void SetValue(string Section, string Name, object Value)
      {
         // if section does not exist create it
         if (!_configSections.ContainsKey(Section))
         {
            _configSections[Section] = new ConfigSection(Section);
         }

         // if the value is different to the prior value
         if (GetValue(Section, Name) != Value)
         {
            // save it
            ((ConfigSection)_configSections[Section]).SetValue(Name, Value);

            // flag ourselves as dirty
            _isDirty = true;
         }
      }

      /// <summary>
      /// Save a value in the calling assemblies section
      /// </summary>
      /// <param name="Name">Name to save it under</param>
      /// <param name="Value">Value to save</param>
      public void SetValue(string Name, object Value)
      {
         // make the request using the calling assembly as the section name
         SetValue(Path.GetFileNameWithoutExtension(Assembly.GetCallingAssembly().Location), Name, Value);
      }
      #endregion

      /// <summary>
      /// Save the current config data
      /// </summary>
      public void Save()
      {
         // get the file
         IsolatedStorageFileStream store = null;
         try
         {
            try
            {
               store = new IsolatedStorageFileStream(__configDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
               // create an empty file
               store = new IsolatedStorageFileStream(__configDataFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }

            // create a writer
            XmlTextWriter xw = new XmlTextWriter(store, null);

            // make it easy to read
            xw.Formatting = Formatting.Indented;

            // write the root element
            xw.WriteStartElement("Config");

            // write each section
            foreach(DictionaryEntry de in _configSections)
            {
               // write an element with the section name
               xw.WriteStartElement(de.Key.ToString());

               // name write the section data
               ((ConfigSection)de.Value).Save(xw);

               // close it
               xw.WriteEndElement();
            }

            // CLose the file
            xw.WriteEndElement();
            xw.Close();
         }
         catch(Exception ex)
         {
            throw ex;
         }
         finally
         {
            // if we have a store close it
            if (store != null)
            {
               store.Close();
            }
         }

         // now that we are saved we cannot be dirty
         _isDirty = false;
      }
      #endregion
	}
}
