// Ultimate Cribbage
// Layouts Assembly

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
using ApplicationConfig;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using ExpressionEvaluator;

namespace Layouts
{
	/// <summary>
	/// This class manages screen layouts
	/// </summary>
	public class Layout
	{
      #region Static Variables
      const string __layoutFile = "Layout.XML"; // the name of the layout file
      static ArrayList __valueNames = new ArrayList(); // list of variables in a layout
      static Layout __layout = null; // singleton
      #endregion

      #region Member Variables
      Hashtable _values = new Hashtable(); // values for the current layout
      string _name = string.Empty; // name of the current layout
      #endregion

      #region Constructors
      /// <summary>
      /// Static constructor
      /// </summary>
      static Layout()
      {
         // load the xml file
         XmlDataDocument doc = new XmlDataDocument();
         doc.Load(__layoutFile);

         // get the definitions
         XmlNode node = doc.SelectSingleNode("/Layouts/Definitions");
         Debug.Assert(node != null, "Illegal Layout.xml file");

         // if we found them
         if (node != null)
         {
            // add each definition to the list of value names
            foreach (XmlNode n in node.ChildNodes)
            {
               __valueNames.Add(n.Name);
            }
         }
      }

      /// <summary>
      /// Loads the layout of the given name
      /// </summary>
      /// <param name="layoutName">Name of layout to load</param>
      public Layout(string layoutName)
      {
         // save the name
         _name = layoutName;

         // load the xml file
         XmlDataDocument doc = new XmlDataDocument();
         doc.Load(__layoutFile);

         // find the layout node
         XmlNode node = doc.SelectSingleNode("/Layouts/Values/Layout[@Name='" + layoutName + "']");

         // check we found it
         if (node == null)
         {
            throw new ApplicationException("Layout name " + layoutName + " not found in " + __layoutFile);
         }
         else
         {
            // go through each data element and add it to the list
            foreach (XmlNode n in node.ChildNodes)
            {
               Debug.Assert(__valueNames.Contains(n.Name), "Layout contains an unknown value " + n.Name);
               _values.Add(n.Name, n.InnerText);
            }

            // check nothing is missing
            bool missingEntries = false;
            foreach (string s in __valueNames)
            {
               if (!_values.ContainsKey(s))
               {
                  Debug.Fail("Layout missing entry for " + s);
                  missingEntries = true;
               }
            }

            // if anything is missing abort now
            if (missingEntries)
            {
               throw new ApplicationException("Layout name " + layoutName + " was missing required entries");
            }

            // replace any variable references
            Regex regex = new Regex("\\[[^\\[]*\\]", RegexOptions.Singleline);

            // this is being cloned so we can do a foreach without modifing the collection
            // we are doing the for each on.
            Hashtable copyValues = (Hashtable)_values.Clone();

            foreach (DictionaryEntry de in copyValues)
            {
               Match match = regex.Match((string)_values[de.Key]);

               // while we find matches
               while (match.Value != string.Empty)
               {
                  // replace them
                  _values[de.Key] = ((string)_values[de.Key]).Replace(match.Value, (string)_values[match.Value.Substring(1, match.Value.Length-2)]);

                  // look for the next match
                  match = regex.Match((string)_values[de.Key]);
               }

               // evaluate the expression
               try
               {
                  _values[de.Key] = Evaluator.EvaluateExpression((string)_values[de.Key]).ToString();
               }
               catch
               {
                  // ignore this as it may be ok
               }
            }
         }
      }
      #endregion

      #region Static Functions
      /// <summary>
      /// Get the singleton instance
      /// </summary>
      public static Layout TheLayout
      {
         get
         {
            if (__layout == null)
            {
               Reload();
            }

            return __layout;
         }
      }

      /// <summary>
      /// Get a list of available layouts
      /// </summary>
      public static StringCollection Layouts
      {
         get
         {
            // create a collection to keep them in
            StringCollection sc = new StringCollection();

            // open the xml document
            XmlDataDocument doc = new XmlDataDocument();
            doc.Load(__layoutFile);

            // find the list of layouts
            XmlNode node = doc.SelectSingleNode("/Layouts/Values");
            Debug.Assert(node != null, "Illegal Layout.xml file");

            if (node != null)
            {
               // add each one to the list
               foreach (XmlNode n in node.ChildNodes)
               {
                  sc.Add(n.Attributes["Name"].Value);
               }
            }

            return sc;
         }
      }

      /// <summary>
      /// Relaod with the current layout
      /// </summary>
      public static void Reload()
      {
         __layout = new Layout((string)Config.TheConfig.GetValue("ultimatecrib", "LayoutName", "Standard"));
      }
      #endregion

      #region public Member Functions

      /// <summary>
      /// Get the name of the layout
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         return _name;
      }

      /// <summary>
      /// Get a value of a variable from the layout
      /// </summary>
      /// <param name="valueName">Name of variable to get</param>
      /// <returns></returns>
      public object GetValue(string valueName)
      {
         Debug.Assert(__valueNames.Contains(valueName), "Value name is not valid : " + valueName);
         Debug.Assert(_values.ContainsKey(valueName), "Named value has not been defined in this layout : " + valueName);
         return _values[valueName];
      }

      /// <summary>
      /// Get a value of a variable from the layout and convert it to an integer
      /// </summary>
      /// <param name="valueName">Name of variable to get</param>
      /// <returns></returns>
      public int GetIntValue(string valueName)
      {
         return Convert.ToInt32(GetValue(valueName));
      }
      #endregion
	}
}
