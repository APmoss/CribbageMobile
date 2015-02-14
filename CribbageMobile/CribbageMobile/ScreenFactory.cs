using System;
using System.Collections.Generic;
using GameStateManagement;

namespace CribbageMobile {
	class ScreenFactory : IScreenFactory {
		public GameScreen CreateScreen(Type screenType) {
			return Activator.CreateInstance(screenType) as GameScreen;
		}
	}
}
