// Ultimate Cribbage
// Cards Assembly

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

// NOTE: Unit tests are sensitive to the provided CardImage.XML. If you change 
// this file you may need to update some of the unit tests

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// You will need to copy the CardIMage.XML file into the bin directory this assembly builds to
// You will need to copy the White.BMP file into the bin directory this assembly builds to
// You will need to copy the CardDeck1.BMP file into the bin directory this assembly builds to
// You will need to copy the CardDeck2.BMP file into the bin directory this assembly builds to

// ============================================================================

using System;
using NUnit.Framework;
using Cards;
using System.Collections.Specialized;
using System.Drawing;

namespace CardsUnitTests
{
   /// <summary>
   /// This class unit tests the Cards Class.
   /// </summary>
   [TestFixture]
   public class CardUnitTest
   {
      [Test]
      public void DealCard()
      {
         Card c = new Card();
         c.Deal();
         Assert.IsTrue(c.Dealt);
         c.Undeal();
         Assert.IsFalse(c.Dealt);
         c.Deal();
         c.Shuffle();
         Assert.IsFalse(c.Dealt);
      }

      [Test]
      public void Constructor()
      {
         Card c = new Card(1);
         Assert.AreEqual(c.FaceValue,1);
         Assert.AreEqual(c.SpecialValue,1);
         Assert.AreEqual(c.Suit,Card.SUIT.CLUBS);
         c = new Card(14);
         Assert.AreEqual(c.FaceValue,1);
         Assert.AreEqual(c.SpecialValue,1);
         Assert.AreEqual(c.Suit,Card.SUIT.DIAMONDS);
         c = new Card(27);
         Assert.AreEqual(c.FaceValue,1);
         Assert.AreEqual(c.SpecialValue,1);
         Assert.AreEqual(c.Suit,Card.SUIT.HEARTS);
         c = new Card(40);
         Assert.AreEqual(c.FaceValue,1);
         Assert.AreEqual(c.SpecialValue,1);
         Assert.AreEqual(c.Suit,Card.SUIT.SPADES);
         c = new Card(53);
         Assert.AreEqual(c.FaceValue,0);
         Assert.AreEqual(c.SpecialValue,0);
         Assert.AreEqual(c.Suit,Card.SUIT.NO_SUIT);
         c = new Card(4,Card.SUIT.HEARTS);
         Assert.AreEqual(c.FaceValue,4);
         Assert.AreEqual(c.SpecialValue,4);
         Assert.AreEqual(c.Suit,Card.SUIT.HEARTS);
         Card c2 = new Card(c);
         Assert.AreEqual(c2.FaceValue,4);
         Assert.AreEqual(c.SpecialValue,4);
         Assert.AreEqual(c2.Suit,Card.SUIT.HEARTS);
      }

      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ConstructorException()
      {
         Card c = new Card(0);
      }

      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ConstructorException2()
      {
         Card c = new Card(54);
      }

      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ConstructorException3()
      {
         Card c = new Card(-1, Card.SUIT.CLUBS);
      }

      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ConstructorException4()
      {
         Card c = new Card(14, Card.SUIT.CLUBS);
      }

      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ConstructorException5()
      {
         Card c = new Card(null);
      }

      [Test]
      public void Operators()
      {
         Card c1 = new Card(4,Card.SUIT.HEARTS);
         Card c2 = new Card(7,Card.SUIT.CLUBS);

         Assert.IsTrue(c1 < c2);
         Assert.IsFalse(c1 > c2);
         Assert.IsTrue(c2 > c1);
         Assert.IsFalse(c2 < c1);
         Assert.IsFalse(c1.Equals(c2));

         Card c3 = new Card(7,Card.SUIT.CLUBS);
         Assert.IsTrue(c3.Equals(c2));
      }

      [Test]
      public void String()
      {
         Card c = new Card(1);
         Assert.AreEqual(c.ToString(),"AC");
         Assert.AreEqual(c.ToLongString()," Ace of Clubs");
         c = new Card(14);
         Assert.AreEqual(c.ToString(),"AD");
         Assert.AreEqual(c.ToLongString()," Ace of Diamonds");
         c = new Card(27);
         Assert.AreEqual(c.ToString(),"AH");
         Assert.AreEqual(c.ToLongString()," Ace of Hearts");
         c = new Card(40);
         Assert.AreEqual(c.ToString(),"AS");
         Assert.AreEqual(c.ToLongString()," Ace of Spades");
         c = new Card(53);
         Assert.AreEqual(c.ToString(),"JOKER");
         Assert.AreEqual(c.ToLongString()," Joker");
         c = new Card(4,Card.SUIT.HEARTS);
         Assert.AreEqual(c.ToString(),"4H");
         Assert.AreEqual(c.ToLongString()," 4 of Hearts");
      }
   }

   /// <summary>
   /// This class unit tests the CardBack Class
   /// </summary>
   [TestFixture]
   public class CardBackUnitTest
   {
      [Test]
      public void AvailableCardBacks()
      {
         StringCollection cardBack = CardBack.CardBacks;
         Assert.AreEqual(cardBack.Count, 59);
         Assert.AreEqual(cardBack[0], "Cards32_1");
      }

