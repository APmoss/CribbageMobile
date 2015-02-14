// Ultimate Cribbage
// CribCards Assembly

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
using Cards;
using System.Diagnostics;
using System.Collections;
using ApplicationConfig;

namespace CribCards
{
	/// <summary>
	/// This class is a card list that is aware of cribbage rules
	/// </summary>
	public class CribCardList : CardList
	{
      #region Constructors
      /// <summary>
      /// Create a cribbage card list
      /// </summary>
		public CribCardList() : base()
		{
         // just in case this has not been done ... lets do it
         CribCard.Initialise();
		}
      #endregion

      #region Private Member Functions
      /// <summary>
      /// This loop function is used to count the number of 15s in the card list
      /// </summary>
      /// <param name="pos">where we are up to in the card list</param>
      /// <param name="totalSoFar">total so far</param>
      /// <param name="previousCards">Previous cards considered</param>
      /// <param name="scoreList">Accumulated list of scores identified</param>
      /// <returns>Number of 15s found</returns>
      protected int Count15sLoop(int pos, int totalSoFar, string previousCards, ref ScoreList scoreList)
      {           
         // our 15s count
         int count = 0;

         // go through the rest of the card list from our current position
         for (int i = ++pos; i < Count; i++)
         {
            // add the cards value to our total
            int totalNow = totalSoFar + this[i].SpecialValue;

            // add the name of the card to our string
            string cards = previousCards + this[i].ToString() + " ";

            // if the total is now 15
            if (totalNow == 15)
            {
               // increment our 15 count
               count++;

               // add a score node
               if (scoreList != null)
               {
                  Scores score = new Scores(cards, 2, Scores.SCORETYPE.FIFTEEN);
                  scoreList.Add(score);
               }
            }
            // if we haven't reached 15 yet
            else if (totalNow < 15)
            {
               // recursively call ourselves
               count = count + Count15sLoop(i, totalNow, cards, ref scoreList);
            }
            // we must have passed it
            else
            {
               // terminate the search
               i = 999;
            }    
         }

         return count;   
      }

      /// <summary>
      /// checks if all cards in list are 1 higher in face value than the prior card
      /// </summary>
      /// <returns></returns>
      bool IsRun()
      {
         // assume it is a run
         bool isRun = true;

         // save our current sort order then resort ascending
         string oldSortOrder = SortOrder;
         SortOrder = "Ascending";
         Sort();

         // last card is initially invalid
         int iLast = -1;

         // look at each card
         foreach (Card card in this)
         {
            // if we have no last card
            if (iLast == -1)
            {
               // save the new current value in the run
               iLast = card.FaceValue;
            }
            else
            {
               // if it is 1 higher than the last card
               if (iLast + 1 == card.FaceValue)
               {
                  // save the new current value in the run
                  iLast = card.FaceValue;
               }
               else
               {
                  // this one is out of sequence
                  isRun = false;

                  // abort the search
                  break;
               }
            }
         }

         SortOrder = oldSortOrder;
         Sort();

         return isRun;
      }
      #endregion

      #region Public Member Functions

