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

// Code complete & commented 9/9/2003 - KW

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using UserMessageEvent;
using System.Diagnostics;

namespace Cards
{
   /// <summary>
   /// Class used to represent a card that can be displayed on the screen
   /// </summary>
   public class DisplayableCard : Card
   {
      #region Enumerations
      /// <summary>
      /// ORIENTATION enumeration is for the direction the cards are being layed out 
      /// This impacts how card selection is shown
      /// </summary>
      public enum ORIENTATION {HORIZONTAL, VERTICAL, NONE};
      #endregion

      #region Static Member Variables
      // static card image factory is used for creating the bitmaps to display the card
      protected static CardImageFactory __cardImageFactory = new CardImageFactory();
      // the bitmap shadow is the card shadow used to give the cards a better appearance
      static System.Drawing.Bitmap __bitmapShadow = __cardImageFactory.GetCardShadow(new Size(Width, Height));
      #endregion

      #region Member Variables
      protected System.Drawing.Bitmap _bitmap = null; // card face bitmap
      int _x = 0; // x location of the card
      int _y = 0; // y location of the card
      bool _hide = false; // if a card is hidden then it is face down
      ORIENTATION _orientation = ORIENTATION.NONE; // orientation to use when displaying a selected card
      protected Deck _deck = null; // deck that owns this card. Required to get the card back when the card is face down
      bool _selected = false; // indicates the card should be displayed as selected
      string _transparentColour = "Transparent"; // colour to treat as transparent
      #endregion

      #region Delegates
      /// <summary>
      /// Fire the repaint required event
      /// </summary>
      /// <param name="oldLocationRect">Where the card was</param>
      void FireRepaintRequired(Rectangle oldLocationRect)
      {
         CardRepaintRequired crr = new CardRepaintRequired(this, oldLocationRect, GetRect());
         crr.Fire();
      }

      /// <summary>
      /// Fire the user message event
      /// </summary>
      /// <param name="message">User message</param>
      void FireUserMessage(string message)
      {
         UserMessage um = new UserMessage(message);
         um.Fire();
      }
      #endregion

      #region Constructors
      /// <summary>
      /// Common constructor code
      /// </summary>
      /// <param name="deck">Deck the card belongs to</param>
      void ConstructorCommon(Deck deck)
      {
         // save the deck we belong to
         _deck = deck;

         // shuffle the card ready for dealing
         Shuffle();

         // get the cards bitmap
         _bitmap = __cardImageFactory.GetCardImage(ToString().Substring(0,ToString().Length-1), ToString().Substring(ToString().Length-1), new Size(Width, Height));
         _transparentColour = __cardImageFactory.CardTransparentColour;
      }
      /// <summary>
      /// Displayable card constructor
      /// </summary>
      /// <param name="deck">Deck the card belongs to</param>
      /// <param name="sequentialValue">Card number</param>
       public DisplayableCard(Deck deck, int sequentialValue) : base(sequentialValue)
      {
         // call the common constructor
         ConstructorCommon(deck);
      }

      /// <summary>
      /// Displayable card constructor
      /// </summary>
      /// <param name="deck">Deck the card belongs to</param>
      /// <param name="faceValue">Cards face value</param>
      /// <param name="suit">Cards suit</param>
      public DisplayableCard(Deck deck, int faceValue, SUIT suit) : base(faceValue, suit)
      {
         // call the common constructor
         ConstructorCommon(deck);
      }

      /// <summary>
      /// Displayable card copy constructor
      /// </summary>
      /// <param name="card">Card to duplicate</param>
      public DisplayableCard(DisplayableCard card) : base(card)
      {
         // copy over all the fields
         _bitmap = card._bitmap;
         _x = card._x;
         _y = card._y;
         _hide = card._hide;
         _orientation = card._orientation;
         _deck = card._deck;
      }

      /// <summary>
      /// Displayable card void constructor
      /// </summary>
      public DisplayableCard() : base()
      {
      }

      #endregion

      #region Public Static Functions