      [Test]
      public void Constructor()
      {
         CardBack c = new CardBack();

         c = new CardBack("Cards32_1");
         Assert.IsTrue(c.Dealt);
         Assert.IsTrue(c.FaceUp);

         // while this creates an error we expect it to fail silently
         c = new CardBack("Error");
      }
   }
   /// <summary>
   /// This class unit tests the CardConfig Class
   /// </summary>
   [TestFixture]
   public class CardConfigUnitTest
   {
      [Test]
      public void Values()
      {
         StringCollection sc = CardConfig.Values;
         Assert.AreEqual(sc.Count, 12);

         Assert.AreEqual(CardConfig.GetIntDefaultValue("ShadowVertical"), 5);
         Assert.AreEqual(CardConfig.GetStringDefaultValue("CardFamily"), "Plain");
         Assert.AreEqual(CardConfig.GetIntValue("ShadowVertical"), 5);
         Assert.AreEqual(CardConfig.GetStringValue("CardFamily"), "Plain");

         CardConfig.SetValue("CardFamily", "Fred");
         Assert.AreEqual(CardConfig.GetStringValue("CardFamily"), "Fred");
         Assert.AreEqual(CardConfig.GetStringDefaultValue("CardFamily"), "Plain");
         CardConfig.SetValue("CardFamily", "Plain");

         CardConfig.SetValue("ShadowVertical", 10);
         Assert.AreEqual(CardConfig.GetIntValue("ShadowVertical"), 10);
         Assert.AreEqual(CardConfig.GetIntDefaultValue("ShadowVertical"), 5);
      }
   }
   /// <summary>
   /// This class unit tests the CardImageFactory Class
   /// </summary>
   [TestFixture]
   public class CardImageFactoryUnitTest
   {
      [Test]
      public void Shadow()
      {
         CardImageFactory cif = new CardImageFactory();

         Size s = new Size(50,50);
         Bitmap b = cif.GetCardShadow(s);
         Assert.AreEqual(b.Size, s);

         Assert.AreEqual(cif.CardTransparentColour, "");
      }

      [Test]
      public void CardBack()
      {
         CardImageFactory cif = new CardImageFactory();

         Size s = new Size(50,50);
         Bitmap b = cif.GetCardBackImage("Cards32_1", s);
         Assert.AreEqual(b.Size, s);
      }

      [Test]
      public void CardFace()
      {
         CardImageFactory cif = new CardImageFactory();

         Size s = new Size(50,50);
         Bitmap b = cif.GetCardImage("4", "H", s);
         Assert.AreEqual(b.Size, s);
      }
   }

   /// <summary>
   /// This class unit tests the CardList Class
   /// </summary>
   [TestFixture]
   public class CardListUnitTest
   {
      [Test]
      public void Constructor()
      {
         CardList cl = new CardList();
         Assert.AreEqual(cl.SortOrder, "Ascending");

         cl = new CardList("Descending");
         Assert.AreEqual(cl.SortOrder, "Descending");
      }

      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ConstructorException()
      {
         CardList cl = new CardList("Fred");
         cl.AddCardAndSort(new Card(4, Card.SUIT.HEARTS));
      }

      [Test]
      public void AddRemoveSort()
      {
         CardList cl = new CardList();

         cl.AddCardAndSort(new Card(4, Card.SUIT.HEARTS));
         cl.AddCardAndSort(new Card(2, Card.SUIT.HEARTS));

         Assert.AreEqual(cl.Count, 2);
         Assert.AreEqual(cl[0].FaceValue, 2);

         cl.SortOrder = "Descending";
         Assert.AreEqual(cl[0].FaceValue, 4);

         CardList cl2 = (CardList)cl.Clone();
         Assert.AreEqual(cl2.Count, 2);
         Assert.AreEqual(cl2[0].FaceValue, 4);

         cl.Remove(new Card(4, Card.SUIT.HEARTS));
         Assert.AreEqual(cl.Count, 1);
         Assert.AreEqual(cl[0].FaceValue, 2);
         Assert.AreEqual(cl.ToString(), "2H ");

         cl = new CardList();
         cl.Add(new Card(4, Card.SUIT.HEARTS));
         cl.Add(new Card(2, Card.SUIT.HEARTS));
         Assert.AreEqual(cl[0].FaceValue, 4);
         cl.Sort();
         Assert.AreEqual(cl[0].FaceValue, 2);
      }
   }
   /// <summary>
   /// This class unit tests the Deck Class
   /// </summary>
   [TestFixture]
   public class DeckUnitTest
   {
      [Test]
      public void Constructor()
      {
         Deck d = new Deck();
         Assert.AreEqual(d.CardsLeft, 52);
         Assert.AreEqual(d.Count, 52);
         Assert.AreEqual(d.DealtCards.Count, 0);

         Deck d2 = new Deck(null, false, false);
         Assert.AreEqual(d2.CardsLeft, 52);
         Assert.AreEqual(d2.Count, 52);
         Assert.AreEqual(d2.DealtCards.Count, 0);

         Deck d3 = new Deck(null, false, true);
         Assert.AreEqual(d3.CardsLeft, 53);
         Assert.AreEqual(d3.Count, 53);
         Assert.AreEqual(d3.DealtCards.Count, 0);
      }

