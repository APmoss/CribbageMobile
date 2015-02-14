using System;
using System.Collections.Generic;
using System.Text;

namespace CribLib {
    /// <summary>
    /// A class to track the cards played in a deck, and to determine the next card that should
    /// come off the deck (for dealing or cutting).
    /// </summary>
    public class Deck {
        #region Private Members
        private bool[] _deck = new bool[52];
        private Random _r = new Random();
        #endregion

        #region Public Methods
        /// <summary>
        /// Method called to re-initialize the deck.  Reset all the elements of the bool array
        /// so the deck has the full complement of cards to deal.
        /// </summary>
        public void Shuffle() {
            _deck = new bool[52];
        }
        
        /// <summary>
        /// Method to call to get a card from the top of the virtual deck.
        /// </summary>
        /// <returns></returns>
        public Int32 NextCard() {
            Int32 nextCard = -1;

            while (nextCard == -1) {
                // Pick a card
                Int32 i = _r.Next(52);

                // This card been dealt?
                if (!_deck[i]) {
                    // No.  Pull it from the deck...
                    _deck[i] = true;

                    // ... and return it to the user.
                    nextCard = i;
                }
            } // Exit the loop when nextCard != -1, meaning an undealt card has been found.

            return nextCard;
        }
        #endregion
    }
}
