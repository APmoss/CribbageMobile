using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using CribbageMobile.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CribbageMobile.Gameplay {
	class BoardScreen : GameScreen {
		int p1score, p2score;

		public BoardScreen(int p1score, int p2score) {
			EnabledGestures = EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.HorizontalDrag;
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
			IsPopup = true;

			this.p1score = p1score;
			this.p2score = p2score;
		}

		public override void HandleInput(GameTime gameTime, InputState input) {
			PlayerIndex p;
			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				ExitScreen();
			}

			base.HandleInput(gameTime, input);
		}

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
			ScreenManager.SpriteBatch.Begin();

			ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(0, 0, 480, 800), new Color(0, 0, 0, 200));

			for (int i = 1; i <= 121; i++) {
				ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(20, (int)(i * 5.5 + 20), 3, 2), Color.Blue);
				ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(40, (int)(i * 5.5 + 20), 3, 2), Color.Blue);
				ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(460, (int)(i * 5.5 + 20), 3, 2), Color.Blue);
				ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(440, (int)(i * 5.5 + 20), 3, 2), Color.Blue);
			}

			ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(20, (int)(p1score * 5.5 + 20), 23, 3), Color.Red);// * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));
			ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, new Rectangle(440, (int)(p2score * 5.5 + 20), 23, 3), Color.Red);// * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));

			ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "P1- " + p1score + " points", new Vector2(55, (float)(p1score * 5.5 + 20)), Color.White, 0, Vector2.Zero, .4f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
			ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "P2- " + p2score + " points", new Vector2(320, (float)(p1score * 5.5 + 20)), Color.White, 0, Vector2.Zero, .4f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
			
			ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Press back to return", new Vector2(45, 750), Color.White);

			ScreenManager.SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
