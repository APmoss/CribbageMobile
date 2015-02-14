using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CribbageMobile.Gameplay {
	class GameBackground : GameScreen {
		Color backgroundTint = Color.White;

		Texture2D background;

		public GameBackground(Color tint) {
			backgroundTint = tint;

			TransitionOnTime = TimeSpan.FromSeconds(.5);
			TransitionOffTime = TimeSpan.FromSeconds(.5);
		}

		public override void Activate(bool instancePreserved) {
			background = ScreenManager.Game.Content.Load<Texture2D>(@"textures\background");

			base.Activate(instancePreserved);
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			base.Update(gameTime, otherScreenHasFocus, false);
		}

		public override void Draw(GameTime gameTime) {
			Rectangle viewport = ScreenManager.Game.GraphicsDevice.Viewport.Bounds;

			ScreenManager.SpriteBatch.Begin();

			ScreenManager.SpriteBatch.Draw(background, new Rectangle(0, 0, 480, 800), backgroundTint);

			ScreenManager.SpriteBatch.End();
			
			base.Draw(gameTime);
		}
	}
}