      /// <summary>
      /// Forces reloading of static data
      /// </summary>
      public static void Reset()
      {
         __cardImageFactory = new CardImageFactory();
         __bitmapShadow = __cardImageFactory.GetCardShadow(new Size(Width, Height));
      }

      /// <summary>
      /// Get the list of available card families
      /// </summary>
      public static StringCollection CardFamilies
      {
         get
         {
            // create a results container
            StringCollection rc = new StringCollection();

            // read in layout xml
            XmlDataDocument xmlCardImage = new XmlDataDocument();
            xmlCardImage.Load(CardImageFactory.CardImageXMLFile);

            // Fing the families
            string s = "/CardImage/Families";
            XmlNode nodeFamilies = xmlCardImage.SelectSingleNode(s);

            // For each family node
            foreach (XmlNode node in nodeFamilies.ChildNodes)
            {
               // Only add family nodes (ie ignore comments)
               if (node.Name == "Family")
               {
                  // add it to our list
                  rc.Add(node.Attributes["Name"].Value);
               }
            }

            // return it
            return rc;
         }
      }
      /// <summary>
      /// Gets the cards display height
      /// </summary>
      public static int Height
      {
         get
         {
            return CardConfig.GetIntValue("CardHeight");
         }
      }

      /// <summary>
      /// Gets the cards display width
      /// </summary>
      public static int Width
      {
         get
         {
            return CardConfig.GetIntValue("CardWidth");
         }
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// Shuffle a card in the deck
      /// </summary>
      override public void Shuffle() 
      {
         // shuffle the underlying card object
         base.Shuffle();

         // cards are by default face up
         FaceUp = true;

         // card are by default not selected
         Selected = false;
      }

      /// <summary>
      /// Store an orientation for a card
      /// cards oriented vertically are offset in a X direction when selected
      /// cards oriented horizontally are offset in a Y direction when selected
      /// </summary>
      /// <param name="orientation">Orientation of the card</param>
      public ORIENTATION Orientation
      {
         get
         {
            return _orientation;
         }
         set
         {
            // if the orientation is different than what we already have
            if (value != _orientation)
            {
               // save where the card is now
               Rectangle oldLocationRect = GetRect();

               // change the orientation
               _orientation = value;

               // tell anyone who is interested that they might need to redraw us
               FireRepaintRequired(oldLocationRect);
            }
         }
      }

      /// <summary>
      /// Display the card onto the given graphics at our internally remembered location
      /// </summary>
      /// <param name="graphics">Device to draw the card on</param>
      public void Display(System.Drawing.Graphics graphics)
      {
         // call the generic display function with our remembered display location
         Display(graphics, _x, _y, _orientation);
      }

      /// <summary>
      /// Display the card at the given location
      /// </summary>
      /// <param name="graphics">Device to draw the card on</param>
      /// <param name="x">X location to draw the card</param>
      /// <param name="y">Y location to draw the card</param>
      /// <param name="orientation">Orientation of the drawn card</param>
      public void Display(System.Drawing.Graphics graphics, int x, int y, ORIENTATION orientation)
      {
         // save the location and orientation
         _x = x;
         _y = y;
         _orientation = orientation;

         // only dealt cards can be displayed
         if (Dealt)
         {
            // calculate exact display position taking into account card selection
            int paintX = _x;
            int iPaintY = _y;

            // if card is selected
            if (Selected)
            {
               // if card is vertical
               if (_orientation == ORIENTATION.VERTICAL)
               {
                  // offset it in a X direction
                  paintX = paintX + CardConfig.GetIntValue("SelectedHorizontalOffset");
               }
               else
               {
                  // offset it in a Y direction
                  iPaintY = iPaintY - CardConfig.GetIntValue("SelectedVerticalOffset");
               }
            }

            // if we are face up
            if (!_hide)
            {
               // draw the card shadow first
               graphics.DrawImage(__bitmapShadow, new Point(paintX + CardConfig.GetIntValue("ShadowHorizontal"), iPaintY + CardConfig.GetIntValue("ShadowVertical")));

               ColorMap[] colorMap = new ColorMap[1];
               colorMap[0] = new ColorMap();
               ImageAttributes attr = new ImageAttributes();

               //TODO This should be read from a config file
               if (_transparentColour != string.Empty)
               {
                  colorMap[0].OldColor = Color.FromName(_transparentColour);
                  if (colorMap[0].OldColor == Color.Empty)
                  {
                     throw new ApplicationException("Transparent Colour " + _transparentColour + " not known.");
                  }
                  colorMap[0].NewColor = Color.Transparent;
                  attr.SetRemapTable(colorMap);
               }

               // now draw the card on top
               graphics.DrawImage(_bitmap, new Rectangle(new Point(paintX,iPaintY), _bitmap.Size), 0, 0, _bitmap.Width, _bitmap.Height, GraphicsUnit.Pixel, attr);
            }
            else
            {
               // card is face down so need to paint the card back
               _deck.CardBack.Display(graphics, paintX, iPaintY, orientation);
            }
         }
      }

      /// <summary>
      /// Returns true if the given point is within the card rect
      /// </summary>
      /// <param name="point">Point to check</param>
      /// <returns>true if point is in card rect</returns>
      public bool IsHit(System.Drawing.Point point)
      {
         // return true if the cards rect contains the given point
         return GetRect().Contains(point);
      }

      /// <summary>
      /// Returns the cards current display rectangle
      /// </summary>
      /// <returns>Rectangle card sits in</returns>
      public System.Drawing.Rectangle GetRect()
      {
         // create a rectangle for the card + shadow
         System.Drawing.Rectangle rc = new System.Drawing.Rectangle(_x, _y, Width + CardConfig.GetIntValue("ShadowHorizontal"), Height + CardConfig.GetIntValue("ShadowVertical"));

         // if the card is selected then the rect moves
         if (Selected)
         {
            // if the card is vertical
            if (_orientation == ORIENTATION.VERTICAL)
            {
               // offset it in an X direction
               rc.X = rc.X + CardConfig.GetIntValue("SelectedHorizontalOffset");
            }
            else
            {
               // offset it in a Y direction
               rc.Y = rc.Y - CardConfig.GetIntValue("SelectedVerticalOffset");
            }
         }

         // return the rectangle
         return rc;
      }

      /// <summary>
      /// Get the cards bitmap
      /// </summary>
      public Bitmap Bitmap
      {
         get
         {
            return _bitmap;
         }
      }

      /// <summary>
      /// Set the cards position on the screen
      /// </summary>
      /// <param name="x">X coordinate</param>
      /// <param name="y">Y coordinate</param>
      public void SetPos(int x, int y)
      {
         // if the position is different to where we think it is now
         if (_x != x || _y != y)
         {
            // extract where it is now
            Rectangle oldLocationRect = GetRect();

            // save the new location
            _x = x;
            _y = y;

            // tell anyone interested that we need repainting
            FireRepaintRequired(oldLocationRect);
         }
      }

      /// <summary>
      /// Gets/sets if the card is face up or not
      /// </summary>
      public bool FaceUp
      {
         get
         {
            // return true if face up
            return !_hide;
         }
         set
         {
            // if new state is different to old state
            if (_hide == value)
            {
               // extract where it is now
               Rectangle oldLocationRect = GetRect();

               // turn it face up/down
               _hide = !value;

               // tell anyone interested that we need repainting
               FireRepaintRequired(oldLocationRect);
            }
         }
      }

      /// <summary>
      /// Gets/sets if the card is selected or not
      /// </summary>
      public bool Selected
      {
         get
         {
            // return selected state
            return _selected;
         }
         set
         {
            // if new state is different from old state
            if (_selected != value)
            {
               // extract where it is now
               Rectangle oldLocationRect = GetRect();

               // set the new value
               _selected = value;

               // tell anyone interested that we need repainting
               FireRepaintRequired(oldLocationRect);
            }
         }
      }

      /// <summary>
      /// Toggle our selection state
      /// </summary>
      /// <returns>New selected state</returns>
      // 
      public bool ToggleSelect()
      {
         // invert our selection state
         Selected = !Selected;

         // return the new state
         return Selected;
      }

      #endregion
   }
}
