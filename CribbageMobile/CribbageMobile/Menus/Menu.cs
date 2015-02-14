using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace CribbageMobile.Menus {
	abstract class Menu : GameScreen {
		public const int CUSHION = 15;

		private List<MenuItem> menuItems = new List<MenuItem>();

		public Menu() {
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

			EnabledGestures = GestureType.Tap;
		}

		public override void Activate(bool instancePreserved) {
			foreach (var menuItem in menuItems) {
				menuItem.LoadContent(ScreenManager.Game.Content);
			}

			base.Activate(instancePreserved);
		}

		/// <summary>
		/// The list containing all menu items in the menu
		/// </summary>
		public List<MenuItem> MenuItems {
			get { return menuItems; }
			set { menuItems = value; }
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			foreach (var menuItem in menuItems) {
				menuItem.Update(gameTime);
			}

			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void HandleInput(GameTime gameTime, InputState input) {
			foreach (var gesture in input.Gestures) {
				Point pos = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
				
				switch (gesture.GestureType) {
					// Menu item tapped
					case GestureType.Tap:
						foreach (var menuItem in menuItems) {
							if (menuItem.Bounds.Contains(pos) && menuItem.Enabled) {
								menuItem.Tap(gesture);
								break;
							}
						}

						break;

					// Menu item held
					case GestureType.Hold:
						foreach (var menuItem in menuItems) {
							if (menuItem.Bounds.Contains(pos) && menuItem.Enabled) {
								menuItem.Hold(gesture);
								break;
							}
						}

						break;

					// Menu item dragged horizontally (like a volume bar)
					case GestureType.HorizontalDrag:
						foreach (var menuItem in menuItems) {
							if (menuItem.Bounds.Contains(pos) && menuItem.Enabled) {
								menuItem.HorizontalDrag(gesture);
								break;
							}
						}

						break;
				}
			}

			base.HandleInput(gameTime, input);
		}

		public override void Draw(GameTime gameTime) {
			// TODO: Special begin parameters
			ScreenManager.SpriteBatch.Begin();
			foreach (var menuItem in menuItems) {
				menuItem.Draw(gameTime, ScreenManager, TransitionAlpha);
			}
			ScreenManager.SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
