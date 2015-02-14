using System;
using System.Collections.Generic;
using System.Text;

namespace CribLib {
    /// <summary>
    /// Class used to contain the static methods that can be called to score and evaluate a hand.
    /// </summary>
    public class Hand {
        #region Binary Sets
        //  1 - {1, 0, 0, 0, 0}     Set of 1
        //  2 - {0, 1, 0, 0, 0}     Set of 1
        //  3 - {1, 1, 0, 0, 0}     Set of 2
        //  4 - {0, 0, 1, 0, 0}     Set of 1
        //  5 - {1, 0, 1, 0, 0}     Set of 2
        //  6 - {0, 1, 1, 0, 0}     Set of 2
        //  7 - {1, 1, 1, 0, 0}     Set of 3
        //  8 - {0, 0, 0, 1, 0}     Set of 1
        //  9 - {1, 0, 0, 1, 0}     Set of 2
        // 10 - {0, 1, 0, 1, 0}     Set of 2
        // 11 - {1, 1, 0, 1, 0}     Set of 3
        // 12 - {0, 0, 1, 1, 0}     Set of 2
        // 13 - {1, 0, 1, 1, 0}     Set of 3
        // 14 - {0, 1, 1, 1, 0}     Set of 3
        // 15 - {1, 1, 1, 1, 0}     Set of 4
        // 16 - {0, 0, 0, 0, 1}     Set of 1
        // 17 - {1, 0, 0, 0, 1}     Set of 2
        // 18 - {0, 1, 0, 0, 1}     Set of 2
        // 19 - {1, 1, 0, 0, 1}     Set of 3
        // 20 - {0, 0, 1, 0, 1}     Set of 2
        // 21 - {1, 0, 1, 0, 1}     Set of 3
        // 22 - {0, 1, 1, 0, 1}     Set of 3
        // 23 - {1, 1, 1, 0, 1}     Set of 4
        // 24 - {0, 0, 0, 1, 1}     Set of 2
        // 25 - {1, 0, 0, 1, 1}     Set of 3
        // 26 - {0, 1, 0, 1, 1}     Set of 3
        // 27 - {1, 1, 0, 1, 1}     Set of 4
        // 28 - {0, 0, 1, 1, 1}     Set of 3
        // 29 - {1, 0, 1, 1, 1}     Set of 4
        // 30 - {0, 1, 1, 1, 1}     Set of 4
        // 31 - {1, 1, 1, 1, 1}     Set of 5
        #endregion

        /// <summary>
        /// Create a 
        /// </summary>
        private static Int32[][][] _setMatrix = new Int32[][][] {
            new Int32[][] {},  // Empty, no 0-length sets
            new Int32[][] {new Int32[] {0},     // Sets of 1 card.
                           new Int32[] {1},     // Not necessary; not used by the library,
                           new Int32[] {2},     // but included for clarity of thought.
                           new Int32[] {3},
                           new Int32[] {4}},               
            new Int32[][] {new Int32[] {0, 1},          // Sets of 2
                           new Int32[] {0, 2},
                           new Int32[] {0, 3},
                           new Int32[] {0, 4},
                           new Int32[] {1, 2},
                           new Int32[] {1, 3},
                           new Int32[] {1, 4},
                           new Int32[] {2, 3},
                           new Int32[] {2, 4},
                           new Int32[] {3, 4}}, 
            new Int32[][] {new Int32[] {0, 1, 2},       // Sets of 3
                           new Int32[] {0, 1, 3},
                           new Int32[] {0, 2, 3},
                           new Int32[] {1, 2, 3},
                           new Int32[] {0, 1, 4},
                           new Int32[] {0, 2, 4},
                           new Int32[] {1, 2, 4},
                           new Int32[] {0, 3, 4},
                           new Int32[] {1, 3, 4},
                           new Int32[] {2, 3, 4}},
            new Int32[][] {new Int32[] {0, 1, 2, 3},    // Sets of 4
                           new Int32[] {0, 1, 2, 4},
                           new Int32[] {0, 1, 3, 4},
                           new Int32[] {0, 2, 3, 4},
                           new Int32[] {1, 2, 3, 4}},
            new Int32[][] {new Int32[] {0, 1, 2, 3, 4}}};   // Set of 5

