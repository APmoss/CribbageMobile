using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CribbageMobile.CardLogic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CribbageMobile.Menus {
	class CardColorMenu : Menu {
		// BackTint, FronTint, LineTint, TextTint Costomization
		IsolatedStorageSettings settings;

		#region Menu Items
		TextButton FrontTintLabel = new TextButton("Card Front", new Rectangle(10, 70, 225, 50));
		TextButton BackTintLabel = new TextButton("Card Back", new Rectangle(245, 70, 225, 50));
		TextButton LineTintLabel = new TextButton("Line Color", new Rectangle(10, 305, 225, 50));
		TextButton TextTintLabel = new TextButton("Text Color", new Rectangle(245, 305, 225, 50));
		TextButton ResetButton = new TextButton("Reset", new Rectangle(10, 535, 460, 50));

		Slider FrontTintR = new Slider(new Rectangle(10, 130, 225, 50));
		Slider FrontTintG = new Slider(new Rectangle(10, 185, 225, 50));
		Slider FrontTintB = new Slider(new Rectangle(10, 240, 225, 50));
		Slider BackTintR = new Slider(new Rectangle(245, 130, 225, 50));
		Slider BackTintG = new Slider(new Rectangle(245, 185, 225, 50));
		Slider BackTintB = new Slider(new Rectangle(245, 240, 225, 50));
		Slider LineTintR = new Slider(new Rectangle(10, 365, 225, 50));
		Slider LineTintG = new Slider(new Rectangle(10, 420, 225, 50));
		Slider LineTintB = new Slider(new Rectangle(10, 475, 225, 50));
		Slider TextTintR = new Slider(new Rectangle(245, 365, 225, 50));
		Slider TextTintG = new Slider(new Rectangle(245, 420, 225, 50));
		Slider TextTintB = new Slider(new Rectangle(245, 475, 225, 50));
		
		#endregion

		DrawableCard frontCard = new DrawableCard(Suite.Spades, 1);
		DrawableCard backCard = new DrawableCard(Suite.Spades, 2);

		Texture2D suiteIcons;

		public CardColorMenu() {
			settings = IsolatedStorageSettings.ApplicationSettings;

			#region Add Menu Items and Stuff
			ResetButton.Tapped += new EventHandler<MenuItemEventArgs>(ResetButton_Tapped);

			FrontTintR.BackgroundTint = BackTintR.BackgroundTint = LineTintR.BackgroundTint = TextTintR.BackgroundTint = Color.Red;
			FrontTintG.BackgroundTint = BackTintG.BackgroundTint = LineTintG.BackgroundTint = TextTintG.BackgroundTint = Color.Green;
			FrontTintB.BackgroundTint = BackTintB.BackgroundTint = LineTintB.BackgroundTint = TextTintB.BackgroundTint = Color.Blue;

			FrontTintR.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			FrontTintG.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			FrontTintB.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			BackTintR.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			BackTintG.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			BackTintB.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			LineTintR.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			LineTintG.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			LineTintB.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			TextTintR.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			TextTintG.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);
			TextTintB.HorizontalDragged += new EventHandler<MenuItemEventArgs>(Slider_ValueChanged);

			MenuItems.Add(FrontTintLabel);
			MenuItems.Add(BackTintLabel);
			MenuItems.Add(LineTintLabel);
			MenuItems.Add(TextTintLabel);
			MenuItems.Add(ResetButton);

			MenuItems.Add(FrontTintR);
			MenuItems.Add(FrontTintG);
			MenuItems.Add(FrontTintB);
			MenuItems.Add(BackTintR);
			MenuItems.Add(BackTintG);
			MenuItems.Add(BackTintB);
			MenuItems.Add(LineTintR);
			MenuItems.Add(LineTintG);
			MenuItems.Add(LineTintB);
			MenuItems.Add(TextTintR);
			MenuItems.Add(TextTintG);
			MenuItems.Add(TextTintB);
			#endregion

			frontCard.IsFaceUp = true;
			frontCard.Position = frontCard.Destination = new Vector2(135, 600);
			backCard.Position = backCard.Destination = new Vector2(245, 600);

			InitializeSettings();

			EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.HorizontalDrag;
		}

		public override void Activate(bool instancePreserved) {
			suiteIcons = ScreenManager.Game.Content.Load<Texture2D>(@"textures\suiteIcons");
			
			base.Activate(instancePreserved);
		}

		public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input) {
			PlayerIndex p;

			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				settings["CardFrontTint"] = new Color(FrontTintR.Amount / 100f, FrontTintG.Amount / 100f, FrontTintB.Amount / 100f);
				settings["CardBackTint"] = new Color(BackTintR.Amount / 100f, BackTintG.Amount / 100f, BackTintB.Amount / 100f);
				settings["CardLineTint"] = new Color(LineTintR.Amount / 100f, LineTintG.Amount / 100f, LineTintB.Amount / 100f);
				settings["CardTextTint"] = new Color(TextTintR.Amount / 100f, TextTintG.Amount / 100f, TextTintB.Amount / 100f);

				settings.Save();

				ExitScreen();
			}
			
			base.HandleInput(gameTime, input);
		}

		public override void Draw(GameTime gameTime) {
			frontCard.Draw(ScreenManager, suiteIcons);
			backCard.Draw(ScreenManager, suiteIcons);
			
			base.Draw(gameTime);
		}

		private void InitializeSettings() {
			if (!settings.Contains("CardBackTint")) {
				settings.Add("CardBackTint", Color.DarkBlue);
			}
			if (!settings.Contains("CardFrontTint")) {
				settings.Add("CardFrontTint", Color.LightBlue);
			}
			if (!settings.Contains("CardLineTint")) {
				settings.Add("CardLineTint", Color.White);
			}
			if (!settings.Contains("CardTextTint")) {
				settings.Add("CardTextTint", Color.White);
			}
			
			FrontTintR.Amount = ((Color)settings["CardFrontTint"]).R / 255f * 100f;
			FrontTintG.Amount = ((Color)settings["CardFrontTint"]).G / 255f * 100f;
			FrontTintB.Amount = ((Color)settings["CardFrontTint"]).B / 255f * 100f;
			BackTintR.Amount = ((Color)settings["CardBackTint"]).R / 255f * 100f;
			BackTintG.Amount = ((Color)settings["CardBackTint"]).G / 255f * 100f;
			BackTintB.Amount = ((Color)settings["CardBackTint"]).B / 255f * 100f;
			LineTintR.Amount = ((Color)settings["CardLineTint"]).R / 255f * 100f;
			LineTintG.Amount = ((Color)settings["CardLineTint"]).G / 255f * 100f;
			LineTintB.Amount = ((Color)settings["CardLineTint"]).B / 255f * 100f;
			TextTintR.Amount = ((Color)settings["CardTextTint"]).R / 255f * 100f;
			TextTintG.Amount = ((Color)settings["CardTextTint"]).G / 255f * 100f;
			TextTintB.Amount = ((Color)settings["CardTextTint"]).B / 255f * 100f;

			frontCard.FrontTint = new Color(FrontTintR.Amount / 100f, FrontTintG.Amount / 100f, FrontTintB.Amount / 100f);
			frontCard.LineTint = new Color(LineTintR.Amount / 100f, LineTintG.Amount / 100f, LineTintB.Amount / 100f);
			backCard.LineTint = new Color(LineTintR.Amount / 100f, LineTintG.Amount / 100f, LineTintB.Amount / 100f);
			backCard.BackTint = new Color(BackTintR.Amount / 100f, BackTintG.Amount / 100f, BackTintB.Amount / 100f);
			backCard.TextTint = new Color(TextTintR.Amount / 100f, TextTintG.Amount / 100f, TextTintB.Amount / 100f);
		}

		void Slider_ValueChanged(object sender, MenuItemEventArgs e) {
			frontCard.FrontTint = new Color(FrontTintR.Amount / 100f, FrontTintG.Amount / 100f, FrontTintB.Amount / 100f);
			frontCard.LineTint = new Color(LineTintR.Amount / 100f, LineTintG.Amount / 100f, LineTintB.Amount / 100f);
			backCard.LineTint = new Color(LineTintR.Amount / 100f, LineTintG.Amount / 100f, LineTintB.Amount / 100f);
			backCard.BackTint = new Color(BackTintR.Amount / 100f, BackTintG.Amount / 100f, BackTintB.Amount / 100f);
			backCard.TextTint = new Color(TextTintR.Amount / 100f, TextTintG.Amount / 100f, TextTintB.Amount / 100f);
		}

		void ResetButton_Tapped(object sender, MenuItemEventArgs e) {
			frontCard.FrontTint = Color.LightBlue;
			frontCard.LineTint = Color.White;
			backCard.LineTint = Color.White;
			backCard.BackTint = Color.DarkBlue;
			backCard.TextTint = Color.White;

			FrontTintR.Amount = frontCard.FrontTint.R / 255f * 100f;
			FrontTintG.Amount = frontCard.FrontTint.G / 255f * 100f;
			FrontTintB.Amount = frontCard.FrontTint.B / 255f * 100f;
			BackTintR.Amount = backCard.BackTint.R / 255f * 100f;
			BackTintG.Amount = backCard.BackTint.G / 255f * 100f;
			BackTintB.Amount = backCard.BackTint.B / 255f * 100f;
			LineTintR.Amount = frontCard.LineTint.R / 255f * 100f;
			LineTintG.Amount = frontCard.LineTint.G / 255f * 100f;
			LineTintB.Amount = frontCard.LineTint.B / 255f * 100f;
			TextTintR.Amount = backCard.TextTint.R / 255f * 100f;
			TextTintG.Amount = backCard.TextTint.G / 255f * 100f;
			TextTintB.Amount = backCard.TextTint.B / 255f * 100f;
		}
	}
}
