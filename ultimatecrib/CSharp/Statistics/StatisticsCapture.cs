// Ultimate Cribbage
// Statistics Assembly

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
using System.Collections;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;

namespace Statistics
{
   /// <summary>
   /// Singleton class which is used to capture player StatisticsCapture
   /// </summary>
   public class StatisticsCapture
	{
      #region Static Variables
      static Hashtable __singletons = new Hashtable(); // holds players for access
      public static string __statisticsDefinitionsFile = "Statistics.xml"; // name of the XML file that defines the stats
      public static string __statisticsDataFile = "StatisticsData.xml"; // name of the XML file that holds the stats data
      #endregion

      #region Member Variables
      Hashtable _rawStats = new Hashtable(); // the raw stats
      string _player1Name; // the name of the player whos Statistics this is for
      string _player2Name; // the name of the openent player
      #endregion

      #region Public Static Functions
      /// <summary>
      /// Singleton accessor function to get an instance of a players Statistics
      /// </summary>
      /// <param name="PlayerName"></param>
      /// <returns></returns>
      static public StatisticsCapture TheStatisticCapture(string PlayerName)
      {
         // if we have that player
         if (__singletons.ContainsKey(PlayerName))
         {
            // return it
            return (StatisticsCapture)__singletons[PlayerName];
         }
         else
         {
            // no player to return
            return null;
         }
      }
      #endregion

      #region Constructors
      /// <summary>
      /// Creates a StatisticsCapture capture instance for player 1 vs player2
      /// </summary>
      /// <param name="Player1"></param>
      /// <param name="Player2"></param>
      public StatisticsCapture(string Player1, string Player2)
      {
         __singletons[Player1] = this;

         _player1Name = Player1;
         _player2Name = Player2;

         Initialise();
      }
      #endregion

      #region Private Member Functions
      void Initialise()
      {
         // Clear out any existing stats
         _rawStats.Clear();

         // load stats document
         XmlDataDocument xmlStats = new XmlDataDocument();
         xmlStats.Load(StatisticsCapture.__statisticsDefinitionsFile);

         // locate current player total
         string s = "/Statistics/RawData/Definition";
         XmlNode nodeDefs = xmlStats.SelectSingleNode(s);

         // check we got a value
         if (nodeDefs == null)
         {
            throw new ApplicationException("Could not find raw stats data definitions.");
         }

         // create an initial value for each raw counter based on the counter type
         foreach (XmlNode node in nodeDefs.ChildNodes)
         {
            if (node.Name == "Data")
            {
               long initialValue = 0; // initial raw value
               switch(node.Attributes["Aggregate"].Value)
               {
                  case "Add":
                     initialValue = 0;
                     break;
                  case "Low":
                     initialValue = 99999;
                     break;
                  case "High":
                     initialValue = 0;
                     break;
                  case "AddOrReset":
                     initialValue = 0;
                     break;
               }

               // add the raw stat
               _rawStats.Add(node.Attributes["Name"].Value, initialValue);
            }
         }
      }
      #endregion

      #region Public Member Functions
      /// <summary>
      /// Alter the current statistic value by the given amount
      /// </summary>
      /// <param name="StatisticName">The raw statistic to alter</param>
      /// <param name="AlterAmount">Amount to alter the statistic by</param>
      public void AlterBy(string StatisticName, long AlterAmount)
      {
         // Add the amount on
         _rawStats[StatisticName] = (long)(_rawStats[StatisticName]) + AlterAmount;
      }

      /// <summary>
      /// Increase the statistic to the given value if the statistic is less than this value
      /// </summary>
      /// <param name="StatisticName">The raw statistic to alter</param>
      /// <param name="NewValue">The value to increase to</param>
      public void IncreaseTo(string StatisticName, long NewValue)
      {
         // if the new value is greater than the current value
         if ((long)_rawStats[StatisticName] < NewValue)
         {
            // overwrite it with the new value
            _rawStats[StatisticName] = NewValue;
         }
      }

      /// <summary>
      /// Set the statistic to the given value 
      /// </summary>
      /// <param name="StatisticName">The raw statistic to alter</param>
      /// <param name="NewValue">The value to increase to</param>
      public void SetTo(string StatisticName, long NewValue)
      {
         // overwrite it with the new value
         _rawStats[StatisticName] = NewValue;
      }

      /// <summary>
      /// Gets the current value of a statistic
      /// </summary>
      /// <param name="StatisticName">Name of the value to get</param>
      /// <returns></returns>
      public long Value(string StatisticName)
      {
         return (long)_rawStats[StatisticName];
      }

      /// <summary>
      /// Decrease the statistic to the given value if the statistic is greater than this value
      /// </summary>
      /// <param name="StatisticName">The raw statistic to alter</param>
      /// <param name="NewValue">The value to decrease to</param>
      public void DecreaseTo(string StatisticName, long NewValue)
      {
         // if the new value is less than the current value
         if ((long)_rawStats[StatisticName] > NewValue)
         {
            // overwrite it with the new value
            _rawStats[StatisticName] = NewValue;
         }
      }