        /// <summary>
        /// Method to call to score a hand full of cards.
        /// </summary>
        /// <param name="hand">An array of Card objects describing a player's hand.</param>
        /// <param name="cutCard">The cut, or community, card shared by both players</param>
        /// <param name="scoringPlays">An ArrayList of ScoringPlay objects, for display
        /// to the player to justify the scoring.</param>
        /// <param name="isCrib">A bool that determines whether the cards will be scored
        /// as a "regular" hand or as a crib.  Slight difference in scoring, explained below.</param>
        /// <returns></returns>
        public static Int32 Count(Card[] hand, Card cutCard, List<ScoreSet> scoringPlays, bool isCrib) {
            Int32 score = 0;

            // Do basic validation
            if (hand.Length != 4)
                throw new InvalidOperationException("A hand must have 4 cards in it.");

            // Append the cut card to the hand strictly for hand scoring purposes.
            Card[] fullHand = new Card[5];
            for(Int32 i = 0; i < 4; i++)
                fullHand[i] = hand[i];

            fullHand[4] = cutCard;

            // Sort the hand to make looking for runs easier.
            Array.Sort(fullHand);

            // Validate the hand.  Ensure no two cards are the same.
            // Look through the sets of indicies in the "2" set.
            for (Int32 setLength = 0; setLength < _setMatrix[2].Length; setLength++)
                // Check the two cards pointed at by the indicies and see if they're the same             
                if (fullHand[_setMatrix[2][setLength][0]].Equals(fullHand[_setMatrix[2][setLength][1]]))
                    throw new InvalidOperationException("Invalid hand (duplicate cards in the hand).");

            // Actually do the counting.
            score = Count15s(fullHand, scoringPlays);
            score += CountRuns(fullHand, scoringPlays);
            score += CountPairs(fullHand, scoringPlays);

            if (isCrib)
                score += CountFlush(fullHand, scoringPlays);
            else
                if (hand[0].Suit == cutCard.Suit)
                    score += CountFlush(fullHand, scoringPlays);
                else
                    score += CountFlush(hand, scoringPlays);

            score += CountNobs(hand, cutCard, scoringPlays);

            return score;
        }


        private static Int32 CountRuns(Card[] hand, List<ScoreSet> scoringPlays) {
            bool runFound = false;
            Int32 score = 0;

            // Look for 5 card runs first, then 4 then 3.
            for (Int32 i = 5; i > 2; i--) {
                Int32[][] sets = _setMatrix[i];

                // Iterate over each of the sets available for the length under examination.
                for (Int32 setIndex = 0; setIndex < sets.Length; setIndex++) {
                    bool localRunFound = true;

                    // Look at each each index in the set, from the first to the second-
                    // to-last.
                    for (Int32 setMember = 0; setMember < sets[setIndex].Length - 1; setMember++) {
                        Card first = hand[sets[setIndex][setMember]];
                        Card second = hand[sets[setIndex][setMember + 1]];

                        // Check to see if the second card is only 1 more than the current
                        // card.  If it isn't, skip out of the loop.  The two cards are
                        // not consecutive, so we can't have a run of this length.
                        if (second.Ordinal - first.Ordinal != 1) {
                            localRunFound = false;
                            break;
                        }
                    }

                    // If we're this far and the localRunFound flag has not been reset, 
                    // the current set defines a group of array indicies that are a run.
                    if (localRunFound) {
                        Card[] localRun = new Card[i];

                        // Copy the cards of the set into an array.
                        for (Int32 setMember = 0; setMember < i; setMember++)
                            localRun[setMember] = hand[sets[setIndex][setMember]];

                        scoringPlays.Add(new ScoreSet(localRun, "Run of " + i.ToString(), i));
                        score += i;
                        runFound = true;
                    }
                }

                // If a run has been found, don't look at smaller runs.
                if (runFound)
                    break;
            }

            return score;
        }

