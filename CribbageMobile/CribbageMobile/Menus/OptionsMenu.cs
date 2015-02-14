using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CribbageMobile.Menus {
	class OptionsMenu : Menu {
		private IsolatedStorageSettings settings;
		TextButton musicLabel = new TextButton("75", new Rectangle(365, 200, 100, 50));
		Slider musicSlider = new Slider(new Rectangle(CUSHION, 265, Stcs.Width - CUSHION * 2, 50));

		TextButton soundLabel = new TextButton("75", new Rectangle(365, 360, 100, 50));
		Slider soundSlider = new Slider(new Rectangle(CUSHION, 425, Stcs.Width - CUSHION * 2, 50));

		public OptionsMenu() : base() {
			settings = IsolatedStorageSettings.ApplicationSettings;
			InitializeSettings();

			musicSlider.ValueChanged += new EventHandler<EventArgs>(musicSlider_ValueChanged);
			soundSlider.ValueChanged += new EventHandler<EventArgs>(soundSlider_ValueChanged);
			
			MenuItems.Add(new TextButton("Music", new Rectangle(CUSHION, 200, 200, 50)));
			MenuItems.Add(new TextButton("Sounds", new Rectangle(CUSHION, 360, 200, 50)));
			
			MenuItems.Add(musicLabel);
			MenuItems.Add(musicSlider);
			MenuItems.Add(soundLabel);
			MenuItems.Add(soundSlider);
			
			EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.HorizontalDrag;
		}

		public override void Activate(bool instancePreserved) {
			base.Activate(instancePreserved);
		}

		public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input) {
			PlayerIndex p;

			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				SaveAndExit();
			}
			
			base.HandleInput(gameTime, input);
		}

		void musicSlider_ValueChanged(object sender, EventArgs e) {
			musicLabel.Text = ((int)musicSlider.Amount).ToString();
		}
		void soundSlider_ValueChanged(object sender, EventArgs e) {
			soundLabel.Text = ((int)soundSlider.Amount).ToString();
		}

		/// <summary>
		/// Checks for needed keys and creates them if they don't exist
		/// </summary>
		private void InitializeSettings() {
			if (!settings.Contains("MusicVolume")) {
				settings.Add("MusicVolume", 75);
			}
			if (!settings.Contains("SoundVolume")) {
				settings.Add("SoundVolume", 75);
			}

			musicSlider.Amount = (int)settings["MusicVolume"];
			soundSlider.Amount = (int)settings["SoundVolume"];

			musicLabel.Text = musicSlider.Amount.ToString();
			soundLabel.Text = soundSlider.Amount.ToString();
		}

		

		private void SaveAndExit() {
			settings["MusicVolume"] = int.Parse(musicLabel.Text);
			settings["SoundVolume"] = int.Parse(soundLabel.Text);

			settings.Save();

			ExitScreen();
		}
	}
}
