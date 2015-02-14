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

// Code complete & commented 10/9/2003 - KW

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Cards
{
	/// <summary>
	/// Summary description for ManualDeal.
	/// </summary>
   public class ManualDeal : System.Windows.Forms.Form
   {
      #region Generated Member Variables
      private System.Windows.Forms.Button _okButton;
      private System.Windows.Forms.Label _promptLabel;
      private System.ComponentModel.Container components = null;
      #endregion

      #region Member Variables
      Deck _deck = null; // deck to use for cards
      DisplayableCard _selectedCard = null; // users selected card
      #endregion

      #region Constructors
      /// <summary>
      /// Create the Manual Deal Dialog
      /// </summary>
      /// <param name="deck">Deck to deal from</param>
      /// <param name="why">Why we are dealing the card</param>
      public ManualDeal(Deck deck, string why)
      {
         // save the deck
         _deck = deck;

         // Required for Windows Form Designer support
         InitializeComponent();

         // set the prompt text
         _promptLabel.Text = why;

         // adjust the screen objects for the card display sizes
         Width = 10 + 4 * CardConfig.GetIntValue("CardWidth") + 40 + 10;
         Height = _promptLabel.Height + 20 + CardConfig.GetIntValue("CardHeight") + 12 * CardConfig.GetIntValue("CardVerticalOffset") + _okButton.Height + 5 + 30;
         _okButton.Top = Height - 30 - _okButton.Height;
         _okButton.Left = Width - 10 - _okButton.Width;
      }
      #endregion

      #region IDisposable
      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">Disposing flag</param>
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
      #endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this._okButton = new System.Windows.Forms.Button();
         this._promptLabel = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _okButton
         // 
         this._okButton.Location = new System.Drawing.Point(208, 240);
         this._okButton.Name = "_okButton";
         this._okButton.TabIndex = 0;
         this._okButton.Text = "Ok";
         this._okButton.Click += new System.EventHandler(this._okButton_Click);
         // 
         // _promptLabel
         // 
         this._promptLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this._promptLabel.Dock = System.Windows.Forms.DockStyle.Top;
         this._promptLabel.Name = "_promptLabel";
         this._promptLabel.Size = new System.Drawing.Size(292, 40);
         this._promptLabel.TabIndex = 1;
         // 
         // ManualDeal
         // 
         this.AcceptButton = this._okButton;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(292, 273);
         this.ControlBox = false;
         this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                      this._promptLabel,
                                                                      this._okButton});
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "ManualDeal";
         this.ShowInTaskbar = false;
         this.Text = "Select the card to deal ...";
         this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManualDeal_MouseDown);
         this.Load += new System.EventHandler(this.ManualDeal_Load);
         this.Paint += new System.Windows.Forms.PaintEventHandler(this.ManualDeal_Paint);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Member Functions
      /// <summary>
      /// Validate the window state
      /// </summary>
      void ValidateWindow()
      {
         // if no card is selected
         if (_selectedCard == null)
         {
            // disable ok button
            _okButton.Enabled = false;      
         }
         else
         {
            // enable ok button
            _okButton.Enabled = true;
         }
      }
      #endregion

      #region Event Handlers

      /// <summary>
      /// Paint the form
      /// </summary>
      /// <param name="sender">Object sending the event</param>
      /// <param name="e">Event params</param>
      private void ManualDeal_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         // set starting paint x location
         int x = 10;

         // for each suit
         for (int i = 0; i < 4; i++)
         {

            // set start paint y location
            int y = _promptLabel.Height + 10;

            // for each card in the suit
            for (int j = 0; j < 13; j++)
            {
               // get the card
               DisplayableCard card = ((DisplayableCard)_deck[j+i*13]);

               // if it has not been dealt
               if (card.Dealt == false)
               {
                  // Mark the card as dealt
                  card.Deal();

                  // Make it face ip
                  card.FaceUp = true;

                  // Paint it
                  card.Display(e.Graphics, x, y, DisplayableCard.ORIENTATION.VERTICAL);

                  // Put it face down again
                  card.FaceUp = false;

                  // UNdeal it
                  card.Undeal();
               }

               // move display down 1 card
               y = y + CardConfig.GetIntValue("CardVerticalOffset");
            }

            // move display across 1 suit
            x = x + CardConfig.GetIntValue("CardWidth") + 10;
         }
      }

      /// <summary>
      /// Handle ok button click event
      /// </summary>
      /// <param name="sender">Object sending the event</param>
      /// <param name="e">Event params</param>
      private void _okButton_Click(object sender, System.EventArgs e)
      {
         // Set the result
         DialogResult = DialogResult.OK;

         // Close the form
         Close();
      }

      /// <summary>
      /// Prepare the dialog for display
      /// </summary>
      /// <param name="sender">Object sending the event</param>
      /// <param name="e">Event params</param>
      private void ManualDeal_Load(object sender, System.EventArgs e)
      {
         // Validate the window
         ValidateWindow();
      }

      /// <summary>
      /// Handle a click on the form
      /// </summary>
      /// <param name="sender">Object sending the event</param>
      /// <param name="e">Event params</param>
      private void ManualDeal_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         // assume it was not on a card
         bool found = false;
         bool handled = false;

         // go through each card in the deck backwards
         // it has to be backwards as the higher cards obscure the lower cards
         for (int i = _deck.Count-1; i >= 0; i--)
         {
            // if the card is not dealt then we might have clicked on it
            if (!_deck[i].Dealt)
            {
               // If we hit this card
               if (!handled && ((DisplayableCard)_deck[i]).IsHit(new Point(e.X, e.Y)))
               {
                  // toggle the selected state
                  ((DisplayableCard)_deck[i]).ToggleSelect();

                  // if it is now selected
                  if (((DisplayableCard)_deck[i]).Selected)
                  {
                     // save it
                     _selectedCard = ((DisplayableCard)_deck[i]);

                     // we found it
                     found = true;
                  }

                  // we have handled the click but we still need to process the 
                  // cards to make sure no others are selected
                  handled = true;
               }
               else
               {
                  // Make sure this card is unselected
                  ((DisplayableCard)_deck[i]).Selected = false;
               }
            }
         }

         // if no card was clicked on and selected
         if (!found)
         {
            // save the selected card
            _selectedCard = null;
         }

         // validate the window
         ValidateWindow();

         // redraw
         Invalidate();
      }
      #endregion

      #region Public Member Functions
      /// <summary>
      /// Get the card the user selected
      /// </summary>
      public DisplayableCard SelectedCard
      {
         get
         {
            return _selectedCard;
         }
      }
      #endregion

   }
}
