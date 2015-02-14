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
using System.Collections;
using System.Xml;
using System.Text;

namespace Cards
{
	/// <summary>
	/// Card represents a playing card. 
	/// For a displayable playing card use DisplayableCard
	/// </summary>
	public class Card
   {

      #region Enums
      
      /// <summary>
      /// SUIT is an enumeration for the suits in a deck of cards
      /// </summary>
      public enum SUIT {NO_SUIT, HEARTS, SPADES, CLUBS, DIAMONDS};
      
      #endregion

      #region Member variables

      bool _dealt = false; // indicates the card has been dealt from a deck
      int _faceValue = -1; // face value of the card. -1 is undefined 1=Ace 13=King
      SUIT _suit = SUIT.NO_SUIT; // suit of the card

      #endregion

      #region Delegates

      // event fired when a card needs repainting
      public delegate int CardSpecialValue(Card card); 
      public static CardSpecialValue OnCardSpecialValue = null; 

      #endregion

      #region Constructors

      /// <summary>
      /// Create a defaukt card - no suit - no face value
      /// </summary>
      public Card()
      {
         // use defaults in member variable definition
      }

      /// <summary>
      /// Create a card based on an integere between 1 and 53
      /// Order is A-K, Clubs Diamonds Hearts Spades, Joker
      /// </summary>
      /// <param name="SequentialValue">A number between 1 and 53</param>
		public Card(int SequentialValue)
		{
         // Check the value is within bounds
         if (SequentialValue < 1 || SequentialValue > 53)
         {
            throw new ApplicationException("Card(int): int must be 1-53. Value passed " + SequentialValue.ToString());
         }

         // reset the card
         Shuffle();

         // work out the real face value
         if (SequentialValue == 53)
         {
            // joker
            _faceValue = 0;
         }
         else
         {
            // normal card - so work out face value
            _faceValue = SequentialValue % 13;
            if (_faceValue == 0)
            {
               _faceValue = 13;
            }
         }

         // work out the cards suit
         if (SequentialValue <= 13)
         {
            _suit = SUIT.CLUBS;
         }
         else if (SequentialValue <= 26)
         {
            _suit = SUIT.DIAMONDS;
         }
         else if (SequentialValue <= 39)
         {              
            _suit = SUIT.HEARTS;
         }
         else if (SequentialValue <= 52)
         {             
            _suit = SUIT.SPADES;
         }            
         else
         {
            _suit = SUIT.NO_SUIT;
         }
      }

      /// <summary>
      /// Create a card based on a face value and a suit
      /// To create a joker pass 0 and no suit
      /// </summary>
      /// <param name="FaceValue">Face value of the card</param>
      /// <param name="Suit">Suit of the card</param>
      public Card(int FaceValue, SUIT Suit)
      {
         // check face value
         if (FaceValue < 0 || FaceValue > 13)
         {
            throw new ApplicationException("Card(int,SUIT): int must be 0-13. Value passed " + FaceValue.ToString());
         }

         // reset the card
         Shuffle();

         // save the specified face value & suit
         _faceValue = FaceValue;
         _suit = Suit;
      }

      /// <summary>
      /// Copy constructor
      /// </summary>
      /// <param name="card">Card to copy</param>
      public Card(Card card)
      {
         // chekc a card was passed
         if (card == null)
         {
            throw new ApplicationException("Card(Card): Copy constructor cannot be passed a null card.");
         }

         // copy over the member variables from the specified card
         _dealt = card.Dealt;
         _faceValue = card.FaceValue;
         _suit = card.Suit;
      }

      #endregion

      #region Operators
      /// <summary>
      /// check if card 1 is greater than card 2
      /// greater is defined as larger face value
      /// </summary>
      /// <param name="card1">First card</param>
      /// <param name="card2">Second card</param>
      /// <returns>True if card1 is greater than card 2</returns>
      public static bool operator>(Card card1, Card card2)
      {
         return card1.FaceValue > card2.FaceValue;
      }

      /// <summary>
      /// check if card 1 is less than card 2
      /// greater is defined as larger face value
      /// </summary>
      /// <param name="card1">First card</param>
      /// <param name="card2">Second Card</param>
      /// <returns>True is card1 is less than card 2</returns>
      public static bool operator<(Card card1, Card card2)
      {
         return card1.FaceValue < card2.FaceValue;
      }

