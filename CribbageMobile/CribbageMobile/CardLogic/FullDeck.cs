using System;
using System.Collections.Generic;

namespace CribbageMobile.CardLogic {
	/// <summary>
	/// A standard deck of 52 cards (without jokers)
	/// </summary>
	class FullDeck : CardStack {
		public FullDeck(bool makeDeckDrawable) {
			cards.Clear();

			// Add all cards to deck in order (not shuffled)
			if (makeDeckDrawable) {
				// Drawable Cards rather than normal ones
				for (int i = 1; i <= 13; i++) {
					cards.Add(new DrawableCard(Suite.Spades, i));
					cards.Add(new DrawableCard(Suite.Hearts, i));
					cards.Add(new DrawableCard(Suite.Clubs, i));
					cards.Add(new DrawableCard(Suite.Diamonds, i));
				}
			}
			else {
				// Normal cards
				for (int i = 1; i <= 13; i++) {
					cards.Add(new Card(Suite.Spades, i));
					cards.Add(new Card(Suite.Hearts, i));
					cards.Add(new Card(Suite.Clubs, i));
					cards.Add(new Card(Suite.Diamonds, i));
				}
			}
		}
	}
}
