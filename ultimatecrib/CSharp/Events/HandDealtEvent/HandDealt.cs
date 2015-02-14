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
using Events;

namespace HandDealtEvent
{
	/// <summary>
	/// This event is created and fired whenever a player plays a card.
	/// </summary>
	public class HandDealt : BaseEvent
	{
      #region Member Variables
      #endregion

      #region Constructors
      /// <summary>
      /// Creates an event to inform others that a hand has been dealt
      /// </summary>
		public HandDealt() : base()
		{
		}
      #endregion

      #region Properties
      #endregion
	}
}
