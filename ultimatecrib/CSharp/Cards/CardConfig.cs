// Ultimate Cribbage
// Cards Assembly

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

// Code complete & commented 3/9/2003 - KW

using System;
using System.Collections;
using System.Collections.Specialized;

namespace Cards
{
	/// <summary>
	/// This class is used to access configuration settings for the card class
	/// Users of the card assembly can either set the values here or set up a 
	/// delegate to return the values
	/// </summary>
	public class CardConfig
	{
      #region Internal Classes
      /// <summary>
      /// The Value item structure is used to store a value
      /// </summary>
      struct ValueItem
      {
         object _value; // value of the value
         object _default; // default value of the value

         /// <summary>
         /// Initialise a value
         /// </summary>
         /// <param name="Name">Name of the value</param>
         /// <param name="Default">Default value of the value</param>
         public ValueItem(object Default)
         {
            _default = Default;
            _value = _default;
         }

         /// <summary>
         /// Get/set the value
         /// </summary>
         public object Value
         {
            get
            {
               return _value;
            }
            set
            {
               _value = value;
            }
         }

         /// <summary>
         /// Get the default value
         /// </summary>
         public object Default
         {
            get
            {
               return _default;
            }
         }
      }
      #endregion

      #region Member variables
      static Hashtable _values = new Hashtable();
      #endregion

      #region Delegates
      // Delegates
      public delegate int GetDelegatedIntValue(string ValueName); 
      public static GetDelegatedIntValue OnGetIntValue = null; 
      public delegate string GetDelegatedStringValue(string ValueName); 
      public static GetDelegatedStringValue OnGetStringValue = null; 
      public delegate bool GetDelegatedBoolValue(string ValueName); 
      public static GetDelegatedBoolValue OnGetBoolValue = null; 
      #endregion

      #region Constructors
      /// <summary>
      /// This constructor should never be called as this is a static class
      /// </summary>
		protected CardConfig()
		{
		}

      /// <summary>
      /// Static constructor that defines the supported values and defaults
      /// </summary>
      static CardConfig()
      {
         _values.Add("CardSort", new ValueItem("Ascending"));
         _values.Add("CardVarticalOffset", new ValueItem(20));
         _values.Add("CardBackName", new ValueItem("Cards32_1"));
         _values.Add("CardFamily", new ValueItem("Plain"));
         _values.Add("CardHorizontalOffset", new ValueItem(15));
         _values.Add("CardVerticalOffset", new ValueItem(20));
         _values.Add("CardHeight", new ValueItem(96));
         _values.Add("CardWidth", new ValueItem(71));
         _values.Add("SelectedHorizontalOffset", new ValueItem(10));
         _values.Add("SelectedVerticalOffset", new ValueItem(10));
         _values.Add("ShadowHorizontal", new ValueItem(5));
         _values.Add("ShadowVertical", new ValueItem(5));
      }
      #endregion

      #region Public Static Functions
      /// <summary>
      /// Get a list of the CardConfig item names
      /// </summary>
      /// <returns>A collection of the value names</returns>
      public static StringCollection Values
      {
         get
         {
            StringCollection rc = new StringCollection();
            foreach (DictionaryEntry de in _values)
            {
               rc.Add((string)de.Key);
            }

            return rc;
         }
      }

      /// <summary>
      /// Get the current value of a string value
      /// </summary>
      /// <param name="ValueName">Name of the value to retrieve</param>
      /// <returns>Current value of this item</returns>
      public static string GetStringValue(string ValueName)
      {
         // if we have a delegate
         if (OnGetStringValue != null)
         {
            try
            {
               // call the delegate
               return OnGetStringValue(ValueName);
            }
            catch
            {
               // dont do anything. Let it pass through
            }
         }

         // return the value we have
         return (string)((ValueItem)(_values[ValueName])).Value;
      }


      /// <summary>
      /// Get the current value of an integer value
      /// </summary>
      /// <param name="ValueName">Name of the value to retrieve</param>
      /// <returns>Current value of this item</returns>
      public static int GetIntValue(string ValueName)
      {
         // if we have a delegate
         if (OnGetIntValue != null)
         {
            try
            {
               // call the delegate
               return OnGetIntValue(ValueName);
            }
            catch
            {
               // dont do anything. Let it pass through
            }
         }

         // return the value we have
         return (int)((ValueItem)(_values[ValueName])).Value;
      }

      /// <summary>
      /// Get the default value of a string value
      /// </summary>
      /// <param name="ValueName">Name of the value to retrieve</param>
      /// <returns>Default value of this item</returns>
      public static string GetStringDefaultValue(string ValueName)
      {
         return (string)((ValueItem)(_values[ValueName])).Default;
      }

      /// <summary>
      /// Get the default value of an integer value
      /// </summary>
      /// <param name="ValueName">Name of the value to retrieve</param>
      /// <returns>Default value of this item</returns>
      public static int GetIntDefaultValue(string ValueName)
      {
         return (int)((ValueItem)(_values[ValueName])).Default;
      }

      /// <summary>
      /// Sets the value of a Value
      /// </summary>
      /// <param name="ValueName">Name of value to set</param>
      /// <param name="Value">New value</param>
      public static void SetValue(string ValueName, string Value)
      {
         try
         {
            // first get the existing value as a string. This should throw an exception if the existing
            // value is not already a string
            GetStringValue(ValueName);

            // now set the value
            ValueItem vi = (ValueItem)_values[ValueName];
            vi.Value = Value;
            _values[ValueName] = vi;
         }
         catch (Exception ex)
         {
            throw new ApplicationException("Value data type different to existing data type", ex);
         }
      }

      /// <summary>
      /// Sets the value of a Value
      /// </summary>
      /// <param name="ValueName">Name of value to set</param>
      /// <param name="Value">New value</param>
      public static void SetValue(string ValueName, int Value)
      {
         try
         {
            // first get the existing value as an integer. This should throw an exception if the existing
            // value is not already an integer
            GetIntValue(ValueName);

            // now set the value
            ValueItem vi = (ValueItem)_values[ValueName];
            vi.Value = Value;
            _values[ValueName] = vi;
         }
         catch (Exception ex)
         {
            throw new ApplicationException("Value data type different to existing data type", ex);
         }
      }

      #endregion
	}
}
