using System;
using System.Collections.Generic;

namespace CribbageMobile.CardLogic {
	/// <summary>
	/// A collection of cards that handles much of the basic card organization logic.
	/// </summary>
	class CardStack {
		/// <summary>
		/// Change to allow/disallow duplicate cards in the stack.
		/// </summary>
		bool allowDuplicates = false;

		/// <summary>
		/// The internal collection of cards. Should never be changed directly outside of the class.
		/// </summary>
		public List<Card> cards = new List<Card>();

		#region Public Methods

		/// <summary>
		/// Simply returns the number of cards in the stack.
		/// </summary>
		/// <returns></returns>
		public int GetCount() {
			return cards.Count;
		}

		/// <summary>
		/// Basic card shuffle using the Fisher-Yates algorithm.
		/// </summary>
		public void Shuffle() {
			Random r = new Random();

			int num = cards.Count;
			while (num > 1) {
				num--;
				int choice = r.Next(num + 1);
				Card temp = cards[choice];
				cards[choice] = cards[num];
				cards[num] = temp;
			}
		}

		/// <summary>
		/// Returns whether the stack contains the card specified.
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public bool HasCard(Card card) {
			return cards.Contains(card);
		}

		/// <summary>
		/// Return and remove the top number of cards from the stack.
		/// </summary>
		/// <param name="numberToDraw"></param>
		/// <returns></returns>
		public List<Card> DrawFromTop(int numberToDraw) {
			List<Card> cardsDrawn = new List<Card>();

			cardsDrawn.AddRange(cards.GetRange(cards.Count - numberToDraw, numberToDraw));

			cards.RemoveRange(cards.Count - numberToDraw, numberToDraw);

			return cardsDrawn;
		}

		/// <summary>
		/// Adds the cards to the top of the stack.
		/// </summary>
		/// <param name="cardsToAdd"></param>
		public void AddToTop(List<Card> cardsToAdd) {
			cards.AddRange(cardsToAdd);
		}

		/// <summary>
		/// Adds the cards to the bottom of the stack
		/// </summary>
		/// <param name="cardsToAdd"></param>
		public void AddToBottom(List<Card> cardsToAdd) {
			List<Card> tempStack = new List<Card>();

			tempStack.AddRange(cardsToAdd);
			tempStack.AddRange(cards);

			cards = tempStack;
		}

		#endregion
	}
}
