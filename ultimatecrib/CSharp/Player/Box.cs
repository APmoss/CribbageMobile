// Ultimate Cribbage
// Player Assembly

// Copyright (C) 2003 - Keith Westley <keithsw1111@hotmail.com>

// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Diagnostics;
using Cards;
using System.Drawing;
using Layouts;

namespace Player
{
	/// <summary>
	/// Represents the Box or Crib.
	/// </summary>
	public class Box : BoxPlayer
	{
      #region Member Variables
      CribbagePlayer _boxOwner = null; // the owner of the crib
      #endregion

      #region Constructors
      /// <summary>
      /// Creates a box owned by a player
      /// </summary>
      /// <param name="owner">Owner of the box</param>
      public Box(CribbagePlayer owner)
      {
         _boxOwner = owner;

         Debug.Assert(_boxOwner != null, "You must specify a crib owner");
      }
      #endregion

      #region Private static Functions
      /// <summary>
      /// Draws outlined text
      /// </summary>
      /// <param name="s">String to draw</param>
      /// <param name="graphics">Surface to draw on</param>
      /// <param name="font">font to draw with</param>
      /// <param name="point">where to draw</param>
      /// <param name="textColour">text colour</param>
      /// <param name="backColour">outline colour</param>
      protected static void DrawStringWithBorder(string s, Graphics graphics, Font font, Point point, Color textColour, Color backColour)
      {
         SolidBrush textBrush = new SolidBrush(textColour);
         SolidBrush backBrush = new SolidBrush(backColour);

         graphics.DrawString(s, font, backBrush, new Point(point.X-1, point.Y-1));
         graphics.DrawString(s, font, backBrush, new Point(point.X-1,point.Y+ 1));
         graphics.DrawString(s, font, backBrush, new Point(point.X+1, point.Y-1));
         graphics.DrawString(s, font, backBrush, new Point(point.X+1, point.Y+1));
         graphics.DrawString(s, font, textBrush, point);
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// Indicates this is a box as opposed to a player
      /// </summary>
      public override bool IsBox
      {
         get
         {
            return true;
         }
      }

      /// <summary>
      /// Gets the box owner
      /// </summary>
      public CribbagePlayer Owner
      {
         get
         {
            return _boxOwner;
         }
      }

      /// <summary>
      /// Gets a string representation of the cards in the crib
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         Debug.Assert(_hand.Count > 0, "Why would I be asking for the box cards names when there are no cards?");
         
         return CardNames;
      }

      /// <summary>
      /// Draws the crib on the game surface
      /// </summary>
      /// <param name="graphics">Surface to draw on</param>
      public void Display(Graphics graphics)
      {
         // get the box display location & orientation for this player
         int y = _boxOwner.CribY;
         int x = _boxOwner.CribX;
         DisplayableCard.ORIENTATION o = _boxOwner.CribOrientation;

         // draw the cards
         _hand.Display(graphics, x, y, o);

         // get where to display the box text for this player
         x = _boxOwner.CribTextX;
         y = _boxOwner.CribTextY;

         // draw the owners name
         DrawStringWithBorder(_boxOwner.ToString() + "'s Crib", graphics, new Font("System", 12), new Point(x,y), Color.FromName((string)Layout.TheLayout.GetValue("TableTextColour")), Color.FromName((string)Layout.TheLayout.GetValue("TableColour")));
      }
      #endregion

