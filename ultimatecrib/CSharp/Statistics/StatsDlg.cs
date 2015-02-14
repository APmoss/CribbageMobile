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
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Printing;
using ExpressionEvaluator;
using ApplicationConfig;
using System.IO;
using System.IO.IsolatedStorage;

namespace Statistics
{
	/// <summary>
	/// Displays player statistics
	/// </summary>
	public class ShowStatistics : System.Windows.Forms.Form
	{
      #region Internal Classes and Structures
      /// <summary>
      /// Holds data used by the print process
      /// </summary>
      struct PrintData
      {
         public string printString; // data being printed
         public int currentDataPos; // where we are up to in the data we are printing
      }
      #endregion
      
      #region Member Variables
      string _playerName = "Error"; // player for whom we are displaying stats
      PrintData _printData; // cross function data used in printing
      #endregion

      #region Windows Form Designer generated Member Variables
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ListView StatisticsListView;
      private System.Windows.Forms.ColumnHeader StatisticColumn;
      private System.Windows.Forms.ColumnHeader ValueColumn;
      private System.Windows.Forms.ContextMenu PlayerMenu;
      private System.Windows.Forms.Button CloseButton;
      private System.Windows.Forms.Button PrintButton;
      private System.Windows.Forms.ComboBox StatisticType;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
      #endregion

