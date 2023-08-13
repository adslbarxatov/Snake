using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Точка входа приложения
	/// </summary>
	public static class SnakeProgram
		{
		private static string[] Pths = {
			// Все фоновые изображения
			"Background\\Back00.xnb",
			"Background\\Back01.xnb",
			"Background\\Back02.xnb",
			"Background\\Back03.xnb",
			"Background\\StartBack.xnb",
			"Background\\SnakeImg.xnb",

            // Шрифты
            "Font\\DefFont.xnb",
			"Font\\BigFont.xnb",
			"Font\\MidFont.xnb",

			// Фон сообщений
			"Messages\\MessageBack.xnb",

			// Все мелодии
			"Sounds\\Ate1.xnb",
			"Sounds\\Ate2.xnb",
			"Sounds\\Ate3.xnb",
			"Sounds\\Ate4.xnb",
			"Sounds\\Completed.xnb",
			"Sounds\\Failed.xnb",
			"Sounds\\Music1.wma",
			"Sounds\\Music1.xnb",
			"Sounds\\Music2.wma",
			"Sounds\\Music2.xnb",
			"Sounds\\SoundOnOff.xnb",
			"Sounds\\SStop.xnb",
			"Sounds\\SStart.xnb",

			// Все tiles
			"Tiles\\Apple1.xnb",
			"Tiles\\Apple2.xnb",
			"Tiles\\Apple3.xnb",
			"Tiles\\Apple4.xnb",
			"Tiles\\Apple5.xnb",
			"Tiles\\Apple6.xnb",
			"Tiles\\Apple7.xnb",
			"Tiles\\Apple8.xnb",
			"Tiles\\Apple9.xnb",
			"Tiles\\Compas.xnb",
			"Tiles\\DoorH.xnb",
			"Tiles\\DoorV.xnb",
			"Tiles\\Empty.xnb",
			"Tiles\\Part_Head.xnb",
			"Tiles\\Part_HeadRush.xnb",
			"Tiles\\Part_Part.xnb",
			"Tiles\\Stone00.xnb",
			"Tiles\\Stone01.xnb",
			"Tiles\\Stone02.xnb",
			"Tiles\\Stone03.xnb",
			"Tiles\\Stone10.xnb",
			"Tiles\\Stone11.xnb",
			"Tiles\\Stone12.xnb",
			"Tiles\\Stone13.xnb"};

		/// <summary>
		/// Главная функция программы
		/// </summary>
		/// <param name="args">Агрументы командной строки</param>
		public static void Main (string[] args)
			{
			// Инициализация
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// Контроль XPUN
			if (!Localization.IsXPUNClassAcceptable)
				return;

			// Проверка запуска единственной копии
			if (!RDGenerics.IsAppInstanceUnique (true))
				return;

			// Отображение справки и запроса на принятие Политики
			if (!RDGenerics.AcceptEULA ())
				return;
			RDGenerics.ShowAbout (true);

			// Контроль прав
			bool _ = RDGenerics.IsRegistryAccessible;

			// Выполнение проверки на наличие всех необходимых файлов
			for (int i = 0; i < Pths.Length; i++)
				if (!File.Exists (".\\Content\\Snake\\" + Pths[i]))
					{
					RDGenerics.LocalizedMessageBox (RDMessageTypes.Error_Center, "MissingFile");
					return;
					}

			using (SnakeGame game = new SnakeGame ())
				game.Run ();
			}
		}
	}
