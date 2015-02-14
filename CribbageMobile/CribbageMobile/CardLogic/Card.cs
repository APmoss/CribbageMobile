using System;
using System.Collections.Generic;

namespace CribbageMobile.CardLogic {
	/// <summary>
	/// The four basic suites in a standard deck of cards
	/// </summary>
	public enum Suite {
		Spades, Hearts, Clubs, Diamonds
	}

	/// <summary>
	/// A single card that contains a suite and a number
	/// </summary>
	class Card {
		protected Suite suite;
		protected int number;

		/// <summary>
		/// Create a new card with the specified data
		/// </summary>
		/// <param name="suite"></param>
		/// <param name="number"></param>
		public Card(Suite suite, int number) {
			this.suite = suite;
			this.number = number;
		}

		#region Public Methods

		public bool IsAce() {
			if (number == 1) {
				return true;
			}

			return false;
		}

		public bool IsNumberCard() {
			if (number >= 2 && number <= 10) {
				return true;
			}

			return false;
		}

		public bool IsFaceCard() {
			if (number >= 11 && number <= 13) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the long name of the card (Ace of Spades, 3 of Hearts, etc.)
		/// </summary>
		/// <returns></returns>
		public string GetLongName() {
			string valueName = string.Empty;

			if (number == 1) {
				valueName = "Ace";
			}
			else if (number == 11) {
				valueName = "Jack";
			}
			else if (number == 12) {
				valueName = "Queen";
			}
			else if (number == 13) {
				valueName = "King";
			}
			else {
				valueName = number.ToString();
			}

			return string.Format("{0} of {1}", valueName, suite);
		}

		/// <summary>
		/// Returns the letter name of the card (A, 1, 2, 3, J, Q, etc.)
		/// </summary>
		/// <returns></returns>
		public string GetLetter() {
			if (number == 1) {
				return "A";
			}
			else if (number == 11) {
				return "J";
			}
			else if (number == 12) {
				return "Q";
			}
			else if (number == 13) {
				return "K";
			}
			else {
				return number.ToString();
			}
		}

		#endregion
	}
}
