// Ultimate Cribbage
// CribCards Assembly

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
using Cards;

namespace CribCards
{
	/// <summary>
	/// This class represents a crib card
	/// </summary>
   public class CribCard
   {
      #region Constructors
      /// <summary>
      /// Static constructor installs the delegate function
      /// </summary>
      static CribCard()
      {
         Card.OnCardSpecialValue = new Card.CardSpecialValue(CribCardValue);
      }
      #endregion

      #region Public Static Functions
      /// <summary>
      /// Delegate function provides the special value ability for cribbage cards
      /// whereby cards over 10 have a value of 10
      /// </summary>
      /// <param name="card">Card to get the special value for</param>
      /// <returns>The cards special value</returns>
      public static int CribCardValue(Card card)
      {
         if (card.FaceValue >= 10)
         {
            return 10;
         }
         else
         {
            return card.FaceValue;
         }
      }

      /// <summary>
      /// Initialises cribbage cards
      /// </summary>
      public static void Initialise()
      {
      }
      #endregion
	}
}
