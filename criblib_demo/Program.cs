using System;
using System.Collections.Generic;
using System.Text;

using CribLib;

namespace CribLibHandCounter {
    class Program {
        static void Main(string[] args) {
            List<ScoreSet> scoringPlays = new List<ScoreSet>();
            Card[] hand = new Card[4];
            Card cut = null;
            bool crib = false;

            if (args.Length == 0) { 
                // Pick a random set of cards to evaluate.
                Deck d = new Deck();
                for (Int32 i = 0; i < 4; i++)
                    hand[i] = new Card(d.NextCard());

                Array.Sort(hand);

                cut = new Card(d.NextCard());
                System.Console.Write("Hand: ");

                for (Int32 i = 0; i < hand.Length; i++)
                    ShowCard(hand[i]);

                System.Console.Write(Environment.NewLine + "Cut:  ");
                ShowCard(cut);
                System.Console.WriteLine(string.Empty);
            } else {
                // The user has specified cards
                hand[0] = new Card(args[0]);
                hand[1] = new Card(args[1]);
                hand[2] = new Card(args[2]);
                hand[3] = new Card(args[3]);

                cut = new Card(args[4]);
            }

            // Does the user want to score this hand as a crib?
            if (args.Length == 6 && args[5].ToLower() == "crib")
                crib = true;

            // Score the hand
            Int32 score = Hand.Count(hand, cut, scoringPlays, crib);

            Array.Sort(hand);

            // Show the results of scoring the hand.
            System.Console.WriteLine(Environment.NewLine + "Scoring:");

            foreach (ScoreSet ss in scoringPlays) {
                System.Console.Write(ss.Name.PadRight(8, ' ') + "  ( ");
                foreach (Card c in ss.Cards) 
                    ShowCard(c);

                System.Console.WriteLine(") for " + ss.Score.ToString());
            }

            System.Console.WriteLine(Environment.NewLine + "Total: " + score.ToString());
        }

        private static void ShowCard(Card card) {
            ConsoleColor fore = Console.ForegroundColor;
            ConsoleColor back = Console.BackgroundColor;

            if (card.Suit == 'S' || card.Suit == 'C')
                Console.ForegroundColor = ConsoleColor.Black;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" " + card.ToString('g') + " ");

            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
        }
    }
}
