using System;
using System.Collections.Generic;
using System.Text;

namespace CribLib {
    /// <summary>
    /// Class to represent a playing card in a deck.  Implements System.IComparable so that hands
    /// (i.e. arrays of cards) can be sorted easily.
    /// </summary>
    public class Card : IComparable {
        #region Private Constants
        private static char[] _cardValues = new char[] {'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K'};
        private static char[] _suits = new char[] { 'S', 'H', 'C', 'D' };
        #endregion

        #region Private Members
        private Int32 _arrayIndex = Int32.MinValue; // The ordinal of the card in the deck (0 - 51)
        private char _name = char.MinValue;         // The name of the card ('A', '2', 'T', 'K', etc.)
        private char _suit = char.MinValue;         // The suit of the card {'S', 'H', 'C', 'D'}
        private Int32 _ordinal = Int32.MinValue;    // The ordinal of the card in the suit (0 - 12)
        private Int32 _value = Int32.MinValue;      // The counting value of the card (1 - 10)
        #endregion

        #region ctors
        /// <summary>
        /// Constructor to call when the array index of the card in the deck is known.
        /// </summary>
        /// <param name="card">An Int32 between 0 and 51, inclusive, describing the index
        /// of the card within the deck.  0 = Ace of Spades, 1 = 2 of Spades, 12 = King of Spades,
        /// 13 = Ace of Hearts, etc.</param>
        public Card(Int32 card) : this(_cardValues[card % 13], _suits[((Int32)card / 13)]) { }

        /// <summary>
        /// Constructor to call when the full name of the card is expressed as a string.  It can only
        /// be a string of length 2, though.  "T" is the name of the 10 card.  "TS" = Ten of Spades.
        /// "KD" = King of Diamonds.  "AH" = Ace of Hearts.
        /// </summary>
        /// <param name="card">The string of length 2 needed to initialize a card.  First character
        /// is the name of the card, the second is the suit.</param>
        public Card(string card) {
            if (card.Length != 2)
                throw new InvalidOperationException("Can only specify a 2 character string.");
            InitializeChars(card[0], card[1]);
        }

        /// <summary>
        /// Constructor to call when the name of the card is expressed as 2 chars.
        /// </summary>
        /// <param name="card">The char definine the name of the card {'A', '2', '9', 'T', 'K'} </param>
        /// <param name="suit">The char defining the suit {'S', 'H', 'C', 'D'}</param>
        public Card(char card, char suit) {
            InitializeChars(card, suit);
        }
        #endregion

        #region Public Properties
        public char Suit { get { return _suit; } }
        public char Name { get { return _name; } }
        public Int32 Ordinal { get { return _ordinal; } }
        public Int32 Value { get { return _value; } }
        public Int32 DeckIndex { get { return _arrayIndex; } }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Method to return a string representation of a card.
        /// </summary>
        /// <param name="specifier">If the char literal 'g' is passed as a parameter ('g' = 
        /// graphical), the suit of the card is replaced by the suit as a console character.</param>
        /// <returns>The name of the card, and a suit character or the suit char literal.</returns>
        public string ToString(char specifier) {
            if (specifier == 'g') {
                switch (_suit) { 
                    case 'S'    :
                        return _name.ToString() + (char)6;
                    case 'H'    :
                        return _name.ToString() + (char)3;
                    case 'D'    :
                        return _name.ToString() + (char)4;
                    case 'C'    :
                        return _name.ToString() + (char)5;
                }
            }

            return this.ToString();
        }

        /// <summary>
        /// Method to return a string representation of a card. 
        /// </summary>
        /// <returns>The name of the card and its suit.</returns>
        public override string ToString() {
            return _name.ToString() + _suit.ToString();
        }

        /// <summary>
        /// Determines whether two Object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare to this.</param>
        /// <returns>true if the two objects describe the same card, false otherwise.</returns>
        public override bool Equals(object obj) {
            Card c = (Card)obj;

            return (c.Name == _name && c.Suit == _suit);
        }
        #endregion

        /// <summary>
        /// Initialize some calculated variable values after client code has insantiated this
        /// </summary>
        /// <param name="card">The name of the card to initialize with.</param>
        /// <param name="suit">The suit of the card to initialize with.</param>
        private void InitializeChars(char card, char suit) {
            Int32 cardIndex = 0;

            for (Int32 i = 0; i < _cardValues.Length; i++) {
                if (_cardValues[i] == card) {
                    _name = card;
                    _ordinal = i;
                    cardIndex = i;

                    if (_ordinal < 9)
                        _value = _ordinal + 1;
                    else
                        _value = 10;

                    break;
                }
            }

            if (_name == char.MinValue)
                throw new InvalidOperationException(_name.ToString() + " is not a valid card type");

            for (Int32 i = 0; i < _suits.Length; i++) {
                if (_suits[i] == suit) {
                    _suit = suit;
                    _arrayIndex = (i * 13) + cardIndex;

                    break;
                }
            }

            if (_suit == char.MinValue)
                throw new InvalidOperationException(_suit.ToString() + " is not a valid suit type");
        }

        #region IComparable Members
        /// <summary>
        /// Interface implementation to allow the sorting of arrays and arraylists of this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj) {
            Int32 ordinal = ((Card)obj).Ordinal;

            if (_ordinal < ordinal)
                return -1;
            else if (_ordinal > ordinal)
                return 1;
            else
                return 0;
        }
        #endregion
    }
}