      #region Constructors
      /// <summary>
      /// Create the statistics dialog
      /// </summary>
      /// <param name="PlayerName">Name of the player to display stats for</param>
		public ShowStatistics(string PlayerName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         // save the player name
         _playerName = PlayerName;
		}
      #endregion
      
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.PrintButton = new System.Windows.Forms.Button();
         this.StatisticsListView = new System.Windows.Forms.ListView();
         this.StatisticColumn = new System.Windows.Forms.ColumnHeader();
         this.ValueColumn = new System.Windows.Forms.ColumnHeader();
         this.PlayerMenu = new System.Windows.Forms.ContextMenu();
         this.CloseButton = new System.Windows.Forms.Button();
         this.StatisticType = new System.Windows.Forms.ComboBox();
         this.label1 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // PrintButton
         // 
         this.PrintButton.Location = new System.Drawing.Point(288, 248);
         this.PrintButton.Name = "PrintButton";
         this.PrintButton.Size = new System.Drawing.Size(80, 24);
         this.PrintButton.TabIndex = 3;
         this.PrintButton.Text = "Print";
         this.PrintButton.Click += new System.EventHandler(this.PrintButton_Click);
         // 
         // StatisticsListView
         // 
         this.StatisticsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                    this.StatisticColumn,
                                                                                    this.ValueColumn});
         this.StatisticsListView.ContextMenu = this.PlayerMenu;
         this.StatisticsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
         this.StatisticsListView.Location = new System.Drawing.Point(8, 40);
         this.StatisticsListView.Name = "StatisticsListView";
         this.StatisticsListView.Size = new System.Drawing.Size(448, 200);
         this.StatisticsListView.TabIndex = 2;
         this.StatisticsListView.View = System.Windows.Forms.View.Details;
         // 
         // Statistic
         // 
         this.StatisticColumn.Text = "Stat vs All";
         this.StatisticColumn.Width = 333;
         // 
         // Value
         // 
         this.ValueColumn.Text = "Value";
         this.ValueColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.ValueColumn.Width = 110;
         // 
         // CloseButton
         // 
         this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.CloseButton.Location = new System.Drawing.Point(376, 248);
         this.CloseButton.Name = "CloseButton";
         this.CloseButton.Size = new System.Drawing.Size(80, 24);
         this.CloseButton.TabIndex = 3;
         this.CloseButton.Text = "Close";
         this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
         // 
         // StatisticType
         // 
         this.StatisticType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.StatisticType.DropDownWidth = 184;
         this.StatisticType.Items.AddRange(new object[] {
                                                      "All Games",
                                                      "Current Match Only"});
         this.StatisticType.Location = new System.Drawing.Point(96, 8);
         this.StatisticType.Name = "StatisticType";
         this.StatisticType.Size = new System.Drawing.Size(184, 21);
         this.StatisticType.TabIndex = 1;
         this.StatisticType.SelectedIndexChanged += new System.EventHandler(this.StatType_SelectedIndexChanged);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(88, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "Statistics For";
         // 
         // Statistics
         // 
         this.AcceptButton = this.CloseButton;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.CloseButton;
         this.ClientSize = new System.Drawing.Size(474, 301);
         this.ControlBox = false;
         this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                      this.PrintButton,
                                                                      this.CloseButton,
                                                                      this.StatisticsListView,
                                                                      this.StatisticType,
                                                                      this.label1});
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "Statistics";
         this.ShowInTaskbar = false;
         this.Text = "All Games";
         this.Closing += new System.ComponentModel.CancelEventHandler(this.Statistics_Closing);
         this.Load += new System.EventHandler(this.Statistics_Load);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Member Functions
      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if(components != null)
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

      /// <summary>
      /// Create the list view columns
      /// </summary>
      /// <param name="OpponentName">Name of the players opponent</param>
      void CreateColumns(string OpponentName)
      {
         StatisticsListView.Columns.Add("Stat vs " + OpponentName, 160, HorizontalAlignment.Left);
         StatisticsListView.Columns.Add("Value", 40, HorizontalAlignment.Right);
      }

      /// <summary>
      /// Gets the names of the openents of the given player
      /// </summary>
      /// <param name="player">Name of the player to get the opponents for</param>
      StringCollection OpponentsOf(string player)
      {
         // create something to return
         StringCollection rc = new StringCollection();

         // open the stats xml file
         IsolatedStorageFileStream store = null;
         try
         {
            store = new IsolatedStorageFileStream(StatisticsCapture.__statisticsDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
         }
         catch
         {
            store = null;
         }

         // if we found the file
         if (store != null)
         {
            // load it
            XmlDataDocument xmlStatsData = new XmlDataDocument();
            xmlStatsData.Load(store);

            // find out players node
            string s = "/Statistics/RawData/Values/Player[@Name='" + _playerName + "']";
            XmlNode nodePlayer = xmlStatsData.SelectSingleNode(s);

            // if we found our player
            if (nodePlayer != null)
            {
               // look for each oponent
               foreach (XmlNode opponentNode in nodePlayer.ChildNodes)
               {
                  // if we found one
                  if (opponentNode.Name == "Total")
                  {
                     // add it to the list
                     rc.Add(opponentNode.Attributes["vs"].Value);
                  }
               }
            }
         }

         // return what we found
         return rc;
      }

      /// <summary>
      /// Load the stats values into the list view
      /// </summary>
      /// <param name="OpponentName"></param>
      void LoadStats(string OpponentName)
      {
         // Suspend control drawing until we are done
         StatisticsListView.BeginUpdate();

         // remove everything from the list view
         StatisticsListView.Clear();

         // create the columns
         CreateColumns(OpponentName);

         // add a line for each stat
         // read in options xml
         XmlDataDocument xmlStats = new XmlDataDocument();
         xmlStats.Load(StatisticsCapture.__statisticsDefinitionsFile);

         // get the data file
         IsolatedStorageFileStream store = null;
         try
         {
            store = new IsolatedStorageFileStream(StatisticsCapture.__statisticsDataFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
         }
         catch
         {
            store = null;
         }

         // if we found the data file
         if (store != null)
         {
            // load it
            XmlDataDocument xmlStatsData = new XmlDataDocument();
            xmlStatsData.Load(store);
            store.Close();

            // find the stats definitions
            string s = "/Statistics/Definition/Stat";
            XmlNodeList nodeValues = xmlStats.SelectNodes(s);

            foreach (XmlNode node in nodeValues)
            {
               if (StatisticType.Text != "All Games" && node.Attributes["MinReport"].Value == "All")
               {
                  // skip this stat as it can only displayed at the all games level
               }
               else
               {
                  ListViewItem item = StatisticsListView.Items.Add(node.Attributes["Title"].Value);

                  // get nodes with results against named opponent
                  s = "/Statistics/RawData/Values/Player[@Name='" + _playerName + "']";
                  XmlNode nodePlayer = xmlStatsData.SelectSingleNode(s);

                  if (nodePlayer != null)
                  {
                     // get the formula
                     // eg ([x]+[y])/[z]
                     string valueFormula = node.Attributes["Formula"].Value;

                     // add (0 and ) around []
                     // ((0[x])+(0[y]))/(0[z])
                     valueFormula = valueFormula.Replace("[", "(0[");
                     valueFormula = valueFormula.Replace("]", "])");
                        
                     XmlNodeList nodeResults = nodePlayer.ChildNodes;
                     foreach (XmlNode node2 in nodeResults)
                     {
                        // if right type of child node
                        if ((StatisticType.Text == "All Games" && node2.LocalName == "Total") || (StatisticType.Text != "All Games" && node2.LocalName == "Match"))
                        {
                           // if opponent is to be included
                           if (OpponentName == "All" || OpponentName == node2.Attributes["vs"].Value)
                           {
                              XmlNodeList nodeVals = node2.ChildNodes;

                              // for each variable look up value and instert +xxx after the (0
                              // ((0+1[x])+(0+2[y]))/(0+3[z])
                              foreach (XmlNode nodeVal in nodeVals)
                              {
                                 string variable = "["+nodeVal.Attributes["Name"].Value+"]";
                                 string tempVariable = "{"+nodeVal.Attributes["Name"].Value+"}";
                                 valueFormula = valueFormula.Replace(variable, "+" + nodeVal.InnerText + tempVariable);
                                 valueFormula = valueFormula.Replace(tempVariable, variable);
                              }
                           }
                        }
                     }

                     // remove the []
                     // ((0+1)+(0+2))/(0+3)
                     while (valueFormula.IndexOf("[") != -1)
                     {
                        valueFormula = valueFormula.Substring(0,valueFormula.IndexOf("[")) + valueFormula.Substring(valueFormula.IndexOf("]")+1);
                     }
                     while (valueFormula.IndexOf("{") != -1)
                     {
                        valueFormula = valueFormula.Substring(0,valueFormula.IndexOf("{")) + valueFormula.Substring(valueFormula.IndexOf("}")+1);
                     }

                     // calculate the value
                     int statisticValue = 0;

                     try
                     {
                        statisticValue = Evaluator.EvaluateExpression(valueFormula);
                     }
                     catch
                     {
                     }

                     // add the value
                     item.SubItems.Add(statisticValue.ToString());
                  }
               }
            }
         }

         // make the column big enough
         foreach (ColumnHeader c in StatisticsListView.Columns)
         {
            c.Width = -1;

            // if it became less than 40 at least make it that big
            if (c.Width < 40)
            {
               c.Width = 40;
            }
         }

         // Resume control drawing
         StatisticsListView.EndUpdate();
      }
      #endregion

      #region Event Handlers
      /// <summary>
      /// Form load event
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Statistics_Load(object sender, System.EventArgs e)
      {
         // restore the window position
         Top = (int)Config.TheConfig.GetValue("StatisticsTop", 10);
         Left = (int)Config.TheConfig.GetValue("StatisticsTop", 10);
         if (Top < 0 || Top > Screen.PrimaryScreen.WorkingArea.Height)
         {
            Top = 0;
         }
         if (Left < 0 || Left > Screen.PrimaryScreen.WorkingArea.Width)
         {
            Left = 0;
         }

         // Set window title
         Text = "Statistics for " + _playerName;
         StatisticType.Text = "All Games";
         
         // Set up the player context menu
         PlayerMenu.MenuItems.Add("vs All",new System.EventHandler(PlayerMenuItemClick));
         foreach (string s in OpponentsOf(_playerName))
         {
            PlayerMenu.MenuItems.Add("vs " + s, new System.EventHandler(PlayerMenuItemClick));
         }
      }

      /// <summary>
      /// Handle the Player Menu item selection
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void PlayerMenuItemClick(object sender, System.EventArgs e)
      {
         string OpponentName = ((MenuItem)sender).Text.Substring(3);

         LoadStats(OpponentName);
      }

      /// <summary>
      /// Handle a click event on the print button
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void PrintButton_Click(object sender, System.EventArgs e)
      {
         // Build the test to print
         _printData.printString = "Statistics for " + _playerName + " vs " + StatisticsListView.Columns[0].Text.Substring(8) + " At " + DateTime.Now.ToString() + "\n \n";
         _printData.printString += "Statistic\t\t\tValue\n";
         _printData.printString += "=========\t\t\t=====\n \n";
         foreach (ListViewItem item in StatisticsListView.Items)
         {
            _printData.printString += item.Text + "\t\t\t" + item.SubItems[1].Text + "\n";
         }

         // Start at the beginning
         _printData.currentDataPos = 0;

         // Create a document
         PrintDocument pd = new PrintDocument();
         pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

         // show the print dialog
         PrintDialog pdlg = new PrintDialog();
         pdlg.Document = pd;
         if (pdlg.ShowDialog() == DialogResult.OK)
         {
            // print it
            pd.PrinterSettings = pdlg.PrinterSettings;
            pd.Print();
         }
      }

      /// <summary>
      /// The PrintPage event is raised for each page to be printed.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="ev"></param>
      private void pd_PrintPage(object sender, PrintPageEventArgs ev) 
      {
         Font printFont = new Font("Arial", 10); // Font to print with
         float linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics); // lines we can print
         float leftMargin = ev.MarginBounds.Left; // left margin
         float topMargin = ev.MarginBounds.Top; // top margin
         string line = null; // line to be printed
         int lineCount = 0; // lines printed
         float yPos = 0; // y position to print at

         StringFormat fmt = new StringFormat();
         fmt.SetTabStops(240, new float[] {8,8});

         // Print each line of the file.
         // while we still have space on the page and data to print
         while (lineCount < linesPerPage && _printData.currentDataPos < _printData.printString.Length)
         {
            // Add a line
            line=_printData.printString.Substring(_printData.currentDataPos, _printData.printString.IndexOf("\n", _printData.currentDataPos)-_printData.currentDataPos+1);

            // move our position
            _printData.currentDataPos += line.Length;

            // Move our yPos 
            yPos = topMargin + (lineCount * printFont.GetHeight(ev.Graphics));

            // Print it 
            ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, fmt);

            // increment our line count
            lineCount++;
         }

         // If more lines exist, print another page.
         if(_printData.currentDataPos < _printData.printString.Length)
         {
            ev.HasMorePages = true;
         }
         else
         {
            ev.HasMorePages = false;
         }
      }

      /// <summary>
      /// Item selected changed in combo box
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void StatType_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         // reload the stats
         LoadStats(StatisticsListView.Columns[0].Text.Substring(8));
      }

      /// <summary>
      /// Close the window
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CloseButton_Click(object sender, System.EventArgs e)
      {
         Close();      
      }

      /// <summary>
      /// Window closing
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Statistics_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         //Save the window location
         Config.TheConfig.SetValue("StatisticsTop", Top);
         Config.TheConfig.SetValue("StatisticsLeft", Left);
         Config.TheConfig.Save();
      }
      #endregion
	}
}
