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

using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace CribbageBoard
{

   /// <summary>
   /// This class represents a player displayed on the board
   /// </summary>
   class BoardPlayer
   {
      #region Member Variables
      int _playerNumber = 0; // The number of this player. Can only be 1 or 2
      int _score = 0; // the players current score
      int _lastScore = 0; // the players previous score
      MAXSCORE _maxScore = MAXSCORE.ONETWENTY; // the maximum score in this game
      Point[] _holes = new Point[121]; // the array that maps a score to a point on the screen
      Point _final = Point.Empty; // the hole to put the peg in when the game is finished
      Image _counter = null; // the counter bitmap
      Image _rotatedCounter = null; // rotated counter
      Color _clearColour = Color.Transparent; // the colour to make invisible when painting the counter
      Size _offset = new Size(0,0); // offset from the top left to use for all points
      SizeF _scale = new SizeF(1,1); // the scale to use to stretch the counter when drawing it
      bool _rotated = false; // if true then board is rotated 90 degrees
      Size _boardSize = new Size(0,0); // size of the board
      string _name = string.Empty; // players name
      #endregion

      #region Properties

      /// <summary>
      /// Players name
      /// </summary>
      public string Name
      {
         set
         {
            _name = value;
         }
         get
         {
            return _name;
         }
      }

      /// <summary>
      /// Board is rotated 90 degrees or not
      /// </summary>
      public bool Rotated
      {
         set
         {
            _rotated = value;
         }
      }

      /// <summary>
      /// Size of the board
      /// </summary>
      public Size BoardSize
      {
         set
         {
            _boardSize = value;
         }
      }

      /// <summary>
      /// Score that is currently being displayed for this player
      /// </summary>
      public int Score
      {
         get
         {
            return _score;
         }
         set
         {
            // score cannot exceed the max score
            if (value > (int)MaxScore)
            {
               _score = (int)MaxScore;
            }
               // score cannot be less than 0
            else if (value < 0)
            {
               _score = 0;
            }
            else
            {
               _score = value;
            }
         }
      }
      /// <summary>
      /// The score before the current score for this player
      /// </summary>
      public int LastScore
      {
         get
         {
            return _lastScore;
         }
         set
         {
            // score cannot exceed the max score
            if (value > (int)MaxScore)
            {
               _lastScore = (int)MaxScore;
            }
               // score cannot be less than 0
            else if (value < 0)
            {
               _lastScore = 0;
            }
            else
            {
               _lastScore = value;
            }
         }
      }
      /// <summary>
      /// The offset from the top left of the control to use as the offset for the counters
      /// </summary>
      public Size Offset
      {
         set 
         { 
            _offset = value;
         }
      }

      /// <summary>
      /// The stretch scale to use on the conters when painting them
      /// </summary>
      public SizeF Scale
      {
         set 
         { 
            _scale = value;
         }
      }

      // The maximum score to which to go.
      public MAXSCORE MaxScore
      {
         get
         {
            return _maxScore;
         }
         set
         {
            _maxScore = value;
         }
      }
      #endregion

      #region Public Methods

      /// <summary>
      /// Returns true if the class has been correctly initialised
      /// </summary>
      public bool IsValid
      {
         get
         {
            // player number must be 1 or 2
            if (_playerNumber == 1 || _playerNumber == 2)
            {

               // all holes must have a valid point
               for (int i = 0; i < 121; i++)
               {
                  if (_holes[i] == Point.Empty)
                  {
                     return false;
                  }
               }

               // final must have a valid point
               if (_final == Point.Empty)
               {
                  return false;
               }

               // counter must have a bitmap
               if (_counter == null || _rotatedCounter == null)
               {
                  return false;
               }

               return true;
            }
            else
            {
               return false;
            }
         }
      }


      /// <summary>
      /// Create a rectangle to draw the counter in
      /// </summary>
      Rectangle CalcCounterRect(int score, Image counter)
      {
         // grab the right hole
         Point loc;

         // if we are at the max score then use the final hole
         if (score == (int)MaxScore)
         {
            loc = _final;
         }
            // else use the hole for this score
         else
         {
            loc = _holes[score];
         }

         // apply the scaling factor
         loc.X = (int)((float)loc.X * _scale.Width);
         loc.Y = (int)((float)loc.Y * _scale.Height);

         // rotate the coordinates if board is rotated
         if (_rotated)
         {
            int x = _boardSize.Height - loc.Y;
            loc.Y = loc.X;
            loc.X = x;
            loc.X = loc.X - counter.Width;
         }

         // add the offset
         loc = loc + _offset;

         // create and return a rectangle
         return new Rectangle(loc.X, loc.Y, (int)(counter.Width * _scale.Width),  (int)(counter.Height * _scale.Height));
      }

      
      /// <summary>
      /// Paint a players counters
      /// </summary>
      /// <param name="g"></param>
      public void Paint(Graphics g)
      {
         // only try to paint if everything is valid
         if (IsValid)
         {
            // create a colour map to make our clear colour clear
            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = _clearColour;
            colorMap[0].NewColor = Color.Transparent;
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);

            // pick the right counter depending on rotation
            Image paintCounter;
            if (_rotated)
            {
               paintCounter = _rotatedCounter;
            }
            else
            {
               paintCounter = _counter;
            }

            // Draw the score counter
            Rectangle dest = CalcCounterRect(_score, paintCounter);
            g.DrawImage(paintCounter, dest, 0, 0, paintCounter.Width, paintCounter.Height, g.PageUnit, attr);

            // Draw the last score counter
            dest = CalcCounterRect(_lastScore, paintCounter);
            g.DrawImage(paintCounter, dest, 0, 0, paintCounter.Width, paintCounter.Height, g.PageUnit, attr);
         }
      }

      
      /// <summary>
      /// Add a given number of points to the players score
      /// </summary>
      /// <param name="score">Number of points to add.</param>
      /// <returns></returns>
      public int AddToScore(int score)
      {
         // Scores less than 0 not allowed
         Debug.Assert(score >= 0);

         // 29 is the maximum score for a hand in cribbage
         Debug.Assert(score <= 29);

         // save the current score in the last score
         _lastScore = _score;

         // increment the score
         _score += score;

         // dont let it go past the maximum
         if (_score > (int)_maxScore)
         {
            _score = (int)_maxScore;
         }

         // return the new score
         return _score;
      }


      /// <summary>
      /// Load the player specific setting from the XML file
      /// </summary>
      /// <param name="nodePlayer"></param>
      public void LoadSettings(XmlNode nodePlayer)
      {
         // reinitialise everything first
         _final = Point.Empty;
         _counter = null;
         for (int i = 0; i < 121; i++)
         {
            _holes[i] = Point.Empty;
         }

         // Load the counter bitmap first
         try
         {
            _counter = Bitmap.FromFile(nodePlayer.Attributes.GetNamedItem("Bitmap").Value);

            // create a rotated copy
            _rotatedCounter = (Image)_counter.Clone();
            _rotatedCounter.RotateFlip(RotateFlipType.Rotate90FlipNone);
         }
         catch (Exception e)
         {
            Debug.WriteLine("Could not load player counter bitmap: " + e.Message);
            return;
         }

         // now load the transparent colour in the bitmap
         try
         {
            _clearColour = Color.FromName(nodePlayer.Attributes.GetNamedItem("Transparent").Value);
         }
         catch
         {
            // just default it to effectively no colour
            _clearColour = Color.Transparent;
         }

         // now load the hole positions
         try
         {
            // for each hole position specifier
            foreach(XmlNode xmlNode in nodePlayer.ChildNodes)
            {
               if (xmlNode.Name == "Hole")
               {
                  // process the node acording to type
                  switch(xmlNode.Attributes["Number"].Value)
                  {
                     case "Range":
                        // for each hole within the range
                        for (int j = Convert.ToInt32(xmlNode.Attributes["Low"].Value,10);  j <= Convert.ToInt32(xmlNode.Attributes["High"].Value,10); j++)
                        {
                           // get the x and y values
                           string sX = xmlNode.Attributes["X"].Value;
                           string sY = xmlNode.Attributes["Y"].Value;

                           // replace '[Score]' with the hole number
                           sX = sX.Replace("[Score]", j.ToString());
                           sY = sY.Replace("[Score]", j.ToString());

                           // calculate the hole position
                           _holes[j] = new Point(ExpressionEvaluator.Evaluator.EvaluateExpression(sX), ExpressionEvaluator.Evaluator.EvaluateExpression(sY));
                        }
                        break;
                     case "Final":
                        // This is the hole to put the pegs in once the player wins

                        // calculate the hole position
                        _final = new Point(Convert.ToInt32(xmlNode.Attributes["X"].Value,10), Convert.ToInt32(xmlNode.Attributes["Y"].Value,10));
                        break;
                     default:

                        // This is a specific hole position

                        // calculate the hole position
                        _holes[Convert.ToInt32(xmlNode.Attributes["Number"].Value, 10)] = new Point(ExpressionEvaluator.Evaluator.EvaluateExpression(xmlNode.Attributes["X"].Value), ExpressionEvaluator.Evaluator.EvaluateExpression(xmlNode.Attributes["Y"].Value));
                        break;
                  }
               }
            }
         }
         catch (Exception e)
         {
            Debug.WriteLine("Error occured loading counter positions: " + e.Message);
         }
      }
      #endregion

      #region Constructors
      public BoardPlayer(int num)
      {
         // only 1 or 2 is valid
         Debug.Assert(num == 1 || num == 2);

         // save the player number
         _playerNumber = num;

         Name = "Player " + num.ToString();

         // initialise the holes array
         for (int i = 0; i < 121; i++)
         {
            _holes[i] = Point.Empty;
         }
      }
      #endregion

   }
}
