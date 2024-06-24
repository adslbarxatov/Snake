namespace RD_AAOW
	{
	/// <summary>
	/// Класс предоставляет доступ к настройкам приложения
	/// </summary>
	public static class SnakeSettings
		{
		/// <summary>
		/// Задаёт или возвращает текущий номер уровня игры
		/// </summary>
		public static int LevelNumber
			{
			get
				{
				return (int)RDGenerics.GetSettings (levelNumberPar, 10) - 10;
				}
			set
				{
				RDGenerics.SetSettings (levelNumberPar, (uint)(value + 10));
				}
			}
		private const string levelNumberPar = "LevelAl";

		/// <summary>
		/// Задаёт или возвращает текущие очки в игре
		/// </summary>
		public static uint GameScore
			{
			get
				{
				return RDGenerics.GetSettings (gameScorePar, 0);
				}
			set
				{
				RDGenerics.SetSettings (gameScorePar, value);
				}
			}
		private const string gameScorePar = "Score";

		/// <summary>
		/// Задаёт или возвращает флаг музыки в игре
		/// </summary>
		public static byte MusicEnabled
			{
			get
				{
				return (byte)RDGenerics.GetSettings (musicEnabledPar, 2);
				}
			set
				{
				uint v = value;
				if (v > 4)
					v = 0;
				RDGenerics.SetSettings (musicEnabledPar, v);
				}
			}
		private const string musicEnabledPar = "Music";

		/// <summary>
		/// Возвращает уровень громкости музыки в игре
		/// </summary>
		public static float MusicVolume
			{
			get
				{
				return MusicEnabled * 0.25f;
				}
			}

		/// <summary>
		/// Задаёт или возвращает флаг звуков в игре
		/// </summary>
		public static byte SoundsEnabled
			{
			get
				{
				return (byte)RDGenerics.GetSettings (soundsEnabledPar, 2);
				}
			set
				{
				uint v = value;
				if (v > 4)
					v = 0;
				RDGenerics.SetSettings (soundsEnabledPar, v);
				}
			}
		private const string soundsEnabledPar = "Sound";

		/// <summary>
		/// Возвращает уровень громкости звуков в игре
		/// </summary>
		public static float SoundVolume
			{
			get
				{
				return SoundsEnabled * 0.25f;
				}
			}

		/// <summary>
		/// Задаёт или возвращает текущее число съеденных яблок
		/// </summary>
		public static uint ApplesEaten
			{
			get
				{
				return RDGenerics.GetSettings (applesEatenPar, 0);
				}
			set
				{
				RDGenerics.SetSettings (applesEatenPar, value);
				}
			}
		private const string applesEatenPar = "EatenApples";
		}
	}
