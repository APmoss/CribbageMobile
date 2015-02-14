using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CribbageMobile {
	class Splash : GameScreen {
		// The amount of time in seconds in the splash screen
		const int targetSplashTime = 3;

		TimeSpan elapsed;

		Texture2D background;

		public Splash() {
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void Activate(bool instancePreserved) {
			background = ScreenManager.Game.Content.Load<Texture2D>(@"textures\splash");

			base.Activate(instancePreserved);
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			elapsed += gameTime.ElapsedGameTime;

			if (elapsed.TotalSeconds > targetSplashTime) {
				ExitScreen();
				ScreenManager.AddScreen(new Menus.MainMenu(), null);
			}

			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw(GameTime gameTime) {
			ScreenManager.SpriteBatch.Begin();

			ScreenManager.SpriteBatch.Draw(background, ScreenManager.Game.GraphicsDevice.Viewport.Bounds, Color.White);

			ScreenManager.SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
