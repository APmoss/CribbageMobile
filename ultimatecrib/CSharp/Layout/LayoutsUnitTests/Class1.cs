// Ultimate Cribbage
// Layouts Assembly

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

// You will need to copy the Layout.XML file into the bin directory this assembly builds to

// ============================================================================

using System;
using NUnit.Framework;
using System.Collections.Specialized;
using Layouts;
using ApplicationConfig;

namespace LayoutsUnitTests
{
   [TestFixture]
   public class LayoutUnitTest
   {
      [Test]
      public void Static()
      {
         Assert.AreEqual(1, Layout.Layouts.Count);
      }
      [Test]
      public void Standard()
      {
         Config.TheConfig.SetValue("ultimatecrib", "Layout", "Standard");
         Layout.Reload();

         Assert.AreEqual("Standard", Layout.TheLayout.ToString());
         Assert.AreEqual(5, Layout.TheLayout.GetIntValue("ShadowOffsetX"));
         Assert.AreEqual(5, Layout.TheLayout.GetIntValue("PlayerPlayedY1"));
         Assert.AreEqual(5+96+5+20, Layout.TheLayout.GetIntValue("PlayerCribTextY1"));
      }
   }
}