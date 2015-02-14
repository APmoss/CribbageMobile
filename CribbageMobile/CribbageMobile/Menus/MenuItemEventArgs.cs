using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace CribbageMobile.Menus {
	/// <summary>
	/// Event args for all menu item interactions
	/// </summary>
	class MenuItemEventArgs : EventArgs {
		private GestureSample gesture;
		private bool isPosOnItem;

		/// <summary>
		/// The gesture applied to the menu item
		/// </summary>
		public GestureSample Gesture {
			get { return gesture; }
			set { gesture = value; }
		}

		public MenuItemEventArgs(GestureSample gesture) {
			this.gesture = gesture;
		}
	}
}
