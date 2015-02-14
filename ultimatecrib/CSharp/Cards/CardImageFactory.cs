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

// Code complete & commented 5/9/2003 - KW

using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using Image;
using UserMessageEvent;

namespace Cards
{
      
   /// <summary>
   /// Class for constructing card images
   /// </summary>
   public class CardImageFactory : IDisposable
   {
      #region Member Variables

      static string CardImageXMLFileFileName = "CardImage.XML"; // file containing card definitions
      XmlDataDocument _cardImageDoc; // xml document that defines all cards
      XmlNode _nodeFamily = null;    // node of current family

      bool _overlayLabel = true;    // true if we need to put the card value on the card for the current family
      bool _useSymbol = true;       // true if we should be using symbol font for the card suit in the current family
      bool _border = true;          // true if we need to draw a border on the card
      string _represents = "Deck";  // what each image represents (Deck, Suit, FaceValue, Card)

      #endregion

      #region Delegates

      /// <summary>
      /// fire the user message event 
      /// </summary>
      /// <param name="message">Message to the user</param>
      void FireUserMessage(string message)
      {
         UserMessage um = new UserMessage(message);
         um.Fire();
      }

      #endregion
   
      #region Constructors

      /// <summary>
      /// Create a card image factory
      /// </summary>
      public CardImageFactory()
      {
         try
         {
            // load xml file
            _cardImageDoc = new XmlDataDocument();
            _cardImageDoc.Load(CardImageXMLFile);

            // create the family search string
            string s = "/CardImage/Families/Family[@Name='" + CardConfig.GetStringValue("CardFamily") + "']";
            try
            {
               // try to find the family
               _nodeFamily = _cardImageDoc.SelectSingleNode(s);

               // if we found it
               if (_nodeFamily != null)
               {
                  // read in family attributes
                  _represents = _nodeFamily.Attributes["Represents"].Value;
                  _overlayLabel = Convert.ToBoolean(_nodeFamily.Attributes["OverlayLabel"].Value);
                  _useSymbol = Convert.ToBoolean(_nodeFamily.Attributes["UseSymbol"].Value);
                  _border = Convert.ToBoolean(_nodeFamily.Attributes["DrawBorder"].Value);
               }
               else
               {
                  // family not found so throw an error
                  throw new ApplicationException("Card Image Family '" + s + " does not exist in " + CardImageXMLFile);
               }
            }
            catch
            {
               // family not found so throw an error
               _nodeFamily = null;
               throw new ApplicationException("Card Image Family '" + s + " does not exist in " + CardImageXMLFile);
            }
         }
         // catch any errors reading XML
         catch(ApplicationException ae)
         {
            // set xml fields to null
            _nodeFamily = null;

            // notify anyone interested in user messages
            FireUserMessage(ae.Message);
         }
         catch
         {
            if (_cardImageDoc.OuterXml == "")
            {
               _cardImageDoc = null;
            }

            // set xml fields to null
            _nodeFamily = null;

            // notify anyone interested in user messages
            FireUserMessage("Error loading " + CardImageXMLFile);
         }
      }
      
      #endregion

      #region IDisposable
      /// <summary>
      /// Dispose of resources
      /// </summary>
      public void Dispose()
      {
         // set xml fields to null
         _cardImageDoc = null;
         _nodeFamily = null;
      }
      #endregion

      #region Private Member Functions

      /// <summary>
      /// Gets the default card bitmap of given size
      /// </summary>
      /// <param name="size">Size of card to create</param>
      /// <param name="faceValue">Face Value of card</param>
      /// <param name="suit">Suit of card</param>
      /// <returns>Card Bitmap</returns>
      Bitmap GetDefaultCard(Size size, string faceValue, string suit)
      {
         return GetImage("Bitmap", "", 0, size, faceValue, suit, 0, 0, 0, 0, "Center", true, true, true);
      }

      /// <summary>
      /// Gets the default card back of given size
      /// </summary>
      /// <param name="size">Size of card to create</param>
      /// <returns>Card Bitmap</returns>
      Bitmap GetDefaultCardBackImage(Size size)
      {
         return GetImage("Bitmap", "", 0, size, "1", "S", 0, 0, 0, 0, "Center", false, false, true);
      }

