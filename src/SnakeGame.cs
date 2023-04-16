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
		private GraphicsDeviceManager graphics;             // Графика
		private SpriteBatch spriteBatch;                    // Sprite-отрисовка
		private KeyboardState keyboardState;                // Состояние клавиатуры
		private SpriteFont defFont, bigFont, midFont;       // Шрифты
		private Random rnd = new Random ();                 // ГСЧ

		// Размеры окна (игровое поле -  32 x 24 клеток; соотношение вынужденное, из-за fullscreen)

		/// <summary>
		/// Ширина окна
		/// </summary>
		public const int BackBufferWidth = Tile.Width * 32;

		/// <summary>
		/// Высота окна
		/// </summary>
		public const int BackBufferHeight = Tile.Height * 24;

		// Основное состояние игры (начало|игра|конец)
		private GameStatus gameStatus = GameStatus.Start;
		// Начальный статус игры (статусы перечислены в Auxilitary.cs)

		// Описатели уровня и окна сообщений
		private SnakeLevel level;                           // Класс-описатель уровня
		private int levelNumber = 0;                        // Номер текущего уровня
		private bool isWorking = false;                     // Флаг паузы
		private Texture2D startBack, snakeImg;              // Разные изображения на старте
		private Vector2 startSnakeVector;                   // Змейка на старте

		// Текущая позиция яблока и его объекты анимации
		private Vector2 applePosition;                      // Текущая позиция
		private Animation[] appleAnimation;                 // Изображение анимации
		private AnimationPlayer appleAnimator;              // Объект-анимация

		// Текущая позиция секций змейки, текущее направление движения
		// и объекты анимации головы и тела
		private List<Vector2> playerPosition = new List<Vector2> ();        // List позиций всех элементов змейки
		private Vector2 playerTo;                                           // Направление движения змейки
		private Animation headAnimation, headRushAnimation, bodyAnimation;
		private AnimationPlayer headAnimator, bodyAnimator;

		// Фон сообщений
		private Animation messageBack;
		private AnimationPlayer messageBackAnimator;

		// Звуковые эффекты и их параметры
		private SoundEffect SCompleted, SFailed,            // Победа, поражение
							SStart, SStop, SOnOff;          // Старт, пауза, звук off/on
		private SoundEffect[] SAte;                         // Разные звуки съедения
		private bool isSound = true, isMusic = true;        // Звук и музыка в игре on/off

		// Скорость змейки, количество яблок на уровне и параметр Alive
		private float speed = 0;
		private int applesQuantity = 0;
		private bool isAlive = true;

		// Камень, на котором произошло столкновение, и коэффициенты предельного расстояния нестолкновения
		private Vector2 collaptedOn;
		private const float StoneOffs = 0.25f,
							BodyOffs = Tile.Width * 0.4f,
							AppleOffs = Tile.Width * 0.6f;

		// Очки
		private int score = 0,                              // Выигрыш
					currentScore = 0,                       // Очки в розыгрыше
					eatenApples = 0;                        // Съедено яблок за всю игру
		private const int SMult = 10;                       // Множитель для очков

		// Флаги отображения сообщений
		private bool showLevelMsg = false,                  // Сообщение о начале уровня
					 showLoseMsg = false,                   // Сообщение о прохождении уровня
					 showWinMsg = false,                    // Сообщение о проигрыше
					 showExitMsg = false;                   // Подтверждение выхода

		// Параметры компаса
		private Texture2D compas;                       // Текстура
		private float compasTurn = 0.0f;                // Угол поворота компаса
		private Rectangle compasPosition, compasSize;   // Позиция и размер компаса
		private Vector2 compasOffs;                     // Координаты предыдущего положения головы

		// Согласователи клавиатуры
		private int kbdDelay = 1,           // Пауза в Update-итерациях перед следующим опросом клавиатуры
					kbdDelayTimer;          // Таймер для delay
		private const int KbdDefDelay = 25; // Базовый delay при нажатии клавиши

		/// <summary>
		/// Конструктор. Инициализирует игру Змейка
		/// </summary>
		public SnakeGame ()
			{
			// Создание "окна" заданного размера и переход в полноэкранный режим
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
			graphics.ToggleFullScreen ();

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

			// ЧТЕНИЕ НАСТРОЕК И РЕЗУЛЬТАТОВ ИГРЫ
			GameSettings (false);

			// НАСТРОЙКА МУЗЫКИ
			MediaPlayer.IsRepeating = true;
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));

			// Инициализация
			base.Initialize ();
			}

		/// <summary>
		/// Метод-обработчик динамических событий игры
		/// </summary>
		/// <param name="gameTime"></param>
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
					startSnakeVector.Y = BackBufferHeight - snakeImg.Height / 2 - 190 +
						12 * (float)Math.Sin (0.007 * startSnakeVector.X + 0.5);

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
						if (isSound)
							SCompleted.Play ();

						// Пересчёт очков
						score += currentScore;
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
						if (isSound)
							SFailed.Play ();

						// Переключение состояния игры
						isAlive = isWorking = false;
						levelNumber--;
						headAnimator.PlayAnimation (headRushAnimation);

						// Пересчёт очков
						score += currentScore / SMult - applesQuantity;
						currentScore = 0;

						// Отображение сообщения
						showLoseMsg = true;

						// Перезапуск уровня произойдёт по нажатию клавиши Space
						}

					// Новое яблоко
					if (IsAte ())
						{
						// Звук
						if (isSound)
							SAte[rnd.Next (SAte.Length)].Play ();

						// Генерация нового яблока (если игра не окончена)
						NewApple ();

						// Наращивание текущего числа очков и всего съеденных ябок
						currentScore += SMult;
						eatenApples++;

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
				if (keyboardState.IsKeyDown (Keys.S))       // Sound on/off
					{
					isSound = !isSound;
					SOnOff.Play ();

					// Была нажата клавиша
					return true;
					}

				if (keyboardState.IsKeyDown (Keys.M))
					{
					if (isMusic)                            // Music on/off
						{
						isMusic = false;
						MediaPlayer.Stop ();
						}
					else
						{
						isMusic = true;
						MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));
						}
					SOnOff.Play ();

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
						levelNumber--;
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

								if (isSound)
									SStop.Play ();
								}

							// Continue
							else
								{
								showLevelMsg = false;
								isWorking = true;

								if (isSound)
									SStart.Play ();
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
							if (isSound)
								SOnOff.Play ();

							return true;
							}
						}

					// Попытка выхода
					if (showExitMsg)
						{
						// Выход из игры (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							{
							this.Exit ();

							return true;
							}

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
				string[] values = Localization.GetText ("ScoreLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stScoreLines.Length; i++)
					stScoreLines[i] = values[i];
				}

			/*string S1,
					S2 = String.Format (" В розыгрыше: {0,4:D} ", currentScore),
					S3 = String.Format (" Выигрыш: {0,6:D} ", score),
					S4 = String.Format (" Съедено: {0,5:D} ", eatenApples),
					S5 = String.Format (" Осталось съесть: {0,2:D} ", applesQuantity - currentScore / SMult);
			if (isWorking)
				S1 = String.Format (" УРОВЕНЬ {0,2:D} ", levelNumber + 1);
			else
				S1 = " ПАУЗА ";*/
			string S00 = string.Format (stScoreLines[0], currentScore);
			string S01 = string.Format (stScoreLines[1], score);
			string S02 = string.Format (stScoreLines[2], eatenApples);
			string S03 = string.Format (stScoreLines[3], applesQuantity - currentScore / SMult);
			string S04 = (isWorking ? string.Format (stScoreLines[4], levelNumber + 1) : stScoreLines[5]);

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
			if (isMusic)
				DrawShadowedString (defFont, "[\x266B]", V6, SnakeGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266B]", V6, SnakeGameColors.Black);

			if (isSound)
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
				string[] values = Localization.GetText ("LevelLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLevelLines.Length; i++)
					stLevelLines[i] = values[i];
				}

			/*string S1 = string.Format ("УРОВЕНЬ {0,2:D}", levelNumber + 1),
					S2 = string.Format ("Необходимо съесть {0,2:D} объектов", applesQuantity),
					S3 = "Нажмите Пробел, чтобы начать";*/
			string S00 = string.Format (stLevelLines[0], levelNumber + 1);
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
				string[] values = Localization.GetText ("SuccessLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stSuccessLines.Length; i++)
					stSuccessLines[i] = values[i];
				}

			/*string S1 = "УРОВЕНЬ ПРОЙДЕН!",
					S2 = "Нажмите Пробел для продолжения";*/

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
				string[] values = Localization.GetText ("LoseLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLoseLines.Length; i++)
					stLoseLines[i] = values[i];
				}

			/*string S1 = "УРОВЕНЬ",
					S2 = "НЕ ПРОЙДЕН!",
					S3 = "Нажмите Пробел, чтобы попробовать снова";*/

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
				string[] values = Localization.GetText ("StartLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stStartLines.Length - 1; i++)
					stStartLines[i] = values[i];
				stStartLines[3] = ProgramDescription.AssemblyTitle;
				}

			/*string S1 = ProgramDescription.AssemblyTitle,
					S2 = RDGenerics.AssemblyCopyright,
					S3 = ProgramDescription.AssemblyLastUpdate,
					S4 = "Нажмите Пробел, чтобы начать игру,\n" +
						 "      F1 для вызова справки,      \n" +
						 "        или Esc для выхода        ";*/

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stStartLines[3]).X) / 2,
						(BackBufferHeight - 300) / 2),
					/*V2 = new Vector2 (BackBufferWidth - midFont.MeasureString (stStartLines[0]).X - 20,
						BackBufferHeight - 70),
					V3 = new Vector2 (BackBufferWidth - midFont.MeasureString (stStartLines[1]).X - 20,
						BackBufferHeight - 40),*/
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[0]).X) / 2,
						BackBufferHeight - 180),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[1]).X) / 2,
						BackBufferHeight - 150),
					V6 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[2]).X) / 2,
						BackBufferHeight - 120);

			spriteBatch.Draw (startBack, Vector2.Zero, SnakeGameColors.White);
			spriteBatch.Draw (snakeImg, startSnakeVector, SnakeGameColors.White);
			spriteBatch.DrawString (bigFont, stStartLines[3], V1, SnakeGameColors.Gold);
			/*spriteBatch.DrawString (midFont, S2, V2, SnakeGameColors.Silver);
			spriteBatch.DrawString (midFont, S3, V3, SnakeGameColors.Silver);*/
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
				string[] values = Localization.GetText ("FinishLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stFinishLines.Length; i++)
					stFinishLines[i] = values[i];
				}

			/*string S1 = "ВЫ ПОБЕДИЛИ!!!",
					S2 = "Ваши результаты:",
					S3 = string.Format ("Всего очков:    {0,6:D}\nВсего съедено:   {1,5:D}",
						score, eatenApples),
					S4 = "Нажмите Пробел для продолжения";*/
			string S02 = string.Format (stFinishLines[2], score);
			string S03 = string.Format (stFinishLines[3], eatenApples);

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
			spriteBatch.DrawString (midFont, stFinishLines[1], V2, SnakeGameColors.Brown);
			spriteBatch.DrawString (midFont, S02, V3, SnakeGameColors.Brown);
			spriteBatch.DrawString (midFont, S03, V3, SnakeGameColors.Brown);
			spriteBatch.DrawString (defFont, stFinishLines[4], V4, SnakeGameColors.DBlue);
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
				string[] values = Localization.GetText ("ExitLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stExitLines.Length; i++)
					stExitLines[i] = values[i];
				}

			/*string S1 = "Вы действительно хотите",
					S2 = "завершить игру?",
					S3 = "Нажмите Y, чтобы выйти из игры,",
					S4 = "или N, чтобы вернуться";*/

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
			graphics.ToggleFullScreen ();
			spriteBatch.End ();

			if (Language)
				RDGenerics.MessageBox ();
			else
				RDGenerics.ShowAbout (false);

			// Возврат в исходное состояние
			spriteBatch.Begin ();
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
			graphics.ToggleFullScreen ();

			// Выход в меню
			gameStatus = GameStatus.Start;
			showingServiceMessage = false;
			}
		private bool showingServiceMessage = false;

		/*
		/// <summary>
		/// Метод отображает справку по игре
		/// </summary>
		private void ShowHelpMessage ()
			{
			string S1 = "Правила игры",
					S2 = "   Змейке необходимо съесть все объекты на каждом уровне. Число объектов\n" +
						 "и скорость змейки будет расти с каждым новым уровнем. Чтобы пройти игру с\n" +
						 "максимальным количеством очков, ей достаточно не сталкиваться со стенками\n" +
						 "уровня и с самой собой. За каждый съеденный объект даётся 10 очков, за\n" +
						 "столкновение отнимается количество несъеденных объектов уровня. Пройдя\n" +
						 "игру, можно начать её заново с уже заработанными очками",
					S3 = "Удачи!!!",
					S4 = "Управление",
					S5 = "Пробел - пауза / возобновление / начало игры     S - включение / выключение звука\n" +
						 "Стрелки - управление змейкой                     M - включение / выключение музыки\n" +
						 "Esc - выход из игры / из справки";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						BackBufferHeight / 2 - 290),
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S2).X) / 2,
						BackBufferHeight / 2 - 240),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						BackBufferHeight / 2 - 90),
					V4 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S4).X) / 2,
						BackBufferHeight / 2 - 40),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S5).X) / 2,
						BackBufferHeight / 2 + 10);

			spriteBatch.Draw (startBack, Vector2.Zero, SnakeGameColors.White);
			spriteBatch.Draw (snakeImg, startSnakeVector, SnakeGameColors.White);
			spriteBatch.DrawString (midFont, S1, V1, SnakeGameColors.Gold);
			spriteBatch.DrawString (defFont, S2, V2, SnakeGameColors.DBlue);
			spriteBatch.DrawString (defFont, S3, V3, SnakeGameColors.DBlue);
			spriteBatch.DrawString (midFont, S4, V4, SnakeGameColors.Gold);
			spriteBatch.DrawString (defFont, S5, V5, SnakeGameColors.DBlue);
			}*/

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
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));

			// Поиск следующей структуры уровня
			while (true)
				{
				// Поиск С АВТОСМЕЩЕНИЕМ НА СЛЕДУЮЩИЙ УРОВЕНЬ
				++levelNumber;
				if (levelNumber < LevelData.LevelsQuantity)
					break;

				// Перезапуск с нулевого уровня в конце игры
				levelNumber = -1;
				gameStatus = GameStatus.Finish;
				if (isMusic)
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// Выгрузка предыдущего уровня и загрузка нового
			if (level != null)
				level.Dispose ();
			level = new SnakeLevel (Services, levelNumber);

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

			// Запись настроек и результатов игры (в зависимости от того, есть они или нет)
			GameSettings (true);
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
				NewV.X = rnd.Next ((int)(level.LevelSize.X * Tile.Width));
				NewV.Y = rnd.Next ((int)(level.LevelSize.Y * Tile.Height));
				} while (IsCollapted (NewV, true));

			applePosition = NewV;

			// Выбор новой анимации
			appleAnimator.PlayAnimation (appleAnimation[rnd.Next (appleAnimation.Length)]);
			}

		/// <summary>
		/// Метод отрисовывает текстовую строку
		/// </summary>
		/// <param name="VFont"></param>
		/// <param name="VString"></param>
		/// <param name="VPosition"></param>
		/// <param name="VColor"></param>
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

		/// <summary>
		/// Метод считывает / сохраняет настройки игры
		/// </summary>
		/// <param name="Write">Флаг указывает на режим записи</param>
		private void GameSettings (bool Write)
			{
			if (Write)
				{
				RDGenerics.SetAppSettingsValue ("Level", levelNumber.ToString ());
				RDGenerics.SetAppSettingsValue ("Score", score.ToString ());
				RDGenerics.SetAppSettingsValue ("EatenApples", eatenApples.ToString ());
				RDGenerics.SetAppSettingsValue ("Music", isMusic.ToString ());
				RDGenerics.SetAppSettingsValue ("Sound", isSound.ToString ());
				}
			else
				{
				try
					{
					levelNumber = int.Parse (RDGenerics.GetAppSettingsValue ("Level"));
					score = int.Parse (RDGenerics.GetAppSettingsValue ("Score"));
					eatenApples = int.Parse (RDGenerics.GetAppSettingsValue ("EatenApples"));
					isMusic = bool.Parse (RDGenerics.GetAppSettingsValue ("Music"));
					isSound = bool.Parse (RDGenerics.GetAppSettingsValue ("Sound"));
					}
				catch { }
				}
			}
		}
	}
