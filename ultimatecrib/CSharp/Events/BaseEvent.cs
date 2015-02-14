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

namespace Events
{
	/// <summary>
	/// This class forms the base for all fired events.
	/// The event model being used is that the firer and listener never need to know each other.
	/// The only knowledge they share is the event. The listener subscribes to the event and the firer
	/// creates and fires the event.
	/// </summary>
	public class BaseEvent
	{
      #region Delegates

      // event listeners
      public static event System.EventHandler OnEvent; 

      #endregion

      #region Public Static Functions
      /// <summary>
      /// Adds a subscriber to the event
      /// </summary>
      /// <param name="e">The hander that will listen to this event</param>
      static public void Subscribe(System.EventHandler e)
      {
         OnEvent += e;
      }

      /// <summary>
      /// Removes a subscriber from the event
      /// </summary>
      /// <param name="e">The handler to remove</param>
      static public void Unsubscribe(System.EventHandler e)
      {
         OnEvent -= e;
      }
      #endregion

      #region Constructor
		public BaseEvent()
		{
		}
      #endregion

      #region Public Member Functions
      /// <summary>
      /// Used to fire the event
      /// </summary>
      public void Fire()
      {
         // If we have any handlers
         if (OnEvent != null)
         {
            // call them
            OnEvent(this, new System.EventArgs());
         }
      }
      #endregion
	}
}
