using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RD_AAOW
	{
	/////////////////////////////////////////////////////////////////////////////////
	// ОПИСАТЕЛИ ПЛИТОК

	/// <summary>
	/// Типы поведения плитки
	/// </summary>
	public enum TileCollision
		{
		/// <summary>
		/// Препятствие
		/// </summary>
		Stone = 1,

		/// <summary>
		/// Пустое поле
		/// </summary>
		Empty = 0
		}

	/// <summary>
	/// Структура, содержащая поведение и позицию каждой плитки
	/// </summary>
	public struct Tile
		{
		/// <summary>
		/// Текстура плитки
		/// </summary>
		public Texture2D Texture;

		/// <summary>
		/// Описатель столкновений плитки
		/// </summary>
		public TileCollision Collision;

		/// <summary>
		/// Ширина плитки
		/// </summary>
		public const int Width = 32;

		/// <summary>
		/// Высота плитки
		/// </summary>
		public const int Height = 32;

		/// <summary>
		/// Сборный размер плитки
		/// </summary>
		public static readonly Vector2 Size = new Vector2 (Width, Height);

		/// <summary>
		/// Конструктор. Создаёт новую плитку
		/// </summary>
		/// <param name="VTexture">Текстура плитки</param>
		/// <param name="VCollision">Описатель столкновений плитки</param>
		public Tile (Texture2D VTexture, TileCollision VCollision)
			{
			Texture = VTexture;
			Collision = VCollision;
			}
		}

	/////////////////////////////////////////////////////////////////////////////////
	// ОПИСАТЕЛИ УРОВНЕЙ

	/// <summary>
	/// Класс описывает уровень игры Змейка
	/// </summary>
	public class SnakeLevel: IDisposable
		{
		// Физическая структура уровня

		/// <summary>
		/// Координаты всех плиток уровня
		/// </summary>
		public Tile[,] Tiles
			{
			get
				{
				return tiles;
				}
			}
		private Tile[,] tiles;

		/// <summary>
		/// Размер уровня
		/// </summary>
		public Vector2 LevelSize
			{
			get
				{
				return levelSize;
				}
			}
		private Vector2 levelSize;

		// Фон уровня
		private Texture2D background;

		// Номер текущей группы текстур фона (фон + тип камней)
		private int currentTextureNumber;

		// Количественные константы для текстур фона и камней
		private const int backgroundVariants = 4;
		private const int stonesVariants = 4;
		private const int stonesTypes = 2;

		// Параметры, читаемые из уровня и отправляемые в класс игры
		private Vector2 playerTo, playerStartPosition;

		/// <summary>
		/// Возвращает направление движения игрока при старте
		/// </summary>
		public Vector2 PlayerTo
			{
			get
				{
				return playerTo;
				}
			}

		/// <summary>
		/// Возвращает начальную позицию игрока
		/// </summary>
		public Vector2 PlayerStartPosition
			{
			get
				{
				return playerStartPosition;
				}
			}

		/// <summary>
		/// Скорость игрока на уровне
		/// </summary>
		public float Speed
			{
			get
				{
				return speed;
				}
			}
		private float speed = 0;

		/// <summary>
		/// Число съедаемых объектов на уровне
		/// </summary>
		public int ApplesQuantity
			{
			get
				{
				return applesQuantity;
				}
			}
		private int applesQuantity = 0;

		/// <summary>
		/// Позиция камеры на сцене
		/// </summary>
		public Vector2 CameraPosition
			{
			get
				{
				return cameraPosition;
				}
			}
		private Vector2 cameraPosition;

		/// <summary>
		/// Контент уровня
		/// </summary>
		public ContentManager Content
			{
			get
				{
				return content;
				}
			}
		private ContentManager content;

		/// <summary>
		/// Конструктор. Инициализирует уровень
		/// </summary>
		/// <param name="LevelNumber">Номер уровня</param>
		public SnakeLevel (IServiceProvider ServiceProvider, int LevelNumber)
			{
			// Генерация нового номера фонового изображения
			currentTextureNumber = RDGenerics.RND.Next (backgroundVariants);

			// Создание контент-менеджера для текущего уровня
			content = new ContentManager (ServiceProvider, "Content/Snake");

			// Загрузка физики уровня
			LoadTiles (LevelNumber);

			// Загрузка фона уровня
			background = Content.Load<Texture2D> ("Background/Back0" + currentTextureNumber);
			}

		/// <summary>
		/// ФУНКЦИЯ ЗАГРУЗКИ ПЛИТОК УРОВНЯ
		/// Пробегает по каждой плитке в файле структуры и загружает её
		/// внешность и поведение. В данной игре удалено чтение из файла
		/// и работает загрузка из класса уровней
		/// </summary>
		/// <param name="LevelNumber">Номер уровня</param>
		private void LoadTiles (int LevelNumber)
			{
			// Доступ к классу с базой уровней
			LevelData LDt = new LevelData ();

			// Загрузка размера уровня и создание массива плиток
			LDt.LevelsData (LevelNumber, 0, 0);
			levelSize = LDt.LevelSize;
			tiles = new Tile[(int)levelSize.X, (int)levelSize.Y];

			// Загрузка каждой отдельной плитки
			for (int y = 0; y < Height; ++y)
				{
				for (int x = 0; x < Width; ++x)
					{
					char tileType = LDt.LevelsData (LevelNumber, x, y);
					tiles[x, y] = LoadTile (tileType, x, y, LevelNumber);
					}
				}
			}

		/// <summary>
		/// ФУНКЦИЯ ЗАГРУЗКИ ОТДЕЛЬНОЙ ПЛИТКИ
		/// Загружает внешность индивидуальной плитки и её поведение. Характер
		/// берётся из файла структуры, который указывает на то, что должно
		/// быть загружено
		/// </summary>
		/// <param name="TileType">Тип плитки</param>
		/// <param name="X">Абсцисса плитки</param>
		/// <param name="Y">Ордината плитки</param>
		/// <param name="LevelNumber">Номер уровня</param>
		private Tile LoadTile (char TileType, int X, int Y, int LevelNumber)
			{
			switch (TileType)
				{
				// Камни
				case 'o':
					return LoadTile (string.Format ("Stone{0:D}{1:D}", currentTextureNumber % stonesTypes,
						RDGenerics.RND.Next (stonesVariants)), TileCollision.Stone);
				case '-':
					return LoadTile ("DoorH", TileCollision.Stone);
				case '|':
					return LoadTile ("DoorV", TileCollision.Stone);
				case 'x':
					return LoadTile ("Empty", TileCollision.Stone);

				// Голова игрока (стартовая позиция)
				case '*':
					playerStartPosition = new Vector2 (X, Y);
					return new Tile (null, TileCollision.Empty);

				// Здесь происходит загрузка параметров уровня (4 направления)
				case '<':
					playerTo.X = -1;
					playerTo.Y = 0;
					return new Tile (null, TileCollision.Empty);
				case '>':
					playerTo.X = 1;
					playerTo.Y = 0;
					return new Tile (null, TileCollision.Empty);
				case '^':
					playerTo.X = 0;
					playerTo.Y = -1;
					return new Tile (null, TileCollision.Empty);
				case 'v':
					playerTo.X = 0;
					playerTo.Y = 1;
					return new Tile (null, TileCollision.Empty);

				// В остальных случаях считается пустой (читается число яблок и скорость)
				default:
					if ((TileType >= '0') && (TileType <= '9'))
						speed = (float)(TileType - '0') / 2.0f + 1.5f;
					if ((TileType >= 'A') && (TileType <= 'Z'))
						applesQuantity = (TileType - 'A' + 1) * 3;
					return new Tile (null, TileCollision.Empty);
				}
			}

		/// <summary>
		/// Метод создает новую плитку. Другая плитка, загружающая методы, обычно приходит
		/// к этому методу после выполнения своей специальной логики
		/// </summary>
		/// <param name="Name">Название текстуры плитки</param>
		/// <param name="Collision">Обработчик столкновений плитки</param>
		private Tile LoadTile (string Name, TileCollision Collision)
			{
			return new Tile (Content.Load<Texture2D> ("Tiles/" + Name), Collision);
			}

		/// <summary>
		/// Выгрузка контента
		/// </summary>
		public void Dispose ()
			{
			Content.Unload ();
			}

		/// <summary>
		/// Ширина уровня (в плитках)
		/// </summary>
		public int Width
			{
			get
				{
				return tiles.GetLength (0);
				}
			}

		/// <summary>
		/// Высота уровня (в плитках)
		/// </summary>
		public int Height
			{
			get
				{
				return tiles.GetLength (1);
				}
			}

		/// <summary>
		/// Метод отрисовывает уровень
		/// </summary>
		/// <param name="VGameTime">Время игры</param>
		/// <param name="PlayerPosition0">Начальная позиция игрока</param>
		public void Draw (GameTime VGameTime, SpriteBatch VSpriteBatch, Vector2 PlayerPosition0)
			{
			// Фон
			for (int xf = 0; xf < 2 * SnakeGame.BackBufferWidth / background.Width; xf++)
				for (int yf = 0; yf < 3 * SnakeGame.BackBufferHeight / background.Height; yf++)
					VSpriteBatch.Draw (background, -cameraPosition + new Vector2 (background.Width * xf,
						background.Height * yf), SnakeGameColors.White);

			// Выполнение смещения игрового поля при выходе за границы окна
			VSpriteBatch.End ();

			ScrollCamera (VSpriteBatch.GraphicsDevice.Viewport, PlayerPosition0);
			Matrix cameraTransform = Matrix.CreateTranslation (-cameraPosition.X, -cameraPosition.Y, 0.0f);

			VSpriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
				DepthStencilState.None, RasterizerState.CullNone, null, cameraTransform);

			// Вывод плиток с учётом смещения игрового поля (вывод только видимых плиток)
			int left = (int)Math.Floor (cameraPosition.X / Tile.Width),
				right = left + VSpriteBatch.GraphicsDevice.Viewport.Width / Tile.Width;
			right = Math.Min (right, Width - 1);        // Ограничение на выход за границу массива

			int up = (int)Math.Floor (cameraPosition.Y / Tile.Height),
				down = up + VSpriteBatch.GraphicsDevice.Viewport.Height / Tile.Height;
			down = Math.Min (down, Height - 1);

			for (int y = up; y <= down; ++y)
				for (int x = left; x <= right; ++x)
					{
					Texture2D texture = tiles[x, y].Texture;

					// Если позиция не пуста
					if (texture != null)
						{
						Vector2 position = new Vector2 (x, y) * Tile.Size;
						VSpriteBatch.Draw (texture, position, SnakeGameColors.White);
						}
					}
			}

		/// <summary>
		/// Функция пересчёта камеры наблюдения
		/// </summary>
		private void ScrollCamera (Viewport VViewport, Vector2 PlayerPosition0)
			{
			// Относительные размеры поля, при выходе за которые начинается смещение камеры
			Vector2 ViewMargin = new Vector2 (0.35f, -0.35f);

			// Абсолютные размеры этого поля и его границы
			Vector2 MarginSize = new Vector2 (VViewport.Width, VViewport.Height) * ViewMargin;
			float MarginLeft = cameraPosition.X + MarginSize.X,
				  MarginRight = cameraPosition.X + VViewport.Width - MarginSize.X,
				  MarginDown = cameraPosition.Y + VViewport.Height + MarginSize.Y,
				  MarginUp = cameraPosition.Y - MarginSize.Y;

			// Вычисление передвижения поля при приближении головы к разным границам окна
			Vector2 CamMov = new Vector2 (0.0f, 0.0f);

			if (PlayerPosition0.X < MarginLeft)
				CamMov.X = PlayerPosition0.X - MarginLeft;
			if (PlayerPosition0.X > MarginRight)
				CamMov.X = PlayerPosition0.X - MarginRight;
			if (PlayerPosition0.Y > MarginDown)
				CamMov.Y = PlayerPosition0.Y - MarginDown;
			if (PlayerPosition0.Y < MarginUp)
				CamMov.Y = PlayerPosition0.Y - MarginUp;

			// Обновление позиции камеры, но с ограничением на выход за границы уровня
			Vector2 MaxCamPos = new Vector2 (Tile.Width * Width - VViewport.Width,
				Tile.Height * Height - VViewport.Height);
			cameraPosition.X = MathHelper.Clamp (cameraPosition.X + CamMov.X, 0.0f, MaxCamPos.X);
			cameraPosition.Y = MathHelper.Clamp (cameraPosition.Y + CamMov.Y, 0.0f, MaxCamPos.Y);
			}
		}
	}
