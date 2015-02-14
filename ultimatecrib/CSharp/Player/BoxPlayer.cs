// Ultimate Cribbage
// Player Assembly

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
using CribCards;
using System.Drawing;
using Cards;
using System.Diagnostics;

namespace Player
{
	/// <summary>
	/// This class represents either a player or the Box
	/// </summary>
	public class BoxPlayer
	{
      #region Member Variables
      protected CribCardList _hand = new CribCardList(); // the box or players hand
      #endregion

      #region Events
      // event fired when a card needs repainting
      public delegate void RepaintRequired(BoxPlayer bp, Rectangle rect); // the callback in the subscriber must have this function definition but may have a different name
      public static event RepaintRequired OnRepaintRequired; // this is the static object that you must add your handler to so it can be called when the event is fired

      /// <summary>
      /// fire the repaint required event
      /// </summary>
      /// <param name="rect">Invalid rectangle</param>
      void FireRepaintRequired(Rectangle rect)
      {
         // if we have anyone listening
         if (OnRepaintRequired != null)
         {
            // fire it telling them which box is involved
            OnRepaintRequired(this, rect);
         }
      }

      #endregion 

      #region Constructor
      public BoxPlayer()
      {
      }
      #endregion

      #region Public Member Functions
      /// <summary>
      /// Returns a flag indicating if this is a crib.
      /// We assume not as most derived classes wont be but the box class will override it with true
      /// </summary>
      public virtual bool IsBox
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Gets the players or cribs cards
      /// </summary>
      public CribCardList Cards
      {
         get
         {
            return _hand;
         }
      }

      /// <summary>
      /// Makes the players cards all face up
      /// </summary>
      public void Show()
      {
         Debug.Assert(_hand.Count > 0, "Why would I be asking the box to be shown when there are no cards?");

         foreach(DisplayableCard card in _hand)
         {
            card.FaceUp = true;
         }
      }

      /// <summary>
      /// Removes a card from the players hand
      /// </summary>
      /// <param name="card">Card to remove</param>
      public void RemoveCard(Card card)
      {
         Debug.Assert(_hand.Contains(card), "Tried to remove a card that is not in the box");
         
         _hand.Remove(card);
         FireRepaintRequired(((DisplayableCard)card).GetRect());
      }

      /// <summary>
      /// Removes all cards from the players hand
      /// </summary>
      public void RemoveAllCards()
      {
         // go through each card in the hand and remove it
         foreach(DisplayableCard card in _hand)
         {
            RemoveCard(card);
         }
      }

      /// <summary>
      /// Add a card to the players hand
      /// </summary>
      /// <param name="card">Card to add</param>
      public void AddCard(DisplayableCard card)
      {
         Debug.Assert(card != null, "Can't add a null card to the box");
         
         // face down and not selected
         card.FaceUp = false;
         card.Selected = false;
         _hand.Add(card);

         // tell anyone interested that we need repainting
         FireRepaintRequired(card.GetRect());
      }

      /// <summary>
      /// Add a set of cards to the players hand
      /// </summary>
      /// <param name="cl">cards to add</param>
      void AddCards(CardList cl)
      {
         Debug.Assert(cl != null, "Can't add a null card list to the box");

         foreach (DisplayableCard card in cl)
         {
            AddCard(card);
         }
      }

      /// <summary>
      /// Evaluate the hand/crib score
      /// </summary>
      /// <param name="starterCard">Starter card to use</param>
      /// <returns>A list of scores</returns>
      public virtual ScoreList EvalScore(Card starterCard)
      {
         Debug.Assert(_hand.Count > 0, "Why would I be asking the score when there are no cards?");
         Debug.Assert(starterCard != null, "To score a box I must have a top card.");

         ScoreList sl = new ScoreList();

         _hand.CountFlushes(starterCard, ref sl, IsBox);

         // now put the top card in the hand and complete the scoring                             
         _hand.Add(starterCard);
      
         // count the various types of scores
         _hand.Count15s(ref sl);
         _hand.CountPairs(ref sl);
         _hand.CountRuns(ref sl);  

         // remove the top card from the hand
         _hand.Remove(starterCard);     

         // now look for the nob in the hand
         if (_hand.Contains(new Card(11, starterCard.Suit)))
         { 
            sl.Add(new Scores(new Card(11, starterCard.Suit).ToString(), 1, Scores.SCORETYPE.KNOB));
         }

         return sl;
      }

      /// <summary>
      /// Gets a string listing all the cards in the hand
      /// </summary>
      public string CardNames
      {
         get
         {
            return _hand.ToString();
         }
      }

      /// <summary>
      /// Show the hand
      /// </summary>
      public void UnhideHand()
      {
         foreach (DisplayableCard card in _hand)
         {
            card.FaceUp = true;
         }
      }
      #endregion
   }
}
