using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using CribbageMobile.CardLogic;
using CribbageMobile.Menus;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using ShakeGestures;
using Microsoft.Devices;

namespace CribbageMobile.Gameplay {
	enum AIType {
		Player, Easy, Medium, Hard
	}

	class GameSession : GameScreen {
		enum Stage {
			Init, P1Deal, P2Deal, Done
		}

		#region Fields/Properties

		VibrateController v = VibrateController.Default;

		private IsolatedStorageSettings settings;

		Stage stage = Stage.Init;
		int subStep = 0;
		int runningTotal;
		int p1score, p2score;

		List<CribLib.ScoreSet> scores = new List<CribLib.ScoreSet>();

		int P1Score {
			get { return p1score; }
			set {
				p1score = value;

				if (p1score >= 121) {
					ScreenManager.AddScreen(new WinScreen(p1score, p2score), null);
					stage = Stage.Done;
				}
			}
		}
		int P2Score {
			get { return p2score; }
			set {
				p2score = value;

				if (p2score >= 121) {
					ScreenManager.AddScreen(new WinScreen(p1score, p2score), null);
					stage = Stage.Done;
				}
			}
		}

		AIType p2logic;

		// Card Stacks
		CardStack deck = new FullDeck(true);
		CardStack trash = new CardStack();
		CardStack crib = new CardStack();
		CardStack p1hand = new CardStack();
		CardStack p1field = new CardStack();
		CardStack p2hand = new CardStack();
		CardStack p2field = new CardStack();
		Card cutCard = null;

		// Textures
		Texture2D suiteIcons;

		// Menu items
		List<MenuItem> menuItems = new List<MenuItem>();
		TextButton messageBox = new TextButton(string.Empty, new Rectangle(0, 770, 480, 30));
		TextButton p1scoreBox = new TextButton("P1- 0", new Rectangle(10, 510, 100, 25), .5f);
		TextButton p2scoreBox = new TextButton("P2- 0", new Rectangle(10, 200, 100, 25), .5f);
		TextButton board = new TextButton("Board", new Rectangle(10, 300, 100, 25), .5f);

