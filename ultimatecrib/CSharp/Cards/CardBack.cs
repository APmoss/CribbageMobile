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

// Code complete & commented 3/9/2003 - KW

using System;
using System.Collections.Specialized;
using System.Xml;
using System.Drawing;

namespace Cards
{
   /// <summary>
   /// Class that represent a card back
   /// </summary>
   public class CardBack : DisplayableCard
   {

      #region Public Static Functions
      /// <summary>
      /// Returns a list of available card backs
      /// </summary>
      public static StringCollection CardBacks
      {
         get
         {
            StringCollection rc = new StringCollection();

            // read in layout xml
            XmlDataDocument xmlCardImage = new XmlDataDocument();
            xmlCardImage.Load(CardImageFactory.CardImageXMLFile);

            // locate the decks node
            string s = "/CardImage/Decks";
            XmlNode nodeDecks = xmlCardImage.SelectSingleNode(s);

            // add each deck name
            foreach (XmlNode node in nodeDecks.ChildNodes)
            {
               if (node.Name == "Deck")
               {
                  rc.Add(node.Attributes["Name"].Value);
               }
            }

            return rc;
         }
      }
      #endregion

      #region Constructors
      /// <summary>
      /// Implements the common piece of the constructor
      /// </summary>
      /// <param name="cardBackName">Name of the card back</param>
      void CommonConstructor(string cardBackName)
      {
         // this card must be face up
         FaceUp = true;
         Dealt = true;

         // use the Card Image Factory to create a bitmap
         _bitmap = __cardImageFactory.GetCardBackImage(cardBackName, new Size(Width, Height));

         // this card never belongs to a deck object
         _deck = null;
      }

      /// <summary>
      /// Create a card back with the named card back
      /// </summary>
      /// <param name="cardBackName">Card back to use from the CardImage XML file</param>
      public CardBack(string cardBackName) : base()
      {
         CommonConstructor(cardBackName);
      }

      /// <summary>
      /// Create a card back based on the card back name in card config
      /// </summary>
      public CardBack() : base()
      {
         CommonConstructor(CardConfig.GetStringValue("CardBackName"));
      }
      #endregion
   }
}
