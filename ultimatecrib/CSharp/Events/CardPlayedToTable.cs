using System;
using Cards;
using Player;

namespace Events
{
	/// <summary>
	/// This event is created and fired whenever a player plays a card.
	/// </summary>
	public class CardPlayedToTable : BaseEvent
	{
      CribbagePlayer _player = null;
      Card _card = null;

		public CardPlayedToTable(CribbagePlayer player, Card card)
		{
         _player = player;
         _card = card;
		}

      public CribbagePlayer Player
      {
         get
         {
            return _player;
         }
      }

      public Card Card
      {
         get
         {
            return _card;
         }
      }
	}
}
