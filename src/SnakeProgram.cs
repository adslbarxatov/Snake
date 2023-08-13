using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// ����� ����� ����������
	/// </summary>
	public static class SnakeProgram
		{
		private static string[] Pths = {
			// ��� ������� �����������
			"Background\\Back00.xnb",
			"Background\\Back01.xnb",
			"Background\\Back02.xnb",
			"Background\\Back03.xnb",
			"Background\\StartBack.xnb",
			"Background\\SnakeImg.xnb",

            // ������
            "Font\\DefFont.xnb",
			"Font\\BigFont.xnb",
			"Font\\MidFont.xnb",

			// ��� ���������
			"Messages\\MessageBack.xnb",

			// ��� �������
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

			// ��� tiles
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
		/// ������� ������� ���������
		/// </summary>
		/// <param name="args">��������� ��������� ������</param>
		public static void Main (string[] args)
			{
			// �������������
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// �������� XPUN
			if (!Localization.IsXPUNClassAcceptable)
				return;

			// �������� ������� ������������ �����
			if (!RDGenerics.IsAppInstanceUnique (true))
				return;

			// ����������� ������� � ������� �� �������� ��������
			if (!RDGenerics.AcceptEULA ())
				return;
			RDGenerics.ShowAbout (true);

			// �������� ����
			bool _ = RDGenerics.IsRegistryAccessible;

			// ���������� �������� �� ������� ���� ����������� ������
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
