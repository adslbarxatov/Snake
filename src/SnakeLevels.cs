using Microsoft.Xna.Framework;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает уровни игры
	/// </summary>
	public class LevelData
		{
		/// <summary>
		/// Число уровней
		/// </summary>
		public const int LevelsQuantity = 12;

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

		// Текущее содержимое уровня и его номер
		private string[] currentLevelMap;
		private int currentLevelNumber = -1;

		/// <summary>
		/// Метод возвращает структуру уровней
		/// </summary>
		/// <param name="LevelNumber">Номер уровня</param>
		/// <param name="X">Абсцисса плитки</param>
		/// <param name="Y">Ордината плитки</param>
		public char LevelsData (int LevelNumber, int X, int Y)
			{
			// Инициализировать, только если уровень изменился
			if (currentLevelNumber != LevelNumber)
				InitLevelMap (LevelNumber);

			return currentLevelMap[Y][X];
			}

		/// <summary>
		/// Метод инициализирует карту уровня
		/// </summary>
		/// <param name="LevelNumber">Номер карты для инициализации</param>
		private void InitLevelMap (int LevelNumber)
			{
			switch (LevelNumber)
				{
				// Уровень 1
				case (0):
					currentLevelMap = new string[]
						{"ooooooooooooooooooooooooxxoooooo",
						 "oxxxxxxxxxxxxxxxxxxxxxx|xx|ooooo",
						 "oxxxxxxxxxxxxxxxxxxxxxxxxxxooooo",
						 "oxxxxxxxxxxxxxxxxo--xxxxxxxooooo",
						 "oo--oooooooooooooooooooo--oooooo",
						 "o                          ooooo",
						 "| *>0B                     ooooo",
						 "o                          ooooo",
						 "ooooo                      ooooo",
						 "ooooo                      ooooo",
						 "ooooo          oo              o",
						 "ooooo         oooo             |",
						 "ooooo         oooo             |",
						 "ooooo          oo              o",
						 "ooooo                      ooooo",
						 "ooooo                      ooooo",
						 "ooooo                      ooooo",
						 "ooooo                      ooooo",
						 "ooooo                      ooooo",
						 "oooooooooooooo    oooooooooooooo",
						 "ooooooooooxxxo    oooooooooooooo",
						 "ooooooooooxxx|    oooooooooooooo",
						 "ooooooooooxxxo    oooooooooooooo",
						 "ooooooooooooooo--ooooooooooooooo"};
					break;

				// Уровень 2
				case (1):
					currentLevelMap = new string[]
						{"ooooooooooooooo--ooxxxxxxxxxxxxxxxxoo--ooooooo",
						 "oooooooooooooo    oxxxxxxxxxxxxxxxxo    oooooo",
						 "oooooooooooooo    oxxxxxxxxxxxxxxxxo    oooooo",
						 "oooooooooooooo    oooooooo--oooooooo    oooooo",
						 "o                              |             o",
						 "|             1C<*             |             |",
						 "o           --------           o             o",
						 "oooo                        oooooooo       ooo",
						 "o                           oooooooo       ooo",
						 "|                           oooooooo       ooo",
						 "|                           oo             ooo",
						 "o           oooooooo        oo             ooo",
						 "oooo        oooooooo        oo             ooo",
						 "oooo                        oo             ooo",
						 "oooo                        oooooooo       ooo",
						 "oooo                        oooooooo       ooo",
						 "oooo                        oooooooo       ooo",
						 "oooo        --------           o--           o",
						 "oooo                                         |",
						 "oooo                                         o",
						 "oooooooooooooo    oooooooooooooooooo    oooooo",
						 "oooooooooooooo    oooooooooooooooooo    oooooo",
						 "oooooooooooooo    oooooooooooooooooo    oooooo",
						 "ooooooooooooooo--oooooooooooooooooooo--ooooooo"};
					break;

				// Уровень 3
				case (2):
					currentLevelMap = new string[]
						{"ooooooooooooooo--oooooooooooooxxxxxxx",
						 "|    ooooooooo       ooooooooo|xx|xxx",
						 "|    ooooooooo       ooooooooooxxooxx",
						 "o    ooooooooo       oooooooooxxxxoxx",
						 "o           oo              ooxxxxoxx",
						 "o           oo              ooxxxxoxx",
						 "o           oooooooo        ooxxxxoxx",
						 "oooo                        ooxxxxoxx",
						 "oooo                        ooxxxxoxx",
						 "oooo          oooo          ooxxxxoxx",
						 "oooo          oooo          ooxxxxooo",
						 "oooo                        ooxxxxooo",
						 "oooo                        oo--ooooo",
						 "oooo        oooooooo             oooo",
						 "oooo              oo             oooo",
						 "oooo              oo             oooo",
						 "ooooooooooo       oooooooooo        o",
						 "ooooooooooo       oxxxxxxxxo      1 o",
						 "xx|xxxxxxoo       oxxxxxxxx|      E o",
						 "xx|xxxxxxoooooo  ooxxxxxxxx|      ^ o",
						 "xxoxxxxxxoooooo  ooxxxxxxxxo      * o",
						 "xxooooooooooooo  ooooooooooooooooo  o",
						 "xxxxxxxxxxxxxx|                     |",
						 "xxxxxxxxxxxxxx|                     |",
						 "ooooooooooooooooooooooooooooooooooooo"};
					break;

				// Уровень 4
				case (3):
					currentLevelMap = new string[]
						{"ooooooooooooooooooooooooooooooooxxx",
						 "ooo               |xxxxxxxxxxxx|xxx",
						 "ooo               |xxxxxxxxxxxx|xxx",
						 "ooo  ooooooooooo  ooooooooooooooxxx",
						 "xxo            |               oxxx",
						 "xx| *>2E       |               oxxx",
						 "xxo         o  oooooooooooooo  oxxx",
						 "oooo  ooooooo               o  oxxx",
						 "o           o               o  oxxx",
						 "o           oooooooo   o    o  o--o",
						 "o  o               o   o          o",
						 "o  o               o   o          o",
						 "o  o        o      o   oooooo  o  o",
						 "o  o        o      o        o  o  o",
						 "o  o        o      o        o  o  o",
						 "o  o        o      o     o  o  o  o",
						 "o  o        o      o     o  o  o  o",
						 "o  o        o            o     o  o",
						 "o  o        o            o     o  o",
						 "o  oooo  oooo  oooooooooooo  ooo  o",
						 "o     o                      o    o",
						 "o     o                      o    o",
						 "oooo  oooooooo  ooooooooooo  o    o",
						 "o               o            o    o",
						 "o               o            o  ooo",
						 "o  oooooooooooooo      o     o  oxx",
						 "o                      o        |xx",
						 "o                      o        |xx",
						 "oooooooooooooooooooooooooooooooooxx"};
					break;

				// Уровень 5
				case (4):
					currentLevelMap = new string[]
						{"oooooooooooooooooooooooooooooooooooooo",
						 "| *>3F          o                    |",
						 "|               o                    |",
						 "o    |                         o     o",
						 "o    |                         o     o",
						 "o    |      ooooooo   o        o     o",
						 "o    |      o         o        o     o",
						 "o    |      o         o        oooo  o",
						 "o           o         o        o     o",
						 "o           o   ooooooo        o     o",
						 "oooooo   oooo   ooooooo        o     o",
						 "o           o         oooooooooo  oooo",
						 "o           o         o              o",
						 "o    |      o         o              o",
						 "o    |      ooooooo   o        o     o",
						 "o    |                         o     o",
						 "o    |                         o     o",
						 "o              o               o     o",
						 "o              o               o     o",
						 "o   oooooooooooo   ooooooooooooooo   o",
						 "o                                    o",
						 "o                                    |",
						 "o                                    |",
						 "oooooooooooooooooooooooooooooooooooooo"};
					break;

				// Уровень 6
				case (5):
					currentLevelMap = new string[]
						{"oo--oooooooooooooooooooooooooooo",
						 "o                 o            o",
						 "o  *              o            |",
						 "o  v o                         |",
						 "o  4 o                         o",
						 "o  F oooo   ooooooo   oooo   ooo",
						 "o    o      o         o        o",
						 "o    o      o         o        o",
						 "o           o         o        o",
						 "o           o    oooooo        o",
						 "oo   oooooooo         o        o",
						 "o           o         oooooo   o",
						 "o           o         o        o",
						 "o    o      o         o        o",
						 "o    oooo   ooooooo   o        o",
						 "o    o                         o",
						 "o    o                         o",
						 "o                     o        o",
						 "o                     o        o",
						 "oo   oooooooooooooooooooooo   oo",
						 "o          o                   o",
						 "o          o                   o",
						 "o          o   oooooo   oooo   o",
						 "o          o        o          o",
						 "o          o        o          |",
						 "o                   o          |",
						 "o                   o          o",
						 "oooooooooooooooooooooooooooooooo"};
					break;

				// Уровень 7
				case (6):
					currentLevelMap = new string[]
						{"oooooooooooooooooooooooooooooooo",
						 "o                              o",
						 "| *>5G                         o",
						 "o                              o",
						 "o   o----------------------o   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   |          oo          o   o",
						 "o   |          oo              o",
						 "o   |                          o",
						 "o   |                         oo",
						 "o   |                        ooo",
						 "o   |                       oooo",
						 "o   |                      ooooo",
						 "o   |                     oooooo",
						 "o   o------o     o       ooooooo",
						 "o               oo         ooooo",
						 "o              ooo         |xxxx",
						 "o             oooo         |xxxx",
						 "oooooooooooooooooooooooooooooooo"};
					break;

				// Уровень 8
				case (7):
					currentLevelMap = new string[]
						{"xxxxooooooooooooooooooooooooooooooooooooooooooooooooooooooooxxxx",
						 "xxxxo                                                      oxxxx",
						 "oo-oo                                                      oo-oo",
						 "o          o     o     o                o     o     o          o",
						 "o                            o    o                            o",
						 "o                            o    o                            o",
						 "o   oooooooooooooooooooooooooo    oooooooooooooooooooooooooo   o",
						 "o                                                              o",
						 "o                                                              o",
						 "o                              oo                              o",
						 "ooooo   oooooooooooooooooooo   oo   oooooooooooooooooooo   ooooo",
						 "|                          o   oo   o                          |",
						 "| *                        o   oo   o                          |",
						 "o v                        o   oo   o                          o",
						 "o 6 o                      o   oo   o                      o   o",
						 "o G o         oooo             oo             oooo         o   o",
						 "o   o         oooo             oo             oooo         o   o",
						 "o   o                         oooo                         o   o",
						 "o   o                        ooxxoo                        o   o",
						 "o   o                       ooxxxxoo                       o   o",
						 "o   o                      ooxxxxxxoo                      o   o",
						 "o   ooooooooooooooooo      oxxxxxxxxo      ooooooooooooooooo   o",
						 "o                          oooo--oooo                          o",
						 "o                                                              o",
						 "o                                                              o",
						 "ooooooooooooooooooooooooooooooo--ooooooooooooooooooooooooooooooo"};
					break;

				// Уровень 9
				case (8):
					currentLevelMap = new string[]
						{"oo--oooooooooooooooooooooooooooo",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "o   o  o----------------o  o   o",
						 "o                              o",
						 "o                              o",
						 "o   o                      o   o",
						 "o * |                      |   o",
						 "o v |                      |   o",
						 "o 7 |                      |   o",
						 "o H |    oo    oo    oo    |   o",
						 "o   |    oo    oo    oo    |   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   |                      |   o",
						 "o   o                      o   o",
						 "o                              o",
						 "o                              o",
						 "o   o  o----------------o  o   o",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "oooooooooooooooooooooooooooo--oo"};
					break;

				// Уровень 10
				case (9):
					currentLevelMap = new string[]
						{"oo--ooooooooooooooooooooooooooooooo--oo",
						 "o                                     o",
						 "o               3T<*                  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o                                     o",
						 "o                                     o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o                                     o",
						 "o                                     o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o                                     o",
						 "o                                     o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o                                     o",
						 "o                                     o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o                                     o",
						 "o                                     o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o  ooooo  ooooo  ooooo  ooooo  ooooo  o",
						 "o                                     o",
						 "o                                     o",
						 "oooooooooooooooooo---oooooooooooooooooo"};
					break;

				// Уровень 11
				case (10):
					currentLevelMap = new string[]
						{"oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo",
						 "|              |                                |              o",
						 "|   *>3H       |                                |              o",
						 "o  o--------o  |  o  o  o--------------------o  |  o--------o  o",
						 "o  |           |  |  |                       |  |           |  o",
						 "o  |           |  |  |                       |  |           |  o",
						 "o  |  o--------o  |  o-----------o--------o  |  o--------o  |  o",
						 "o  |  |           |              |           |              |  o",
						 "o  |  |           |              |           |              |  o",
						 "o  |  |  o--------o  o--------o  o  o--------o  o-----------o  o",
						 "o  |  |                       |                                o",
						 "o  |  |                       |                                o",
						 "o  o  o--------o  o--------o  |  o--------------o  o--------o  o",
						 "o              |  |           |                 |           |  o",
						 "o              |  |           |                 |           |  o",
						 "o  o           o  |  o-----o  o--------------o  o--------o  |  o",
						 "o  |              |        |  |              |              |  o",
						 "o  |              |        |  |              |              |  o",
						 "o  o-----------o  o-----o  |  o  o--------o  o  o-----------o  o",
						 "o              |           |     |              |              o",
						 "o              |           |     |              |              o",
						 "o  o--------o  |           ooooooo  o--------o  |  o-----------o",
						 "o  |           |              |              |  |              o",
						 "o  |           |              |              |  |              o",
						 "o  |  o--------o-----------o  |  o--------o  |  o  o--------o  o",
						 "o  |                          |  |           |              |  o",
						 "o  |                          |  |           |              |  o",
						 "o  o-----------------------o  o  |  o--------o  o--------o  |  o",
						 "o                                |              |           |  o",
						 "o                                |              |           |  o",
						 "oooo-----------o  o-----------o  |  o--------o  |  o--------o  o",
						 "|              |  |              |           |  |           |  o",
						 "|              |  |              |           |  |           |  o",
						 "o  o           o  |  o--o--------o--------o  |  o--------o  |  o",
						 "o  |              |     |                    |              |  o",
						 "o  |              |     |                    |              |  o",
						 "o  o--------------o  o  o  o-----------------o-----------o  o  o",
						 "o                    |                                         |",
						 "o                    |                                         |",
						 "oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo"};
					break;

				// Уровень 12
				case (11):
					currentLevelMap = new string[]
						{"oooooooo--ooooo--ooooo--oooooooo",
						 "o                              o",
						 "o *>9I                         o",
						 "o                              o",
						 "|                              |",
						 "|                              |",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "|                              |",
						 "|                              |",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "|                              |",
						 "|                              |",
						 "o                              o",
						 "o                              o",
						 "o                              o",
						 "oooooooo--ooooo--ooooo--oooooooo"};
					break;

				default:
					return;
				}

			levelSize = new Vector2 (currentLevelMap[0].Length, currentLevelMap.Length);
			currentLevelNumber = LevelNumber;
			}
		}
	}
