// Ultimate Cribbage
// ApplicationConfig Assembly

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

// This file contains the automated unit tests for the classes in the ApplicationConfig
// assembly (excluding UI code).
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// ============================================================================

using System;
using NUnit.Framework;
using ApplicationConfig;

namespace ApplicationConfigUnitTests
{
	/// <summary>
   /// This class unit tests the ApplicationConfig Class.
   /// </summary>
   [TestFixture]
   public class ApplicationConfigUnitTest
   {
      [Test]
      public void ApplicationConfig()
      {
         Config.TheConfig.SetValue("VAR1", "A");
         Config.TheConfig.SetValue("VAR2", "B");
         Assert.AreEqual("A", Config.TheConfig.GetValue("VAR1"));

         Config.TheConfig.SetValue("SEC", "VAR3", "C");
         Config.TheConfig.SetValue("SEC", "VAR4", "D");
         Assert.AreEqual("A", Config.TheConfig.GetValue("VAR1"));
         Assert.AreEqual("C", Config.TheConfig.GetValue("SEC", "VAR3"));

         Config.TheConfig.Save();
         Config.TheConfig.Reload();

         Assert.AreEqual("A", Config.TheConfig.GetValue("VAR1"));
         Assert.AreEqual("C", Config.TheConfig.GetValue("SEC", "VAR3"));

         Assert.AreEqual(1, Config.TheConfig.GetValue("FRED", 1));
         Assert.AreEqual(2, Config.TheConfig.GetValue("SEC", "FRED", 2));
         Assert.AreEqual(2, Config.TheConfig.GetValue("SEC2", "FRED2", 2));

         Config.TheConfig.SetValue("VAR5", 1);
         DateTime dt = new DateTime(2003, 12, 5, 2, 22, 22);
         Config.TheConfig.SetValue("VAR6", dt);
         Assert.AreEqual(1, Config.TheConfig.GetValue("VAR5"));
         Assert.AreEqual(dt, Config.TheConfig.GetValue("VAR6"));

         Config.TheConfig.Save();
         Config.TheConfig.Reload();

         Assert.AreEqual(1, Config.TheConfig.GetValue("VAR5"));
         Assert.AreEqual(dt, Config.TheConfig.GetValue("VAR6"));
      }
   }
}