      /// <summary>
      /// check if card1 is the same as card2.
      /// equality is defined as same face value and same suit
      /// </summary>
      /// <param name="card">Card to compare to</param>
      /// <returns>True if cards are the same</returns>
      public bool Equals(Card card)
      {
         return (_faceValue == card.FaceValue && _suit == card.Suit);
      }
      #endregion

      #region Public Member Functions
      /// <summary>
      /// Generate the short text name of the card
      /// </summary>
      /// <returns>Text name for card</returns>
      override public string ToString()
      {
         // return value
         StringBuilder rc = new StringBuilder(10);

         // convert the face value into something displayable. A,2-10,J,Q,K
         switch (_faceValue)
         {
            case 11:
               rc.Append("J");
               break;
            case 12:
               rc.Append("Q");
               break;
            case 13:
               rc.Append("K");
               break;
            case 1:
               rc.Append("A");
               break;
            case 0:
               rc.Append("JOKER");
               break;
            default:
               rc.Append(_faceValue.ToString());
               break;
         }

         // convert the suit into something displayable D,H,S,C,N
         switch (_suit)
         {
            case SUIT.DIAMONDS:
               rc.Append("D");
               break;
            case SUIT.HEARTS:
               rc.Append("H");
               break;
            case SUIT.SPADES:
               rc.Append("S");
               break;
            case SUIT.CLUBS:
               rc.Append("C");
               break;
            case SUIT.NO_SUIT:
               break;
         }

         // return the result
         return rc.ToString();
      }

      /// <summary>
      /// Generate a long name for the card
      /// </summary>
      /// <returns>Long name for the card</returns>
      public string ToLongString()
      {
         // return value
         StringBuilder rc = new StringBuilder(" ", 30);

         // generate the face value part of the name
         switch (_faceValue)
         {
            case 11:
               rc.Append("Jack");
               break;
            case 12:
               rc.Append("Queen");
               break;
            case 13:
               rc.Append("King");
               break;
            case 1:
               rc.Append("Ace");
               break;
            case 0:
               rc.Append("Joker");
               break;
            default:
               rc.Append(FaceValue.ToString());
               break;
         }

         // generate the suit part of the name
         switch (_suit)
         {
            case SUIT.DIAMONDS:
               rc.Append(" of Diamonds");
               break;
            case SUIT.HEARTS:
               rc.Append(" of Hearts");
               break;
            case SUIT.SPADES:
               rc.Append(" of Spades");
               break;
            case SUIT.CLUBS:
               rc.Append(" of Clubs");
               break;
            case SUIT.NO_SUIT:
               break;
         }

         // return the name
         return rc.ToString();
      }

      /// <summary>
      /// get the value of the card in a game.
      /// this is not the face value but the value of the card in a specific game
      /// </summary>
      public int SpecialValue
      {
         get
         {
            // if no delegate defined
            if (OnCardSpecialValue == null)
            {
               // just return the face value
               return _faceValue;
            }
            else
            {
               // return the delgated value
               return OnCardSpecialValue(this);
            }
         }
      }

      /// <summary>
      /// Get the face value of the card
      /// </summary>
      public int FaceValue
      {
         get
         {
            return _faceValue;
         }
      }

      /// <summary>
      /// Get the cards suit
      /// </summary>
      public SUIT Suit
      {
         get
         {
            return _suit;
         }
      }

      /// <summary>
      /// Mark the card as dealt
      /// </summary>
      public void Deal()
      {
         _dealt = true;
      }

      /// <summary>
      /// Mark the card as undealt
      /// </summary>
      public void Undeal()
      {
         _dealt = false;
      }

      /// <summary>
      /// Reset a card in the deck
      /// </summary>
      public virtual void Shuffle()
      {
         Dealt = false;
      }

      /// <summary>
      /// Get/set dealt property
      /// </summary>
      public bool Dealt
      {
         get
         {
            return _dealt;
         }
         set
         {
            _dealt = value;
         }
      }
      #endregion
	}


}
