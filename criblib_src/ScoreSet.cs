using System;
using System.Collections.Generic;
using System.Text;

namespace CribLib {
    public class ScoreSet {
        private Card[] _cards = null;
        private Int32 _score = 0;
        private string _name = string.Empty;

        public ScoreSet(Card[] cards, string name, Int32 score) {
            _cards = cards;
            _name = name;
            _score = score;
        }

        public Card[] Cards { get { return _cards; } }
        public string Name { get { return _name; } }
        public Int32 Score { get { return _score; } }

        public string ToString(char specifier) {
            string result = _name + " (";

            foreach (Card c in _cards)
                result += c.ToString(specifier) + " ";

            result = result.Substring(0, result.Length - 1) + ") for " + _score.ToString();

            return result;
        }
        public override string ToString() {
            string result = _name + " (";

            foreach (Card c in _cards)
                result += c.ToString() + " ";

            result = result.Substring(0, result.Length - 1) + ") for " + _score.ToString();

            return result;
        }
    }
}