        /// <summary>
        /// Method to call to determine if the hand is entitled to a point for nobs.
        /// The hand gets 1 point if it has a Jack in the same suit as the cut card.
        /// </summary>
        /// <param name="hand">An array of Card objects that constitute a crib hand.</param>
        /// <param name="cutCard">A Card object representing the cut card.</param>
        /// <param name="scoringPlays">A List of scoring plays accumulated by the hand.</param>
        /// <returns>1 if the hand has a Jack of the same suit as the cut card, or
        /// 0 if not.</returns>
        private static Int32 CountNobs(Card[] hand, Card cutCard, List<ScoreSet> scoringPlays) {
            // Loop over all the cards in the hand, check to see if it's a Jack, and if it
            // is check the suit.
            for (Int32 i = 0; i < hand.Length; i++) {
                if (hand[i].Name == 'J' && hand[i].Suit == cutCard.Suit) {
                    scoringPlays.Add(new ScoreSet(new Card[] { hand[i], cutCard }, "Nobs", 1));
                    return 1;
                }
            }

            // We haven't left the routine yet, so the right jack is not in the hand.
            return 0;
        }

        /// <summary>
        /// A method to determine if the hand is entitled to points for a flush.
        /// A flush is a hand all of the same suit.
        /// </summary>
        /// <param name="hand">The hand to analyze.</param>
        /// <param name="scoringPlays">A List of scoring plays accumulated by the hand.</param>
        /// <returns>A single point for each card of the flush, if one exists, or 0
        /// if no flush is present.</returns>
        private static Int32 CountFlush(Card[] hand, List<ScoreSet> scoringPlays) {
            char suit = hand[0].Suit;

            // Check the suit of each Card in the hand against the suit of the first
            // card.  As soon as they don't match, skip out of the routine.
            for (Int32 i = 1; i < hand.Length; i++)
                if (hand[i].Suit != suit)
                    return 0;

            // If we're here, a flush exists.
            scoringPlays.Add(new ScoreSet(hand, "Flush", hand.Length));

            return hand.Length;
        }

        /// <summary>
        /// A method to call to count the number of pairs in the hand.
        /// </summary>
        /// <param name="hand">An array of Card objects to check for pairs.</param>
        /// <param name="scoringPlays">A List of scoring plays accumulated by the hand.</param>
        /// <returns>2 points for every pair in the hand.</returns>
        private static Int32 CountPairs(Card[] hand, List<ScoreSet> scoringPlays) {
            Int32 score = 0;

            // Look through the sets of indicies in the "2" set.
            for (Int32 setLength = 0; setLength < _setMatrix[2].Length; setLength++) {
                // Check the two cards pointed at by the indicies and see if they're the same             
                if (hand[_setMatrix[2][setLength][0]].Name == hand[_setMatrix[2][setLength][1]].Name) {
                    scoringPlays.Add(new ScoreSet(new Card[] { hand[_setMatrix[2][setLength][0]], hand[_setMatrix[2][setLength][1]] }, "Pair", 2));
                    score += 2;
                }
            }

            return score;
        }

        /// <summary>
        /// A method to call to count the number of 15s in the hand.
        /// </summary>
        /// <param name="hand">An array of Card objects to check for pairs.</param>
        /// <param name="scoringPlays">A List of scoring plays accumulated by the hand.</param>
        /// <returns>2 points for every pair in the hand.</returns>
        private static Int32 Count15s(Card[] hand, List<ScoreSet> scoringPlays) {
            Int32 score = 0;

            // Look through the "2" sets, then the 3, 4 and 5
            for (Int32 setIndex = 2; setIndex < 6; setIndex++) {
                for (Int32 setLength = 0; setLength < _setMatrix[setIndex].Length; setLength++ ) {
                    Int32 sum = 0;

                    // Add up the cards pointed to by the indicies of the set.
                    for (Int32 setMember = 0; setMember < _setMatrix[setIndex][setLength].Length; setMember++ ) 
                        sum += hand[_setMatrix[setIndex][setLength][setMember]].Value;

                    if (sum == 15) {
                        // They added up to 15.  Make note of the cards.
                        Card[] temp = new Card[_setMatrix[setIndex][setLength].Length];
                        for (Int32 setMember = 0; setMember < _setMatrix[setIndex][setLength].Length; setMember++)
                            temp[setMember] = hand[_setMatrix[setIndex][setLength][setMember]];

                        // Add two points for a 15 to the list of scoring plays.
                        scoringPlays.Add(new ScoreSet(temp, "15", 2));
                        score += 2;
                    }
                }
            }

            return score;
        }
    }
}
