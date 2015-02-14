// Ultimate Cribbage
// Events Assembly

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
using Cards;
using Events;

namespace Player
{
	/// <summary>
	/// This event is created and fired whenever a player plays a card.
	/// </summary>
	public class CardPlayedToTable : BaseEvent
	{
      #region Member Variables
      CribbagePlayer _player = null; // player who played the card
      Card _card = null; // card played
      #endregion

      #region Constructors
      /// <summary>
      /// Creates an event to inform others that a card has been played
      /// </summary>
      /// <param name="player">Player playing the card</param>
      /// <param name="card">Card played</param>
		public CardPlayedToTable(CribbagePlayer player, Card card) : base()
		{
         _player = player;
         _card = card;
		}
      #endregion

      #region Properties
      /// <summary>
      /// Gets the player who played the card
      /// </summary>
      public CribbagePlayer Player
      {
         get
         {
            return _player;
         }
      }

      /// <summary>
      /// Gets the played card
      /// </summary>
      public Card Card
      {
         get
         {
            return _card;
         }
      }
      #endregion
	}
}