		#region Deck Bounds and Stuff
		TextButton deckBounds = new TextButton(string.Empty, new Rectangle(10, 330, 100, 140));
		List<MenuItem> p1deckBounds = new List<MenuItem>() {
			new TextButton(string.Empty, new Rectangle(25, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(135, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(245, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 620, 100, 140)),
			new TextButton(string.Empty, new Rectangle(245, 470, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 470, 100, 140))
		};
		List<MenuItem> p1fieldBounds = new List<MenuItem>() {
			new TextButton(string.Empty, new Rectangle(135, 470, 100, 140)),
			new TextButton(string.Empty, new Rectangle(180, 470, 100, 140)),
			new TextButton(string.Empty, new Rectangle(300, 470, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 470, 100, 140)),
		};
		List<MenuItem> p2deckBounds = new List<MenuItem>() {
			new TextButton(string.Empty, new Rectangle(25, 10, 100, 140)),
			new TextButton(string.Empty, new Rectangle(135, 10, 100, 140)),
			new TextButton(string.Empty, new Rectangle(245, 10, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 10, 100, 140)),
			new TextButton(string.Empty, new Rectangle(245, 160, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 160, 100, 140))
		};
		List<MenuItem> p2fieldBounds = new List<MenuItem>() {
			new TextButton(string.Empty, new Rectangle(135, 160, 100, 140)),
			new TextButton(string.Empty, new Rectangle(180, 160, 100, 140)),
			new TextButton(string.Empty, new Rectangle(300, 160, 100, 140)),
			new TextButton(string.Empty, new Rectangle(355, 160, 100, 140)),
		};
		List<MenuItem> cribBounds = new List<MenuItem>() {
			new TextButton(String.Empty, new Rectangle(135, 318, 100, 140)),
			new TextButton(String.Empty, new Rectangle(180, 318, 100, 140)),
			new TextButton(String.Empty, new Rectangle(300, 318, 100, 140)),
			new TextButton(String.Empty, new Rectangle(355, 318, 100, 140))
		};
		#endregion

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

			settings = IsolatedStorageSettings.ApplicationSettings;

			// Set the logic for the second player (a person, easy, medium, or hard)
			p2logic = ai;

			foreach (var card in deck.cards) {
			//TODO: remove this
			//Random r = new Random();
				if (card is DrawableCard) {
					((DrawableCard)card).BackTint = (Color)settings["CardBackTint"];
					((DrawableCard)card).FrontTint = (Color)settings["CardFrontTint"];
					((DrawableCard)card).LineTint = (Color)settings["CardLineTint"];
					((DrawableCard)card).TextTint = (Color)settings["CardTextTint"];
					((DrawableCard)card).Position = new Vector2(10, 330);
					((DrawableCard)card).Destination = new Vector2(10, 330);
					//((DrawableCard)card).IsFaceUp = (r.Next(2) == 0 ? false : true);
				}
			}

			messageBox.TextScale = .5f;
			messageBox.Text = "Shuffle: Shake the phone to shuffle!";
			messageBox.Tapped += new EventHandler<MenuItemEventArgs>(messageBox_Tapped);
			menuItems.Add(messageBox);
			menuItems.Add(new TextButton("Deck", new Rectangle(10, 480, 100, 25), .4f));
			menuItems.Add(p1scoreBox);
			menuItems.Add(p2scoreBox);
			#region Ugly Deck Bounds Stuff
			deckBounds.Tapped += new EventHandler<MenuItemEventArgs>(cardBounds_Tapped);
			menuItems.Add(deckBounds);
			foreach (var item in p1deckBounds) {
				item.BackgroundTint = Color.Transparent;
				item.BorderTint = Color.Transparent;//item.BorderTint = Color.Yellow;
				item.Tapped += new EventHandler<MenuItemEventArgs>(cardBounds_Tapped);
			}
			menuItems.AddRange(p1deckBounds);
			foreach (var item in p1fieldBounds) {
				item.BackgroundTint = Color.Transparent;
				item.BorderTint = Color.Transparent;// = Color.LightYellow;
				item.Tapped += new EventHandler<MenuItemEventArgs>(cardBounds_Tapped);
			}
			menuItems.AddRange(p1fieldBounds);
			foreach (var item in p2deckBounds) {
				item.BackgroundTint = Color.Transparent;
				item.BorderTint = Color.Transparent;// = Color.Red;
				item.Tapped += new EventHandler<MenuItemEventArgs>(cardBounds_Tapped);
			}
			menuItems.AddRange(p2deckBounds);
			foreach (var item in p2fieldBounds) {
				item.BackgroundTint = Color.Transparent;
				item.BorderTint = Color.Transparent;// = Color.Pink;
				item.Tapped += new EventHandler<MenuItemEventArgs>(cardBounds_Tapped);
			}
			menuItems.AddRange(p2fieldBounds);
			foreach (var item in cribBounds) {
				item.BackgroundTint = Color.Transparent;
				item.BorderTint = Color.Transparent;// = Color.White;
				item.Tapped += new EventHandler<MenuItemEventArgs>(cardBounds_Tapped);
			}
			menuItems.AddRange(cribBounds);

			board.Tapped += new EventHandler<MenuItemEventArgs>(board_Tapped);
			menuItems.Add(board);
			#endregion

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

			p1scoreBox.Text = "P1- " + P1Score;
			p2scoreBox.Text = "P2- " + P2Score;
			
			// Update ALL the cards!
			foreach (var card in deck.cards) {
				if (card is DrawableCard) {
					DrawableCard dCard = ((DrawableCard)card);
					((DrawableCard)card).IsFaceUp = false;
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
			if (cutCard is DrawableCard) {
				DrawableCard dCard = ((DrawableCard)cutCard);
				((DrawableCard)cutCard).Position += CM_Math.SmoothTranslate(dCard.Position, dCard.Destination);
			}
			#endregion

			switch (stage) {
				case Stage.Init:
					UpdateInit(gameTime);
					break;
				case Stage.P1Deal:
					UpdateP1Deal(gameTime);
					break;
				case Stage.P2Deal:
					UpdateP2Deal(gameTime);
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
				subStep++;
				v.Start(TimeSpan.FromMilliseconds(100));

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
				subStep++;
				v.Start(TimeSpan.FromMilliseconds(100));

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
				subStep++;

				foreach (var card in deck.cards) {
					if (card is DrawableCard) {
						((DrawableCard)card).Destination = new Vector2(10, 330);
					}
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 3) {
				messageBox.Text = "Who goes first?: Tap the deck to draw.";

				if (deckBounds.IsTapped) {
					elapsedTime = new TimeSpan();
					subStep++;

					p1hand.AddToTop(deck.DrawFromTop(1));
					p2hand.AddToTop(deck.DrawFromTop(1));
					if (p1hand.cards[0] is DrawableCard && p2hand.cards[0] is DrawableCard) {
						((DrawableCard)p1hand.cards[0]).IsFaceUp = true;
						((DrawableCard)p1hand.cards[0]).Destination = new Vector2(p1deckBounds[5].Bounds.X, p1deckBounds[5].Bounds.Y);
						((DrawableCard)p2hand.cards[0]).IsFaceUp = true;
						((DrawableCard)p2hand.cards[0]).Destination = new Vector2(p2deckBounds[5].Bounds.X, p2deckBounds[5].Bounds.Y);
					}					
				}
			}

			if(elapsedTime.TotalSeconds > 3 && subStep == 4) {
				elapsedTime = new TimeSpan();

				if (p1hand.cards[0].Number == p2hand.cards[0].Number) {
					messageBox.Text = "Your cards are the same! Try again.";
					subStep = 3;
				}
				else if (p1hand.cards[0].Number < p2hand.cards[0].Number) {
					messageBox.Text = "Your card is lower! You deal first.";
					stage = Stage.P1Deal;
					subStep = 0;
				}
				else {
					messageBox.Text = "Your card is higher! Player 2 deals first.";
					stage = Stage.P2Deal;
					subStep = 0;
				}

				if (p1hand.cards[0] is DrawableCard && p2hand.cards[0] is DrawableCard) {
					((DrawableCard)p1hand.cards[0]).Destination = ((DrawableCard)p2hand.cards[0]).Destination = new Vector2(deckBounds.Bounds.X, deckBounds.Bounds.Y);
					((DrawableCard)p1hand.cards[0]).IsFaceUp = ((DrawableCard)p2hand.cards[0]).IsFaceUp = false;
					deck.AddToBottom(p1hand.DrawFromTop(1));
					deck.AddToBottom(p2hand.DrawFromTop(1));
				}
			}
		}

		protected void UpdateP1Deal(GameTime gameTime) {
			for (int i = 0; i < deck.GetCount(); i++) {
				((DrawableCard)deck.cards[i]).Destination = new Vector2(deckBounds.Bounds.X, deckBounds.Bounds.Y);
			}
			for (int i = 0; i < p1hand.GetCount(); i++) {
				((DrawableCard)p1hand.cards[i]).Destination = new Vector2(p1deckBounds[i].Bounds.X, p1deckBounds[i].Bounds.Y);
			}
			for (int i = 0; i < p2hand.GetCount(); i++) {
				((DrawableCard)p2hand.cards[i]).Destination = new Vector2(p2deckBounds[i].Bounds.X, p2deckBounds[i].Bounds.Y);
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 0) {
				messageBox.Text = "Tap the deck to deal the cards";

				if (deckBounds.IsTapped) {
					elapsedTime = new TimeSpan();
					subStep++;

					p1hand.AddToTop(deck.DrawFromTop(6));
					p2hand.AddToTop(deck.DrawFromTop(6));

					for (int i = 0; i < p1hand.cards.Count; i++) {
						if(p1hand.cards[i] is DrawableCard) {
							((DrawableCard)p1hand.cards[i]).Destination = new Vector2(p1deckBounds[i].Bounds.X, p1deckBounds[i].Bounds.Y);
							((DrawableCard)p1hand.cards[i]).IsFaceUp = true;
						}
						if (p2hand.cards[i] is DrawableCard) {
							((DrawableCard)p2hand.cards[i]).Destination = new Vector2(p2deckBounds[i].Bounds.X, p2deckBounds[i].Bounds.Y);
						}
					}
				}
			}
			// Two cards to the crib
			if (elapsedTime.TotalSeconds > 1 && subStep == 1) {
				messageBox.Text = "Tap two cards to send to the crib";

				for (int i = 0; i < p1hand.GetCount(); i++) {
					if (((TextButton)p1deckBounds[i]).IsTapped) {
						((DrawableCard)p1hand.cards[i]).Destination = new Vector2(cribBounds[crib.GetCount()].Bounds.X, cribBounds[crib.GetCount()].Bounds.Y);
						((DrawableCard)p1hand.cards[i]).IsFaceUp = false;
						crib.AddToTop(new List<Card>() { p1hand.Draw(p1hand.cards[i]) });
						//TODO: AI stuff
						switch (p2logic) {
							case AIType.Easy:
								DrawableCard chosen = (DrawableCard)p2hand.DrawFromTop(1)[0];
								chosen.Destination = new Vector2(cribBounds[crib.GetCount()].Bounds.X, cribBounds[crib.GetCount()].Bounds.Y);
								chosen.IsFaceUp = false;
								crib.AddToTop(new List<Card>() { chosen });
								break;
						}

						if (crib.GetCount() == 4) {
							subStep++;
							elapsedTime = new TimeSpan();
						}
						break;
					}
				}
			}
			// Cut the deck
			if (elapsedTime.TotalSeconds > 1 && subStep == 2) {
				elapsedTime = new TimeSpan();
				subStep++;
				messageBox.Text = "Player 2 cuts the deck";

				switch (p2logic) {
					case AIType.Easy:
					case AIType.Medium:
					case AIType.Hard:
						cutCard = deck.DrawFromTop(1)[0];

						if(cutCard.Number == 11) {
							P2Score++;
						}

						if (cutCard is DrawableCard) {
							((DrawableCard)cutCard).Destination = new Vector2(deckBounds.Bounds.X + 110, deckBounds.Bounds.Y);
							((DrawableCard)cutCard).IsFaceUp = true;
						}
						break;
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 3) {
				elapsedTime = new TimeSpan();
				subStep = 4;

				List<CribLib.Card> newHand = new List<CribLib.Card>();
				foreach (var card in p2hand.cards)
				{
					((DrawableCard)card).IsFaceUp = true;
					newHand.Add(new CribLib.Card(card.GetLetter()[0], card.GetSuiteLetter()[0]));
				}

				scores.Clear();
				CribLib.Hand.Count(newHand.ToArray(), new CribLib.Card(cutCard.GetLetter()[0], cutCard.GetSuiteLetter()[0]), scores, false);
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 4) {
				elapsedTime = new TimeSpan();
				if (scores.Count > 0) {
					messageBox.Text = "Player 2- " + scores[0].Name + " for " + scores[0].Score + "!";
					P2Score += scores[0].Score;
					scores.RemoveAt(0);
				}
				else {
					subStep = 5;
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 5) {
				elapsedTime = new TimeSpan();
				subStep = 6;

				List<CribLib.Card> newHand = new List<CribLib.Card>();
				foreach (var card in p1hand.cards) {
					((DrawableCard)card).IsFaceUp = true;
					newHand.Add(new CribLib.Card(card.GetLetter()[0], card.GetSuiteLetter()[0]));
				}

				scores.Clear();
				CribLib.Hand.Count(newHand.ToArray(), new CribLib.Card(cutCard.GetLetter()[0], cutCard.GetSuiteLetter()[0]), scores, false);
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 6) {
				elapsedTime = new TimeSpan();
				if (scores.Count > 0) {
					messageBox.Text = "Player 1- " + scores[0].Name + " for " + scores[0].Score + "!";
					P1Score += scores[0].Score;
					scores.RemoveAt(0);
				}
				else {
					subStep = 7;
				}
			}
			// Score Crib
			if (elapsedTime.TotalSeconds > 1 && subStep == 7) {
				elapsedTime = new TimeSpan();
				subStep = 8;

				List<CribLib.Card> newHand = new List<CribLib.Card>();
				foreach (var card in crib.cards) {
					((DrawableCard)card).IsFaceUp = true;
					newHand.Add(new CribLib.Card(card.GetLetter()[0], card.GetSuiteLetter()[0]));
				}

				scores.Clear();
				CribLib.Hand.Count(newHand.ToArray(), new CribLib.Card(cutCard.GetLetter()[0], cutCard.GetSuiteLetter()[0]), scores, false);
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 8) {
				elapsedTime = new TimeSpan();
				if (scores.Count > 0) {
					messageBox.Text = "Player 1 (Crib) - " + scores[0].Name + " for " + scores[0].Score + "!";
					P1Score += scores[0].Score;
					scores.RemoveAt(0);
				}
				else {
					subStep = 9;
				}
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 9) {
				elapsedTime = new TimeSpan();
				subStep = 10;

				trash.AddToTop(p1field.DrawFromTop(p1field.GetCount()));
				trash.AddToTop(p1hand.DrawFromTop(p1hand.GetCount()));
				trash.AddToTop(p2field.DrawFromTop(p2field.GetCount()));
				trash.AddToTop(p2hand.DrawFromTop(p2hand.GetCount()));
				trash.AddToTop(crib.DrawFromTop(crib.GetCount()));

				trash.AddToTop(new List<Card>() { cutCard });
				((DrawableCard)cutCard).Destination = new Vector2(deckBounds.Bounds.X, deckBounds.Bounds.Y);
				((DrawableCard)cutCard).IsFaceUp = false;
				//cutCard = null;

				foreach (var card in trash.cards) {
					((DrawableCard)card).IsFaceUp = false;
				}
				
				trash.Shuffle();

				deck.AddToBottom(trash.DrawFromTop(trash.GetCount()));

				messageBox.Text = "Player 2's turn!";
			}
			if (elapsedTime.TotalSeconds > 4 && subStep == 10) {
				stage = Stage.P2Deal;
				subStep = 0;
			}

			#region Old Game Thingy
			// Start the game thingy
			//if (elapsedTime.TotalSeconds > .5 && subStep == 3) {
			//    for (int i = 0; i < p1field.GetCount(); i++) {
			//        ((DrawableCard)p1field.cards[i]).Destination = new Vector2(p1fieldBounds[i].Bounds.X, p1fieldBounds[i].Bounds.Y);
			//        ((DrawableCard)p1field.cards[i]).IsFaceUp = true;
			//    }

			//    if ((p1hand.GetCount() + p2hand.GetCount()) == 0) {
			//        elapsedTime = new TimeSpan();
			//        subStep = 5;
			//    }
			//    else {
			//        subStep = 4;
			//    }
			//}
			//// Player 1
			//if (elapsedTime.TotalSeconds > .5 && subStep == 4) {
			//    for (int i = 0; i < p1hand.GetCount(); i++) {
			//        if (((TextButton)p1deckBounds[i]).IsTapped) {
			//            if ((p1hand.cards[i].GetCribValue() + runningTotal) <= 31) {
			//                Card cardToAdd = p2hand.DrawFromTop(1)[0];
			//                runningTotal += cardToAdd.GetCribValue();
			//                if (runningTotal == 15) {
			//                    P1Score += 2;
			//                }
			//                p1field.AddToTop(new List<Card>() { cardToAdd });
			//            }

			//            break;
			//        }
			//    }
			//}
			//// Player 2
			//if (elapsedTime.TotalSeconds > .5 && subStep == 5) {
			//    //TODO: AI stuff
			//    if ((p2hand.cards[p2hand.GetCount() - 1].GetCribValue() + runningTotal) <= 31) {
			//        Card cardToAdd = p2hand.DrawFromTop(1)[0];
			//        runningTotal += cardToAdd.GetCribValue();
			//        if (runningTotal == 15) {
			//            P2Score += 2;
			//        }
			//        p2field.AddToTop(new List<Card>() { cardToAdd });

			//        subStep = 3;
			//    }
			//}
			#endregion
		}

		protected void UpdateP2Deal(GameTime gameTime) {
			for (int i = 0; i < p1hand.GetCount(); i++) {
				((DrawableCard)p1hand.cards[i]).Destination = new Vector2(p1deckBounds[i].Bounds.X, p1deckBounds[i].Bounds.Y);
			}
			for (int i = 0; i < p2hand.GetCount(); i++) {
				((DrawableCard)p2hand.cards[i]).Destination = new Vector2(p2deckBounds[i].Bounds.X, p2deckBounds[i].Bounds.Y);
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 0) {
				messageBox.Text = "Player 2 deals the cards";

				elapsedTime = new TimeSpan();
				subStep++;

				p1hand.AddToTop(deck.DrawFromTop(6));
				p2hand.AddToTop(deck.DrawFromTop(6));

				for (int i = 0; i < p1hand.cards.Count; i++) {
					if (p1hand.cards[i] is DrawableCard) {
						((DrawableCard)p1hand.cards[i]).Destination = new Vector2(p1deckBounds[i].Bounds.X, p1deckBounds[i].Bounds.Y);
						((DrawableCard)p1hand.cards[i]).IsFaceUp = true;
					}
					if (p2hand.cards[i] is DrawableCard) {
						((DrawableCard)p2hand.cards[i]).Destination = new Vector2(p2deckBounds[i].Bounds.X, p2deckBounds[i].Bounds.Y);
					}
				}
				
			}
			// Two cards to the crib
			if (elapsedTime.TotalSeconds > 1 && subStep == 1) {
				messageBox.Text = "Tap two cards to send to the crib";

				for (int i = 0; i < p1hand.GetCount(); i++) {
					if (((TextButton)p1deckBounds[i]).IsTapped) {
						((DrawableCard)p1hand.cards[i]).Destination = new Vector2(cribBounds[crib.GetCount()].Bounds.X, cribBounds[crib.GetCount()].Bounds.Y);
						((DrawableCard)p1hand.cards[i]).IsFaceUp = false;
						crib.AddToTop(new List<Card>() { p1hand.Draw(p1hand.cards[i]) });
						//TODO: AI stuff
						switch (p2logic) {
							case AIType.Easy:
								DrawableCard chosen = (DrawableCard)p2hand.DrawFromTop(1)[0];
								chosen.Destination = new Vector2(cribBounds[crib.GetCount()].Bounds.X, cribBounds[crib.GetCount()].Bounds.Y);
								chosen.IsFaceUp = false;
								crib.AddToTop(new List<Card>() { chosen });
								break;
						}

						if (crib.GetCount() == 4) {
							subStep++;
							elapsedTime = new TimeSpan();
						}
						break;
					}
				}
			}
			// Cut the deck
			if (elapsedTime.TotalSeconds > 1 && subStep == 2) {
				messageBox.Text = "Tap the deck to cut the deck";

				if(deckBounds.IsTapped) {
					elapsedTime = new TimeSpan();
					subStep++;

					cutCard = deck.DrawFromTop(1)[0];

					if (cutCard.Number == 11) {
						P2Score++;
					}

					if (cutCard is DrawableCard) {
						((DrawableCard)cutCard).Destination = new Vector2(deckBounds.Bounds.X + 110, deckBounds.Bounds.Y);
						((DrawableCard)cutCard).IsFaceUp = true;
					}
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 3) {
				elapsedTime = new TimeSpan();
				subStep = 4;

				List<CribLib.Card> newHand = new List<CribLib.Card>();
				foreach (var card in p1hand.cards) {
					((DrawableCard)card).IsFaceUp = true;
					newHand.Add(new CribLib.Card(card.GetLetter()[0], card.GetSuiteLetter()[0]));
				}

				scores.Clear();
				CribLib.Hand.Count(newHand.ToArray(), new CribLib.Card(cutCard.GetLetter()[0], cutCard.GetSuiteLetter()[0]), scores, false);
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 4) {
				elapsedTime = new TimeSpan();
				if (scores.Count > 0) {
					messageBox.Text = "Player 1- " + scores[0].Name + " for " + scores[0].Score + "!";
					P1Score += scores[0].Score;
					scores.RemoveAt(0);
				}
				else {
					subStep = 5;
				}
			}

			if (elapsedTime.TotalSeconds > 1 && subStep == 5) {
				elapsedTime = new TimeSpan();
				subStep = 6;

				List<CribLib.Card> newHand = new List<CribLib.Card>();
				foreach (var card in p2hand.cards) {
					((DrawableCard)card).IsFaceUp = true;
					newHand.Add(new CribLib.Card(card.GetLetter()[0], card.GetSuiteLetter()[0]));
				}

				scores.Clear();
				CribLib.Hand.Count(newHand.ToArray(), new CribLib.Card(cutCard.GetLetter()[0], cutCard.GetSuiteLetter()[0]), scores, false);
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 6) {
				elapsedTime = new TimeSpan();
				if (scores.Count > 0) {
					messageBox.Text = "Player 2- " + scores[0].Name + " for " + scores[0].Score + "!";
					P2Score += scores[0].Score;
					scores.RemoveAt(0);
				}
				else {
					subStep = 7;
				}
			}
			// Score Crib
			if (elapsedTime.TotalSeconds > 1 && subStep == 7) {
				elapsedTime = new TimeSpan();
				subStep = 8;

				List<CribLib.Card> newHand = new List<CribLib.Card>();
				foreach (var card in crib.cards) {
					((DrawableCard)card).IsFaceUp = true;
					newHand.Add(new CribLib.Card(card.GetLetter()[0], card.GetSuiteLetter()[0]));
				}

				scores.Clear();
				CribLib.Hand.Count(newHand.ToArray(), new CribLib.Card(cutCard.GetLetter()[0], cutCard.GetSuiteLetter()[0]), scores, false);
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 8) {
				elapsedTime = new TimeSpan();
				if (scores.Count > 0) {
					messageBox.Text = "Player 2 (Crib) - " + scores[0].Name + " for " + scores[0].Score + "!";
					P2Score += scores[0].Score;
					scores.RemoveAt(0);
				}
				else {
					subStep = 9;
				}
			}
			if (elapsedTime.TotalSeconds > 1 && subStep == 9) {
				elapsedTime = new TimeSpan();
				subStep = 10;

				trash.AddToTop(p1field.DrawFromTop(p1field.GetCount()));
				trash.AddToTop(p1hand.DrawFromTop(p1hand.GetCount()));
				trash.AddToTop(p2field.DrawFromTop(p2field.GetCount()));
				trash.AddToTop(p2hand.DrawFromTop(p2hand.GetCount()));
				trash.AddToTop(crib.DrawFromTop(crib.GetCount()));

				trash.AddToTop(new List<Card>() { cutCard });
				((DrawableCard)cutCard).Destination = new Vector2(deckBounds.Bounds.X, deckBounds.Bounds.Y);
				((DrawableCard)cutCard).IsFaceUp = false;
				//cutCard = null;

				trash.Shuffle();

				deck.AddToBottom(trash.DrawFromTop(trash.GetCount()));

				messageBox.Text = "Player 1's turn!";
			}
			if (elapsedTime.TotalSeconds > 4 && subStep == 10) {
				stage = Stage.P1Deal;
				subStep = 0;
			}
		}

		public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime, InputState input) {
			PlayerIndex p;
			//TODO make save or something
			if (input.IsNewButtonPress(Buttons.Back, null, out p)) {
				LoadingScreen.Load(ScreenManager, new List<GameScreen>() { new GameBackground(Color.White), new MainMenu() });
			}

			#region Menu Item Event Stuff and Bounds Stuff
			foreach (var menuItem in menuItems) {
				if (menuItem is TextButton) {
					((TextButton)menuItem).IsTapped = false;
				}
			}

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
			if (cutCard is DrawableCard) {
				((DrawableCard)cutCard).Draw(ScreenManager, suiteIcons);
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

		void cardBounds_Tapped(object sender, MenuItemEventArgs e) {
			((TextButton)sender).IsTapped = true;
		}

		void board_Tapped(object sender, MenuItemEventArgs e) {
			ScreenManager.AddScreen(new BoardScreen(p1score, p2score), null);
		}
	}
}
