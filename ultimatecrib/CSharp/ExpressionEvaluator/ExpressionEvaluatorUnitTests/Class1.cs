// Ultimate Cribbage
// ExpressionEvaluator Assembly

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

// This file contains the automated unit tests for the classes in the ExpressionEvaluator
// assembly.
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// ============================================================================

using System;
using NUnit.Framework;
using ExpressionEvaluator;

namespace ExpressionEvaluatorUnitTests
{
   /// <summary>
   /// This class unit tests the ExpressionEvaluator Class.
   /// </summary>
   [TestFixture]
   public class ExpressionEvaluatorUnitTest
   {
      [Test]
      public void EvaluatorTest()
      {
         Assert.AreEqual(Evaluator.EvaluateExpression("1+1"),2);
         Assert.AreEqual(Evaluator.EvaluateExpression("((1+1*4)*5+100)/5"),25);
         Assert.AreEqual(Evaluator.EvaluateExpression("10%3"),1);
      }      
   }
}
