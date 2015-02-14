using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace CribbageMobile.Menus {
	class StageSelect : Menu {
		TextButton bronzeLabel = new TextButton("Bronze", new Rectangle(CUSHION, CUSHION, Stcs.Width - CUSHION * 2, 50));

		public StageSelect() : base() {
			bronzeLabel.Tapped += new EventHandler<MenuItemEventArgs>(bronzeLabel_Tapped);
			MenuItems.Add(bronzeLabel);

			EnabledGestures = GestureType.Tap | GestureType.HorizontalDrag;
		}

		public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input) {
			PlayerIndex p;

			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				ExitScreen();
			}
			
			base.HandleInput(gameTime, input);
		}

		void bronzeLabel_Tapped(object sender, MenuItemEventArgs e) {
			ScreenManager.AddScreen(new BronzeLevelSelect(), null);
		}
	}
}
