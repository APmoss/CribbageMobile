using System;
using System.Collections.Generic;
using CribbageMobile.Gameplay;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace CribbageMobile.Menus {
	class MainMenu : Menu {
		TextButton PvPlayerButton = new TextButton("2 Player", new Rectangle(CUSHION, 300, Stcs.Width - CUSHION * 2, 50));
		TextButton PvEasyButton = new TextButton("Easy Computer", new Rectangle(CUSHION, 380, Stcs.Width - CUSHION * 2, 50));
		TextButton PvMedButton = new TextButton("Medium Computer", new Rectangle(CUSHION, 460, Stcs.Width - CUSHION * 2, 50));
		TextButton PvHardButton = new TextButton("Hard Computer", new Rectangle(CUSHION, 540, Stcs.Width - CUSHION * 2, 50));
		TextButton optionsButton = new TextButton("Options", new Rectangle(CUSHION, 700, Stcs.Width - CUSHION * 2, 50));

		public MainMenu() : base() {
			PvPlayerButton.Tapped += new EventHandler<MenuItemEventArgs>(PvPlayerButton_Tapped);
			MenuItems.Add(PvPlayerButton);

			PvEasyButton.Tapped += new EventHandler<MenuItemEventArgs>(PvEasyButton_Tapped);
			MenuItems.Add(PvEasyButton);

			PvMedButton.Tapped += new EventHandler<MenuItemEventArgs>(PvMedButton_Tapped);
			MenuItems.Add(PvMedButton);

			PvHardButton.Tapped += new EventHandler<MenuItemEventArgs>(PvHardButton_Tapped);
			MenuItems.Add(PvHardButton);

			optionsButton.Tapped += new EventHandler<MenuItemEventArgs>(optionsButton_Tapped);
			MenuItems.Add(optionsButton);
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw(GameTime gameTime) {
			base.Draw(gameTime);
		}

		void PvPlayerButton_Tapped(object sender, MenuItemEventArgs e) {
			PvPlayerButton.Text = "Nope!";
		}
		void PvEasyButton_Tapped(object sender, MenuItemEventArgs e) {
			LoadingScreen.Load(ScreenManager, new List<GameScreen>() { new GameBackground(Color.White), new GameSession(AIType.Easy) });
		}
		void PvMedButton_Tapped(object sender, MenuItemEventArgs e) {
			PvMedButton.Text = "Nope!";
		}
		void PvHardButton_Tapped(object sender, MenuItemEventArgs e) {
			PvHardButton.Text = "Nope!";
		}

		void optionsButton_Tapped(object sender, EventArgs e) {
			ScreenManager.AddScreen(new OptionsMenu(), null);
		}
	}
}