      /// <summary>
      /// Gets a card image based on an XML node
      /// </summary>
      /// <param name="imageNode">xml node to use to get other details</param>
      /// <param name="size">Size of the bitmap to create</param>
      /// <param name="faceValue">Face value of the card</param>
      /// <param name="suit">Suit of the card</param>
      /// <param name="overlay">Indicates if the card value and suit should be painted onto the card</param>
      /// <param name="symbol">Inidcates if we should display the initial of the suit or a suit image</param>
      Bitmap GetImage(XmlNode imageNode, Size size, string faceValue, string suit, bool overlay, bool symbol, bool border)
      {
         // if it is a bitmap
         if (imageNode.Attributes["Type"].Value == "Bitmap")
         {
            // decode xmlNode and call get image
            return GetImage(imageNode.Attributes["Type"].Value, imageNode.Attributes["FileName"].Value, 0, size, faceValue, suit,Convert.ToInt32(imageNode.Attributes["CropLeft"].Value, 10), Convert.ToInt32(imageNode.Attributes["CropRight"].Value, 10),Convert.ToInt32(imageNode.Attributes["CropTop"].Value, 10),Convert.ToInt32(imageNode.Attributes["CropBottom"].Value, 10), imageNode.Attributes["Render"].Value, overlay, symbol, border);
         }
         else
         {
            // needs to be loaded from a dll
            // decode xmlNode and call get image
            return GetImage(imageNode.Attributes["Type"].Value, imageNode.Attributes["FileName"].Value, Convert.ToInt32(imageNode.Attributes["Resource"].Value,10), size, faceValue, suit, Convert.ToInt32(imageNode.Attributes["CropLeft"].Value, 10), Convert.ToInt32(imageNode.Attributes["CropRight"].Value, 10),Convert.ToInt32(imageNode.Attributes["CropTop"].Value, 10),Convert.ToInt32(imageNode.Attributes["CropBottom"].Value, 10), imageNode.Attributes["Render"].Value, overlay, symbol, border);
         }
      }


      #endregion

