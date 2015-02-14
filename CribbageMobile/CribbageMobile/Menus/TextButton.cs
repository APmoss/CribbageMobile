using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace CribbageMobile.Menus {
	class TextButton : MenuItem {
		public bool IsTapped = false;
		public float TextScale = 1;

		public override void Tap(Microsoft.Xna.Framework.Input.Touch.GestureSample gesture) {
			base.Tap(gesture);
		}

		public TextButton(string text, Rectangle bounds) {
			this.Text = text;
			this.Bounds = bounds;
		}
		public TextButton(string text, Rectangle bounds, float textScale) {
			this.Text = text;
			this.Bounds = bounds;
			this.TextScale = textScale;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, ScreenManager screenManager, float tAlpha) {
			// Draws the background in the bounds of the button
			DrawBackground(screenManager, Bounds, tAlpha);

			// Draws the border around the button
			DrawBorder(screenManager, Bounds, tAlpha);

			// Text should be centered with the button
			Vector2 pos = new Vector2();
			Vector2 textDimenstions = screenManager.Font.MeasureString(Text);
			pos.X = (Bounds.X + Bounds.Width / 2) - textDimenstions.X / 2 * TextScale;
			pos.Y = (Bounds.Y + Bounds.Height / 2) - textDimenstions.Y / 2 * TextScale;

			screenManager.SpriteBatch.DrawString(screenManager.Font, Text, pos, TextTint * tAlpha, 0, Vector2.Zero, TextScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);

			base.Draw(gameTime, screenManager, tAlpha);
		}
	}
}
