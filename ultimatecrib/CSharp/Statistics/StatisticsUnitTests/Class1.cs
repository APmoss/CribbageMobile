// Ultimate Cribbage
// Statistics Assembly

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

// This file contains the automated unit tests for the classes in the Statistics
// assembly (excluding UI code).
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// You will need to have Statistics.XML available in the bin directory

// ============================================================================

using System;
using NUnit.Framework;
using Statistics;

namespace StatisticsUnitTests
{
   /// <summary>
   /// This class unit tests the Statistics Class.
   /// </summary>
   [TestFixture]
   public class StatisticsUnitTests
   {
      [Test]
      public void CaptureStats()
      {
         StatisticsCapture sc1 = new StatisticsCapture("P1", "P2");
         StatisticsCapture sc2 = new StatisticsCapture("P2", "P1");

         StatisticsCapture.TheStatisticCapture("P1").SetTo("Abandoned", 0);
         StatisticsCapture.TheStatisticCapture("P1").AlterBy("Abandoned", 1);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Abandoned"), 1);
         StatisticsCapture.TheStatisticCapture("P1").AlterBy("Abandoned", -1);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Abandoned"), 0);

         StatisticsCapture.TheStatisticCapture("P1").SetTo("Winning Game Streak", 0);
         StatisticsCapture.TheStatisticCapture("P1").SetTo("Losing Game Streak", 0);

         StatisticsCapture.TheStatisticCapture("P1").SetTo("Longest Game Winning Streak", 1);
         StatisticsCapture.TheStatisticCapture("P1").IncreaseTo("Longest Game Winning Streak", 0);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Longest Game Winning Streak"), 1);
         StatisticsCapture.TheStatisticCapture("P1").IncreaseTo("Longest Game Winning Streak", 1);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Longest Game Winning Streak"), 1);
         StatisticsCapture.TheStatisticCapture("P1").IncreaseTo("Longest Game Winning Streak", 2);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Longest Game Winning Streak"), 2);
         
         StatisticsCapture.TheStatisticCapture("P1").SetTo("Minimum Hands", 10);
         StatisticsCapture.TheStatisticCapture("P1").DecreaseTo("Minimum Hands", 11);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Minimum Hands"), 10);
         StatisticsCapture.TheStatisticCapture("P1").DecreaseTo("Minimum Hands", 10);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Minimum Hands"), 10);
         StatisticsCapture.TheStatisticCapture("P1").DecreaseTo("Minimum Hands", 9);
         Assert.AreEqual(StatisticsCapture.TheStatisticCapture("P1").Value("Minimum Hands"), 9);

         Assert.AreEqual("P1", StatisticsCapture.TheStatisticCapture("P1").ToString());

         StatisticsCapture.TheStatisticCapture("P1").SaveGame();
         StatisticsCapture.TheStatisticCapture("P1").SaveMatch();
      }
      [Test]
      public void ShowStats()
      {
         ShowStatistics d = new ShowStatistics("P1");

         d.ShowDialog();
      }
   }
}
