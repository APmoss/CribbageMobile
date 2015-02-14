using System;
using System.Diagnostics;
using System.Collections;

namespace CribCards
{
	/// <summary>
	/// This represents a scoring instance
	/// </summary>
   public class Scores
   {
      public enum SCORETYPE {FIFTEEN, PAIR, KNOB, RUN, FLUSH};
      public enum SCOREREASON {UNKNOWN, PLAY, CRIB, HAND, PENALTY};

      protected string _cards;
      protected int _score;
      protected SCORETYPE _scoreType;
      protected SCOREREASON _scoreReason;

      /// <summary>
      /// Create a scoring instance
      /// </summary>
      /// <param name="cards">Cards used for the score</param>
      /// <param name="score">The amount scored</param>
      /// <param name="scoreType">The type of score</param>
      public Scores(string cards, int score, SCORETYPE scoreType)
      {
         _cards = cards;
         _score = score;
         _scoreType = scoreType;
         _scoreReason = SCOREREASON.UNKNOWN;
      }

      /// <summary>
      /// Get/Set the score reason
      /// </summary>
      public SCOREREASON ScoreReason
      {
         get
         {
            return _scoreReason;
         }
         set
         {
            _scoreReason = value;
         }
      }

      /// <summary>
      /// Get the amount scored
      /// </summary>
      public int Score
      {
         get
         {
            return _score;
         }
      }

      /// <summary>
      /// The score as a string
      /// </summary>
      public string ScoreValue
      {
         get
         {
            return _score.ToString();
         }
      }

      /// <summary>
      /// The cards used in the score
      /// </summary>
      public string Cards
      {
         get
         {
            return _cards;
         }
      }

      /// <summary>
      /// The type of score
      /// </summary>
      public SCORETYPE ScoreType
      {
         get
         {
            return _scoreType;
         }
      }

      /// <summary>
      /// Decode the score reason
      /// </summary>
      /// <param name="e">Reason to decode</param>
      /// <returns></returns>
      public static string ScoreReasonDecode(SCOREREASON e)
      {
         switch(e)
         {
            case SCOREREASON.UNKNOWN:
               Debug.Fail("Score reason unknown");
               return "Unknown";
            case SCOREREASON.CRIB:
               return "from crib";
            case SCOREREASON.HAND:
               return "from hand";
            case SCOREREASON.PENALTY:
               return "penalty points";
            case SCOREREASON.PLAY:
               return "from play";
            default:
               Debug.Fail("Invalid score reason");
               return string.Empty;
         }
      }

      /// <summary>
      /// Decode the score type
      /// </summary>
      /// <param name="e">Type to decode</param>
      /// <returns></returns>
      public static string ScoreTypeDecode(SCORETYPE e)
      {
         switch(e)
         {
            case SCORETYPE.FIFTEEN:
               return "Fifteen";
               //break;
            case SCORETYPE.PAIR:
               return "Pair";
               //break;
            case SCORETYPE.KNOB:
               return "His Knob";
               //break;
            case SCORETYPE.RUN:
               return "Run";
               //break;
            case SCORETYPE.FLUSH:
               return "Flush";
               //break;
            default:
               Debug.Fail("Invalid score type");
               return string.Empty;
               //break;
         }
      }
   }

   /// <summary>
   /// Represents a collection of score lists
   /// </summary>
   public class ScoreList : ArrayList
   {
      /// <summary>
      /// Sets the reason code across all scores in the score list
      /// </summary>
      public Scores.SCOREREASON Reason
      {
         set
         {
            // go through each score and set the reason
            foreach (Scores s in this)
            {
               s.ScoreReason = value;
            }
         }
      }

      /// <summary>
      /// Gets the total of all scores in the list
      /// </summary>
      public int TotalScore
      {
         get
         {
            int total = 0;

            foreach (Scores s in this)
            {
               total += s.Score;
            }

            return total;
         }
      }

      /// <summary>
      /// Remove any flushes from the list of scores
      /// </summary>
      public void RemoveFlush()
      {
         // go through each item
         for (int i = 0; i < this.Count; i++)
         {
            // if the item is a flush item
            if (((Scores)this[i]).ScoreType == Scores.SCORETYPE.FLUSH)
            {
               // remove it
               this.RemoveAt(i);

               // go back one
               i--;
            }
         }
      }
   }
}
