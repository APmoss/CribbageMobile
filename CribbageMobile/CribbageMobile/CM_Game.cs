using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;

using GameStateManagement;
using System.Windows;

namespace CribbageMobile {
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class CM_Game : Microsoft.Xna.Framework.Game {
		GraphicsDeviceManager graphics;
		ScreenManager screenManager;
		ScreenFactory screenFactory;

		public CM_Game() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// Set framerate to 60 fps
			TargetElapsedTime = TimeSpan.FromSeconds(1.0/60.0);

			// Set resolution and fullscreen
			graphics.PreferredBackBufferWidth = 480;
			graphics.PreferredBackBufferHeight = 800;
			graphics.IsFullScreen = true;
			// Set allowed orientations
			graphics.SupportedOrientations = DisplayOrientation.Portrait;

			// Extend battery life under lock.
			InactiveSleepTime = TimeSpan.FromSeconds(1);

			// Add the screenManager
			screenManager = new ScreenManager(this);
			Components.Add(screenManager);

			// Add the screenFactory
			screenFactory = new ScreenFactory();
			Services.AddService(typeof(IScreenFactory), screenFactory);

			// Add application events
			PhoneApplicationService.Current.Launching += new EventHandler<LaunchingEventArgs>(GameLaunching);
			PhoneApplicationService.Current.Activated += new EventHandler<ActivatedEventArgs>(GameActivated);
			PhoneApplicationService.Current.Deactivated += new EventHandler<DeactivatedEventArgs>(GameDeactivated);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);


			base.Draw(gameTime);
		}

		private void AddInitialScreens() {
			// Background
			screenManager.AddScreen(new Gameplay.GameBackground(Color.White), null);
			// Splash screen
			screenManager.AddScreen(new Splash(), null);
		}

		void GameLaunching(object sender, LaunchingEventArgs e) {
			// Start the initial screens
			AddInitialScreens();
		}

		void GameActivated(object sender, ActivatedEventArgs e) {
			// Try to deserialize the screenManager
			if (!screenManager.Activate(e.IsApplicationInstancePreserved)) {
				// If it fails, just start the initial screens
				AddInitialScreens();
			}
		}

		void GameDeactivated(object sender, DeactivatedEventArgs e) {
			// Serialize the screenManager
			screenManager.Deactivate();
		}
	}
}
