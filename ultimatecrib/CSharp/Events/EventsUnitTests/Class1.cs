// Ultimate Cribbage
// Events Assembly

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

// This file contains the automated unit tests for the classes in the Events
// assembly 
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// ============================================================================

using System;
using NUnit.Framework;
using Events;
using UserMessageEvent;
using StarterCardDisplayedEvent;
using Cards;
using HandDealtEvent;

namespace EventsUnitTests
{
   /// <summary>
   /// Tests the base event class
   /// </summary>
   [TestFixture]
   public class BaseEventUnitTest
   {
      bool beh = false;
      private void BaseEventHandler(object sender, System.EventArgs e)
      {
         beh = true;
      }

      [Test]
      public void BaseEventTest()
      {
         beh = false;

         BaseEvent be = new BaseEvent();

         Assert.IsFalse(beh);
         
         be.Fire();

         Assert.IsFalse(beh);
         
         BaseEvent.Subscribe(new System.EventHandler(BaseEventHandler));

         Assert.IsFalse(beh);

         be.Fire();

         Assert.IsTrue(beh);

         beh = false;

         Assert.IsFalse(beh);

         BaseEvent.Unsubscribe(new System.EventHandler(BaseEventHandler));

         Assert.IsFalse(beh);

         be.Fire();

         Assert.IsFalse(beh);
      }
   }
   /// <summary>
   /// Tests the user message event class
   /// </summary>
   [TestFixture]
   public class UserMessageEventUnitTest
   {
      string um = string.Empty;
      private void UserMessageEventHandler(object sender, System.EventArgs e)
      {
         um = ((UserMessage)sender).Message;
      }

      [Test]
      public void UserMessageEventTest()
      {
         um = string.Empty;

         UserMessage ume = new UserMessage("FRED");

         Assert.AreEqual(string.Empty, um);
         
         ume.Fire();

         Assert.AreEqual(string.Empty, um);
         
         UserMessage.Subscribe(new System.EventHandler(UserMessageEventHandler));

         Assert.AreEqual(string.Empty, um);

         ume.Fire();

         Assert.AreEqual("FRED", um);

         um = string.Empty;

         Assert.AreEqual(string.Empty, um);

         UserMessage.Unsubscribe(new System.EventHandler(UserMessageEventHandler));

         Assert.AreEqual(string.Empty, um);

         ume.Fire();

         Assert.AreEqual(string.Empty, um);
      }
   }
   /// <summary>
   /// Tests the starter card displayed event class
   /// </summary>
   [TestFixture]
   public class StarterCardDisplayedEventUnitTest
   {
      Card sc = null;
      private void StarterCardDisplayedEventHandler(object sender, System.EventArgs e)
      {
         sc = ((StarterCardDisplayed)sender).Card;
      }

      [Test]
      public void StarterCardDisplayedEventTest()
      {
         sc = null;

         StarterCardDisplayed sce = new StarterCardDisplayed(new Card(10, Card.SUIT.HEARTS));

         Assert.AreEqual(null, sc);
         
         sce.Fire();

         Assert.AreEqual(null, sc);
         
         StarterCardDisplayed.Subscribe(new System.EventHandler(StarterCardDisplayedEventHandler));

         Assert.AreEqual(null, sc);

         sce.Fire();

         Assert.AreEqual(new Card(10, Card.SUIT.HEARTS).ToString(), sc.ToString());

         sc = null;

         Assert.AreEqual(null, sc);

         StarterCardDisplayed.Unsubscribe(new System.EventHandler(StarterCardDisplayedEventHandler));

         Assert.AreEqual(null, sc);

         sce.Fire();

         Assert.AreEqual(null, sc);
      }
   }
   /// <summary>
   /// Tests the base event class
   /// </summary>
   [TestFixture]
   public class HandDealtEventUnitTest
   {
      bool hd = false;
      private void HandDealtEventHandler(object sender, System.EventArgs e)
      {
         hd = true;
      }

      [Test]
      public void HandDealtEventTest()
      {
         hd = false;

         HandDealt hde = new HandDealt();

         Assert.IsFalse(hd);
         
         hde.Fire();

         Assert.IsFalse(hd);
         
         HandDealt.Subscribe(new System.EventHandler(HandDealtEventHandler));

         Assert.IsFalse(hd);

         hde.Fire();

         Assert.IsTrue(hd);

         hd = false;

         Assert.IsFalse(hd);

         HandDealt.Unsubscribe(new System.EventHandler(HandDealtEventHandler));

         Assert.IsFalse(hd);

         hde.Fire();

         Assert.IsFalse(hd);
      }
   }
}
