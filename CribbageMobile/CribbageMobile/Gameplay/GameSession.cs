using System;
using System.Collections.Generic;
using CribbageMobile.Menus;
using CribbageMobile.CardLogic;
using GameStateManagement;
using ShakeGestures;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CribbageMobile.Gameplay {
	enum AIType {
		Player, Easy, Medium, Hard
	}

	class GameSession : GameScreen {
		enum Stage {
			Init, Deal, PlaceTwo
		}

		#region Fields

		Stage stage = Stage.Init;
		int subStep = 0;

		AIType p2logic;

		// Card Stacks
		CardStack deck = new FullDeck(true);
		CardStack trash = new CardStack();
		CardStack crib = new CardStack();
		CardStack p1hand = new CardStack();
		CardStack p1field = new CardStack();
		CardStack p2hand = new CardStack();
		CardStack p2field = new CardStack();

		// Textures
		Texture2D suiteIcons;

		// Menu items
		List<MenuItem> menuItems = new List<MenuItem>();
		TextButton messageBox = new TextButton(string.Empty, new Rectangle(0, 770, 480, 30));
		List<MenuItem> p1deckBounds = new List<MenuItem>() {
			new TextButton(string.Empty, new Rectangle(25, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(135, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(245, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(245, 470, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 470, 100, 140))
		};

		// Shake! \o/
		bool shaked = false;
		ShakeGesturesHelper shake = ShakeGesturesHelper.Instance;

		// Timer
		TimeSpan elapsedTime = new TimeSpan();

		#endregion

		public GameSession(AIType ai) {
			EnabledGestures = EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.HorizontalDrag;
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

			// Set the logic for the second player (a person, easy, medium, or hard)
			p2logic = ai;

			//TODO: remove this
			Random r = new Random();
			foreach (var card in deck.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Position = new Vector2(10, 330);
					((DrawableCard)card).Destination = new Vector2(10, 330);
					((DrawableCard)card).IsFaceUp = (r.Next(2) == 0 ? false : true);
				}
			}

			messageBox.TextScale = .5f;
			messageBox.Text = "Shuffle: Shake the phone to shuffle!";
			messageBox.Tapped += new EventHandler<MenuItemEventArgs>(messageBox_Tapped);
			menuItems.Add(messageBox);
			menuItems.Add(new TextButton("Deck", new Rectangle(10, 480, 100, 25), .4f));
			#region Ugly Deck Bounds Stuff
			foreach (var item in p1deckBounds) {
				item.BackgroundTint = Color.Transparent;
				item.BorderTint = Color.Yellow;
			}
			#endregion
			menuItems.AddRange(p1deckBounds);

			shake.Active = true;
			shake.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(shake_ShakeGesture);
		}

		public override void Activate(bool instancePreserved) {
			suiteIcons = ScreenManager.Game.Content.Load<Texture2D>(@"textures\suiteIcons");

			base.Activate(instancePreserved);
		}

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
			elapsedTime += gameTime.ElapsedGameTime;

			#region Update MenuItems and Cards
			// Update all the menu items
			foreach (var menuItem in menuItems) {
				menuItem.Update(gameTime);
			}
			
			// Update ALL the cards!
			foreach (var card in deck.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			foreach (var card in trash.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			foreach (var card in crib.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			foreach (var card in p1hand.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			foreach (var card in p1field.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			foreach (var card in p2hand.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			foreach (var card in p2field.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
				}
			}
			#endregion

			switch (stage) {
				case Stage.Init:
					UpdateInit(gameTime);
					break;
			}

			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		protected void UpdateInit(GameTime gameTime) {
			if (elapsedTime.TotalSeconds > 1.5 && shaked && subStep == 0) {
				elapsedTime = new TimeSpan();
				messageBox.Text = "Shuffling...";
				shaked = false;
				shake.Active = false;
				subStep = 1;

				// Randomize destination
				Random r = new Random();
				foreach (var card in deck.cards) {
					if (card is DrawableCard) {
						((DrawableCard)card).Destination = new Vector2(r.Next(380), r.Next(660));
					}
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 1) {
				elapsedTime = new TimeSpan();
				subStep = 2;

				// Actually shuffle the deck
				deck.Shuffle();

				// Randomize destination
				Random r = new Random();
				foreach (var card in deck.cards) {
					if (card is DrawableCard) {
						((DrawableCard)card).Destination = new Vector2(r.Next(380), r.Next(660));
					}
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 2) {
				elapsedTime = new TimeSpan();
				subStep = 3;

				foreach (var card in deck.cards) {
					if (card is DrawableCard) {
						((DrawableCard)card).Destination = new Vector2(10, 330);
					}
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 3) {
				elapsedTime = new TimeSpan();
				messageBox.Text = "Who goes first?: Tap the deck to draw.";


				//REMOVEOMEOJMAJLHFAJEFLHLAEF

				shake.Active = true;
				subStep = 0;
			}
		}

		public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime, InputState input) {
			PlayerIndex p;
			//TODO make save or something
			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				LoadingScreen.Load(ScreenManager, new List<GameScreen>() { new GameBackground(Color.White), new MainMenu() });
			}

			#region Menu Item Event Stuff
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
			#endregion

			base.HandleInput(gameTime, input);
		}

		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
			#region Draw Menu Items and Cards
			// Draw all the menu items
			ScreenManager.SpriteBatch.Begin();
			foreach (var menuItem in menuItems) {
				menuItem.Draw(gameTime, ScreenManager, TransitionAlpha);
			}
			ScreenManager.SpriteBatch.End();

			// Draw ALL the cards!
			foreach (var card in deck.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			foreach (var card in trash.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			foreach (var card in crib.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			foreach (var card in p1hand.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			foreach (var card in p1field.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			foreach (var card in p2hand.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			foreach (var card in p2field.cards) {
				if (card is DrawableCard) {
					((DrawableCard)card).Draw(ScreenManager, suiteIcons);
				}
			}
			#endregion

			base.Draw(gameTime);
		}

		void shake_ShakeGesture(object sender, ShakeGestureEventArgs e) {
			shaked = true;
		}

		void messageBox_Tapped(object sender, MenuItemEventArgs e) {
			switch (stage) {
				case Stage.Init:
					#region Init
					switch (subStep) {
						case 0:
							elapsedTime = new TimeSpan();
							shaked = false;
							shake.Active = false;
							subStep = 1;

							// Randomize destination
							Random r = new Random();
							foreach (var card in deck.cards) {
								if (card is DrawableCard) {
									((DrawableCard)card).Destination = new Vector2(r.Next(380), r.Next(660));
								}
							}
							break;
						case 1:
							elapsedTime = new TimeSpan();
							subStep = 2;

							// Actually shuffle the deck
							deck.Shuffle();

							// Randomize destination
							//Random r = new Random();
							r = new Random();
							foreach (var card in deck.cards) {
								if (card is DrawableCard) {
									((DrawableCard)card).Destination = new Vector2(r.Next(380), r.Next(660));
								}
							}
							break;
						case 2:
							elapsedTime = new TimeSpan();
							subStep = 3;

							foreach (var card in deck.cards) {
								if (card is DrawableCard) {
									((DrawableCard)card).Destination = new Vector2(10, 330);
								}
							}
							break;
						case 3:
							elapsedTime = new TimeSpan();
							messageBox.Text = "Who goes first?: Tap the deck to draw.";


							//REMOVEOMEOJMAJLHFAJEFLHLAEF

							shake.Active = true;
							subStep = 0;
							break;
					}
					break;
					#endregion
			}
		}
	}
}
