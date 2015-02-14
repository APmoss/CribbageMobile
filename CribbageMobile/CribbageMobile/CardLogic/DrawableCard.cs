using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace CribbageMobile.CardLogic {
	class DrawableCard : Card {
		public const int WIDTH = 100;
		public const int HEIGHT = 140;

		public Vector2 Position;
		public Vector2 Destination;
		public Color BackTint = Color.DarkBlue;
		public Color FrontTint = Color.LightBlue;
		public Color LineTint = Color.White;
		public Color TextTint = Color.White;

		public bool IsFaceUp;

		public DrawableCard(Suite suite, int number)
			: base(suite, number) {

		}

		public virtual void Draw(ScreenManager screenManager, Texture2D suiteIcons) {
			screenManager.SpriteBatch.Begin();

			// Draw back or front tint and items
			if (IsFaceUp) {
				screenManager.SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)Position.X, (int)Position.Y, WIDTH, HEIGHT), FrontTint);

				// Red or black depending on suite
				Color TextColor;
				if (suite == Suite.Spades || suite == Suite.Clubs) {
					TextColor = Color.Black;
				}
				else {
					TextColor = Color.Red;
				}

				screenManager.SpriteBatch.DrawString(screenManager.Font, GetLetter(), Position + new Vector2(5, 0),
																			TextColor, 0, Vector2.Zero, .5f, SpriteEffects.None, 0);
				screenManager.SpriteBatch.Draw(suiteIcons, new Rectangle((int)Position.X, (int)Position.Y + 30, 30, 35),
																			GetSuiteIconSource(suiteIcons), Color.White);
				screenManager.SpriteBatch.DrawString(screenManager.Font, GetLetter(), Position + new Vector2(WIDTH - 27, HEIGHT - 27),
																			TextColor, 0, Vector2.Zero, .5f, SpriteEffects.FlipVertically, 0);
				screenManager.SpriteBatch.Draw(suiteIcons, new Rectangle((int)Position.X + WIDTH - 33, (int)Position.Y + HEIGHT - 57, 30, 35),
																			GetSuiteIconSource(suiteIcons), Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
			}
			else {
				screenManager.SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)Position.X, (int)Position.Y, WIDTH, HEIGHT), BackTint);
				screenManager.SpriteBatch.DrawString(screenManager.Font, "C", Position + new Vector2(31, 5), TextTint, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
				screenManager.SpriteBatch.DrawString(screenManager.Font, "M", Position + new Vector2(25, 65), TextTint, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
			}

			// Draw four sides
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)Position.X, (int)Position.Y, WIDTH, 1), LineTint);
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)Position.X, (int)Position.Y, 1, HEIGHT), LineTint);
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)Position.X, (int)Position.Y + HEIGHT, WIDTH, 1), LineTint);
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)Position.X + WIDTH, (int)Position.Y + 1, 1, HEIGHT), LineTint);

			screenManager.SpriteBatch.End();
		}

		/// <summary>
		/// Returns the source rectangle that contains the suite icon
		/// </summary>
		/// <param name="suiteIconsTexture"></param>
		/// <returns></returns>
		public Rectangle GetSuiteIconSource(Texture2D suiteIconsTexture) {
			Rectangle suiteIconSource = new Rectangle();

			if (suite == Suite.Spades) {
				suiteIconSource = new Rectangle(suiteIconsTexture.Width / 2, suiteIconsTexture.Height / 2, suiteIconsTexture.Width / 2, suiteIconsTexture.Height / 2);
			}
			else if (suite == Suite.Hearts) {
				suiteIconSource = new Rectangle(0, suiteIconsTexture.Height / 2, suiteIconsTexture.Width / 2, suiteIconsTexture.Height / 2);
			}
			else if (suite == Suite.Clubs) {
				suiteIconSource = new Rectangle(0, 0, suiteIconsTexture.Width / 2, suiteIconsTexture.Height / 2);
			}
			else {
				suiteIconSource = new Rectangle(suiteIconsTexture.Width / 2, 0, suiteIconsTexture.Width / 2, suiteIconsTexture.Height / 2);
			}

			return suiteIconSource;
		}
	}
}
