// Ultimate Cribbage
// Player Assembly

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
using CribCards;
using Cards;
using Layouts;
using System.Diagnostics;

namespace Player
{
	/// <summary>
	/// This class is an abstract base class generically representing a cribbage player
	/// </summary>
	abstract public class CribbagePlayer : BoxPlayer
	{
      #region Member Variables

      string _name = string.Empty; // the players name
      int _playerNumber = -1; // the number that identifies the player.

      #region Game Related Data
      int _score = 0; // the score for the player in this game
      bool _forfeit = false; // true if the player forfeited the game
      #endregion

      #region Round Related Data
      CribCardList _played = new CribCardList(); // cards the player has played to the table
      CribCardList _cribDiscards = new CribCardList(); // cards the player discarded to the crib
      bool _isMyBox = false; // true if the box belongs to this player this round
      bool _passed = false; // true if the player passed this round
      int _gapLeftThatICouldntPlayOn = 999; // smallest gap between the played total and 31 that the player could not play on
      bool _discardedToCrib = false; // true if player has discarded to the crib
      #endregion
      
      #region Statistic collectors
      int _playPointsThisGame = 0;
      int _roundPlayPoints = 0;
      int _roundPenalty = 0;
      int _roundHand = 0;
      int _roundCrib = 0;
      int _penaltyPointsThisGame = 0;
      int _handPointsThisGame = 0;
      int _handsScored = 0;
      int _handsStarted = 0;
      int _cribPointsThisGame = 0;
      int _cribsScored = 0;
      int _iKnownHandPoints = 0;
      #endregion
      #endregion

      #region Constructors
		public CribbagePlayer(int playerNumber)
		{
         _playerNumber = playerNumber;
		}
      #endregion

      #region Private Member Functions
      #endregion

      #region Public Member Functions
      public int CribTextX
      {
         get
         {
            return Layout.TheLayout.GetIntValue("PlayerCribTextX" + _playerNumber.ToString());
         }
      }
      public int CribTextY
      {
         get
         {
            return Layout.TheLayout.GetIntValue("PlayerCribTextY" + _playerNumber.ToString());
         }
      }
      public int CribX
      {
         get
         {
            return Layout.TheLayout.GetIntValue("PlayerCribX" + _playerNumber.ToString());
         }
      }
      public int CribY
      {
         get
         {
            return Layout.TheLayout.GetIntValue("PlayerCribY" + _playerNumber.ToString());
         }
      }
      public DisplayableCard.ORIENTATION CribOrientation
      {
         get
         {
            return (DisplayableCard.ORIENTATION)Enum.Parse(DisplayableCard.ORIENTATION.VERTICAL.GetType(), (string)Layout.TheLayout.GetValue("PlayerCribOrientation" + _playerNumber.ToString()), true);
         }
      }
      public override string ToString()
      {
         return _name;
      }
      #endregion

	}
}
