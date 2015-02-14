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

// ============================================================================

// This file contains the automated unit tests for the classes in the Cards 
// assembly (excluding CardCollection as this is generated code).
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// ============================================================================

using System;
using NUnit.Framework;
using CribCards;
using Cards;
using ApplicationConfig;

namespace CribCardsUnitTests
{
   /// <summary>
   /// This class unit tests the CribCard Class.
   /// </summary>
   [TestFixture]
   public class CribCardUnitTest
   {
      [Test]
      public void CribCardUT()
      {
         CribCard.Initialise();
         Card c = null;

         for (int i = 1; i < 10; i++)
         {
            c = new Card(i, Card.SUIT.CLUBS);
            Assert.AreEqual(i, c.SpecialValue);
         }

         for (int i = 11; i <= 13; i++)
         {
            c = new Card(i, Card.SUIT.CLUBS);
            Assert.AreEqual(10, c.SpecialValue);
         }
      }
   }
   /// <summary>
   /// This class unit tests the CribCardList Class.
   /// </summary>
   [TestFixture]
   public class CribCardListUnitTest
   {
      [Test]
      public void CribCardList52false()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", false);
         Assert.AreEqual(5, Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6));
         Assert.AreEqual(2, Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2));
         Assert.AreEqual(false, Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false));

         CribCardList ccl = new CribCardList();

         ccl.Add(new Card(1, Card.SUIT.CLUBS));
         ccl.Add(new Card(2, Card.SUIT.CLUBS));
         ccl.Add(new Card(3, Card.SUIT.CLUBS));

         Assert.AreEqual(3, ccl.CountRunsFast());
         Assert.AreEqual(0, ccl.Count15sFast());
         Assert.AreEqual(0, ccl.CountPairsFast());
         Assert.AreEqual(3, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), false));
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), true));
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), false));
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), true));

         ScoreList s = new ScoreList();
         Assert.AreEqual(0, ccl.Count15s(ref s));
         Assert.AreEqual(0, s.Count);

         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, true));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountPairs(ref s));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountRuns(ref s));
         Assert.AreEqual(1, s.Count);         

         ccl = new CribCardList();
         ccl.Add(new Card(5, Card.SUIT.DIAMONDS));
         ccl.Add(new Card(5, Card.SUIT.CLUBS));
         ccl.Add(new Card(5, Card.SUIT.HEARTS));

         Assert.AreEqual(0, ccl.CountRunsFast());
         Assert.AreEqual(1, ccl.Count15sFast());
         Assert.AreEqual(3, ccl.CountPairsFast());
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.CLUBS), false));
         s = new ScoreList();
         Assert.AreEqual(1, ccl.Count15s(ref s));
         Assert.AreEqual(1, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(0, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountPairs(ref s));
         Assert.AreEqual(3, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountRuns(ref s));
         Assert.AreEqual(0, s.Count);         
      }
      [Test]
      public void CribCardList51true()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 1);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", true);
         Assert.AreEqual(5, Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6));
         Assert.AreEqual(1, Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2));
         Assert.AreEqual(true, Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false));

         CribCardList ccl = new CribCardList();

         ccl.Add(new Card(1, Card.SUIT.CLUBS));
         ccl.Add(new Card(2, Card.SUIT.CLUBS));
         ccl.Add(new Card(3, Card.SUIT.CLUBS));
         ccl.Add(new Card(4, Card.SUIT.CLUBS));

         Assert.AreEqual(4, ccl.CountRunsFast());
         Assert.AreEqual(0, ccl.Count15sFast());
         Assert.AreEqual(0, ccl.CountPairsFast());
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), false));
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), true));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), false));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), true));

         ScoreList s = new ScoreList();
         Assert.AreEqual(0, ccl.Count15s(ref s));
         Assert.AreEqual(0, s.Count);

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountPairs(ref s));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountRuns(ref s));
         Assert.AreEqual(1, s.Count);         

         ccl = new CribCardList();
         ccl.Add(new Card(5, Card.SUIT.DIAMONDS));
         ccl.Add(new Card(5, Card.SUIT.CLUBS));
         ccl.Add(new Card(5, Card.SUIT.HEARTS));
         ccl.Add(new Card(5, Card.SUIT.SPADES));

         Assert.AreEqual(0, ccl.CountRunsFast());
         Assert.AreEqual(4, ccl.Count15sFast());
         Assert.AreEqual(6, ccl.CountPairsFast());
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.CLUBS), false));
         s = new ScoreList();
         Assert.AreEqual(4, ccl.Count15s(ref s));
         Assert.AreEqual(4, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(0, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(6, ccl.CountPairs(ref s));
         Assert.AreEqual(6, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountRuns(ref s));
         Assert.AreEqual(0, s.Count);         
      }
      [Test]
      public void CribCardList51false()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 1);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", false);
         Assert.AreEqual(5, Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6));
         Assert.AreEqual(1, Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2));
         Assert.AreEqual(false, Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false));

         CribCardList ccl = new CribCardList();

         ccl.Add(new Card(1, Card.SUIT.CLUBS));
         ccl.Add(new Card(2, Card.SUIT.CLUBS));
         ccl.Add(new Card(3, Card.SUIT.CLUBS));
         ccl.Add(new Card(4, Card.SUIT.CLUBS));

         Assert.AreEqual(4, ccl.CountRunsFast());
         Assert.AreEqual(0, ccl.Count15sFast());
         Assert.AreEqual(0, ccl.CountPairsFast());
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), false));
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), true));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), false));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), true));

         ScoreList s = new ScoreList();
         Assert.AreEqual(0, ccl.Count15s(ref s));
         Assert.AreEqual(0, s.Count);

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, true));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountPairs(ref s));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountRuns(ref s));
         Assert.AreEqual(1, s.Count);         

         ccl = new CribCardList();
         ccl.Add(new Card(5, Card.SUIT.DIAMONDS));
         ccl.Add(new Card(5, Card.SUIT.CLUBS));
         ccl.Add(new Card(5, Card.SUIT.HEARTS));
         ccl.Add(new Card(5, Card.SUIT.SPADES));

         Assert.AreEqual(0, ccl.CountRunsFast());
         Assert.AreEqual(4, ccl.Count15sFast());
         Assert.AreEqual(6, ccl.CountPairsFast());
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.CLUBS), false));
         s = new ScoreList();
         Assert.AreEqual(4, ccl.Count15s(ref s));
         Assert.AreEqual(4, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(0, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(6, ccl.CountPairs(ref s));
         Assert.AreEqual(6, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountRuns(ref s));
         Assert.AreEqual(0, s.Count);         
      }
      [Test]
      public void CribCardList52true()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", true);
         Assert.AreEqual(5, Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6));
         Assert.AreEqual(2, Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2));
         Assert.AreEqual(true, Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false));

         CribCardList ccl = new CribCardList();

         ccl.Add(new Card(1, Card.SUIT.CLUBS));
         ccl.Add(new Card(2, Card.SUIT.CLUBS));
         ccl.Add(new Card(3, Card.SUIT.CLUBS));

         Assert.AreEqual(3, ccl.CountRunsFast());
         Assert.AreEqual(0, ccl.Count15sFast());
         Assert.AreEqual(0, ccl.CountPairsFast());
         Assert.AreEqual(3, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), false));
         Assert.AreEqual(3, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), true));
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), false));
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), true));

         ScoreList s = new ScoreList();
         Assert.AreEqual(0, ccl.Count15s(ref s));
         Assert.AreEqual(0, s.Count);

         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountPairs(ref s));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountRuns(ref s));
         Assert.AreEqual(1, s.Count);         

         ccl = new CribCardList();
         ccl.Add(new Card(5, Card.SUIT.DIAMONDS));
         ccl.Add(new Card(5, Card.SUIT.CLUBS));
         ccl.Add(new Card(5, Card.SUIT.HEARTS));

         Assert.AreEqual(0, ccl.CountRunsFast());
         Assert.AreEqual(1, ccl.Count15sFast());
         Assert.AreEqual(3, ccl.CountPairsFast());
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.CLUBS), false));
         s = new ScoreList();
         Assert.AreEqual(1, ccl.Count15s(ref s));
         Assert.AreEqual(1, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(0, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(3, ccl.CountPairs(ref s));
         Assert.AreEqual(3, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountRuns(ref s));
         Assert.AreEqual(0, s.Count);         
      }
      [Test]
      public void CribCardList62false()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 6);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", false);
         Assert.AreEqual(6, Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6));
         Assert.AreEqual(2, Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2));
         Assert.AreEqual(false, Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false));

         CribCardList ccl = new CribCardList();

         ccl.Add(new Card(1, Card.SUIT.CLUBS));
         ccl.Add(new Card(2, Card.SUIT.CLUBS));
         ccl.Add(new Card(3, Card.SUIT.CLUBS));
         ccl.Add(new Card(4, Card.SUIT.CLUBS));

         Assert.AreEqual(4, ccl.CountRunsFast());
         Assert.AreEqual(0, ccl.Count15sFast());
         Assert.AreEqual(0, ccl.CountPairsFast());
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), false));
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), true));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), false));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), true));

         ScoreList s = new ScoreList();
         Assert.AreEqual(0, ccl.Count15s(ref s));
         Assert.AreEqual(0, s.Count);

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, true));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountPairs(ref s));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountRuns(ref s));
         Assert.AreEqual(1, s.Count);         

         ccl = new CribCardList();
         ccl.Add(new Card(5, Card.SUIT.DIAMONDS));
         ccl.Add(new Card(5, Card.SUIT.CLUBS));
         ccl.Add(new Card(5, Card.SUIT.HEARTS));
         ccl.Add(new Card(5, Card.SUIT.SPADES));

         Assert.AreEqual(0, ccl.CountRunsFast());
         Assert.AreEqual(4, ccl.Count15sFast());
         Assert.AreEqual(6, ccl.CountPairsFast());
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.CLUBS), false));
         s = new ScoreList();
         Assert.AreEqual(4, ccl.Count15s(ref s));
         Assert.AreEqual(4, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(0, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(6, ccl.CountPairs(ref s));
         Assert.AreEqual(6, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountRuns(ref s));
         Assert.AreEqual(0, s.Count);         
      }
      [Test]
      public void CribCardList62true()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 6);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", true);
         Assert.AreEqual(6, Config.TheConfig.GetValue("ultimatecrib", "CardsToDeal", 6));
         Assert.AreEqual(2, Config.TheConfig.GetValue("ultimatecrib", "CardsToDiscard", 2));
         Assert.AreEqual(true, Config.TheConfig.GetValue("ultimatecrib", "CribFlushesOnHand", false));

         CribCardList ccl = new CribCardList();

         ccl.Add(new Card(1, Card.SUIT.CLUBS));
         ccl.Add(new Card(2, Card.SUIT.CLUBS));
         ccl.Add(new Card(3, Card.SUIT.CLUBS));
         ccl.Add(new Card(4, Card.SUIT.CLUBS));

         Assert.AreEqual(4, ccl.CountRunsFast());
         Assert.AreEqual(0, ccl.Count15sFast());
         Assert.AreEqual(0, ccl.CountPairsFast());
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), false));
         Assert.AreEqual(4, ccl.CountFlushesFast(new Card(1, Card.SUIT.HEARTS), true));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), false));
         Assert.AreEqual(5, ccl.CountFlushesFast(new Card(9, Card.SUIT.CLUBS), true));

         ScoreList s = new ScoreList();
         Assert.AreEqual(0, ccl.Count15s(ref s));
         Assert.AreEqual(0, s.Count);

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountFlushes(new Card(1, Card.SUIT.HEARTS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(5, ccl.CountFlushes(new Card(9, Card.SUIT.CLUBS), ref s, true));
         Assert.AreEqual(1, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountPairs(ref s));
         Assert.AreEqual(0, s.Count);         

         s = new ScoreList();
         Assert.AreEqual(4, ccl.CountRuns(ref s));
         Assert.AreEqual(1, s.Count);         

         ccl = new CribCardList();
         ccl.Add(new Card(5, Card.SUIT.DIAMONDS));
         ccl.Add(new Card(5, Card.SUIT.CLUBS));
         ccl.Add(new Card(5, Card.SUIT.HEARTS));
         ccl.Add(new Card(5, Card.SUIT.SPADES));

         Assert.AreEqual(0, ccl.CountRunsFast());
         Assert.AreEqual(4, ccl.Count15sFast());
         Assert.AreEqual(6, ccl.CountPairsFast());
         Assert.AreEqual(0, ccl.CountFlushesFast(new Card(1, Card.SUIT.CLUBS), false));
         s = new ScoreList();
         Assert.AreEqual(4, ccl.Count15s(ref s));
         Assert.AreEqual(4, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountFlushes(new Card(1, Card.SUIT.CLUBS), ref s, false));
         Assert.AreEqual(0, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(6, ccl.CountPairs(ref s));
         Assert.AreEqual(6, s.Count);         
         s = new ScoreList();
         Assert.AreEqual(0, ccl.CountRuns(ref s));
         Assert.AreEqual(0, s.Count);         
      }
   }
   /// <summary>
   /// This class unit tests the Scores Class.
   /// </summary>
   [TestFixture]
   public class ScoresUnitTest
   {
      [Test]
      public void ScoresUT()
      {
         Scores s = new Scores("TEST", 5, Scores.SCORETYPE.PAIR);
         Assert.AreEqual(Scores.SCOREREASON.UNKNOWN, s.ScoreReason);
         Assert.AreEqual(Scores.SCORETYPE.PAIR, s.ScoreType);
         Assert.AreEqual("TEST", s.Cards);
         Assert.AreEqual(5, s.Score);
      }
   }
   /// <summary>
   /// This class unit tests the ScoresList Class.
   /// </summary>
   [TestFixture]
   public class ScoresListUnitTest
   {
      [Test]
      public void ScoresListUT()
      {
         ScoreList sl = new ScoreList();

         sl.Add(new Scores("Test", 2, Scores.SCORETYPE.FIFTEEN));
         Assert.AreEqual(1, sl.Count);
         sl.Add(new Scores("Test", 2, Scores.SCORETYPE.FLUSH));
         Assert.AreEqual(2, sl.Count);
         sl.Add(new Scores("Test", 2, Scores.SCORETYPE.PAIR));
         Assert.AreEqual(3, sl.Count);

         sl.Reason = Scores.SCOREREASON.PENALTY;
         Assert.AreEqual(Scores.SCOREREASON.PENALTY, ((Scores)sl[0]).ScoreReason);
         Assert.AreEqual(Scores.SCOREREASON.PENALTY, ((Scores)sl[1]).ScoreReason);

         sl.RemoveFlush();
         Assert.AreEqual(2, sl.Count);

         sl.Reason = Scores.SCOREREASON.PLAY;
         Assert.AreEqual(Scores.SCOREREASON.PLAY, ((Scores)sl[0]).ScoreReason);
         Assert.AreEqual(Scores.SCOREREASON.PLAY, ((Scores)sl[1]).ScoreReason);
      }
   }
}