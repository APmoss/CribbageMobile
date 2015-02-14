// Ultimate Cribbage
// CribbageBoard Assembly

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

// This file contains the automated unit tests for the classes in the CribbageBoard 
// assembly.
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// NOTE: Unit tests are sensitive to the provided Board.XML. If you change 
// this file you may need to update some of the unit tests

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// You will need to copy the Board.XML file into the bin directory this assembly builds to
// You will need to copy the Board.BMP file into the bin directory this assembly builds to
// You will need to copy the Counter.BMP file into the bin directory this assembly builds to
// You will need to copy the CribBoard1.BMP file into the bin directory this assembly builds to
// You will need to copy the bluepeg.BMP file into the bin directory this assembly builds to
// You will need to copy the yellowpeg.BMP file into the bin directory this assembly builds to

// ============================================================================

using System;
using NUnit.Framework;
using CribbageBoard;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace CribbageBoardUnitTests
{
   /// <summary>
   /// This class unit tests the CribbageBoard Class.
   /// </summary>
   [TestFixture]
   public class CribbageBoardUnitTest
   {
      [Test]
      public void BoardFail()
      {
         CribbageBoard.CribbageBoard b = new CribbageBoard.CribbageBoard();
         b.CribbageBoardXMLFile = "BoardDummy.XML";
         b.BoardName = "Basic";
         Assert.IsFalse(b.IsValid);
      }
      [Test]
      public void Board()
      {
         CribbageBoard.CribbageBoard b = new CribbageBoard.CribbageBoard();

         b.CribbageBoardXMLFile = "Board.XML";
         b.BoardName = "Basic";
         b.AllowUserBoardChange = false;
         b.AllowUserRotate = false;
         b.MaxScore = CribbageBoard.MAXSCORE.ONETWENTY;
         b.SetPlayerName(1,"Fred1");
         b.SetPlayerName(2,"Fred2");
         Assert.IsTrue(b.IsValid);

         StringCollection sc = b.Boards;
         Assert.AreEqual(7, sc.Count);

         b.AddToScore(1,10);
         b.AddToScore(2,11);
         Assert.AreEqual("Fred1: 10\nFred2: 11", b.ToString());
         Assert.IsTrue(b.IsValid);

         b.AddToScore(1,10);
         b.AddToScore(2,11);
         Assert.AreEqual("Fred1: 20\nFred2: 22", b.ToString());
         Assert.IsTrue(b.IsValid);

         b.SetScore(1, 10,11);
         b.SetScore(2, 11,14);
         Assert.IsTrue(b.IsValid);

         Assert.AreEqual("Fred1: 11\nFred2: 14", b.ToString());
         Assert.IsTrue(b.IsValid);
      }
      [Test]
      public void BoardAppearance()
      {
         Form1 f = new Form1();
         Assert.AreEqual(DialogResult.OK, f.ShowDialog());
      }
   }
}
