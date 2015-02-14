using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices;

namespace CribbageMobile.Menus {
	abstract class MenuItem {
		private bool enabled = true;
		private Rectangle bounds;
		private VibrateController v = VibrateController.Default;
		
		// Public stuff, because it's easier to read :)

		/// <summary>
		/// The text of the menu item
		/// </summary>
		public string Text = "";

		/// <summary>
		/// The tint applied to the menu item background
		/// </summary>
		public Color BackgroundTint = Color.SaddleBrown;

		/// <summary>
		/// The tint applied to the menu item border
		/// </summary>
		public Color BorderTint = Color.RosyBrown;

		/// <summary>
		/// The tint applied to the text of the menu item
		/// </summary>
		public Color TextTint = Color.SandyBrown;

		/// <summary>
		/// The thickness (in pixels) of the menu item border
		/// </summary>
		public int BorderThickness = 2;

		public event EventHandler<MenuItemEventArgs> Tapped;
		public event EventHandler<MenuItemEventArgs> Held;
		public event EventHandler<MenuItemEventArgs> HorizontalDragged;

		public MenuItem() {

		}
		public MenuItem(Rectangle bounds) {
			this.bounds = bounds;
		}

		/// <summary>
		/// The button is only usable when it is enabled
		/// </summary>
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}

		/// <summary>
		/// The drawing/pressing boundaries of the menu item
		/// </summary>
		public Rectangle Bounds {
			get { return bounds; }
			set { bounds = value; }
		}

		// Used to load specific content for certain menu items
		public virtual void LoadContent(ContentManager content) {

		}

		// For buttons that need to be updated
		public virtual void Update(GameTime gameTime) {

		}

		// For button with specific drawing instructions
		public virtual void Draw(GameTime gameTime, ScreenManager screenManager, float tAlpha) {
			// If the menu item is not enabled, gray it out a little
			if (!enabled) {
				screenManager.SpriteBatch.Draw(screenManager.BlankTexture, bounds, new Color(0, 0, 0, 128) * tAlpha);
			}
		}

		/// <summary>
		/// Fires the menu item's tap event
		/// </summary>
		public virtual void Tap(GestureSample gesture) {
			if (Tapped != null) {
				v.Start(TimeSpan.FromMilliseconds(50));
				Tapped(this, new MenuItemEventArgs(gesture));
			}
		}

		/// <summary>
		/// Fires the menu item's hold event
		/// </summary>
		public virtual void Hold(GestureSample gesture) {
			if (Held != null) {
				Held(this, new MenuItemEventArgs(gesture));
			}
		}

		public virtual void HorizontalDrag(GestureSample gesture) {
			if (HorizontalDragged != null) {
				HorizontalDragged(this, new MenuItemEventArgs(gesture));
			}
		}

		/// <summary>
		/// Draws the beckground in the bounds of the passed rectangle
		/// </summary>
		protected void DrawBackground(ScreenManager screenManager, Rectangle bounds, float tAlpha) {
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture, bounds, BackgroundTint * tAlpha);
		}

		/// <summary>
		/// Draws a border around the bounds of the menu item
		/// </summary>
		protected void DrawBorder(ScreenManager screenManager, Rectangle bounds, float tAlpha) {
			// Top
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture,
										new Rectangle(Bounds.X - BorderThickness, Bounds.Y - BorderThickness, Bounds.Width + BorderThickness * 2, BorderThickness), BorderTint * tAlpha);
			// Bottom
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture,
										new Rectangle(Bounds.X - BorderThickness, Bounds.Bottom, Bounds.Width + BorderThickness * 2, BorderThickness), BorderTint * tAlpha);
			// Left
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture,
										new Rectangle(Bounds.X - BorderThickness, Bounds.Y, BorderThickness, Bounds.Height), BorderTint * tAlpha);
			// Right
			screenManager.SpriteBatch.Draw(screenManager.BlankTexture,
										new Rectangle(Bounds.Right, Bounds.Y, BorderThickness, Bounds.Height), BorderTint * tAlpha);
		}
	}
}
