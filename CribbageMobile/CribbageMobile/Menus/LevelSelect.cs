using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace CribbageMobile.Menus {
	class LevelSelect : Menu {
		protected List<TextButton> levelButtons = new List<TextButton>();

		public LevelSelect() : base() {
			for (int i = 0; i < 50; i++) {
				//TODO: actually organize buttons
				Rectangle bounds = new Rectangle();
				if (i == 0) {
					bounds = new Rectangle(10, 10, 50, 50);
				}
				else {
					bounds = new Rectangle(70, 10, 50, 50);
				}

				TextButton levelButton = new TextButton("" + (i + 1), bounds);

				levelButtons.Add(levelButton);
			}

			foreach (var levelButton in levelButtons) {
				MenuItems.Add(levelButton);
			}
			
			EnabledGestures = GestureType.Tap | GestureType.HorizontalDrag;
		}

		public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input) {
			PlayerIndex p;

			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				ExitScreen();
			}
			
			base.HandleInput(gameTime, input);
		}
	}
}
