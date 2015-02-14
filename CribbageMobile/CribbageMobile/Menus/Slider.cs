using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace CribbageMobile.Menus {
	class Slider : MenuItem {
		const int SLIDER_CUSHIONS = 15;
		const int VERTICAL_CUSHIONS = 2;
		const int NOTCH_WIDTH = 30;

		/// <summary>
		/// The tint of the slider notch
		/// </summary>
		public Color notchTint = Color.LightBlue;

		private float amount = 50;
		private Vector2 notchPos;

		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// The amount from 0-100 that the slider is currently at
		/// </summary>
		public float Amount {
			get { return amount; }
			set {
				amount = value;

				amount = MathHelper.Clamp(amount, 0, 100);

				notchPos.X = Bounds.Left + SLIDER_CUSHIONS + (TotalNotchDistance * (amount / 100));

				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		public Vector2 NotchPos {
			get { return notchPos; }
			set {
				notchPos = value;

				notchPos.X = MathHelper.Clamp(notchPos.X, Bounds.Left + SLIDER_CUSHIONS, Bounds.Right - SLIDER_CUSHIONS);

				float notchDistance = notchPos.X - (Bounds.Left + SLIDER_CUSHIONS);

				amount = (notchDistance / TotalNotchDistance) * 100f;

				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// The total distance that the notch can travel
		/// </summary>
		private float TotalNotchDistance {
			get { return (Bounds.Right - SLIDER_CUSHIONS) - (Bounds.Left + SLIDER_CUSHIONS); }
		}

		public Slider(Rectangle bounds) {
			this.Bounds = bounds;

			// Set notch position to 50%
			NotchPos = new Vector2(bounds.X + bounds.Width / 2, NotchPos.Y);
		}

		public Slider(Rectangle bounds, float startingAmount) {
			this.Bounds = bounds;

			NotchPos = new Vector2(bounds.X + bounds.Width / 2, NotchPos.Y);
		}

		public override void Draw(GameTime gameTime, GameStateManagement.ScreenManager screenManager, float tAlpha) {
			DrawBackground(screenManager, Bounds, tAlpha);
			DrawBorder(screenManager, Bounds, tAlpha);

			// Draw notch
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture,
										new Rectangle((int)NotchPos.X - NOTCH_WIDTH / 2, Bounds.Top + VERTICAL_CUSHIONS, NOTCH_WIDTH, Bounds.Height - VERTICAL_CUSHIONS * 2),
										notchTint * tAlpha);

			base.Draw(gameTime, screenManager, tAlpha);
		}

		public override void Tap(GestureSample gesture) {
			if (gesture.Position.X < NotchPos.X) {
				NotchPos = new Vector2(NotchPos.X - 10, NotchPos.Y);
			}
			else if (gesture.Position.X > NotchPos.X) {
				NotchPos = new Vector2(NotchPos.X + 10, NotchPos.Y);
			}

			base.Tap(gesture);
		}

		// Hold only happens at the instant :/
		//public override void Hold(GestureSample gesture) {
		//    if (gesture.Position.X < NotchPos.X) {
		//        NotchPos = new Vector2(NotchPos.X - 10, NotchPos.Y);
		//    }
		//    else if (gesture.Position.X > NotchPos.X) {
		//        NotchPos = new Vector2(NotchPos.X + 10, NotchPos.Y);
		//    }

		//    base.Hold(gesture);
		//}

		public override void HorizontalDrag(GestureSample gesture) {
			NotchPos = new Vector2(NotchPos.X + gesture.Delta.X, NotchPos.Y);

			base.HorizontalDrag(gesture);
		}
	}
}
