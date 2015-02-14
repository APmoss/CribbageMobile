using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace CribbageMobile.Menus {
	class LoadingScreen : GameScreen {
		List<GameScreen> screensToLoad = new List<GameScreen>();

		public LoadingScreen(List<GameScreen> screensToLoad) {
			this.screensToLoad = screensToLoad;

			TransitionOffTime = TimeSpan.FromSeconds(.5);
			TransitionOnTime = TimeSpan.FromSeconds(.5);
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			// If there are screens still transitioning off, then we don't want to continue
			if (ScreenManager.GetScreens().Length <= 1) {
				// Exit this loading screen
				ExitScreen();

				// Add all screens to be added
				foreach (var screen in screensToLoad) {
					ScreenManager.AddScreen(screen, null);
				}

				// Reset the game's elapsed time
				ScreenManager.Game.ResetElapsedTime();
			}

			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw(GameTime gameTime) {
			Rectangle viewport = ScreenManager.Game.GraphicsDevice.Viewport.Bounds;

			ScreenManager.SpriteBatch.Begin();

			ScreenManager.SpriteBatch.Draw(ScreenManager.BlankTexture, viewport, Color.Black * TransitionAlpha);
			ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Loading and stuff", new Vector2(10, 10), Color.White);

			ScreenManager.SpriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Used when loading large screens to exit all screens currently open,
		/// add a loading screen, and to launch all loaded screens.
		/// </summary>
		/// <param name="screenManager"></param>
		/// <param name="screensToLoad"></param>
		public static void Load(ScreenManager screenManager, List<GameScreen> screensToLoad) {
			// Exit all screen that are open
			screenManager.ExitAllScreens();

			// Add the loading screen
			screenManager.AddScreen(new LoadingScreen(screensToLoad), null);
		}
	}
}
