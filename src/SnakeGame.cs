using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает игру Змейка
	/// </summary>
	public class SnakeGame: Game
		{
		/////////////////////////////////////////////////////////////////////////////////
		// ПЕРЕМЕННЫЕ

		// Драйвера игры

		// Графика
		private GraphicsDeviceManager graphics;

		// Sprite-отрисовка
		private SpriteBatch spriteBatch;

		// Состояние клавиатуры
		private KeyboardState keyboardState;

		// Шрифты
		private SpriteFont defFont, bigFont, midFont;

		// Размеры окна (игровое поле -  32 x 24 клеток; соотношение вынужденное, из-за fullscreen)

		/// <summary>
		/// Ширина окна
		/// </summary>
		public const int BackBufferWidth = Tile.Width * 32;

		/// <summary>
		/// Высота окна
		/// </summary>
		public const int BackBufferHeight = Tile.Height * 20;

		// Основное состояние игры (начало|игра|конец)
		private GameStatus gameStatus = GameStatus.Start;
		// Начальный статус игры (статусы перечислены в Auxilitary.cs)

		// Описатели уровня и окна сообщений

		// Класс-описатель уровня
		private SnakeLevel level;

		// Флаг паузы
		private bool isWorking = false;

		// Разные изображения на старте
		private Texture2D startBack, snakeImg;

		// Змейка на старте
		private Vector2 startSnakeVector;

		// Текущая позиция яблока и его объекты анимации

		// Текущая позиция
		private Vector2 applePosition;

		// Изображение анимации собираемых объектов
		private Animation[] appleAnimation;

		// Объект-анимация
		private AnimationPlayer appleAnimator;

		// Текущая позиция секций змейки, текущее направление движения и объекты анимации головы и тела

		// List позиций всех элементов змейки
		private List<Vector2> playerPosition = new List<Vector2> ();

		// Направление движения змейки
		private Vector2 playerTo;

		// Анимации объектов
		private Animation headAnimation, headRushAnimation, bodyAnimation;
		private AnimationPlayer headAnimator, bodyAnimator;

		// Фон сообщений
		private Animation messageBack;
		private AnimationPlayer messageBackAnimator;

		// Звуковые эффекты

		// Победа, поражение
		private SoundEffect SCompleted, SFailed;

		// Старт, пауза, звук off/on
		private SoundEffect SStart, SStop, SOnOff;

		// Разные звуки съедения
		private SoundEffect[] SAte;

		// Скорость змейки, количество яблок на уровне и параметр Alive
		private float speed = 0;
		private int applesQuantity = 0;
		private bool isAlive = true;

		// Камень, на котором произошло столкновение, и коэффициенты предельного расстояния нестолкновения
		private Vector2 collaptedOn;
		private const float StoneOffs = 0.25f;
		private const float BodyOffs = Tile.Width * 0.4f;
		private const float AppleOffs = Tile.Width * 0.6f;

		// Счётчики игры

		// Очки в розыгрыше
		private int currentScore = 0;

		// Множитель для очков
		private const int SMult = 10;

		// Состояния игры

		// Сообщение о начале уровня
		private bool showLevelMsg = false;

		// Сообщение о прохождении уровня
		private bool showLoseMsg = false;

		// Сообщение о проигрыше
		private bool showWinMsg = false;

		// Подтверждение выхода
		private bool showExitMsg = false;

		// Параметры компаса

		// Текстура
		private Texture2D compas;

		// Угол поворота компаса
		private float compasTurn = 0.0f;

		// Позиция и размер компаса
		private Rectangle compasPosition, compasSize;

		// Координаты предыдущего положения головы
		private Vector2 compasOffs;

		// Согласователи клавиатуры

		// Пауза в Update-итерациях перед следующим опросом клавиатуры
		private int kbdDelay = 1;

		// Таймер для delay
		private int kbdDelayTimer;

		// Базовый delay при нажатии клавиши
		private const int KbdDefDelay = 25;

		/// <summary>
		/// Конструктор. Инициализирует игру Змейка
		/// </summary>
		public SnakeGame ()
			{
			// Создание "окна" заданного размера и переход в полноэкранный режим
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
			//graphics.ToggleFullScreen ();

			// Задание content-директории игры
			Content.RootDirectory = "Content/Snake";
			}

		/// <summary>
		/// ИНИЦИАЛИЗАЦИЯ ОКНА
		/// Функция выполняется один раз за игру (при её запуске).
		/// Здесь располагаются все инициализации и начальные значения
		/// </summary>
		protected override void Initialize ()
			{
			// НАСТРОЙКА АППАРАТА ПРОРИСОВКИ
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// СОЗДАНИЕ ОБЪЕКТОВ АНИМАЦИИ
			// Разные виды яблок
			appleAnimation = new Animation[]    {
				new Animation (Content.Load<Texture2D> ("Tiles/Apple1"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple2"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple3"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple4"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple5"), Tile.Width, 0.06f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple6"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple7"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple8"), Tile.Width, 0.1f, true),
				new Animation (Content.Load<Texture2D> ("Tiles/Apple9"), Tile.Width, 0.1f, true)
												};

			// Голова при столкновении и движении
			headAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Part_Head"), Tile.Width, 0.1f, true);
			headRushAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Part_HeadRush"), Tile.Width, 0.1f, true);
			headAnimator.PlayAnimation (headAnimation);         // По умолчанию - анимация при движении

			// Тело змейки
			bodyAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Part_Part"), Tile.Width, 0.1f, false);
			bodyAnimator.PlayAnimation (bodyAnimation);

			// Фон сообщений
			messageBack = new Animation (Content.Load<Texture2D> ("Messages/MessageBack"), 512, 0.1f, true);
			messageBackAnimator.PlayAnimation (messageBack);

			// СОЗДАНИЕ ЗВУКОВЫХ ЭФФЕКТОВ
			SCompleted = Content.Load<SoundEffect> ("Sounds/Completed");
			SFailed = Content.Load<SoundEffect> ("Sounds/Failed");
			SOnOff = Content.Load<SoundEffect> ("Sounds/SoundOnOff");
			SStart = Content.Load<SoundEffect> ("Sounds/SStart");
			SStop = Content.Load<SoundEffect> ("Sounds/SStop");
			SAte = new SoundEffect[]    {
					Content.Load<SoundEffect> ("Sounds/Ate1"),
					Content.Load<SoundEffect> ("Sounds/Ate2"),
					Content.Load<SoundEffect> ("Sounds/Ate3"),
					Content.Load<SoundEffect> ("Sounds/Ate4")
										};

			// СОЗДАНИЕ ШРИФТОВ
			defFont = Content.Load<SpriteFont> ("Font/DefFont");
			bigFont = Content.Load<SpriteFont> ("Font/BigFont");
			midFont = Content.Load<SpriteFont> ("Font/MidFont");

			// ЗАГРУЗКА ДОПОЛНИТЕЛЬНЫХ ТЕКСТУР
			startBack = Content.Load<Texture2D> ("Background/StartBack");
			snakeImg = Content.Load<Texture2D> ("Background/SnakeImg");

			// КОМПАС
			compas = Content.Load<Texture2D> ("Tiles/Compas");
			compasPosition = compasSize = new Rectangle (0, 0, compas.Width, compas.Height);

			// НАСТРОЙКА МУЗЫКИ
			MediaPlayer.IsRepeating = true;
			if (SnakeSettings.MusicEnabled > 0)
				{
				MediaPlayer.Volume = SnakeSettings.MusicVolume;
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// Инициализация
			base.Initialize ();
			}

		/// <summary>
		/// Метод-обработчик динамических событий игры
		/// </summary>
		protected override void Update (GameTime gameTime)
			{
			// Опрос клавиатуры с предотвращением повторов
			kbdDelayTimer++;
			kbdDelayTimer %= kbdDelay;
			if (kbdDelayTimer == 0)
				{
				if (KeyboardProc ())
					{
					kbdDelay = KbdDefDelay;
					kbdDelayTimer = 0;
					}
				else
					{
					kbdDelay = 1;
					}
				}
			KeyboardMoveProc ();

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
				case GameStatus.Help:
				case GameStatus.Language:
					// Движение по синусоиде
					startSnakeVector.X += 2;
					startSnakeVector.Y = BackBufferHeight - snakeImg.Height / 2 - 90 +
						20 * (float)Math.Sin (0.007 * startSnakeVector.X + 0.5);

					if (startSnakeVector.X > BackBufferWidth + snakeImg.Width)
						startSnakeVector.X = -snakeImg.Width;

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// Модуль движения (в случае IsAlive)
					if (isAlive && isWorking)
						{
						playerPosition[0] += playerTo;

						for (int n = 1; n < playerPosition.Count; n++)
							{
							//////////////////////////////////////////////////////////////////////
							// ФОРМУЛА ПЛАВНОГО ДВИЖЕНИЯ ЗМЕЙКИ
							playerPosition[n] += speed * (playerPosition[n - 1] - playerPosition[n]) / Tile.Size;
							//////////////////////////////////////////////////////////////////////

							// Если часть тела пытается залезть на камень, она
							// отодвигается от него ровно в противоположном направлении
							if (IsCollapted (playerPosition[n], false))
								playerPosition[n] += new Vector2 (Math.Sign (playerPosition[n].X - collaptedOn.X),
									Math.Sign (playerPosition[n].Y - collaptedOn.Y));
							}
						}

					// Новый уровень с паузой (выигрыш)
					if (currentScore == SMult * applesQuantity)
						{
						// Звук
						MediaPlayer.Stop ();
						if (SnakeSettings.SoundsEnabled > 0)
							SCompleted.Play (SnakeSettings.SoundVolume, 0, 0);

						// Пересчёт очков
						SnakeSettings.GameScore += (uint)currentScore;
						currentScore = 0;

						// Отображение сообщения
						showWinMsg = true;

						// Запуск нового уровня с паузой
						isAlive = isWorking = false;

						// Перезапуск уровня произойдёт по нажатию клавиши
						}

					// Проверка столкновений
					if (IsCollapted (playerPosition[0], true) && isAlive)
						{
						// Звук
						MediaPlayer.Stop ();
						if (SnakeSettings.SoundsEnabled > 0)
							SFailed.Play (SnakeSettings.SoundVolume, 0, 0);

						// Переключение состояния игры
						isAlive = isWorking = false;
						SnakeSettings.LevelNumber--;
						headAnimator.PlayAnimation (headRushAnimation);

						// Пересчёт очков
						SnakeSettings.GameScore += (uint)(currentScore / SMult - applesQuantity);
						currentScore = 0;

						// Отображение сообщения
						showLoseMsg = true;

						// Перезапуск уровня произойдёт по нажатию клавиши Space
						}

					// Новое яблоко
					if (IsAte ())
						{
						// Звук
						if (SnakeSettings.SoundsEnabled > 0)
							SAte[RDGenerics.RND.Next (SAte.Length)].Play (SnakeSettings.SoundVolume, 0, 0);

						// Генерация нового яблока (если игра не окончена)
						NewApple ();

						// Наращивание текущего числа очков и всего съеденных ябок
						currentScore += SMult;
						SnakeSettings.ApplesEaten++;

						// Добавление части змейки
						playerPosition.Add (playerPosition[playerPosition.Count - 1]);
						}

					break;
					//////////////////////////////////////////////////////////////////
				}

			// Обновление игры
			base.Update (gameTime);
			}

		/// <summary>
		/// ОБРАБОТКА СОБЫТИЙ КЛАВИАТУРЫ
		/// Низкоскоростные события
		/// </summary>
		private bool KeyboardProc ()
			{
			// Запрос к клавиатуре
			keyboardState = Keyboard.GetState ();

			// В НЕЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			// Настройки звука
			if (!showExitMsg)
				{
				// Sound on/off
				if (keyboardState.IsKeyDown (Keys.S))
					{
					SnakeSettings.SoundsEnabled++;
					SOnOff.Play (SnakeSettings.SoundVolume, 0, 0);

					// Была нажата клавиша
					return true;
					}

				// Music on/off
				if (keyboardState.IsKeyDown (Keys.M))
					{
					SnakeSettings.MusicEnabled++;
					if (SnakeSettings.MusicEnabled == 0)
						{
						MediaPlayer.Stop ();
						}
					else
						{
						MediaPlayer.Volume = SnakeSettings.MusicVolume;
						MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));
						}
					SOnOff.Play (SnakeSettings.SoundVolume, 0, 0);

					return true;
					}
				}

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// Немедленный выход
					if (keyboardState.IsKeyDown (Keys.Escape))
						this.Exit ();

					// Справка
					if (keyboardState.IsKeyDown (Keys.F1))
						{
						gameStatus = GameStatus.Help;

						return true;
						}

					// Выбор языка интерфеса
					if (keyboardState.IsKeyDown (Keys.L))
						{
						gameStatus = GameStatus.Language;

						return true;
						}

					// Переход далее
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// Переключение параметров
						gameStatus = GameStatus.Playing;

						// Загрузка уровня
						SnakeSettings.LevelNumber--;
						LoadNextLevel ();

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
				case GameStatus.Language:
					// Возврат
					if (keyboardState.IsKeyDown (Keys.Escape))
						{
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:

					// Нажатие паузы и продолжения
					if (!showExitMsg)           // Нельзя ничего делать, если появилось сообщение о выходе
						{
						// Pause
						if (isAlive && keyboardState.IsKeyDown (Keys.Space))
							{
							if (isWorking)
								{
								isWorking = false;

								if (SnakeSettings.SoundsEnabled > 0)
									SStop.Play (SnakeSettings.SoundVolume, 0, 0);
								}

							// Continue
							else
								{
								showLevelMsg = false;
								isWorking = true;

								if (SnakeSettings.SoundsEnabled > 0)
									SStart.Play (SnakeSettings.SoundVolume, 0, 0);
								}

							return true;
							}

						// Нажатие клавиши продолжения
						if (keyboardState.IsKeyDown (Keys.Space) && !isWorking && !isAlive)
							{
							LoadNextLevel ();

							return true;
							}

						// Проверка на выход
						if (keyboardState.IsKeyDown (Keys.Escape))
							{
							// Пауза
							isWorking = false;

							// Сообщение
							showExitMsg = true;

							// Звук
							if (SnakeSettings.SoundsEnabled > 0)
								SOnOff.Play (SnakeSettings.SoundVolume, 0, 0);

							return true;
							}
						}

					// Попытка выхода
					if (showExitMsg)
						{
						// Выход из игры (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							this.Exit ();

						// Продолжение (back)
						if (keyboardState.IsKeyDown (Keys.N))
							{
							showExitMsg = false;

							return true;
							}
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Finish:
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// Переключение
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

					//////////////////////////////////////////////////////////////////
				}

			// Не было ни одного нажатия
			return false;
			}

		/// <summary>
		/// ОБРАБОТКА СОБЫТИЙ КЛАВИАТУРЫ
		/// Высокоскоростные события
		/// </summary>
		private void KeyboardMoveProc ()
			{
			// Запрос к клавиатуре
			keyboardState = Keyboard.GetState ();

			// Нажатие клавиш управления
			if ((gameStatus == GameStatus.Playing) && !showExitMsg && isWorking)
				{
				if (keyboardState.IsKeyDown (Keys.Up) && !(playerTo.X == 0))
				// Второе условие отвечает за запрет заднего хода на месте
				// и лишних нажатий клавиш, совпадающих по направлению с
				// движением змейки
					{
					playerTo.X = 0;
					playerTo.Y = -speed;
					}

				if (keyboardState.IsKeyDown (Keys.Down) && !(playerTo.X == 0))
					{
					playerTo.X = 0;
					playerTo.Y = speed;
					}

				if (keyboardState.IsKeyDown (Keys.Left) && !(playerTo.Y == 0))
					{
					playerTo.X = -speed;
					playerTo.Y = 0;
					}

				if (keyboardState.IsKeyDown (Keys.Right) && !(playerTo.Y == 0))
					{
					playerTo.X = speed;
					playerTo.Y = 0;
					}
				}
			}

		/////////////////////////////////////////////////////////////////////////////////
		// ПРОРИСОВКА ИГРОВОГО ПОЛЯ

		/// <summary>
		/// Метод отрисовывает информационное поле игры (очки, уровень, состояние)
		/// </summary>
		private void DrawInfo ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stScoreLines[0]))
				{
				string[] values = RDLocale.GetText ("ScoreLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stScoreLines.Length; i++)
					stScoreLines[i] = values[i];
				}

			string S00 = string.Format (stScoreLines[0], currentScore);
			string S01 = string.Format (stScoreLines[1], SnakeSettings.GameScore);
			string S02 = string.Format (stScoreLines[2], SnakeSettings.ApplesEaten);
			string S03 = string.Format (stScoreLines[3], applesQuantity - currentScore / SMult);
			string S04 = (isWorking ? string.Format (stScoreLines[4], SnakeSettings.LevelNumber + 1) : stScoreLines[5]);

			float StrUp = Tile.Height * 0.15f,
				  StrDown = BackBufferHeight - Tile.Height * 0.8f;

			// Векторы позиций для отображения элементов учитывают смещение камеры наблюдения
			Vector2 V1 = new Vector2 (BackBufferWidth * 0.05f, StrUp) + level.CameraPosition,
					V2 = new Vector2 (BackBufferWidth * 0.21f, StrUp) + level.CameraPosition,
					V3 = new Vector2 (BackBufferWidth * 0.50f, StrUp) + level.CameraPosition,
					V4 = new Vector2 (BackBufferWidth * 0.80f, StrUp) + level.CameraPosition,
					V5 = new Vector2 (BackBufferWidth * 0.05f, StrDown) + level.CameraPosition,
					V6 = new Vector2 (BackBufferWidth * 0.89f, StrDown) + level.CameraPosition,
					V7 = new Vector2 (BackBufferWidth * 0.92f, StrDown) + level.CameraPosition;

			DrawShadowedString (defFont, S04, V1, SnakeGameColors.LRed);
			DrawShadowedString (defFont, S00, V2, SnakeGameColors.Yellow);
			DrawShadowedString (defFont, S01, V3, SnakeGameColors.Green);
			DrawShadowedString (defFont, S02, V4, SnakeGameColors.LBlue);

			// Если игра идёт, выводить строку "осталось съесть"
			if (isAlive)
				DrawShadowedString (defFont, S03, V5, SnakeGameColors.Silver);

			// Если есть музыка или звук, выводить соответствующий знак
			if (SnakeSettings.MusicEnabled > 0)
				DrawShadowedString (defFont, "[\x266B]", V6, SnakeGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266B]", V6, SnakeGameColors.Black);

			if (SnakeSettings.SoundsEnabled > 0)
				DrawShadowedString (defFont, "[\x266A]", V7, SnakeGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266A]", V7, SnakeGameColors.Black);

			// КОМПАС
			// Смена цвета стрелки компаса
			Color compasColor = SnakeGameColors.CompasRed;
			if (GameAuxFunctions.VDist (playerPosition[0], applePosition) <
				GameAuxFunctions.VDist (compasOffs, applePosition))
				compasColor = SnakeGameColors.CompasGreen;

			// Положение стрелки компаса
			compasOffs = playerPosition[0];
			compasPosition.X = (int)playerPosition[0].X;
			compasPosition.Y = (int)playerPosition[0].Y;
			Vector2 V8 = applePosition - playerPosition[0];

			// Формула движения стрелки компаса и её отображение
			compasTurn = (float)Math.Acos (V8.X / GameAuxFunctions.VDist (V8, Vector2.Zero)) *
				GameAuxFunctions.NNSign (V8.Y, true);
			spriteBatch.Draw (compas, compasPosition, compasSize, compasColor, compasTurn,
				new Vector2 (compas.Width, compas.Height) / 2, SpriteEffects.None, 0.0f);
			}
		private string[] stScoreLines = new string[6];
		private char[] splitter = new char[] { '\t' };

		/// <summary>
		/// Метод отображает сообщения об уровне
		/// </summary>
		private void ShowLevelMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stLevelLines[0]))
				{
				string[] values = RDLocale.GetText ("LevelLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLevelLines.Length; i++)
					stLevelLines[i] = values[i];
				}
			string S00 = string.Format (stLevelLines[0], SnakeSettings.LevelNumber + 1);
			string S01 = string.Format (stLevelLines[1], applesQuantity);

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S00).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S01).X) / 2,
						(BackBufferHeight - 50) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stLevelLines[2]).X) / 2,
						(BackBufferHeight + 150) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, S00, V1, SnakeGameColors.LBlue);
			spriteBatch.DrawString (midFont, S01, V2, SnakeGameColors.Orange);
			spriteBatch.DrawString (defFont, stLevelLines[2], V3, SnakeGameColors.DBlue);
			}
		private string[] stLevelLines = new string[3];

		/// <summary>
		/// Метод отображает сообщения о победе
		/// </summary>
		private void ShowWinMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stSuccessLines[0]))
				{
				string[] values = RDLocale.GetText ("SuccessLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stSuccessLines.Length; i++)
					stSuccessLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stSuccessLines[0]).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stSuccessLines[1]).X) / 2,
						(BackBufferHeight + 150) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, stSuccessLines[0], V1, SnakeGameColors.Green);
			spriteBatch.DrawString (defFont, stSuccessLines[1], V2, SnakeGameColors.DBlue);
			}
		private string[] stSuccessLines = new string[2];

		/// <summary>
		/// Метод отображает сообщения о проигрыше
		/// </summary>
		private void ShowLoseMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stLoseLines[0]))
				{
				string[] values = RDLocale.GetText ("LoseLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLoseLines.Length; i++)
					stLoseLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stLoseLines[0]).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stLoseLines[1]).X) / 2,
						(BackBufferHeight - 150) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stLoseLines[2]).X) / 2,
						(BackBufferHeight + 150) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, stLoseLines[0], V1, SnakeGameColors.Red);
			spriteBatch.DrawString (bigFont, stLoseLines[1], V2, SnakeGameColors.Red);
			spriteBatch.DrawString (defFont, stLoseLines[2], V3, SnakeGameColors.DBlue);
			}
		private string[] stLoseLines = new string[3];

		/// <summary>
		/// Метод отображает сообщения о начале игры
		/// </summary>
		private void ShowStartMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stStartLines[0]))
				{
				string[] values = RDLocale.GetText ("StartLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stStartLines.Length - 1; i++)
					stStartLines[i] = values[i];
				stStartLines[3] = ProgramDescription.AssemblyTitle;
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stStartLines[3]).X) / 2,
						(BackBufferHeight - 300) / 2),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[0]).X) / 2,
						BackBufferHeight - 180),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[1]).X) / 2,
						BackBufferHeight - 150),
					V6 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[2]).X) / 2,
						BackBufferHeight - 120);

			spriteBatch.Draw (startBack, Vector2.Zero, SnakeGameColors.White);
			spriteBatch.Draw (snakeImg, startSnakeVector, SnakeGameColors.White);
			spriteBatch.DrawString (bigFont, stStartLines[3], V1, SnakeGameColors.Gold);
			spriteBatch.DrawString (defFont, stStartLines[0], V4, SnakeGameColors.DBlue);
			spriteBatch.DrawString (defFont, stStartLines[1], V5, SnakeGameColors.DBlue);
			spriteBatch.DrawString (defFont, stStartLines[2], V6, SnakeGameColors.DBlue);
			}
		private string[] stStartLines = new string[4];

		/// <summary>
		/// Метод отображает сообщения об окончании игры
		/// </summary>
		private void ShowFinishMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stFinishLines[0]))
				{
				string[] values = RDLocale.GetText ("FinishLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stFinishLines.Length; i++)
					stFinishLines[i] = values[i];
				}
			string S02 = string.Format (stFinishLines[2], SnakeSettings.GameScore);
			string S03 = string.Format (stFinishLines[3], SnakeSettings.ApplesEaten);

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stFinishLines[0]).X) / 2,
						(BackBufferHeight - 250) / 2),
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stFinishLines[1]).X) / 2,
						(BackBufferHeight - 100) / 2),
					V3 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S02).X) / 2,
						(BackBufferHeight) / 2),
					V4 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S03).X) / 2,
						(BackBufferHeight + 60) / 2),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stFinishLines[4]).X) / 2,
						(BackBufferHeight + 200) / 2);

			spriteBatch.Draw (startBack, Vector2.Zero, SnakeGameColors.White);
			spriteBatch.DrawString (bigFont, stFinishLines[0], V1, SnakeGameColors.Gold);
			spriteBatch.DrawString (midFont, stFinishLines[1], V2, SnakeGameColors.Orange);
			spriteBatch.DrawString (midFont, S02, V3, SnakeGameColors.Orange);
			spriteBatch.DrawString (midFont, S03, V4, SnakeGameColors.Orange);
			spriteBatch.DrawString (defFont, stFinishLines[4], V5, SnakeGameColors.DBlue);
			}
		private string[] stFinishLines = new string[5];

		/// <summary>
		/// Метод отображает запрос на подтверждение выхода из игры
		/// </summary>
		private void ShowExitMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stExitLines[0]))
				{
				string[] values = RDLocale.GetText ("ExitLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stExitLines.Length; i++)
					stExitLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stExitLines[0]).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stExitLines[1]).X) / 2,
						(BackBufferHeight - 150) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stExitLines[2]).X) / 2,
						(BackBufferHeight + 100) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stExitLines[3]).X) / 2,
						(BackBufferHeight + 140) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, stExitLines[0], V1, SnakeGameColors.Yellow);
			spriteBatch.DrawString (bigFont, stExitLines[1], V2, SnakeGameColors.Yellow);
			spriteBatch.DrawString (defFont, stExitLines[2], V3, SnakeGameColors.DBlue);
			spriteBatch.DrawString (defFont, stExitLines[3], V4, SnakeGameColors.DBlue);
			}
		private string[] stExitLines = new string[4];

		/// <summary>
		/// Отображение вспомогательных интерфейсов
		/// </summary>
		private void ShowServiceMessage (bool Language)
			{
			// Защита от множественного входа
			if (showingServiceMessage)
				return;
			showingServiceMessage = true;

			// Блокировка отрисовки и запуск справки
			spriteBatch.End ();

			if (Language)
				RDGenerics.MessageBox ();
			else
				RDGenerics.ShowAbout (false);

			// Возврат в исходное состояние
			spriteBatch.Begin ();
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;

			// Выход в меню
			gameStatus = GameStatus.Start;
			showingServiceMessage = false;
			}
		private bool showingServiceMessage = false;

		/// <summary>
		/// Метод отрисовывает уровень игры
		/// </summary>
		/// <param name="VGameTime">Время игры</param>
		protected override void Draw (GameTime VGameTime)
			{
			// Создание чистого окна и запуск рисования
			graphics.GraphicsDevice.Clear (SnakeGameColors.DGreen);
			spriteBatch.Begin ();

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					ShowStartMessage ();

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					ShowServiceMessage (false);
					break;

				case GameStatus.Language:
					ShowServiceMessage (true);
					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// ОТОБРАЖЕНИЕ УРОВНЯ 
					level.Draw (VGameTime, spriteBatch, playerPosition[0]);

					// ОТОБРАЖЕНИЕ АНИМИРОВАННЫХ ИХОБРАЖЕНИЙ
					// Яблоко
					appleAnimator.Draw (VGameTime, spriteBatch, applePosition, SpriteEffects.None,
						SnakeGameColors.White, 0.0);

					// Тело змейки
					for (int n = 1; n < playerPosition.Count; n++)
						bodyAnimator.Draw (VGameTime, spriteBatch, playerPosition[n], SpriteEffects.None,
							SnakeGameColors.White, 0.0);

					// Голова змейки
					headAnimator.Draw (VGameTime, spriteBatch, playerPosition[0], SpriteEffects.None,
						SnakeGameColors.White,
						// Изменение угла поворота текстуры
						Math.Acos (Math.Sign (playerTo.X)) * GameAuxFunctions.NNSign (playerTo.Y, true));


					// ОТОБРАЖЕНИЕ ИНФОРМАЦИИ
					DrawInfo ();

					// Отображение сообщений
					Vector2 backBufferSize = new Vector2 (SnakeGame.BackBufferWidth, SnakeGame.BackBufferHeight);
					if (showLevelMsg)
						{
						messageBackAnimator.Draw (VGameTime, spriteBatch, GameAuxFunctions.CenterOf (backBufferSize,
							level.CameraPosition), SpriteEffects.None, SnakeGameColors.LBlue_B, 0.0);
						ShowLevelMessage ();
						}

					if (showWinMsg)
						{
						messageBackAnimator.Draw (VGameTime, spriteBatch, GameAuxFunctions.CenterOf (backBufferSize,
							level.CameraPosition), SpriteEffects.None, SnakeGameColors.Green_B, 0.0);
						ShowWinMessage ();
						}

					if (showLoseMsg)
						{
						messageBackAnimator.Draw (VGameTime, spriteBatch, GameAuxFunctions.CenterOf (backBufferSize,
							level.CameraPosition), SpriteEffects.None, SnakeGameColors.Red_B, 0.0);
						ShowLoseMessage ();
						}

					if (showExitMsg)
						{
						messageBackAnimator.Draw (VGameTime, spriteBatch, GameAuxFunctions.CenterOf (backBufferSize,
							level.CameraPosition), SpriteEffects.None, SnakeGameColors.Yellow_B, 0.0);
						ShowExitMessage ();
						}

					break;

				//////////////////////////////////////////////////////////////////

				case GameStatus.Finish:
					ShowFinishMessage ();

					break;
				}

			// Отключение устройства рисования
			spriteBatch.End ();

			// Перерисовка
			base.Draw (VGameTime);
			}

		/// <summary>
		/// Загрузка следующего уровня игры
		/// </summary>
		private void LoadNextLevel ()
			{
			// Запуск фоновой мелодии
			MediaPlayer.Stop ();
			if (SnakeSettings.MusicEnabled > 0)
				{
				MediaPlayer.Volume = SnakeSettings.MusicVolume;
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));
				}

			// Поиск следующей структуры уровня
			while (true)
				{
				// Поиск С АВТОСМЕЩЕНИЕМ НА СЛЕДУЮЩИЙ УРОВЕНЬ
				++SnakeSettings.LevelNumber;
				if (SnakeSettings.LevelNumber < LevelData.LevelsQuantity)
					break;

				// Перезапуск с нулевого уровня в конце игры
				SnakeSettings.LevelNumber = -1;
				gameStatus = GameStatus.Finish;
				if (SnakeSettings.MusicEnabled > 0)
					{
					MediaPlayer.Volume = SnakeSettings.MusicVolume;
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
					}
				}

			// Выгрузка предыдущего уровня и загрузка нового
			if (level != null)
				level.Dispose ();
			level = new SnakeLevel (Services, (int)SnakeSettings.LevelNumber);

			// СТАРТОВОЕ СОСТОЯНИЕ ИГРЫ
			// Чтение параметров уровня
			applesQuantity = level.ApplesQuantity;
			speed = level.Speed;
			playerTo = level.PlayerTo * speed;

			// Генерация нового яблока
			NewApple ();

			// Установка параметров
			headAnimator.PlayAnimation (headAnimation);
			isAlive = true;

			// Смена сообщения
			showWinMsg = showLoseMsg = false;
			showLevelMsg = true;

			// ПЕРЕЗАГРУЗКА ТЕЛА ЗМЕЙКИ
			// Очистка
			playerPosition.Clear ();

			// Голова
			Vector2 pp2 = Tile.Size / 2 + Tile.Size * level.PlayerStartPosition;
			playerPosition.Add (pp2);

			// Вторая часть тела (располагается за головой)
			pp2.X -= Tile.Width * Math.Sign (playerTo.X);
			pp2.Y -= Tile.Height * Math.Sign (playerTo.Y);
			playerPosition.Add (pp2);
			}

		/////////////////////////////////////////////////////////////////////////////////
		// ПРОВЕРКА СТОЛКНОВЕНИЙ

		/// <summary>
		/// Метод проверяет столкновение данной точки с любой точкой стенок или тела змейки
		/// </summary>
		/// <param name="Collaptor">Проверяемая точка</param>
		/// <param name="WithBody">Проверка столкновения с телом</param>
		private bool IsCollapted (Vector2 Collaptor, bool WithBody)
			{
			// Чтобы не тестировать с каждым обновлением все камни уровня,
			// имеет смысл проверить только те клетки игры, которые находятся
			// на минимальном расстоянии от головы. Таких должно быть всего
			// четыре: на StoneOffs вверх, влево, вправо и вниз. По координатам
			// таких смещений легко посчитать масштабный индекс массива для
			// ближайшей позиции и проверить её collision
			Vector2[] V1 = new Vector2[]    {
					new Vector2(Collaptor.X / Tile.Width + StoneOffs, Collaptor.Y / Tile.Height + StoneOffs),
					new Vector2(Collaptor.X / Tile.Width + StoneOffs, Collaptor.Y / Tile.Height - StoneOffs),
					new Vector2(Collaptor.X / Tile.Width - StoneOffs, Collaptor.Y / Tile.Height + StoneOffs),
					new Vector2(Collaptor.X / Tile.Width - StoneOffs, Collaptor.Y / Tile.Height - StoneOffs)
											};

			// Проверка на столкновение со стенами
			for (int i = 0; i < V1.Length; i++)
				// Вынужденное ограничение индекса (исключение может возникнуть
				// при генерации нового яблока)
				if (level.Tiles[(int)V1[i].X % (int)level.LevelSize.X,
								(int)V1[i].Y % (int)level.LevelSize.Y].Collision == TileCollision.Stone)
					{
					collaptedOn.X = (int)V1[i].X * Tile.Width + Tile.Width / 2;
					collaptedOn.Y = (int)V1[i].Y * Tile.Height + Tile.Width / 2;
					return true;
					}

			// Проверка на столкновение с собой (если требуется)
			if (WithBody)
				for (int i = 1; i < playerPosition.Count; i++)
					if (GameAuxFunctions.VDist (Collaptor, playerPosition[i]) < BodyOffs)
						return true;

			// Не было столкновений
			return false;
			}

		/// <summary>
		/// Метод проверяет столкновение с яблоком
		/// </summary>
		/// <returns></returns>
		private bool IsAte ()
			{
			if (GameAuxFunctions.VDist (applePosition, playerPosition[0]) < AppleOffs)
				return true;

			return false;
			}

		/// <summary>
		/// Метод генерирует новое яблоко
		/// </summary>
		private void NewApple ()
			{
			Vector2 NewV;

			// Генерация новой позиции, не попадающей на камни и игрока
			do
				{
				NewV.X = RDGenerics.RND.Next ((int)(level.LevelSize.X * Tile.Width));
				NewV.Y = RDGenerics.RND.Next ((int)(level.LevelSize.Y * Tile.Height));
				} while (IsCollapted (NewV, true));

			applePosition = NewV;

			// Выбор новой анимации
			appleAnimator.PlayAnimation (appleAnimation[RDGenerics.RND.Next (appleAnimation.Length)]);
			}

		/// <summary>
		/// Метод отрисовывает текстовую строку
		/// </summary>
		private void DrawShadowedString (SpriteFont VFont, string VString, Vector2 VPosition, Color VColor)
			{
			// Строка табуляций берётся потому, что в шрифте этот символ был
			// исключён за ненадобностью. В результате он при отображении
			// заменяется на default, который в том же шрифте выставлен как
			// символ-прямоугольник
			string SubStr = "█".PadRight (30, '█');

			spriteBatch.DrawString (VFont, SubStr.Remove (VString.Length), VPosition, SnakeGameColors.DGreen);
			spriteBatch.DrawString (VFont, VString, VPosition, VColor);
			}
		}
	}
