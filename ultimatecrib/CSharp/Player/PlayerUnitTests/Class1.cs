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

// ============================================================================

// This file contains the automated unit tests for the classes in the Player 
// assembly 
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// You will need to copy the Layout.XML file into the bin directory this assembly builds to
// You will need to copy the white.BMP file into the bin directory this assembly builds to
// You will need to copy the cardimage.XML file into the bin directory this assembly builds to

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// ============================================================================

using System;
using NUnit.Framework;
using Player;
using Cards;
using ApplicationConfig;
using CribCards;
using System.Windows.Forms;

namespace PlayerUnitTests
{
   /// <summary>
   /// Unit Tests for Box
   /// </summary>
   [TestFixture]
   public class BoxUnitTest
   {
      [Test]
      public void IsBox()
      {
         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);
         Assert.IsTrue(b.IsBox);

         Assert.AreEqual(p, b.Owner);
      }
      [Test]
      public void Appearance()
      {
         Form1 f = new Form1();
         Assert.AreEqual(DialogResult.OK, f.ShowDialog());
      }
      [Test]
      public void Box51false()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 1);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", false);

         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);

         Deck d = new Deck();

         b.AddCard((DisplayableCard)d.DealCard(1, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(2, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(3, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(4, Card.SUIT.CLUBS));

         ScoreList sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(4+4, sl.TotalScore);
         Assert.AreEqual(3, sl.Count);

         sl = b.EvalScore(new Card(9, Card.SUIT.CLUBS));
         Assert.AreEqual(4+4+5, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         b = new Box(p);
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.HEARTS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.DIAMONDS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.SPADES));

         sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(12 + 8, sl.TotalScore);
         Assert.AreEqual(10, sl.Count);
      }
      [Test]
      public void Box52false()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", false);

         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);

         Deck d = new Deck();

         b.AddCard((DisplayableCard)d.DealCard(1, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(2, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(3, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(4, Card.SUIT.CLUBS));

         ScoreList sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(4+4, sl.TotalScore);
         Assert.AreEqual(3, sl.Count);

         sl = b.EvalScore(new Card(9, Card.SUIT.CLUBS));
         Assert.AreEqual(4+4+5, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         b = new Box(p);
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.HEARTS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.DIAMONDS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.SPADES));

         sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(12 + 8, sl.TotalScore);
         Assert.AreEqual(10, sl.Count);
      }
      [Test]
      public void Box51true()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 1);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", true);

         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);

         Deck d = new Deck();

         b.AddCard((DisplayableCard)d.DealCard(1, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(2, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(3, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(4, Card.SUIT.CLUBS));

         ScoreList sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(4+4+4, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         sl = b.EvalScore(new Card(9, Card.SUIT.CLUBS));
         Assert.AreEqual(4+4+5, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         b = new Box(p);
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.HEARTS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.DIAMONDS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.SPADES));

         sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(12 + 8, sl.TotalScore);
         Assert.AreEqual(10, sl.Count);
      }
      [Test]
      public void Box52true()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 5);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", true);

         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);

         Deck d = new Deck();

         b.AddCard((DisplayableCard)d.DealCard(1, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(2, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(3, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(4, Card.SUIT.CLUBS));

         ScoreList sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(4+4+4, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         sl = b.EvalScore(new Card(9, Card.SUIT.CLUBS));
         Assert.AreEqual(4+4+5, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         b = new Box(p);
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.HEARTS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.DIAMONDS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.SPADES));

         sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(12 + 8, sl.TotalScore);
         Assert.AreEqual(10, sl.Count);
      }
      [Test]
      public void Box62false()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 6);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", false);

         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);

         Deck d = new Deck();

         b.AddCard((DisplayableCard)d.DealCard(1, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(2, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(3, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(4, Card.SUIT.CLUBS));

         ScoreList sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(4+4, sl.TotalScore);
         Assert.AreEqual(3, sl.Count);

         sl = b.EvalScore(new Card(9, Card.SUIT.CLUBS));
         Assert.AreEqual(4+4+5, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         b = new Box(p);
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.HEARTS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.DIAMONDS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.SPADES));

         sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(12 + 8, sl.TotalScore);
         Assert.AreEqual(10, sl.Count);
      }
      [Test]
      public void Box62true()
      {
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDeal", 6);
         Config.TheConfig.SetValue("ultimatecrib", "CardsToDiscard", 2);
         Config.TheConfig.SetValue("ultimatecrib", "CribFlushesOnHand", true);

         HumanPlayer p = new HumanPlayer(1);
         Box b = new Box(p);

         Deck d = new Deck();

         b.AddCard((DisplayableCard)d.DealCard(1, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(2, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(3, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(4, Card.SUIT.CLUBS));

         ScoreList sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(4+4+4, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         sl = b.EvalScore(new Card(9, Card.SUIT.CLUBS));
         Assert.AreEqual(4+4+5, sl.TotalScore);
         Assert.AreEqual(4, sl.Count);

         b = new Box(p);
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.CLUBS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.HEARTS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.DIAMONDS));
         b.AddCard((DisplayableCard)d.DealCard(5, Card.SUIT.SPADES));

         sl = b.EvalScore(new Card(9, Card.SUIT.HEARTS));
         Assert.AreEqual(12 + 8, sl.TotalScore);
         Assert.AreEqual(10, sl.Count);
      }
   }


   /// <summary>
	/// Unit Tests for player
	/// </summary>
   [TestFixture]
   public class PlayerUnitTest
   {
      [Test]
      public void Player()
      {
         Assert.Fail("Not ready");
      }
   }

   /// <summary>
   /// Unit Tests for human player
   /// </summary>
   [TestFixture]
   public class HumanPlayerUnitTest
   {
      [Test]
      public void HumanPlayer()
      {
         Assert.Fail("Not ready");
      }
   }

   /// <summary>
   /// Unit Tests for automatic player
   /// </summary>
   [TestFixture]
   public class AutoPlayerUnitTest
   {
      [Test]
      public void AutoPlayer()
      {
         Assert.Fail("Not ready");
      }
   }

   /// <summary>
   /// Unit Tests for RemotePlayer
   /// </summary>
   [TestFixture]
   public class RemotePlayerUnitTest
   {
      [Test]
      public void RemotePlayer()
      {
         Assert.Fail("Not ready");
      }
   }

   /// <summary>
   /// Unit Tests for computer player
   /// </summary>
   [TestFixture]
   public class ComputerPlayerUnitTest
   {
      [Test]
      public void ComputerPlayer()
      {
         Assert.Fail("Not ready");
      }
   }
}
