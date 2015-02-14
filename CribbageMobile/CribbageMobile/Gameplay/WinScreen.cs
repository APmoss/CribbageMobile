using System;
using System.Collections.Generic;
using CribbageMobile.Menus;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices;

namespace CribbageMobile.Gameplay {
	class WinScreen : GameScreen {
		VibrateController v = VibrateController.Default;

		int p1score, p2score;

		List<MenuItem> menuItems = new List<MenuItem>();
		TextButton title = new TextButton("Player 1 Wins!", new Rectangle(5, 15, 470, 50));
		TextButton cont = new TextButton("Tap back to return", new Microsoft.Xna.Framework.Rectangle(5, 745, 470, 50));

		public WinScreen(int p1score, int p2score) {
			EnabledGestures = GestureType.Tap;
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
			IsPopup = true;

			title.BackgroundTint = Color.DarkGreen;
			cont.BackgroundTint = Color.GreenYellow;

			if (p1score > p2score) {
				title.Text = "Player 1 Wins!";
			}
			else {
				title.Text = "Player 2 Wins!";
			}

			menuItems.Add(new TextButton("P1- " + p1score, new Rectangle(5, 100, 230, 50)));
			menuItems.Add(new TextButton("P2- " + p2score, new Rectangle(245, 100, 230, 50)));

			if (MathHelper.Min(p1score, p2score) <= 60) {
				menuItems.Add(new TextButton("Double Skunked!", new Rectangle(5, 160, 470, 50)));
			}
			else if (MathHelper.Min(p1score, p2score) <= 90) {
				menuItems.Add(new TextButton("Skunked!", new Rectangle(5, 160, 470, 50)));
			}

			menuItems.Add(title);
			menuItems.Add(cont);

			v.Start(TimeSpan.FromMilliseconds(200));
		}

		public override void HandleInput(GameTime gameTime, InputState input) {
			PlayerIndex p;
			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				LoadingScreen.Load(ScreenManager, new List<GameScreen>() { new GameBackground(Color.White), new MainMenu() });
			}
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			foreach (var menuItem in menuItems) {
				menuItem.Update(gameTime);
			}

			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw(GameTime gameTime) {
			ScreenManager.SpriteBatch.Begin();
			ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(0, 0, 480, 800), new Color(0, 0, 0, 200));

			foreach (var menuItem in menuItems) {
				menuItem.Draw(gameTime, ScreenManager, TransitionAlpha);
			}
			ScreenManager.SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