      [Test]
      public void DealingAndShuffling()
      {
         Deck d = new Deck(null, false, false);
         Assert.AreEqual(d.CardsLeft, 52);
         Assert.AreEqual(d.Count, 52);
         Assert.AreEqual(d.DealtCards.Count, 0);

         Card c = d.DealOne();
         Assert.AreEqual(d.CardsLeft, 51);
         Assert.AreEqual(d.Count, 52);
         Assert.AreEqual(d.DealtCards.Count, 1);

         CardList cl = d.DealtCards;
         Assert.AreEqual(cl.Count, 1);
         Assert.AreEqual(cl[0], c);

         d.Shuffle();
         Assert.AreEqual(d.CardsLeft, 52);
         Assert.AreEqual(d.Count, 52);
         Assert.AreEqual(d.DealtCards.Count, 0);

         c = d.DealCard(4, Card.SUIT.HEARTS);
         Assert.AreEqual(c.FaceValue, 4);
         Assert.AreEqual(c.Suit, Card.SUIT.HEARTS);
         Assert.AreEqual(c.SpecialValue, 4);
         Assert.IsTrue(c.Dealt);
         Assert.AreEqual(d.CardsLeft, 51);
         Assert.AreEqual(d.Count, 52);
         Assert.AreEqual(d.DealtCards.Count, 1);

         cl = d.DealtCards;
         Assert.AreEqual(cl.Count, 1);
         Assert.AreEqual(cl[0], c);

         DisplayableCard x = (DisplayableCard)c;

         CardBack cb = d.CardBack;
      }
   }

   /// <summary>
   /// This class unit tests the DisplayableCard Class
   /// </summary>
   [TestFixture]
   public class DisplayableCardUnitTest
   {
      [Test]
      public void Constructor()
      {
         DisplayableCard c = new DisplayableCard();

         Deck d = new Deck();
         c = new DisplayableCard(d, 4, Card.SUIT.HEARTS);
         Assert.AreEqual(c.FaceValue, 4);
         Assert.AreEqual(c.Suit, Card.SUIT.HEARTS);
         Assert.IsFalse(c.Dealt);
         Assert.IsTrue(c.FaceUp);
      }

      [Test]
      public void CardFamilies()
      {
         StringCollection al = DisplayableCard.CardFamilies;
         Assert.AreEqual(9, al.Count);
      }
      [Test]
      public void HeightWidth()
      {
         Assert.AreEqual(DisplayableCard.Height, 96);
         Assert.AreEqual(DisplayableCard.Width, 71);
      }

      [Test]
      public void DisplayAttribs()
      {
         Deck d = new Deck();
         DisplayableCard c = new DisplayableCard(d, 4, Card.SUIT.HEARTS);
         Assert.AreEqual(c.FaceValue, 4);
         Assert.AreEqual(c.Suit, Card.SUIT.HEARTS);
         Assert.IsFalse(c.Dealt);
         Assert.IsTrue(c.FaceUp);
         Assert.IsFalse(c.Selected);
         Assert.AreEqual(c.Orientation, DisplayableCard.ORIENTATION.NONE);
         c.Orientation = DisplayableCard.ORIENTATION.HORIZONTAL;
         Assert.AreEqual(c.Orientation, DisplayableCard.ORIENTATION.HORIZONTAL);

         Assert.AreEqual(c.GetRect(), new Rectangle(0, 0, DisplayableCard.Width+CardConfig.GetIntValue("ShadowHorizontal"), DisplayableCard.Height+CardConfig.GetIntValue("ShadowVertical")));

         Assert.IsTrue(c.IsHit(new Point(1,1)));
         Assert.IsFalse(c.IsHit(new Point(100,100)));

         c.SetPos(50, 50);
         Assert.IsTrue(c.IsHit(new Point(100,100)));

         c.Deal();
 
         c.ToggleSelect();
         Assert.IsTrue(c.Selected);
         c.Shuffle();

         Assert.IsFalse(c.Dealt);
         Assert.IsTrue(c.FaceUp);
         Assert.IsFalse(c.Selected);
      }
   }
   /// <summary>
   /// This class tests card appearance through the manual deal screen
   /// </summary>
   [TestFixture]
   public class CardAppearanceUnitTest
   {
      [Test]
      public void DavidsDeck1()
      {
         CardConfig.SetValue("CardFamily", "Davids Deck 1");
         Assert.AreEqual(CardConfig.GetStringValue("CardFamily"), "Davids Deck 1");

         DisplayableCard.Reset();

         Deck d = new Deck(null, true, false);
         Card c = d.DealOne();
      }
      [Test]
      public void DavidsDeck2()
      {
         CardConfig.SetValue("CardFamily", "Davids Deck 2");
         Assert.AreEqual(CardConfig.GetStringValue("CardFamily"), "Davids Deck 2");

         DisplayableCard.Reset();

         Deck d = new Deck(null, true, false);
         Card c = d.DealOne();
      }
   }
}