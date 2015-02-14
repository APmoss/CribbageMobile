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

// Code complete & commented 9/9/2003 - KW

using System;
using System.Drawing;

namespace Cards
{
	/// <summary>
	/// Summary description for this.
	/// </summary>
   public class CardList : CardCollection, ICloneable
   {
      #region Member Variables
      
      protected string _sortOrder; // name of the current sort sequence
      
      #endregion

      #region Constructors
      /// <summary>
      /// Create a card list with default sort order
      /// </summary>
      public CardList()
      {
         _sortOrder = CardConfig.GetStringValue("CardSort");
      }

      /// <summary>
      /// Create a card list with the named sort order
      /// </summary>
      /// <param name="sortOrder">Sort order to use</param>
      public CardList(string sortOrder)
      {
         _sortOrder = sortOrder;
      }
      #endregion

      #region IClonable
      /// <summary>
      /// Clone the card list
      /// </summary>
      /// <returns>Returns a copy of the card list</returns>
      public override object Clone()
      {
         CardList rc = new CardList(SortOrder);
         rc.AddRange(this);

         return rc;
      }
      #endregion

      #region Private Member Functions

      /// <summary>
      /// Sort the crad list in the current sort order
      /// </summary>
      public void Sort()
      {
         // start off assuming it is not sorted
         bool sorted = false;

         // As card lists are typically very very small we will use a bubble sort

         // process according to the sort order
         switch(SortOrder)
         {
            case "Unsorted":
               // do nothing
               break;
            case "Ascending":
               // sort by face value up
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if (this[i] > this[i+1])
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Ascending special value":
               // sort by face value up
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if (this[i].SpecialValue > this[i+1].SpecialValue)
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Descending":
               // sort by face value down
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if (this[i] < this[i+1])
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Descending special value":
               // sort by face value down
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if (this[i].SpecialValue < this[i+1].SpecialValue)
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Ascending within suit":
               // sort by suit and then face value up
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if ((this[i].Suit > this[i+1].Suit) || 
                        ((this[i].Suit == this[i+1].Suit) &&
                        (this[i].FaceValue > this[i+1].FaceValue)))
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Ascending special value within suit":
               // sort by suit and then face value up
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if ((this[i].Suit > this[i+1].Suit) || 
                        ((this[i].Suit == this[i+1].Suit) &&
                        (this[i].SpecialValue > this[i+1].SpecialValue)))
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Descending within suit":
               // sort by suit and then face value down
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if ((this[i].Suit > this[i+1].Suit) || 
                        ((this[i].Suit == this[i+1].Suit) &&
                        (this[i].FaceValue < this[i+1].FaceValue)))
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            case "Descending special value within suit":
               // sort by suit and then face value down
               while (!sorted)
               {
                  sorted = true;
                  for (int i=0; i < Count-1; i++)
                  {
                     if ((this[i].Suit > this[i+1].Suit) || 
                        ((this[i].Suit == this[i+1].Suit) &&
                        (this[i].SpecialValue < this[i+1].SpecialValue)))
                     {
                        Card cardTemp = this[i];
                        this[i] = this[i+1];
                        this[i+1] = cardTemp;
                        sorted = false;
                     }
                  }
               }
               break;
            default:
               throw new ApplicationException("Unrecognised Sort Order");
         }
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// Add a card if it does not already exist in the right sort order
      /// </summary>
      /// <param name="card">Card to add</param>
      public void AddCardAndSort(Card card)
      {
         if (!this.Contains(card))
         {
            base.Add(card);
            Sort();
         }
      }

      /// <summary>
      /// Remove a list of cards from this card list
      /// </summary>
      /// <param name="cards">List of card to remove</param>
      public void Remove(CardList cards)
      {
         // process each card
         foreach (Card card in cards)
         {
            // remove it
            Remove(card);
         }
      }

      /// <summary>
      /// Add a list of cards in the right sort order
      /// </summary>
      /// <param name="cards">List of cards to add</param>
      public void Add(CardList cards)
      {
         base.AddRange(cards);
         Sort();
      }

      /// <summary>
      /// Gets or sets the sort order of the cards
      /// </summary>
      public string SortOrder
      {
         get
         {
            return _sortOrder;
         }
         set
         {
            // save the new value
            _sortOrder = value;

            // resort
            Sort();
         }
      }

      /// <summary>
      /// Display the card list
      /// </summary>
      /// <param name="graphics">Device on which to paint the cards</param>
      /// <param name="x">X offset to paint them</param>
      /// <param name="y">Y offset to paint them</param>
      /// <param name="orientation">Paint them up or down</param>
      public void Display(Graphics graphics, int x, int y, DisplayableCard.ORIENTATION orientation)
      {
         // initialise the starting position
         int currX = x;
         int currY = x;
      
         // process each card
         foreach (DisplayableCard card in this)
         {
            // display the card
            card.Display(graphics, currX, currY, orientation);

            // depending on orientation increment the next card position
            if (orientation == DisplayableCard.ORIENTATION.HORIZONTAL)
            {
               currX = currX + CardConfig.GetIntValue("CardHorizontalOffset");
            }
            else
            {
               currY = currY + CardConfig.GetIntValue("CardVerticalOffset");
            }
         }
      }

      /// <summary>
      /// Get a string representation of the card in the list
      /// </summary>
      /// <returns>All the names of the cards in the list</returns>
      public override string ToString()
      {
         string rc = "";

         // get each card name
         foreach (Card card in this)
         {
            rc = rc + card.ToString() + " ";
         }  

         return rc;
      }

      #endregion

	}
}