      #region Private Static Functions
      /// <summary>
      /// Gets a card image
      /// </summary>
      /// <param name="type">Type of image. Can be 'Bitmap' or 'Resource'</param>
      /// <param name="fileName">Bitmap file name or file containing the resource</param>
      /// <param name="resource">Resource identifier</param>
      /// <param name="size">Size of the bitmap to create</param>
      /// <param name="faceValue">Face value of the card</param>
      /// <param name="suit">Suit of the card</param>
      /// <param name="cropLeft">Pixels to crop of the left of the image</param>
      /// <param name="cropRight">Pixels to crop of the right of the image</param>
      /// <param name="cropTop">Pixels to crop of the top of the image</param>
      /// <param name="cropBottom">Pixels to crop of the bottom of the image</param>
      /// <param name="render">How to render the image. Can be 'Centre', 'Center', 'Scale' or 'Tile'</param>
      /// <param name="overlay">Indicates if the card value and suit should be painted onto the card</param>
      /// <param name="symbol">Inidcates if we should display the initial of the suit or a suit image</param>
      /// <returns>Card Bitmap</returns>
      static Bitmap GetImage(string type, string fileName, int resource, Size size, string faceValue, string suit, int cropLeft, int cropRight, int cropTop, int cropBottom, string render, bool overlay, bool symbol, bool border)
      {
         // use the base class to create image
         Bitmap RC = ImageFactory.GetImage(type, fileName, resource, size, cropLeft, cropRight, cropTop, cropBottom, render, Color.Transparent);

         // apply card features
         Graphics graphics = Graphics.FromImage(RC);

         if (border)
         {
            // apply border
            Point[] arrayPoints = {new Point(2,0),
                                     new Point(size.Width-3, 0), 
                                     new Point(size.Width-1, 2), 
                                     new Point(size.Width-1, size.Height-3), 
                                     new Point(size.Width-3, size.Height-1),
                                     new Point(2, size.Height-1),
                                     new Point(0, size.Height-3),
                                     new Point(0, 2),
                                     new Point(2,0)
                                  };
            graphics.DrawLines(new Pen(Color.Black), arrayPoints);
         }

         // if we are meant to write on the card
         if (overlay)
         {
            // create a brush to write on card with
            Brush brush = null;
            if (suit == "H" || suit == "D")
            {
               brush = new SolidBrush(Color.Red);
            }
            else
            {
               brush = new SolidBrush(Color.Black);
            }

            // create a font for the value
            Font font = font = new Font(FontFamily.GenericSerif, size.Height/10);

            // create a font for the suit
            Font fontSuit = null;
            if (symbol)
            {
               fontSuit = new Font("Symbol", size.Height/10);

               switch(suit)
               {
                  case "C":
                     suit = "\xA7";
                     break;
                  case "D":
                     suit = "\xA8";
                     break;
                  case "H":
                     suit = "\xA9";
                     break;
                  case "S":
                     suit = "\xAA";
                     break;
               }
            }
            else
            {
               fontSuit = font;
            }

            // create a text outline brush
            Brush whiteBrush = new SolidBrush(Color.White);

            // set up draw format
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;

            // where we start writing
            int iXOrigin = font.Height/2 + 2;
            int iYOrigin = 3;

            // write on the value in upper left
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin-1,iYOrigin), drawFormat);
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin+1,iYOrigin), drawFormat);
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin,iYOrigin-1), drawFormat);
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin,iYOrigin+1), drawFormat);
            graphics.DrawString(faceValue, font, brush, new Point(iXOrigin,iYOrigin), drawFormat);

            // write on the font in upper left
            int iY = iYOrigin + font.Height - 3;
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin-1, iY), drawFormat);
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin+1, iY), drawFormat);
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin, iY-1), drawFormat);
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin, iY+1), drawFormat);
            graphics.DrawString(suit, fontSuit, brush, new Point(iXOrigin,iY), drawFormat);

            // write on the font in lower right
            iXOrigin = size.Width - iXOrigin;
            iYOrigin = size.Height - 3 - font.Height - fontSuit.Height;
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin-1,iYOrigin), drawFormat);
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin+1,iYOrigin), drawFormat);
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin,iYOrigin-1), drawFormat);
            graphics.DrawString(suit, fontSuit, whiteBrush, new Point(iXOrigin,iYOrigin+1), drawFormat);
            graphics.DrawString(suit, fontSuit, brush, new Point(iXOrigin,iYOrigin), drawFormat);

            // write on the value in lower right
            iY = iYOrigin + fontSuit.Height - 3;
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin-1, iY), drawFormat);
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin+1, iY), drawFormat);
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin, iY-1), drawFormat);
            graphics.DrawString(faceValue, font, whiteBrush, new Point(iXOrigin, iY+1), drawFormat);
            graphics.DrawString(faceValue, font, brush, new Point(iXOrigin,iY), drawFormat);

            // delete everything I no longer need
            brush.Dispose();
            whiteBrush.Dispose();
            font.Dispose();
            fontSuit.Dispose();
         }

         // delete everything I no longer need
         graphics.Dispose();

         // return the card image
         return RC;
      }
      #endregion

      #region Public Static Functions

      /// <summary>
      /// Get the name of the file that contains the card definitions
      /// </summary>
      public static String CardImageXMLFile
      {
         get
         {
            return CardImageXMLFileFileName;
         }
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// Gets a card shadow bitmap
      /// </summary>
      /// <param name="size">Size of the card</param>
      /// <returns>Card shadow image</returns>
      public Bitmap GetCardShadow(Size size)
      {
         // create a bitmap of the right size
         Bitmap RC = new Bitmap(size.Width, size.Height);

         // create an empty graphics containing just our bitmap
         Graphics graphics = Graphics.FromImage(RC);

         // create a hatched brush to draw our shadow with
         Brush brush = new HatchBrush(HatchStyle.Percent50, Color.Black, Color.Transparent);

         // create an array of points that outline a polygon for our card
         Point[] arrayPoints = {new Point(2,0),
                                  new Point(size.Width-3, 0), 
                                  new Point(size.Width-1, 2), 
                                  new Point(size.Width-1, size.Height-3), 
                                  new Point(size.Width-3, size.Height-1),
                                  new Point(2, size.Height-1),
                                  new Point(0, size.Height-3),
                                  new Point(0, 2),
                                  new Point(2,0)
                               };

         // fill the polygon with our brush
         graphics.FillPolygon(brush,arrayPoints);

         // lose the brush
         brush.Dispose();

         // lose the graphics
         graphics.Dispose();

         // return the bitmap
         return RC;
      }

      /// <summary>
      /// Get a card back image
      /// </summary>
      /// <param name="name">Name of the card back</param>
      /// <param name="size">Size of the card back</param>
      /// <returns>Card back image</returns>
      public Bitmap GetCardBackImage(string name, Size size)
      {
         // crate an empty bitmap of the right size
         Bitmap RC = new Bitmap(size.Width, size.Height);

         try
         {
            // if we have an xml document
            if (_cardImageDoc != null)
            {
               XmlNode imageNode = null;
               try
               {
                  // find the node for this card back
                  imageNode = _cardImageDoc.SelectSingleNode("CardImage/Decks/Deck[@Name='" + name + "']");

                  // if we did not find it
                  if (imageNode == null)
                  {
                     throw new ApplicationException("");
                  }
               }
               catch
               {
                  // error reading
                  throw new ApplicationException("Could not find definition for " + name + " in " + CardImageXMLFile);
               }
  
               // go and create the image
               RC = GetImage(imageNode, size, "1", "S", false, false, false);
            }
            else
            {
               // throw and create a default card back
               throw new ApplicationException("Default internal card back used.");
            }
         }
         catch(ApplicationException ae)
         {
            // tell the user what is happening
            FireUserMessage(ae.Message);

            // create an image using internal data only
            RC = GetDefaultCardBackImage(size);
         }

         // return the card bitmap
         return RC;
      }

      /// <summary>
      /// 
      /// </summary>
      public string CardTransparentColour
      {
         get
         {
            if (_nodeFamily != null)
            {
               return _nodeFamily.Attributes["TransparentColourName"].Value.ToString();
            }
            else
            {
               return string.Empty;
            }
         }
      }

      /// <summary>
      /// Get a card image
      /// </summary>
      /// <param name="faceValue">Face value of the card</param>
      /// <param name="suit">suit of the card</param>
      /// <param name="size">Size of the card</param>
      /// <returns>Card image</returns>
      public Bitmap GetCardImage(string faceValue, string suit, Size size)
      {
         // crate an empty bitmap of the right size
         Bitmap RC = new Bitmap(size.Width, size.Height);

         // get the image xml element
         string name = "";
         switch(_represents)
         {
            case "Deck":
               name = "";
               break;
            case "Suit":
               name = suit;
               break;
            case "FaceValue":
               name = faceValue;
               break;
            case "Card":
               name = faceValue + suit;
               break;
         }

         try
         {
            // if we have an xml document
            if (_cardImageDoc != null)
            {
               XmlNode imageNode = null;
               try
               {
                  // find the node for this card
                  imageNode = _nodeFamily.SelectSingleNode("Images/Image[@Name='" + name + "']");

                  // if we did not find it
                  if (imageNode == null)
                  {
                     throw new ApplicationException("");
                  }
               }
               catch
               {
                  // error reading
                  throw new ApplicationException("Could not find definition for " + faceValue + " in " + CardImageXMLFile);
               }

               // create the card image
               RC = GetImage(imageNode, size, faceValue, suit, _overlayLabel, _useSymbol, _border);
            }
            else
            {
               throw new ApplicationException("");
            }
         }
         catch(ApplicationException ae)
         {
            // tell the user what is happening
            if (ae.Message != "")
            {
               FireUserMessage(ae.Message);
            }

            // create an image using internal data only
            RC = GetDefaultCard(size, faceValue, suit);
         }

         // return the card bitmap
         return RC;
      }

      #endregion
   }
}
