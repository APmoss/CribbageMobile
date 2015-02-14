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
using System.Collections;
using System.Windows.Forms;

namespace Cards
{
   /// <summary>
   /// Describes a deck of cards.
   /// </summary>
   public class Deck
   {
      #region Static Member Variables
      static Random __random = new Random();
      #endregion

      #region Member Variables
      CardBack _cardBack = new CardBack(); // card back to use for cards when face down
      ArrayList _cards = new ArrayList(); // the cards
      int _cardsLeft; // number of cards left in the deck
      int _cardCount; // number of cards in the deck
      bool _manualDeal = false; // flag indicating if the user should deal their own cards ... useful for testing
      #endregion

      #region Delegates
      public delegate int GetRandom(int iMax);
      public GetRandom OnRandom = null;
      #endregion

      #region Constructors
      /// <summary>
      /// Implements common constructor code
      /// </summary>
      /// <param name="cardSpecialValueFunction">Function Deck should call to work out a cards special value in your game</param>
      /// <param name="manualDeal">Flag to indicate if the deck class should prompt the user to select the card to deal. Very useful for debugging.</param>
      /// <param name="includeJoker">Flag to indicate if the deck should include a joker</param>
      void CommonConstructor(Card.CardSpecialValue cardSpecialValueFunction, bool manualDeal, bool includeJoker)
      {
         // set the number of cards in the deck
         if (includeJoker)
         {
            _cardCount = 53;
         }
         else
         {
            _cardCount = 52;
         }

         // all cards are left
         _cardsLeft = _cardCount;

         // save the manual deal flag
         _manualDeal = manualDeal;

         // Tell the card class the OnCardSpecialValue Function
         Card.OnCardSpecialValue = cardSpecialValueFunction;

         // create the cards
         for (int i = 1; i <= _cardCount; i++)
         {
            _cards.Add(new DisplayableCard(this, i));
         }

         // now shuffle them all
         Shuffle();
      }

      /// <summary>
      /// Create a deck of cards
      /// </summary>
      /// <param name="cardSpecialValueFunction">Function Deck should call to work out a cards special value in your game</param>
      /// <param name="manualDeal">Flag to indicate if the deck class should prompt the user to select the card to deal. Very useful for debugging.</param>
      /// <param name="includeJoker">Flag to indicate if the deck should include a joker</param>
      public Deck(Card.CardSpecialValue cardSpecialValueFunction, bool manualDeal, bool includeJoker)
      {
         CommonConstructor(cardSpecialValueFunction, manualDeal, includeJoker);
      }

      /// <summary>
      /// Create a default deck
      /// </summary>
      public Deck()
      {
         CommonConstructor(null, false, false);
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// This gets the list of cards that have already been dealt. In some circles this is seen as cheating but it can be useful when writing computer player logic
      /// </summary>
      public CardList DealtCards
      {
         get
         {
            // create a card list
            CardList dealtCards = new CardList();

            // look at each card
            foreach (Card card in _cards)
            {
               // if dealt add it to our list
               if (card.Dealt)
               {
                  dealtCards.Add(card);
               }
            }

            // return the list
            return dealtCards;
         }
      }

      /// <summary>
      /// Shuffle the deck ready for dealing
      /// </summary>
      public void Shuffle()
      {
         // For each card in the deck shuffle it
         foreach(Card card in _cards)
         {
            card.Shuffle();
         }

         // reset the cards left count
         _cardsLeft = _cardCount;
      }

      /// <summary>
      /// Get a card at the specified index
      /// </summary>
      /// <param name="index">Index of the card to get</param>
      /// <returns>Card at the given index</returns>
      public Card this[int index]
      {
         get
         {
            return (Card)_cards[index];
         }
      }

      /// <summary>
      /// Get the number of cards in the deck
      /// </summary>
      public int Count
      {
         get
         {
            return _cardCount;
         }
      }

      /// <summary>
      /// Deal a card randomly
      /// </summary>
      /// <returns>A card or null if none left</returns>
      public Card DealOne()
      {
         return DealOne(string.Empty);
      }

      /// <summary>
      /// Deal a card randomly
      /// </summary>
      /// <param name="reason">Reason card is to be dealt</param>
      /// <returns>A Card or null if none left</returns>
      public Card DealOne(string reason)
      {
         Card card = null; // dealt card

         // if we have a card
         if (_cardsLeft > 0)
         {
            // if we are to manually deal
            if (_manualDeal)
            {
               // display the deal dialog box
               ManualDeal manualDeal = new ManualDeal(this, reason);
               manualDeal.ShowDialog(Form.ActiveForm);

               // get the dealt card
               card = manualDeal.SelectedCard;

               // mark it as dealt
               card.Deal();

               // One less card
               _cardsLeft--;
            }
            else
            {
               int cardNumber; // number of the card to deal

               // if the user has specified their own random number provider
               if (this.OnRandom != null)
               {
                  // geta number between 0 and cards left -1
                  cardNumber = OnRandom(_cardsLeft -1) + 1;
               }
               else
               {
                  // geta number between 0 and cards left -1
                  cardNumber = __random.Next(_cardsLeft -1) + 1;
               }

               // go through the deck looking for the <cardNumber>'th undealt card
               int undealt = 0;
               for (int i = 1 ; i <= _cardCount; i++)
               {
                  // if card is not dealt
                  if (!((Card)_cards[i]).Dealt)
                  {
                     // increment the number of undealt cards founf
                     undealt++;

                     // if it is equal to our random number
                     if (undealt == cardNumber)
                     {
                        // deal it
                        card = (Card)_cards[i];
                        card.Deal();
                        _cardsLeft--;
                        break;
                     }
                  }
               }
            }
         }

         // return the card
         return card;
      }

      /// <summary>
      /// Deal a specific card
      /// </summary>
      /// <param name="faceValue">Face value of card to deal</param>
      /// <param name="suit">Suit of card to deal</param>
      /// <returns>The card</returns>
      public Card DealCard(int faceValue, Card.SUIT suit)
      {
         Card card = null;

         // go through the deck looking for the nominated card
         for (int i = 0; i < _cards.Count; i++)
         {
            // if we have found it
            if (((Card)_cards[i]).FaceValue == faceValue && ((Card)_cards[i]).Suit == suit)
            {
               // greb it
               card = (Card)_cards[i];

               // if not dealt
               if (!card.Dealt)
               {
                  // deal it
                  card.Deal();
                  _cardsLeft--;
               }
            }
         }

         // return the dealt card
         return card;
      }

      /// <summary>
      /// Get the number of cards left in the deck
      /// </summary>
      public int CardsLeft
      {
         get
         {
            return _cardsLeft;
         }
      }

      /// <summary>
      /// Get the card back
      /// </summary>
      public CardBack CardBack
      {
         get
         {
            return _cardBack;
         }
      }


      #endregion

   }
}
