using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace CribbageMobile.Menus {
	class BronzeLevelSelect : LevelSelect {
		public BronzeLevelSelect() : base() {
			//TODO:dont hardwire and make generic tapped that changes levels
			levelButtons[0].Tapped += new EventHandler<MenuItemEventArgs>(BronzeLevelSelect_Tapped);
			
			EnabledGestures = GestureType.Tap | GestureType.HorizontalDrag;
		}

		void BronzeLevelSelect_Tapped(object sender, MenuItemEventArgs e) {
			//LoadingScreen.Load(ScreenManager, )
		}
	}
}
