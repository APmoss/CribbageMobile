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

// Code complete & commented 26/9/2003 - KW

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace Cards
{
	/// <summary>
	/// This allows a coder to paint a deck onto a form.
	/// It actually implements almost no logic as it passes everything on to the deck class
	/// </summary>
	public class DeckCtl : System.ComponentModel.Component
	{
      #region Generated Member Variables
      private System.ComponentModel.Container components = null;
      #endregion

      #region Member Variables
      Deck _deck = null; // the deck object
      Card.CardSpecialValue _cardSpecialValueFunction = null; // the special value function to call
      bool _manualDeal = false; // manual deal indicator
      bool _includeJoker = false; // include joker
      #endregion

      #region Constructors
		
      /// <summary>
      /// Create a deck control
      /// </summary>
      /// <param name="container">Contain holding the control</param>
      public DeckCtl(System.ComponentModel.IContainer container)
      {
         // Required for Windows.Forms Class Composition Designer support
         container.Add(this);
         InitializeComponent();
      }

      /// <summary>
      /// Create a deck control
      /// </summary>
      public DeckCtl()
      {
         // Required for Windows.Forms Class Composition Designer support
         InitializeComponent();

         RecreateDeck();
      }
      
      #endregion

      #region Private Member Functions
      /// <summary>
      /// Recreate the deck based on the current properties
      /// </summary>
      void RecreateDeck()
      {
         _deck = new Deck(_cardSpecialValueFunction, _manualDeal, _includeJoker);
      }
      #endregion

		#region Component Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         components = new System.ComponentModel.Container();
      }
		#endregion

      #region Properties
      /// <summary>
      /// Get/Set whether to include a joker in the deck. Setting this property will destroy and recreate the deck.
      /// </summary>
      [Browsable(true), Category("Deck"), DefaultValue(false), Description("Include joker in the deck.")]
      public bool IncludeJoker
      {
         get 
         { 
            return _includeJoker;
         }
         set 
         { 
            _includeJoker = value;

            RecreateDeck();
         }
      }
      /// <summary>
      /// Get/Set whether to deal manually or automatically. Setting this property will destroy and recreate the deck.
      /// </summary>
      [Browsable(true), Category("Deck"), DefaultValue(false), Description("Manually deal cards.")]
      public bool ManualDeal
      {
         get 
         { 
            return _manualDeal;
         }
         set 
         { 
            _manualDeal = value;
            RecreateDeck();
         }
      }

      /// <summary>
      /// Get/Set the card special value function. Setting this property will destroy and recreate the deck.
      /// </summary>
      [Browsable(false), Category("Deck"), DefaultValue(null), Description("Card special value function.")]
      public Card.CardSpecialValue CardSpecialValueFunction
      {
         get 
         { 
            return _cardSpecialValueFunction;
         }
         set 
         { 
            _cardSpecialValueFunction = value;

            RecreateDeck();
         }
      }

      /// <summary>
      /// Get the underlying deck object
      /// </summary>
      public Deck Deck
      {
         get
         {
            return _deck;
         }
      }
      #endregion
   }
}
