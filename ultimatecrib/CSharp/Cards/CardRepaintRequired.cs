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

using System;
using Events;
using System.Drawing;

namespace Cards
{
	/// <summary>
	/// This event is created and fired whenever a card requires painting.
	/// </summary>
	public class CardRepaintRequired : BaseEvent
	{
      #region Member Variables
      DisplayableCard _card; // card requiring repaint
      Rectangle _oldLocationRect; // where the card was
      Rectangle _rectNew; // where the card is
      #endregion

      #region Constructors
      /// <summary>
      /// Creates an event to inform others that a card needs repainting
      /// </summary>
      /// <param name="card">Card requiring repaint</param>
      /// <param name="oldLocationRect">Where it was</param>
      /// <param name="rectNew">Where it is</param>
		public CardRepaintRequired(DisplayableCard card, Rectangle oldLocationRect, Rectangle rectNew) : base()
		{
         _card = card;
         _oldLocationRect = oldLocationRect;
         _rectNew = rectNew;
		}
      #endregion

      #region Properties
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

      /// <summary>
      /// Gets where the card was
      /// </summary>
      public Rectangle Old
      {
         get
         {
            return _oldLocationRect;
         }
      }

      /// <summary>
      /// Gets where the card is now
      /// </summary>
      public Rectangle New
      {
         get
         {
            return _rectNew;
         }
      }
      #endregion
	}
}