      /// <summary>
      /// Apply the games statistics to the next level of aggregation ... a match
      /// </summary>
      public void SaveGame()
      {
         // open xml file
         XmlDataDocument xmlStats = new XmlDataDocument();
         xmlStats.Load(StatisticsCapture.__statisticsDefinitionsFile);

         IsolatedStorageFileStream store = null;
         try
         {
            store = new IsolatedStorageFileStream(__statisticsDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
         }
         catch
         {
            // create an empty file
            store = new IsolatedStorageFileStream(__statisticsDataFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            System.IO.StreamWriter sw = new StreamWriter(store, System.Text.Encoding.UTF8);
            string sEmpty = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Statistics>\n  <RawData>\n    <Values>\n    </Values>\n  </RawData>\n</Statistics>";
            sw.Write(sEmpty);
            sw.Close();
            store = new IsolatedStorageFileStream(__statisticsDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
         }

         // load the data document
         XmlDataDocument xmlStatsData = new XmlDataDocument();
         xmlStatsData.Load(store);

         // locate current player match
         string s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']/Match[@vs='"+_player2Name+"']";
         XmlNode nodeMatch = xmlStatsData.SelectSingleNode(s);

         if (nodeMatch == null)
         {
            // find the player node
            s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']";
            XmlNode nodePlayer = xmlStatsData.SelectSingleNode(s);

            // if player data is not there
            if (nodePlayer == null)
            {
               // create the player
               s = "/Statistics/RawData/Values";
               XmlNode nodeP = xmlStatsData.SelectSingleNode(s);

               XmlNode newNode = xmlStatsData.CreateElement("Player");
               newNode.Attributes.Append(xmlStatsData.CreateAttribute("Name"));
               newNode.Attributes["Name"].Value = _player1Name;
               nodePlayer = nodeP.AppendChild(newNode);
            }

            // create the match
            XmlElement newElement2 = xmlStatsData.CreateElement("Match");
            newElement2.SetAttribute("vs", _player2Name);
            nodeMatch = nodePlayer.AppendChild(newElement2);
         }

         foreach (DictionaryEntry de in _rawStats)
         {
            // find the definition for this stat type
            s = "/Statistics/RawData/Definition/Data[@Name='"+de.Key+"']";
            XmlNode nodeDef = xmlStats.SelectSingleNode(s);

            // find the matching total node
            s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']/Match[@vs='"+_player2Name+"']/Data[@Name='"+de.Key+"']";
            XmlNode nodeMatchValue = xmlStatsData.SelectSingleNode(s);

            // if node not found create it
            if (nodeMatchValue == null)
            {
               XmlNode newNode = xmlStatsData.CreateElement("Data");
               newNode.Attributes.Append(xmlStatsData.CreateAttribute("Name"));
               newNode.Attributes["Name"].Value = de.Key.ToString();
               newNode.InnerText = "-99999";
               nodeMatchValue = nodeMatch.AppendChild(newNode);
            }

            long Value = Convert.ToInt32(de.Value);
            long totalValue = Convert.ToInt32(nodeMatchValue.InnerText);

            // based on node rollup type
            switch (nodeDef.Attributes["Aggregate"].Value)
            {
               case "Add":
                  if (totalValue == -99999)
                  {
                     totalValue = Value;
                  }
                  else
                  {
                     totalValue = totalValue + Value;
                  }
                  break;
               case "Low":
                  if (totalValue == -99999)
                  {
                     totalValue = Value;
                  }
                  else
                  {
                     if (Value < totalValue)
                     {
                        totalValue = Value;
                     }
                  }
                  break;
               case "High":
                  if (totalValue == -99999)
                  {
                     totalValue = Value;
                  }
                  else
                  {
                     if (Value > totalValue)
                     {
                        totalValue = Value;
                     }
                  }
                  break;
               case "AddOrReset":
                  if (totalValue == -99999)
                  {
                     totalValue = Value;
                  }
                  else
                  {
                     if (Value == 0)
                     {
                        totalValue = 0;
                     }
                     else
                     {
                        totalValue = totalValue + Value;
                     }
                  }
                  break;
            }

            // update total node value
            nodeMatchValue.InnerText = totalValue.ToString();
         }

         // save xml file
         store.Seek(0, SeekOrigin.Begin);
         store.SetLength(0);
         xmlStatsData.Save(store);

         store.Close();

         // resetfor the next game
         Initialise();
      }

      /// <summary>
      /// Apply the matches statistics to the next level of aggregation ... a player
      /// </summary>
      public void SaveMatch()
      {
         // open xml file
         XmlDataDocument xmlStats = new XmlDataDocument();
         xmlStats.Load(StatisticsCapture.__statisticsDefinitionsFile);

         XmlDataDocument xmlStatsData = new XmlDataDocument();
         IsolatedStorageFileStream store;
         try
         {
            store = new IsolatedStorageFileStream(__statisticsDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
         }
         catch
         {
            // create an empty file
            store = new IsolatedStorageFileStream(__statisticsDataFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            System.IO.StreamWriter sw = new StreamWriter(store, System.Text.Encoding.UTF8);
            string sEmpty = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<StatisticsCapture>\n  <RawData>\n    <Values>\n    </Values>\n  </RawData>\n</StatisticsCapture>";
            sw.Write(sEmpty);
            sw.Close();
            store = new IsolatedStorageFileStream(__statisticsDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
         }

         xmlStatsData.Load(store);

         // locate current player total
         string s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']/Total[@vs='"+_player2Name+"']";
         XmlNode nodeTotal = xmlStatsData.SelectSingleNode(s);

         if (nodeTotal == null)
         {
            // create it
            // find the player node
            s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']";
            XmlNode nodePlayer = xmlStatsData.SelectSingleNode(s);

            if (nodePlayer == null)
            {
               s = "/Statistics/RawData/Values";
               XmlNode nodeP = xmlStatsData.SelectSingleNode(s);

               XmlNode newNode = xmlStatsData.CreateElement("Player");
               newNode.Attributes.Append(xmlStatsData.CreateAttribute("Name"));
               newNode.Attributes["Name"].Value = _player1Name;
               nodePlayer = nodeP.AppendChild(newNode);
            }

            XmlNode newNode2 = xmlStatsData.CreateElement("Total");
            newNode2.Attributes.Append(xmlStatsData.CreateAttribute("vs"));
            newNode2.Attributes["vs"].Value = _player2Name;
            nodeTotal = nodePlayer.AppendChild(newNode2);
         }

         // locate current player match against current opponent
         s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']/Match[@vs='"+_player2Name+"']";
         XmlNode nodeMatch = xmlStatsData.SelectSingleNode(s);

         if (nodeMatch != null)
         {
            // for each stat
            foreach (XmlNode node in nodeMatch.ChildNodes)
            {
               // find the definition for this stat type
               s = "/Statistics/RawData/Definition/Data[@Name='"+node.Attributes["Name"].Value+"']";
               XmlNode nodeDef = xmlStats.SelectSingleNode(s);

               // find the matching total node
               s = "/Statistics/RawData/Values/Player[@Name='" + _player1Name + "']/Total[@vs='"+_player2Name+"']/Data[@Name='"+node.Attributes["Name"].Value+"']";
               XmlNode nodeTotalValue = xmlStatsData.SelectSingleNode(s);

               // if node not found create it
               if (nodeTotalValue == null)
               {
                  XmlNode newNode = xmlStatsData.CreateElement("Data");
                  newNode.Attributes.Append(xmlStatsData.CreateAttribute("Name"));
                  newNode.Attributes["Name"].Value = node.Attributes["Name"].Value;
                  newNode.InnerText = "-99999";
                  nodeTotalValue = nodeTotal.AppendChild(newNode);
               }

               long Value = Convert.ToInt32(node.InnerText);
               long totalValue = Convert.ToInt32(nodeTotalValue.InnerText);

               // based on node rollup type
               switch (nodeDef.Attributes["Aggregate"].Value)
               {
                  case "Add":
                     if (totalValue == -99999)
                     {
                        totalValue = Value;
                     }
                     else
                     {
                        totalValue = totalValue + Value;
                     }
                     break;
                  case "Low":
                     if (totalValue == -99999)
                     {
                        totalValue = Value;
                     }
                     else
                     {
                        if (Value < totalValue)
                        {
                           totalValue = Value;
                        }
                     }
                     break;
                  case "High":
                     if (totalValue == -99999)
                     {
                        totalValue = Value;
                     }
                     else
                     {
                        if (Value > totalValue)
                        {
                           totalValue = Value;
                        }
                     }
                     break;
                  case "AddOrReset":
                     if (totalValue == -99999)
                     {
                        totalValue = Value;
                     }
                     else
                     {
                        if (Value == 0)
                        {
                           totalValue = 0;
                        }
                        else
                        {
                           totalValue = totalValue + Value;
                        }
                     }
                     break;
               }

               // update total node value
               nodeTotalValue.InnerText = totalValue.ToString();
            }

            nodeMatch.RemoveAll();

            // save xml file
            store.Seek(0, SeekOrigin.Begin);
            store.SetLength(0);
            xmlStatsData.Save(store);

            store.Close();
         }
      }

      /// <summary>
      /// Returns the player for whom these statistics have been captured
      /// </summary>
      /// <returns>Player 1 name</returns>
      public override string ToString()
      {
         return _player1Name;
      }
      #endregion
   }
}
