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

using System;
using System.Data;

namespace ExpressionEvaluator
{
	/// <summary>
	/// This class provides Expression evaluation capabilities.
	/// </summary>
	public class Evaluator
	{
      /// <summary>
      /// Evaluate an integer Expression
      /// </summary>
      /// <param name="Expression">Expression to evaluate</param>
      /// <returns>result</returns>
      public static int EvaluateExpression(string Expression)
      {
         int rc = 0; // result value

         // Create a data table
         DataTable dt = new DataTable("Table");

         // add an integer column with the Expression as the column definition
         int i = 0;
         dt.Columns.Add("Column", i.GetType(), Expression);

         // create a new row
         DataRow dr = dt.NewRow();
         dt.Rows.Add(dr);

         // check we got a value
         if (dr["Column"] != DBNull.Value)
         {
            // read out the value of the Expression
            rc = (int)dr["Column"];
         }
         else
         {
            // Expression is invalid
            throw new ApplicationException("Invalid Expression : " + Expression);
         }

         return rc;
      }
   }
}
