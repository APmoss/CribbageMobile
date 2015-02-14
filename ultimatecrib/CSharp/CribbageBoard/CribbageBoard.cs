// Ultimate Cribbage
// CribbageBoard Assembly

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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace CribbageBoard
{
   /// <summary>
   /// Possible play up to scores
   /// </summary>
   public enum MAXSCORE {SIXTY=61, ONETWENTY=121};

   /// <summary>
   /// Possible display locations
   /// </summary>
   public enum IMAGELOCATION {TOPLEFT, FILL, CENTRE, RESIZECONTROL};

   #region CribbageBoard
   /// <summary>
	/// This is a control which displays a cribbage board.
	/// </summary>
   [ToolboxBitmap(typeof(PictureBox), "CribbageBoard.bmp")]
   [System.Runtime.InteropServices.ComVisible(false)]
   public class CribbageBoard : UserControl
	{
      #region Component Designed generated member variables
      private System.ComponentModel.IContainer components;
      #endregion

      #region Member Variables
      IMAGELOCATION _imageLocation = IMAGELOCATION.TOPLEFT; // where on the control to draw the board
      Image _board = null; // the board bitmap
      Image _rotatedBoard = null; // the rotated board
      BoardPlayer _player1 = new BoardPlayer(1); // player 1
      BoardPlayer _player2 = new BoardPlayer(2); // player 2
      string _xmlFile = "board.xml"; // the xml file to find the board in
      string _boardName = "Basic"; // the name of the board to load from the xml file
      private System.Windows.Forms.ContextMenu contextMenu1;
      private System.Windows.Forms.MenuItem menuRotate;
      private System.Windows.Forms.MenuItem menuItem2; // the name of the board in the xml file
      Size _offset = new Size(0,0); // the offset from the top left of the control to draw the board
      bool _rotated = false; // indicates if board is rotated through 90 degrees
      bool _allowUserRotate = true; // indiates if user is allowed to rotate board
      bool _allowUserBoardChange = true; // indicates if user is allowed to select another board from the xml file
      private System.Windows.Forms.ToolTip toolTip; // indicates if user is allowed to change the board
      bool _showToolTip = false; // controls showing of score tooltip
      #endregion

      #region Events

      /// <summary>
      /// Delegate definition for how to receive a board name changed event
      /// </summary>
      public delegate void BoardChangedEventHandler(object sender, System.EventArgs e);   // delegate declaration

      /// <summary>
      /// Add your handler here to receive notifications that the board name has been changed
      /// </summary>
      public event BoardChangedEventHandler OnBoardChanged;

      /// <summary>
      /// Used to fire the board name changed event
      /// </summary>
      void FireBoardChanged()
      {
         if (OnBoardChanged != null)
         {
            OnBoardChanged(this, new System.EventArgs());
         }
      }

      /// <summary>
      /// Delegate definition for how to receive a rotated changed event
      /// </summary>
      public delegate void RotatedChangedEventHandler(object sender, System.EventArgs e);   // delegate declaration

      /// <summary>
      /// Add your handler here to receive notifications that the board rotation has been changed
      /// </summary>
      public event RotatedChangedEventHandler OnRotatedChanged;

      /// <summary>
      /// Used to fire the rotated changed event
      /// </summary>
      void FireRotatedChanged()
      {
         if (OnRotatedChanged != null)
         {
            OnRotatedChanged(this, new System.EventArgs());
         }
      }

      #endregion

      #region Constructors
		public CribbageBoard()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}
      #endregion

      #region IDisposable
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}
      #endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         this.contextMenu1 = new System.Windows.Forms.ContextMenu();
         this.menuRotate = new System.Windows.Forms.MenuItem();
         this.menuItem2 = new System.Windows.Forms.MenuItem();
         this.toolTip = new System.Windows.Forms.ToolTip(this.components);
         // 
         // contextMenu1
         // 
         this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.menuRotate,
                                                                                     this.menuItem2});
         this.contextMenu1.Popup += new System.EventHandler(this.menuRotate_Popup);
         // 
         // menuRotate
         // 
         this.menuRotate.Index = 0;
         this.menuRotate.Text = "Rotate";
         this.menuRotate.Click += new System.EventHandler(this.menuRotate_Click);
         // 
         // menuItem2
         // 
         this.menuItem2.Index = 1;
         this.menuItem2.Text = "-";
         // 
         // toolTip
         // 
         this.toolTip.Active = false;
         // 
         // CribbageBoard
         // 
         this.ContextMenu = this.contextMenu1;
         this.DockPadding.All = 5;
         this.Name = "CribbageBoard";
         this.Size = new System.Drawing.Size(216, 72);
         this.Resize += new System.EventHandler(this.CribbageBoard_Resize);
         this.Load += new System.EventHandler(this.CribbageBoard_Load);
         this.SizeChanged += new System.EventHandler(this.CribbageBoard_SizeChanged);
         this.Paint += new System.Windows.Forms.PaintEventHandler(this.CribbageBoard_Paint);
      }
		#endregion

      #region Private Methods

      /// <summary>
      /// Controls the visibility and text of the tooltip
      /// </summary>
      void SetToolTip()
      {
         // if we are to show tool tips
         if (_showToolTip)
         {
            // update the tip text
            toolTip.SetToolTip(this, ToString());

            // activate the tip
            toolTip.Active = true;
         }
         else
         {
            // deactive the tip
            toolTip.Active = false;
         }
      }

      /// <summary>
      /// Used to set the counter offset and stretch attributes so they are drawn like the board
      /// </summary>
      void SetBaseCounterOffset()
      {
         // no point doing this until everything is valid
         if (IsValid)
         {
            // initialise them to the do nothing values
            int x = 0;
            int y = 0;
            float w = 1;
            float h = 1;

            // strecth and offset is based on the image location
            switch(_imageLocation)
            {
               case IMAGELOCATION.CENTRE:
                  // no stretching but the board is not in top left hand corner
                  x = Width/2 - _board.Width/2;
                  y = Height/2 - _board.Height/2;
                  break;
               case IMAGELOCATION.FILL:
                  // board is in top left hand corner but it may be distorted due to the fill.
                  w = (float)Width / (float)_board.Width;
                  h = (float)Height / (float)_board.Height;
                  break;
               case IMAGELOCATION.RESIZECONTROL:
                  // no streching but board is offset by the provided offset value
                  x = _offset.Width;
                  y = _offset.Height;
                  break;
               case IMAGELOCATION.TOPLEFT:
                  // no streching but board is offset by the provided offset value
                  x = _offset.Width;
                  y = _offset.Height;
                  break;
            }

            // tell the players so they can raw their counters correctly
            _player1.Offset = new Size(x,y);
            _player2.Offset = new Size(x,y);
            _player1.Scale = new SizeF(w,h);
            _player2.Scale = new SizeF(w,h);
            _player1.BoardSize = _board.Size;
            _player2.BoardSize = _board.Size;
         }
      }

      
      /// <summary>
      /// This method is used to resize the control to the size of the baord bitmap.
      /// </summary>
      void ResizeToBoard()
      {
         // no point doing this until everything is valid
         if (IsValid)
         {
            if (_rotated)
            {
               Debug.Assert(_rotatedBoard != null);
               
               // resize the control to match the bitmap
               Width = _rotatedBoard.Width + _offset.Width + DockPadding.Left + DockPadding.Right;
               Height = _rotatedBoard.Height + _offset.Height + DockPadding.Top + DockPadding.Bottom;
            }
            else
            {
               // resize the control to match the bitmap
               Width = _board.Width + _offset.Width;
               Height = _board.Height + _offset.Height;
            }
         }
      }

      /// <summary>
      /// This method is used to load the board settings from the XML file
      /// </summary>
      void LoadBoard()
      {
         // clear out previous board settings
         _board = null;
         _rotatedBoard = null;

         XmlDataDocument doc; // xml document
         try
         {
            // open the XML file
            doc = new XmlDataDocument();
            doc.Load(_xmlFile);
         }
         catch(Exception e)
         {
            Debug.WriteLine("Could not load board xml document: "+ e.Message);
            return;
         }

         XmlNode nodeBoard; // the board xml node
         
         // work out the board node name
         string findBoard = "Boards/Board[@Name=\"" + _boardName + "\"]";

         try
         {
            // locate the board
            nodeBoard = doc.SelectSingleNode(findBoard);

            if (nodeBoard == null)
            {
               Debug.WriteLine("Could not find the nominated board: " + _boardName);
               return;
            }
         }
         catch (Exception e)
         {
            Debug.WriteLine("Could not find the nominated board: " + e.Message);
            return;
         }

         // load the board bitmap
         try
         {
            _board = Bitmap.FromFile(nodeBoard.Attributes.GetNamedItem("Bitmap").Value);

            // rotate the board
            _rotatedBoard = (Image)_board.Clone();
            _rotatedBoard.RotateFlip(RotateFlipType.Rotate90FlipNone);
         }
         catch(Exception e)
         {
            Debug.WriteLine("Could not load board bitmap: " + e.Message);
            return;
         }

         // if we are to resize control then do this now
         if (_imageLocation == IMAGELOCATION.RESIZECONTROL)
         {
            ResizeToBoard();
         }

         // initialise each player settings
         XmlNode nodePlayer = doc.SelectSingleNode(findBoard + "/Player[@Number=1]");
         if (nodePlayer != null)
         {
            _player1.LoadSettings(nodePlayer);
         
            nodePlayer = doc.SelectSingleNode(findBoard + "/Player[@Number=2]");
            if (nodePlayer != null)
            {
               _player2.LoadSettings(nodePlayer);
            }
         }

         // set the base counter offset if it all loaded ok
         SetBaseCounterOffset();

         // repaint
         Invalidate();
      }

      #endregion

      #region Windows Event Handlers

      /// <summary>
      /// Handles clicking of the rotate menu item
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void menuRotate_Click(object sender, System.EventArgs e)
      {
         menuRotate.Checked = !menuRotate.Checked;
         Rotated = menuRotate.Checked;
      }

      /// <summary>
      /// Adds the board names to the popup menu when displayed
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void menuRotate_Popup(object sender, System.EventArgs e)
      {
         // if they are not allowed to rotate then hide rotate and hide seperator
         if (!_allowUserRotate)
         {
            menuRotate.Visible = false;
            menuItem2.Visible = false;
         }

         // remove eveything below the seperator

         // have not found the seperator yet
         bool pastSeperator = false;

         // go throug each item
         for (int i = 0; i < contextMenu1.MenuItems.Count; i++)
         {
            // if we have already seen the seperator
            if (pastSeperator)
            {
               // remove this item
               contextMenu1.MenuItems.RemoveAt(i);

               // go back one as we removed the current one
               i--;
            }
            else
            {
               // if the current item is a seperator
               if (contextMenu1.MenuItems[i].Text == "-")
               {
                  // we have seen the seperator
                  pastSeperator = true;
               }
            }
         }

         // if they are not allowed to board change then hide the seperator
         if (!_allowUserBoardChange)
         {
            menuItem2.Visible = false;
         }
         else
         {
            // if they can also rotate then show the seperator
            if (_allowUserRotate)
            {
               menuItem2.Visible = true;
            }

            // for each board in the XML file
            foreach (string s in Boards)
            {
               // add a menu item for the board name
               MenuItem menuItem = contextMenu1.MenuItems.Add(s, new EventHandler(OnBoardName));

               // if this is the current board then put a check mark against it
               if (s == BoardName)
               {
                  menuItem.Checked = true;
               }
            }
         }
      }

      /// <summary>
      /// Handles the selection of a board name
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void OnBoardName(object sender, System.EventArgs e)
      {
         BoardName = ((MenuItem)sender).Text;
      }

      /// <summary>
      /// Load the cribbage board
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CribbageBoard_Load(object sender, System.EventArgs e)
      {
         // call the load board
         LoadBoard();
      }

      /// <summary>
      /// Paint the cribbage board
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CribbageBoard_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         // only paint if we are valid
         if (IsValid)
         {
            // work out which board to paint
            Image paintBoard = null;
            if (_rotated)
            {
               paintBoard = _rotatedBoard;
            }
            else
            {
               paintBoard = _board;
            }

            // how to paint varies by location
            switch(_imageLocation)
            {
               case IMAGELOCATION.CENTRE:
                  e.Graphics.DrawImageUnscaled(paintBoard, Width/2 - _board.Width/2, Height/2 - _board.Height/2);
                  break;
               case IMAGELOCATION.FILL:
                  e.Graphics.DrawImage(paintBoard, 0 + _offset.Width,0 + _offset.Height, Width, Height);
                  break;
               case IMAGELOCATION.RESIZECONTROL:
                  e.Graphics.DrawImageUnscaled(paintBoard, 0 + _offset.Width, 0 + _offset.Height);
                  break;
               case IMAGELOCATION.TOPLEFT:
                  e.Graphics.DrawImageUnscaled(paintBoard, 0 + _offset.Width, 0 + _offset.Height);
                  break;
            }

            // now paint the players counters
            _player1.Paint(e.Graphics);
            _player2.Paint(e.Graphics);
         }
      }

      /// <summary>
      /// This is called when the cribbage board control is resized
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CribbageBoard_Resize(object sender, System.EventArgs e)
      {
         // if we mean the control to be the same as the bitmap the return it to original size
         if (_imageLocation == IMAGELOCATION.RESIZECONTROL)
         {
            ResizeToBoard();
         }

         // repaint
         Invalidate();
      }

      private void CribbageBoard_SizeChanged(object sender, System.EventArgs e)
      {
         // if we mean the control to be the same as the bitmap the return it to original size
         if (_imageLocation == IMAGELOCATION.RESIZECONTROL)
         {
            ResizeToBoard();
         }

         // repaint
         Invalidate();
      }

      #endregion

      #region Cribbage User Modifiable Properties

      /// <summary>
      /// Controls whether to display player score tooltip
      /// </summary>
      [Browsable(true), Category("Cribbage"), DefaultValue(false), Description("Inidcates if the players scores should be displayed in a tooltip.")]
      public bool ShowToolTip
      {
         get 
         { 
            return _showToolTip; 
         }
         set 
         { 
            _showToolTip = value;

            // do the actual tip activation/deactivation
            SetToolTip();
         }
      }

      /// <summary>
      /// Controls whether rotate is on the context menu
      /// </summary>
      [Browsable(true), Category("Cribbage"), DefaultValue(true), Description("Inidcates if the user is allowed to rotate the board.")]
      public bool AllowUserRotate
      {
         get 
         { 
            return _allowUserRotate; 
         }
         set 
         { 
            _allowUserRotate = value;
         }
      }

      /// <summary>
      /// Controls whether board names are on the context menu
      /// </summary>
      [Browsable(true), Category("Cribbage"), DefaultValue(true), Description("Inidcates if the user is allowed to change the board.")]
      public bool AllowUserBoardChange
      {
         get 
         { 
            return _allowUserBoardChange; 
         }
         set 
         { 
            _allowUserBoardChange = value;
         }
      }

      /// <summary>
      /// Set the offset from top left of the control to paint the cribbage board
      /// </summary>
      [Browsable(true), Category("Appearance"), Description("Offset from top left corner of control where board should be painted.")]
      public Size Offset
      {
         get
         {
            return _offset;
         }
         set
         {
            _offset = value;
         }
      }

      /// <summary>
      /// Set the image location
      /// </summary>
      [Browsable(true), Category("Appearance"), DefaultValue(IMAGELOCATION.TOPLEFT), Description("Describes how to paint the image in the control.")]
      public IMAGELOCATION ImageLocation
      {
         get 
         { 
            return _imageLocation; 
         }
         set 
         { 
            _imageLocation = value; 

            // force the resize to board if appropriate
            if (_imageLocation == IMAGELOCATION.RESIZECONTROL)
            {
               ResizeToBoard();
            }

            // recalculate where the counters go
            SetBaseCounterOffset();

            Invalidate();
         }
      }

      /// <summary>
      /// Gets/sets the name of the board
      /// </summary>
      [Browsable(true), Category("Cribbage"), DefaultValue("Basic"), Description("The name of the board within the XML file.")]
      public string BoardName
      {
         get 
         { 
            return _boardName; 
         }
         set 
         { 
            // only do anything if the name is different
            if (_boardName != value)
            {
               _boardName = value;

               // relead the board
               LoadBoard();

               // tell anyone interested the board changed
               FireBoardChanged();
            }
         }
      }

      /// <summary>
      /// Sets/gets the maximum score in the game
      /// </summary>
      [Browsable(true), Category("Cribbage"), DefaultValue(121), Description("The score at which the cribbage game will finish.")]
      public MAXSCORE MaxScore
      {
         get 
         { 
            // get it from one of the players
            return _player1.MaxScore; 
         }
         set 
         { 
            // tell both players the maximum score
            _player1.MaxScore = value;
            _player2.MaxScore = value;
         }
      }

      [Browsable(true), Category("Cribbage"), DefaultValue(false), Description("If true rotates the board 90 degrees.")]
      public bool Rotated
      {
         get 
         { 
            // get it from one of the players
            return _rotated; 
         }
         set 
         { 
            // only do anything if the rotation has changed
            if (_rotated != value)
            {
               // tell both players the maximum score
               _rotated = value;

               // tell our players we have rotated
               _player1.Rotated = _rotated;
               _player2.Rotated = _rotated;

               // change our size to reflect rotation
               System.Drawing.Size current = this.Size;
               Width = current.Height;
               Height = current.Width;

               // redraw
               Invalidate();

               // tell anyone interested we rotated the board
               FireRotatedChanged();

            }
         }
      }

      /// <summary>
      /// Sets/gets the board xml file
      /// </summary>
      [Browsable(true), Category("Cribbage"), DefaultValue("board.xml"), Description("The XML file containing the board definitions.")]
      public string CribbageBoardXMLFile
      {
         get 
         { 
            return _xmlFile; 
         }
         set 
         { 
            _xmlFile = value;

            // now load it
            LoadBoard();
         }
      }
      #endregion

      #region Public Methods

      /// <summary>
      /// Returns a string representing the current player scores displayed on the board
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         return _player1.Name + ": " + _player1.Score.ToString(System.Globalization.CultureInfo.CurrentUICulture.NumberFormat) + "\n" + _player2.Name + ": " + _player2.Score.ToString(System.Globalization.CultureInfo.CurrentUICulture.NumberFormat);
      }
      
      /// <summary>
      /// Check if we are valid and ready to use
      /// </summary>
      public bool IsValid
      {
         get
         {
            // check player 1
            if (!_player1.IsValid)
            {
               return false;
            }

            // check player 2
            if (!_player2.IsValid)
            {
               return false;
            }

            // check we have a board bitmap
            if (_board == null)
            {
               return false;
            }

            return true;
         }
      }

      /// <summary>
      /// Adds points to the nominated players score
      /// </summary>
      /// <param name="player">Player to add the points to</param>
      /// <param name="score">Points to add</param>
      /// <returns>New score</returns>
      public int AddToScore(int player, int score)
      {
         int rc = 0; // new score

         // switch based on player and add the points
         switch (player)
         {
            case 1:
               rc = _player1.AddToScore(score);
               break;
            case 2:
               rc = _player2.AddToScore(score);
               break;
         }

         // update the tooltip
         SetToolTip();

         // repaint
         Invalidate();

         // return the new score
         return rc;
      }

      /// <summary>
      /// Sets a player to a specific score and last score
      /// </summary>
      /// <param name="player">player to set</param>
      /// <param name="lastScore">Last Score Value</param>
      /// <param name="score">New Score Value</param>
      public void SetScore(int player, int lastScore, int score)
      {
         // switch based on player and set the scores
         switch (player)
         {
            case 1:
               _player1.Score = score;
               _player1.LastScore = lastScore;
               break;
            case 2:
               _player2.Score = score;
               _player2.LastScore = lastScore;
               break;
         }

         // Update the tool tip
         SetToolTip();

         // repaint
         Invalidate();
      }

      /// <summary>
      /// Gets a players score
      /// </summary>
      /// <param name="player">Number of player to get the score for</param>
      /// <returns>Players score</returns>
      public int GetPlayerScore(int player)
      {
         // switch based on player and set the scores
         switch (player)
         {
            case 1:
               return _player1.Score;
               break;
            case 2:
               return _player2.Score;
               break;
            default:
               Trace.Fail("Invalid player id");
               return 0;
               break;
         }
      }

      /// <summary>
      /// Set the players name
      /// </summary>
      /// <param name="player"></param>
      /// <param name="name"></param>
      public void SetPlayerName(int player, string name)
      {
         switch (player)
         {
            case 1:
               _player1.Name = name;
               break;
            case 2:
               _player2.Name = name;
               break;
         }

         // update the tooltip
         SetToolTip();
      }

      /// <summary>
      /// Get a list of boards in the XML file
      /// </summary>
      public StringCollection Boards
      {
         get
         {
            StringCollection rc = new StringCollection();

            try
            {
               // open the XML file
               XmlDataDocument doc = new XmlDataDocument();
               doc.Load(_xmlFile);

               // find the boards node
               XmlNode nodeBoards = doc.SelectSingleNode("Boards");

               // got though each of the boards
               foreach(XmlNode nodeBoard in nodeBoards.ChildNodes)
               {
                  if (nodeBoard.Name == "Board")
                  {
                     // add the name to our list
                     rc.Add(nodeBoard.Attributes["Name"].Value);
                  }
               }
            }
            catch (Exception e)
            {
               Debug.WriteLine("Error reading available boards: " + e.Message);
            }

            return rc;
         }
      }
      #endregion
	}
   #endregion

}