      #region Fast Scoring Functions
      /// <summary>
      /// This quickly checks the card list for a flush
      /// </summary>
      /// <param name="starterCard">starter card</param>
      /// <param name="isCrib">true if we are scoring a crib</param>
      /// <returns></returns>
      public int CountFlushesFast(Card starterCard, bool isCrib)
      {
         int cardsToDeal = (int)Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6);
         int cardsToDiscard = (int)Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2);
         bool scoreCribFlushOnHandAlone = (bool)Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false);

         // get the suit of the first card
         Trace.Assert(this.Count > 0);
         Card.SUIT suit = this[0].Suit;

         // now look at each card to see if any are a different suit
         foreach(Card card in this)
         {
            if (card.Suit != suit)
            {
               // a different suit so there cannot be a flush
               return 0;
            }
         }

         // if we have a starter card and it matches then add one to the value returned
         if (starterCard != null && starterCard.Suit == suit)
         {
            return this.Count + 1;
         }
         else if (!isCrib && this.Count == this.Count)
         {
            return this.Count;
         }
         else if (isCrib && scoreCribFlushOnHandAlone)
         {
            return this.Count;
         }

         // obviously did not meet the rules
         return 0;
      }

      /// <summary>
      /// This function quickly counts the number of 15s without saving the scores
      /// </summary>
      /// <returns></returns>
      public int Count15sFast()
      {
         // save our card face values
         int[] cardFaceValues = new int[5];
         int x = 0;
         foreach (Card card in this)
         {
            cardFaceValues[x++] = card.FaceValue;
         }

         // set the rest to zero
         for (int y = x; y < 5; y++)
         {
            cardFaceValues[y] = 0;
         }

         int i15s = 0;

         for (int i = 0; i < 5; i++)
         {
            int total = cardFaceValues[i];

            for (int j = i+1; j < 5; j++)
            {
               total += cardFaceValues[j];

               if (total == 15)
               {
                  i15s++;
               }
               else if (total < 15)
               {
                  for (int k = j+1; k < 5; k++)
                  {
                     total += cardFaceValues[k];

                     if (total == 15)
                     {
                        i15s++;
                     }
                     else if (total < 15)
                     {
                        for (int l = k+1; l < 5; l++)
                        {
                           total += cardFaceValues[l];

                           if (total == 15)
                           {
                              i15s++;
                           }
                           else if (total < 15)
                           {
                              for (int m = l+1; m < 5; m++)
                              {
                                 total += cardFaceValues[m];

                                 if (total == 15)
                                 {
                                    i15s++;
                                 }

                                 total -= cardFaceValues[m];
                              }
                           }
                           total -= cardFaceValues[l];
                        }
                     }
                     total -= cardFaceValues[k];
                  }
               }
               total -= cardFaceValues[j];
            }
         }

         return i15s;
      }

      /// <summary>
      /// This function quickly counts the number of cards in runs
      /// </summary>
      /// <returns></returns>
      public int CountRunsFast()
      {
         Debug.Assert(Count <= 5, "Code won't work on more than 5 cards");

         CardList cl = (CardList)this.MemberwiseClone();
         cl.SortOrder = "Ascending";
         cl.Sort();

         int iRunCount = 0;
         int[] cardFaceValues = new int[5];

         int x = 0;
         foreach(Card card in cl)
         {
            cardFaceValues[x] = card.FaceValue;
            x++;
         }

         for (int y = x; y < 5; y++)
         {
            cardFaceValues[y] = 0xFF;
         }

         // look for runs
         int iMultiplier = 1;
         int runLength = 1;
         int iMultiplierIncrement = 1;
         int iLastValue = -255;

         for (int i = 0; i < 5; i++)
         {
            if (cardFaceValues[i] == iLastValue + 1)
            {
               runLength++;
               iMultiplierIncrement = iMultiplier;
            }
            else if (cardFaceValues[i] == iLastValue)
            {
               iMultiplier = iMultiplier + iMultiplierIncrement;
            }
            else
            {
               if (runLength > 2)
               {
                  iRunCount += runLength * iMultiplier;
               }

               runLength = 1;
               iMultiplier = 1;
               iMultiplierIncrement = 1;
            }

            iLastValue = cardFaceValues[i];

            if ((runLength == 1) && (i == 4))
            {
               // we aint going to make a run
               break;
            }
         }

         if (runLength > 2)
         {
            iRunCount += runLength * iMultiplier;
         }

         return iRunCount;
      }

      /// <summary>
      ///  This function quickly counts the number of pairs
      /// </summary>
      /// <returns></returns>
      public int CountPairsFast()
      {
         int iPairs = 0;

         int[] count = {0,0,0,0,0,0,0,0,0,0,0,0,0};

         for (int j = 0; j < this.Count; j++)
         {
            count[this[j].FaceValue-1]++;
         }

         for (int i = 0; i < 13; i++)
         {
            switch(count[i])
            {
               case 0:
               case 1:
                  break;
               case 2:
                  iPairs++;
                  break;
               case 3:
                  iPairs += 3;
                  break;
               case 4:
                  iPairs += 6;
                  break;
               default:
                  break;
            }
         }

         return iPairs;
      }
      #endregion

      #region Full Scoring Functions
      /// <summary>
      /// Count the number of 15s in a hand
      /// </summary>
      /// <param name="scoreList">The list of scores</param>
      /// <returns>Number of 15s found</returns>
      public int Count15s(ref ScoreList scoreList)
      {
         // we have found no 15s so far
         int count = 0;

         for (int i = 0; i < Count; i++)
         {
            // add the name of the card to our string
            string cards = this[i].ToString() + " ";

            // call our recursive function to count the 15s
            count = count + Count15sLoop(i, this[i].SpecialValue, cards, ref scoreList);
         }  
   
         // return number of 15s found
         return count;   
      }

      /// <summary>
      /// Count the number of runs in the hand
      /// </summary>
      /// <param name="scoreList">List of scores</param>
      /// <returns>run size found</returns>
      public int CountRuns(ref ScoreList scoreList)
      {
         // initialise counters
         int total = 0;
         int runLength = 0;        
         int factor = 1;
         int priorFaceValue = 99;
         int lastDuplicateInRun = 99;
         ArrayList cards = new ArrayList();

         // Copy ourselves
         CardList cl = (CardList)this.MemberwiseClone();
         cl.SortOrder = "Ascending";
         cl.Sort();

         foreach (Card card in cl)
         {
            // get it's face value
            int faceValue = card.FaceValue;
      
            // if it is the same as the last card in the run then we may have a multiplier
            if (faceValue == priorFaceValue)               
            {

               // if it is the same as the last duplicate card in the run then we may have a multiplier
               if (faceValue == lastDuplicateInRun)
               {
                  // get the run string
                  string priorCard = (string)cards[factor -1];

                  // chop off the last character
                  string newCards = priorCard.Substring(0, priorCard.Length - 1);

                  // keep chopping until we have a space
                  while ((newCards != "") && (newCards.Substring(newCards.Length - 1) != " "))
                  {
                     newCards = newCards.Substring(0, newCards.Length - 1);
                  }

                  // add a new run string with the current card at the end
                  cards.Insert(factor, newCards + card.ToString() + " ");

                  // increment the multiplication factor
                  factor++;
               }
               else
               {
                  // we need to process all our run strings
                  for (int i = 0; i < factor; i++)
                  {
                     // get the run string
                     string priorCard = (string)cards[i];

                     // chop off the last character (this will be a space)
                     string newCards = priorCard.Substring(0, priorCard.Length - 1);

                     // keep chopping until we find a space
                     while ((newCards != "") && (newCards.Substring(newCards.Length - 1) != " "))
                     {
                        newCards = newCards.Substring(0, newCards.Length - 1);
                     }

                     // now create a new run string
                     cards.Insert(factor + i, newCards + card.ToString() + " ");
                  }

                  // double the multiplier factor
                  factor = factor * 2;  
               }

               // remember this face value
               lastDuplicateInRun = faceValue;   
            }
               // check if it is one greater than the last face value
            else if (faceValue == priorFaceValue + 1)
            {
               // process each of our current runs
               for (int i = 0; i < factor; i++)
               {
                  // add out current card to each
                  cards.Insert(i, cards[i] + card.ToString() + " ");
               }

               // increase our run length
               runLength++;
            } 
            else
            {
               // if we had a run it is now over

               // if it was long enough to count
               if (runLength >= 3)
               {
                  // calculate the total run length with multipliers
                  total = total + runLength * factor;
            
                  // if we have a score list add our scores to it
                  if (scoreList != null)
                  {
                     for (int i = 0; i < factor; i++)
                     {
                        Scores score = new Scores((string)cards[i], runLength, Scores.SCORETYPE.RUN);
                        scoreList.Add(score);
                     }
                  }
               }

               // reset runs
               factor = 1;
               runLength = 1;       
               lastDuplicateInRun = 99;

               // clear the list of run strings
               cards.Clear();
               cards.Insert(0, card.ToString() + " ");
            }

            // reset the prior
            priorFaceValue = faceValue;
         }          

         // catch runs that ended with the last card in the hand
         if (runLength >= 3)
         {
            // calculate the total run length with multipliers
            total = total + runLength * factor;

            // if we have a score list add our scores to it
            if (scoreList != null)
            {
               for (int i = 0; i < factor; i++)
               {
                  Scores score = new Scores((string)cards[i], runLength, Scores.SCORETYPE.RUN);
                  scoreList.Add(score);
               }
            }
         }

         // return the run points
         return total;   
      }

      /// <summary>
      /// Count the number of pairs in the card list
      /// </summary>
      /// <param name="scoreList">List of scores</param>
      /// <returns>Number of pairs</returns>
      public int CountPairs(ref ScoreList scoreList)
      {
         // number of pairs
         int count = 0;

         // Copy ourselves
         CardList cl = (CardList)this.MemberwiseClone();
         cl.SortOrder = "Ascending";
         cl.Sort();

         // go through each card
         for (int i = 0; i < cl.Count - 1; i++)
         {
            // go through each subsequent card
            for (int j = i+1; j < cl.Count; j++)
            {
               // if it is a pair with our card 1
               if (cl[j].FaceValue == cl[i].FaceValue)
               {
                  // increment the count
                  count++;  

                  // if we have a score list add in a score
                  if (scoreList != null)
                  {
                     string s = cl[i].ToString() + " " + cl[j].ToString();
                     Scores score = new Scores(s, 2, Scores.SCORETYPE.PAIR);
                     scoreList.Add(score);
                  }
               }
            }
         }

         // return the number of pairs
         return count;
      }

      /// <summary>
      /// This is the slower flush counter but this updates the score list
      /// </summary>
      /// <param name="starterCard">starter card</param>
      /// <param name="scoreList">score list</param>
      /// <returns></returns>
      /// It can be a flush in the hand if ...
      ///   All cards in the players hand are in the flush or
      ///   All cards in the players hand + the starter card are in the flush
      /// It can be a flush in the crib if ...
      ///   All cards in the crib + the starter card are in the flush
      ///   - If 'ScoreCribFlushesOnHandAlone is set then a flush can be scored if all cards in the crib are in the flush
      public int CountFlushes(Card starterCard, ref ScoreList scoreList, bool isCrib)
      {
         bool scoreCribFlushOnHandAlone = (bool)Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false);

         // initialise our suit counters
         int hearts = 0;
         int diamonds = 0;
         int spades = 0;
         int clubs = 0;

         // initialise our flush card strings
         string heartNames = "";
         string diamondNames = "";
         string spadeNames = "";
         string clubNames = "";

         foreach (Card card in this)
         {
            // check the suit
            switch(card.Suit)
            {
               case Card.SUIT.HEARTS:
                  // score one for hearts
                  hearts++;
                  heartNames = heartNames + card.ToString() + " ";
                  break;

               case Card.SUIT.DIAMONDS:
                  // score one for diamonds
                  diamonds++;
                  diamondNames = diamondNames + card.ToString() + " ";
                  break;

               case Card.SUIT.SPADES:
                  // score one for spades
                  spades++;
                  spadeNames = spadeNames + card.ToString() + " ";
                  break;

               case Card.SUIT.CLUBS:
                  // score one for clubs
                  clubs++;
                  clubNames = clubNames + card.ToString() + " ";
                  break;

               default:
                  Debug.Assert(false, "Cards must have a valid suit");
                  break;
            }
         }          

         // if it looks like hearts are a flush
         if (hearts == this.Count)
         {

            // if the top card is also a heart add it to our flush
            if (starterCard != null && starterCard.Suit == Card.SUIT.HEARTS)
            {
               hearts ++;
               heartNames = heartNames + starterCard.ToString() + " ";
            }
         }

         // if it looks like hearts are a flush
         if ((!isCrib && hearts >= this.Count) ||
             (isCrib && hearts > this.Count) ||
             (isCrib && scoreCribFlushOnHandAlone && hearts == this.Count))
         {
            // create a score object and add it to the list
            Scores score = new Scores(heartNames, hearts, Scores.SCORETYPE.FLUSH);
            if (scoreList != null)
            {
               scoreList.Add(score);
            }

            // return number of cards in flush
            return hearts;
         }

         // if it looks like diamonds are a flush
         if (diamonds == this.Count) 
         {

            // if the top card is also a diamond add it to our flush
            if (starterCard != null && starterCard.Suit == Card.SUIT.DIAMONDS)
            {
               diamonds ++;
               diamondNames = diamondNames + starterCard.ToString() + " ";
            }

         }

         // if it looks like diamonds are a flush
         if ((!isCrib && diamonds >= this.Count) ||
            (isCrib && diamonds > this.Count) ||
            (isCrib && scoreCribFlushOnHandAlone && diamonds == this.Count))
         {
            // create a score object and add it to the list
            Scores score = new Scores(diamondNames, diamonds, Scores.SCORETYPE.FLUSH);
            if (scoreList != null)
            {
               scoreList.Add(score);
            }

            // return number of cards in flush
            return diamonds;
         }

         // if it looks like spades are a flush
         if (spades == this.Count)
         {

            // if the top card is also a spade add it to our flush
            if (starterCard != null && starterCard.Suit == Card.SUIT.SPADES)
            {
               spades ++;
               spadeNames = spadeNames + starterCard.ToString() + " ";
            }

         }

         // if it looks like spades are a flush
         if ((!isCrib && spades >= this.Count) ||
            (isCrib && spades > this.Count) ||
            (isCrib && scoreCribFlushOnHandAlone && spades == this.Count))
         {
            // create a score object and add it to the list
            Scores score = new Scores(spadeNames, spades, Scores.SCORETYPE.FLUSH);
            if (scoreList != null)
            {
               scoreList.Add(score);
            }

            // return number of cards in flush
            return spades;
         }

         // if it looks like spades are a flush
         if (clubs == this.Count) 
         {

            // if the top card is also a club add it to our flush
            if (starterCard != null && starterCard.Suit == Card.SUIT.CLUBS)
            {
               clubs++;
               clubNames = clubNames + starterCard.ToString() + " ";
            }

         }

         // if it looks like clubs are a flush
         if ((!isCrib && clubs >= this.Count) ||
            (isCrib && clubs > this.Count) ||
            (isCrib && scoreCribFlushOnHandAlone && clubs == this.Count))
         {
            // create a score object and add it to the list
            Scores score = new Scores(clubNames, clubs, Scores.SCORETYPE.FLUSH);
            if (scoreList != null)
            {
               scoreList.Add(score);
            }

            // return number of cards in flush
            return clubs;
         }       

         // obviou score not a flush so return 0
         return 0;   
      }
      #endregion

      #endregion   
   }
}
