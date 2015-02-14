using System;
using Microsoft.Xna.Framework;

namespace CribbageMobile {
	static class CM_Math {
		private static Random r = new Random();

		/// <summary>
		/// Return a random float between the two specified values
		/// </summary>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static float NextFloat(float minValue, float maxValue) {
			float randomFloat = 0;
			
			

			return randomFloat;
		}

		/// <summary>
		/// Similar to above method, but from 0 to maxValue
		/// </summary>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static float NextFloat(float maxValue) {
			return NextFloat(0, maxValue);
		}

		/// <summary>
		/// Returns an angle relative to the original modified by the magnitude
		/// </summary>
		/// <param name="originalRadians"></param>
		/// <param name="magnitude"></param>
		/// <returns></returns>
		public static float DisplaceAngle(float originalRadians, int magnitude) {
			//Return an angle that the magnitide has changed, in the positiove or negative direction
			return MathHelper.ToRadians(MathHelper.ToDegrees(originalRadians) + r.Next(magnitude) - magnitude / 2);
		}

		/// <summary>
		/// Returns a smooth increment towards a destination dependong on distance
		/// </summary>
		/// <param name="itemPosition"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static Vector2 SmoothTranslate(Vector2 itemPosition, Vector2 destination) {
			Vector2 displacement = destination - itemPosition;

			//If it is as close as one pixel, then just move there :P (this is to prevent flashing)
			if (displacement.Length() < 1) {
				return displacement;
			}

			//Get the absolute value so we can square root it
			Vector2 modified = new Vector2(Math.Abs(displacement.X), Math.Abs(displacement.Y));

			//Square root the absolute displacement
			modified = new Vector2((float)Math.Sqrt(modified.X), (float)Math.Sqrt(modified.Y));

			//Find the speed from the vector
			float speed = modified.Length();

			//Normalize the displacement just to get direction
			displacement.Normalize();

			//Return the displacement (direction) times the speed
			return displacement * speed;

			//return new Vector2((float)(Math.Abs(10 - menuItem.X))));
		}

		/// <summary>
		/// Helper for moving a value around in a circle.
		/// </summary>
		public static Vector2 MoveInCircle(GameTime gameTime, float speed) {
			double time = gameTime.TotalGameTime.TotalSeconds * speed;

			float x = (float)Math.Cos(time);
			float y = (float)Math.Sin(time);

			return new Vector2(x, y);
		}
	}
}