      #region Stuff that belongs elsewhere but I dont want to loose it
      /*
      #region Static Variables
      // I got this data from someones cribbage discard research years ago and I dont remember
      // who it was. Basically it represents the frequency that a given card is discarded by the
      // dealer and non dealer over a set of games. This data can be used by probability algorithms
      // to work out likely crib contents
      static int[,] __dealerDiscardFrequency = new int[,] 
                     {
                           {2111, 3089, 4010, 3180, 786,  1242, 1384, 1518, 1082, 1202, 1198, 1265, 1688},
                     {0,    2757, 7554, 4204, 1040, 1924, 3145, 2869, 1981, 1227, 1161, 1355, 2000},
                     {0,    0,    2625, 4309, 1150, 1787, 2295, 2496, 1736, 1100, 1000, 1304, 1947},
                     {0,    0,    0,    1833, 946,  915,  1519, 1309, 964,  792,  807,  1036, 1460},
                     {0,    0,    0,    0,    666,  1335, 1743, 1235, 935,  2297, 2282, 2522, 2960},
                     {0,    0,    0,    0,    0,    2332, 5584, 6022, 5976, 1105, 885,  574,  1024},
                     {0,    0,    0,    0,    0,    0,    3103, 9835, 5201, 661,  1060, 1061, 1860},
                     {0,    0,    0,    0,    0,    0,    0,    3478, 5615, 2355, 777,  867,  936 },
                     {0,    0,    0,    0,    0,    0,    0,    0,    2580, 3466, 1474, 679,  870 },
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    1570, 4689, 2306, 1728},
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    1017, 5211, 4078},
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    1902, 5045},
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    2383}
                     };
      static int __dealerTotal = 201441;

      static int[,] __nonDealerDiscardFrequency = new int[,] 
                     {
                           {505,  2358, 2050, 1033, 254,  1915, 2112, 2750, 2581, 3217, 2524, 2452,  4726 },
                     {0,    530,  445,  1785, 317,  2569, 3919, 4206, 3588, 2618, 1760, 3740,  4522 },
                     {0,    0,    345,  1040, 160,  3006, 2459, 3370, 3002, 2112, 1462, 3132,  3702 },
                     {0,    0,    0,    273,  149,  627,  2428, 1755, 1922, 2276, 1754, 3203,  3908 },
                     {0,    0,    0,    0,    28,   110,  84,   359,  392,  171,  156,  153,   153  },
                     {0,    0,    0,    0,    0,    329,  1089, 2500, 1352, 2791, 1431, 3358,  4548 },
                     {0,    0,    0,    0,    0,    0,    342,  661,  3984, 3508, 1739, 4525,  5600 },
                     {0,    0,    0,    0,    0,    0,    0,    712,  2472, 1417, 2779, 4341,  5881 },
                     {0,    0,    0,    0,    0,    0,    0,    0,    818,  944,  1645, 5006,  5883 },
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    434,  1441, 4531,  11128},
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    243,  1575,  2316 },
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    626,   6899 },
                     {0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,     771  },
      };
      static int __nonDealerTotal = 201786;
      #endregion

      #region Static Functions
      /// <summary>
      /// Calculate the cribs potential worth using statistical probability
      /// ***************************************************************************************
      /// ***** THIS FUNCTION DOES NOT BELONG HERE. IT SHOULD BE IN THE STRATEGY CLASSES ********
      /// ***** BUT THESE HAVE NOT BEEN WRITTEN YET                                      ********
      /// ***************************************************************************************
      /// </summary>
      /// <param name="guaranteedCribValue">Returned value for the definite value of the box</param>
      /// <param name="speculatedCribValue">Returned value for the statistical extra value of the box</param>
      /// <param name="cardOdds">A list of known card odds for each type of card taking into account cards we know about</param>
      /// <param name="card1">First card being discarded</param>
      /// <param name="card2">Second card being discarded</param>
      /// <param name="opponentOwnsCrib">true if the players opponent owns the crib</param>
      /// <param name="score">Players current score</param>
      public static void CalculateCribWorth(out int guaranteedCribValue, out double speculatedCribValue, CardOdds cardOdds, CCard card1, CCard card2, bool fOpponentOwnsCrib, int iPlayerScore)
      {
         // start with a value of zero
         guaranteedCribValue = 0;
         speculatedCribValue = 0;

         // if the players score has already exceeded the crib invalid after score then just return the zeros
         if (iPlayerScore >= Config.TheConfig.GetValue("ultimatecrib", "CribInvalid", 121))
         {
            return;
         }

         int cardsToDeal = Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6);

         // create a card list with our known cards
         CribCardList ccl = new CribCardList();
         Debug.Assert(card1 != null);
         ccl.Add(card1);
         if (card2 != null)
         {
            ccl.Add(card2);
         }

         // create some cards to test with
         Card[] cardArray = new Card[13];
   
         for (int i = 0; i < 13; i++)
         {
            cardArray[i] = new Card(i+1, CCard.SUIT.CLUBS);
         }

         // score the guarenteed portion
         guaranteedCribValue = ccl.Count15sFast() * 2 + ccl.CountPairsFast() * 2;

         if (Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2) == 2)
         {
            Debug.Assert(card2 != null, "Only one card when 2 required.");

            double[] probabilityCardInPlayerHand = new double[13];

            for (int i = 0; i < 13; i++)
            {
               // calculate the prob the cards were in the players hand
               probabilityCardInPlayerHand[i] = cardOdds.GetFaceValueProbability(i+1, cardsToDeal);
            }

            // for each possible crib discard
            for (int i = 0; i < 13; i++)
            {
               for (int j = i; j < 13; j++)
               {
                  // calculate the probability the card would be discarded
                  double probabilityDiscarded;
                  if (opponentOwnsCrib)
                  {
                     probabilityDiscarded = (double)__dealerDiscardFrequency[i,j] / (double)__dealerTotal;
                  }
                  else
                  {
                     probabilityDiscarded = (double)__nonDealerDiscardFrequency[i,j] / (double)__nonDealerTotal;
                  }

                  Debug.Assert(probabilityDiscarded != 0);

                  // calculate the prob the cards were in the players hand
                  double probabilityInPlayerHand = probabilityCardInPlayerHand[i] * probabilityCardInPlayerHand[j];

                  // no point looking any further if it is impossible
                  if (probabilityInPlayerHand != 0)
                  {
                     cardOdds.RemoveCard(j+1);
                     ccl.Add(cardArray[j]);

                     cardOdds.RemoveCard(i+1);
                     ccl.Add(cardArray[i]);

                     // for each possible starter card
                     for (int k = 0; k < 13; k++)
                     {
                        // calculate probability of starter card (given possible crib discard)
                        double starterProbability = cardOdds.GetFaceValueProbability(k+1, 1);

                        // no point evaluating it if it is impossible
                        if (starterProbability != 0)
                        {

                           // now create a suitable hand
                           ccl.Add(cardArray[k]);

                           int cribScore = ccl.Count15sFast() * 2 +
                              ccl.CountPairsFast() * 2 +
                              ccl.CountRunsFast();

                           ccl.Remove(cardArray[k]);
            
                           // multiple by resulting crib score
                           double probableScore = cribScore * starterProbability * probabilityInPlayerHand * probabilityDiscarded;

                           // add to our total
                           speculatedCribValue += probableScore;
                        }
                     }

                     ccl.Remove(cardArray[j]);
                     cardOdds.AddCard(j+1);

                     ccl.Remove(cardArray[i]);
                     cardOdds.AddCard(i+1);
                  }
               }
            }

            // add in probable points for a flush
            if (card1.Suit == card2.Suit)
            {
               // calculate probability opponent has 2 cards of same suit
               double sameAsDiscard = cardOdds.GetSuitProbability(card1.Suit, 6);
               cardOdds.RemoveCard(card1.Suit);
               sameAsDiscard = sameAsDiscard * cardOdds.GetSuitProbability(card1.Suit, 5);
               cardOdds.RemoveCard(card1.Suit);

               // assume 1/6 * 1/5 chance of discarding them
               sameAsDiscard = sameAsDiscard * ((double)1/(double)30);

               if ((bool)Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false))
               {
                  // base it on probability that opponent has 2 cards of same suit in hand and discards them
                  sameAsDiscard = sameAsDiscard * 4;

                  // now chance of extra point through starter card
                  sameAsDiscard = sameAsDiscard + 1 * cardOdds.GetSuitProbability(card1.Suit);
               }
               else
               {
                  // base it on probability that starter card is same suit and opponent has 2 cards of same suit in hand and discards them
                  sameAsDiscard = sameAsDiscard * cardOdds.GetSuitProbability(card1.Suit) * 5;
               }
            
               cardOdds.AddCard(card1.Suit);
               cardOdds.AddCard(card1.Suit);
            
               speculatedCribValue = speculatedCribValue + sameAsDiscard;

            }
            else
            {
               // if we don't discard same suit then flush is impossible
            }
         }
         else
         {
            // for each possible crib deal
            for (int i = 0; i < 13; i++)
            {
               for (int j = i; j < 13; j++)
               {
                  // calculate probability of being dealt
                  double dealtProbability = cardOdds.GetFaceValueProbability(i+1, 1);
                  cardOdds.RemoveCard(i+1);
                  dealtProbability = dealtProbability * cardOdds.GetFaceValueProbability(j+1, 1);
                  cardOdds.RemoveCard(j+1);

                  if (dealtProbability != 0)
                  {
                     ccl.Add(cardArray[i]);
                     ccl.Add(cardArray[j]);

                     // for each possible discard
                     for (int l = 0; l < 13; l++)
                     {

                        double probabilityDiscarded = 0;

                        if (opponentOwnsCrib)
                        {
                           for (int x = 0; x < l; x++)
                           {
                              probabilityDiscarded += (double)__dealerDiscardFrequency[x,l];
                           }

                           for (int x = l+1; x < 13; x++)
                           {
                              probabilityDiscarded += (double)__dealerDiscardFrequency[l,x];
                           }

                           probabilityDiscarded = probabilityDiscarded / (double)__dealerTotal;
                        }
                        else
                        {
                           for (int x = 0; x < l; x++)
                           {
                              probabilityDiscarded += (double)__nonDealerDiscardFrequency[x,l];
                           }

                           for (int x = l+1; x < 13; x++)
                           {
                              probabilityDiscarded += (double)__nonDealerDiscardFrequency[l,x];
                           }

                           probabilityDiscarded = probabilityDiscarded / (double)__nonDealerTotal;
                        }

                        double probabilityInPlayerHand = cardOdds.GetFaceValueProbability(l+1, cardsToDeal);

                        if (probabilityDiscarded != 0 && probabilityInPlayerHand != 0)
                        {

                           cardOdds.RemoveCard(l+1);
                           ccl.Add(cardArray[l]);

                           // for each possible starter card
                           for (int k = 0; k < 13; k++)
                           {
                              // calculate probability of starter card (given possible crib discard)
                              double starterProbability = cardOdds.GetFaceValueProbability(k+1, 1);

                              if (starterProbability != 0)
                              {
                                 // now create a suitable hand
                                 ccl.Add(cardArray[k]);

                                 int cribScore = ccl.Count15sFast() * 2 +
                                    ccl.CountPairsFast() * 2 +
                                    ccl.CountRunsFast();

                                 ccl.Remove(cardArray[k]);
                  
                                 // multiple by resulting crib score
                                 double probableScore = cribScore * starterProbability * probabilityInPlayerHand * probabilityDiscarded * dealtProbability;

                                 // add to our total
                                 speculatedCribValue += probableScore;
                              }
                           }

                           ccl.Remove(cardArray[l]);
                           cardOdds.AddCard(l+1);
                        }
                     }

                     ccl.Remove(cardArray[i]);
                     ccl.Remove(cardArray[j]);
                  }

                  cardOdds.AddCard(i+1);
                  cardOdds.AddCard(j+1);
               }
            }

            // add in probable points for a flush
            // calculate probability 2 cards of same suit as our discard were dealt to crib
            double sameAsDiscard = cardOdds.GetSuitProbability(card1.Suit);
            cardOdds.RemoveCard(card1.Suit);
            sameAsDiscard = sameAsDiscard * cardOdds.GetSuitProbability(card1.Suit);
            cardOdds.RemoveCard(card1.Suit);

            // calculate probability our opponent was also dealt a card from this suit
            sameAsDiscard = sameAsDiscard * cardOdds.GetSuitProbability(card1.Suit, 5);
            cardOdds.RemoveCard(card1.Suit);

            // assume 1/5 chance of discarding them
            sameAsDiscard = sameAsDiscard * ((double)1/(double)5);

            if ((bool)Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false))
            {
               // base it on probability that opponent has 2 cards of same suit in hand and discards them
               sameAsDiscard = sameAsDiscard * 4;

               // now chance of extra point through starter card
               sameAsDiscard = sameAsDiscard + 1 * cardOdds.GetSuitProbability(card1.Suit);
            }
            else
            {
               // base it on probability that starter card is same suit and opponent has 2 cards of same suit in hand and discards them
               sameAsDiscard = sameAsDiscard * cardOdds.GetSuitProbability(card1.Suit) * 5;
            }
               
            cardOdds.AddCard(card1.Suit);
            cardOdds.AddCard(card1.Suit);
            cardOdds.AddCard(card1.Suit);
            
            speculatedCribValue = speculatedCribValue + sameAsDiscard;
         }

         // if we discarded a jack add on probability of cut card being the same suit
         if (card1.FaceValue == 11)
         {
            speculatedCribValue += cardOdds.GetSuitProbability(card1.Suit);
         }

         if ((card2 != null) && (card2.FaceValue == 11))
         {
            speculatedCribValue += cardOdds.GetSuitProbability(card2.Suit);
         }

         // subtract guaranteed from speculative
         speculatedCribValue -= guaranteedCribValue;
      }
      #endregion
      */
      #endregion
   }
}
