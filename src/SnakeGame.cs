using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace RD_AAOW
	{
	/// <summary>
	/// ����� ��������� ���� ������
	/// </summary>
	public class SnakeGame: Game
		{
		/////////////////////////////////////////////////////////////////////////////////
		// ����������

		// �������� ����
		private GraphicsDeviceManager graphics;             // �������
		private SpriteBatch spriteBatch;                    // Sprite-���������
		private KeyboardState keyboardState;                // ��������� ����������
		private SpriteFont defFont, bigFont, midFont;       // ������
		private Random rnd = new Random ();                 // ���

		// ������� ���� (������� ���� -  32 x 24 ������; ����������� �����������, ��-�� fullscreen)

		/// <summary>
		/// ������ ����
		/// </summary>
		public const int BackBufferWidth = Tile.Width * 32;

		/// <summary>
		/// ������ ����
		/// </summary>
		public const int BackBufferHeight = Tile.Height * 24;

		// �������� ��������� ���� (������|����|�����)
		private GameStatus gameStatus = GameStatus.Start;   // ��������� ������ ���� (������� ����������� � Auxilitary.cs)

		// ��������� ������ � ���� ���������
		private SnakeLevel level;                           // �����-��������� ������
		private int levelNumber = 0;                        // ����� �������� ������
		private bool isWorking = false;                     // ���� �����
		private Texture2D startBack, snakeImg;              // ������ ����������� �� ������
		private Vector2 startSnakeVector;                   // ������ �� ������

		// ������� ������� ������ � ��� ������� ��������
		private Vector2 applePosition;                      // ������� �������
		private Animation[] appleAnimation;                 // ����������� ��������
		private AnimationPlayer appleAnimator;              // ������-��������

		// ������� ������� ������ ������, ������� ����������� ��������
		// � ������� �������� ������ � ����
		private List<Vector2> playerPosition = new List<Vector2> ();        // List ������� ���� ��������� ������
		private Vector2 playerTo;                                           // ����������� �������� ������
		private Animation headAnimation, headRushAnimation, bodyAnimation;
		private AnimationPlayer headAnimator, bodyAnimator;

		// ��� ���������
		private Animation messageBack;
		private AnimationPlayer messageBackAnimator;

		// �������� ������� � �� ���������
		private SoundEffect SCompleted, SFailed,            // ������, ���������
							SStart, SStop, SOnOff;          // �����, �����, ���� off/on
		private SoundEffect[] SAte;                         // ������ ����� ��������
		private bool isSound = true, isMusic = true;        // ���� � ������ � ���� on/off

		// �������� ������, ���������� ����� �� ������ � �������� Alive
		private float speed = 0;
		private int applesQuantity = 0;
		private bool isAlive = true;

		// ������, �� ������� ��������� ������������, � ������������ ����������� ���������� ��������������
		private Vector2 collaptedOn;
		private const float StoneOffs = 0.25f,
							BodyOffs = Tile.Width * 0.4f,
							AppleOffs = Tile.Width * 0.6f;

		// ����
		private int score = 0,                              // �������
					currentScore = 0,                       // ���� � ���������
					ateApples = 0;                          // ������� ����� �� ��� ����
		private const int SMult = 10;                       // ��������� ��� �����

		// ����� ����������� ���������
		private bool showLevelMsg = false,                  // ��������� � ������ ������
					 showLoseMsg = false,                   // ��������� � ����������� ������
					 showWinMsg = false,                    // ��������� � ���������
					 showExitMsg = false;                   // ������������� ������

		// ��������� �������
		private Texture2D compas;                       // ��������
		private float compasTurn = 0.0f;                // ���� �������� �������
		private Rectangle compasPosition, compasSize;   // ������� � ������ �������
		private Vector2 compasOffs;                     // ���������� ����������� ��������� ������

		// ������������� ����������
		private int kbdDelay = 1,           // ����� � Update-��������� ����� ��������� ������� ����������
					kbdDelayTimer;          // ������ ��� delay
		private const int KbdDefDelay = 25; // ������� delay ��� ������� �������

		/// <summary>
		/// �����������. �������������� ���� ������
		/// </summary>
		public SnakeGame ()
			{
			// �������� "����" ��������� ������� � ������� � ������������� �����
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
			graphics.ToggleFullScreen ();

			// ������� content-���������� ����
			Content.RootDirectory = "Content/Snake";
			}

		/// <summary>
		/// ������������� ����
		/// ������� ����������� ���� ��� �� ���� (��� � �������).
		/// ����� ������������� ��� ������������� � ��������� ��������
		/// </summary>
		protected override void Initialize ()
			{
			// ��������� �������� ����������
			//this.IsMouseVisible = true;
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// �������� �������� ��������
			// ������ ���� �����
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

			// ������ ��� ������������ � ��������
			headAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Part_Head"), Tile.Width, 0.1f, true);
			headRushAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Part_HeadRush"), Tile.Width, 0.1f, true);
			headAnimator.PlayAnimation (headAnimation);         // �� ��������� - �������� ��� ��������

			// ���� ������
			bodyAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Part_Part"), Tile.Width, 0.1f, false);
			bodyAnimator.PlayAnimation (bodyAnimation);

			// ��� ���������
			messageBack = new Animation (Content.Load<Texture2D> ("Messages/MessageBack"), 512, 0.1f, true);
			messageBackAnimator.PlayAnimation (messageBack);

			// �������� �������� ��������
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

			// �������� �������
			defFont = Content.Load<SpriteFont> ("Font/DefFont");
			bigFont = Content.Load<SpriteFont> ("Font/BigFont");
			midFont = Content.Load<SpriteFont> ("Font/MidFont");

			// �������� �������������� �������
			startBack = Content.Load<Texture2D> ("Background/StartBack");
			snakeImg = Content.Load<Texture2D> ("Background/SnakeImg");

			// ������
			compas = Content.Load<Texture2D> ("Tiles/Compas");
			compasPosition = compasSize = new Rectangle (0, 0, compas.Width, compas.Height);

			// ������ �������� � ����������� ����
			GameSettings (false);

			// ��������� ������
			MediaPlayer.IsRepeating = true;
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));

			// �������������
			base.Initialize ();
			}

		/// <summary>
		/// �����-���������� ������������ ������� ����
		/// </summary>
		/// <param name="gameTime"></param>
		protected override void Update (GameTime gameTime)
			{
			// ����� ���������� � ��������������� ��������
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

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
				case GameStatus.Help:
					// �������� �� ���������
					startSnakeVector.X += 2;
					startSnakeVector.Y = BackBufferHeight - snakeImg.Height / 2 - 190 +
						12 * (float)Math.Sin (0.007 * startSnakeVector.X + 0.5);

					if (startSnakeVector.X > BackBufferWidth + snakeImg.Width)
						startSnakeVector.X = -snakeImg.Width;

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// ������ �������� (� ������ IsAlive)
					if (isAlive && isWorking)
						{
						playerPosition[0] += playerTo;

						for (int n = 1; n < playerPosition.Count; n++)
							{
							//////////////////////////////////////////////////////////////////////
							// ������� �������� �������� ������
							playerPosition[n] += speed * (playerPosition[n - 1] - playerPosition[n]) / Tile.Size;
							//////////////////////////////////////////////////////////////////////

							// ���� ����� ���� �������� ������� �� ������, ���
							// ������������ �� ���� ����� � ��������������� �����������
							if (IsCollapted (playerPosition[n], false))
								playerPosition[n] += new Vector2 (Math.Sign (playerPosition[n].X - collaptedOn.X),
									Math.Sign (playerPosition[n].Y - collaptedOn.Y));
							}
						}

					// ����� ������� � ������ (�������)
					if (currentScore == SMult * applesQuantity)
						{
						// ����
						MediaPlayer.Stop ();
						if (isSound)
							SCompleted.Play ();

						// �������� �����
						score += currentScore;
						currentScore = 0;

						// ����������� ���������
						showWinMsg = true;

						// ������ ������ ������ � ������
						isAlive = isWorking = false;

						// ���������� ������ ��������� �� ������� �������
						}

					// �������� ������������
					if (IsCollapted (playerPosition[0], true) && isAlive)
						{
						// ����
						MediaPlayer.Stop ();
						if (isSound)
							SFailed.Play ();

						// ������������ ��������� ����
						isAlive = isWorking = false;
						levelNumber--;
						headAnimator.PlayAnimation (headRushAnimation);

						// �������� �����
						score += currentScore / SMult - applesQuantity;
						currentScore = 0;

						// ����������� ���������
						showLoseMsg = true;

						// ���������� ������ ��������� �� ������� ������� Space
						}

					// ����� ������
					if (IsAte ())
						{
						// ����
						if (isSound)
							SAte[rnd.Next (SAte.Length)].Play ();

						// ��������� ������ ������ (���� ���� �� ��������)
						NewApple ();

						// ����������� �������� ����� ����� � ����� ��������� ����
						currentScore += SMult;
						ateApples++;

						// ���������� ����� ������
						playerPosition.Add (playerPosition[playerPosition.Count - 1]);
						}

					break;
					//////////////////////////////////////////////////////////////////
				}

			// ���������� ����
			base.Update (gameTime);
			}

		/// <summary>
		/// ��������� ������� ����������
		/// ��������������� �������
		/// </summary>
		private bool KeyboardProc ()
			{
			// ������ � ����������
			keyboardState = Keyboard.GetState ();

			// � ������������� �� ��������� ����
			// ��������� �����
			if (!showExitMsg)
				{
				if (keyboardState.IsKeyDown (Keys.S))       // Sound on/off
					{
					isSound = !isSound;
					SOnOff.Play ();

					// ���� ������ �������
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

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// ����������� �����
					if (keyboardState.IsKeyDown (Keys.Escape))
						this.Exit ();

					// �������
					if (keyboardState.IsKeyDown (Keys.F1))
						{
						gameStatus = GameStatus.Help;

						return true;
						}

					// ������� �����
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// ������������ ����������
						gameStatus = GameStatus.Playing;

						// �������� ������
						levelNumber--;
						LoadNextLevel ();

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					// �������
					if (keyboardState.IsKeyDown (Keys.Escape))
						{
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:

					// ������� ����� � �����������
					if (!showExitMsg)           // ������ ������ ������, ���� ��������� ��������� � ������
						{
						if (isAlive && keyboardState.IsKeyDown (Keys.Space))    // Pause
							{
							if (isWorking)
								{
								isWorking = false;

								if (isSound)
									SStop.Play ();
								}
							else                                                // Continue
								{
								showLevelMsg = false;
								isWorking = true;

								if (isSound)
									SStart.Play ();
								}

							return true;
							}

						// ������� ������� �����������
						if (keyboardState.IsKeyDown (Keys.Space) && !isWorking && !isAlive)
							{
							LoadNextLevel ();

							return true;
							}

						// �������� �� �����
						if (keyboardState.IsKeyDown (Keys.Escape))
							{
							// �����
							isWorking = false;

							// ���������
							showExitMsg = true;

							// ����
							if (isSound)
								SOnOff.Play ();

							return true;
							}
						}

					// ������� ������
					if (showExitMsg)
						{
						// ����� �� ���� (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							this.Exit ();

						// ����������� (back)
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
						// ������������
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

					//////////////////////////////////////////////////////////////////
				}

			// �� ���� �� ������ �������
			return false;
			}

		/// <summary>
		/// ��������� ������� ����������
		/// ���������������� �������
		/// </summary>
		private void KeyboardMoveProc ()
			{
			// ������ � ����������
			keyboardState = Keyboard.GetState ();

			// ������� ������ ����������
			if ((gameStatus == GameStatus.Playing) && !showExitMsg && isWorking)
				{
				if (keyboardState.IsKeyDown (Keys.Up) && !(playerTo.X == 0))
				// ������ ������� �������� �� ������ ������� ���� �� �����
				// � ������ ������� ������, ����������� �� ����������� �
				// ��������� ������
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
		// ���������� �������� ����

		/// <summary>
		/// ����� ������������ �������������� ���� ���� (����, �������, ���������)
		/// </summary>
		private void DrawInfo ()
			{
			string S1,
					S2 = String.Format (" � ���������: {0,4:D} ", currentScore),
					S3 = String.Format (" �������: {0,6:D} ", score),
					S4 = String.Format (" �������: {0,5:D} ", ateApples),
					S5 = String.Format (" �������� ������: {0,2:D} ", applesQuantity - currentScore / SMult);
			if (isWorking)
				S1 = String.Format (" ������� {0,2:D} ", levelNumber + 1);
			else
				S1 = " ����� ";

			float StrUp = Tile.Height * 0.15f,
				  StrDown = BackBufferHeight - Tile.Height * 0.8f;

			// ������� ������� ��� ����������� ��������� ��������� �������� ������ ����������
			Vector2 V1 = new Vector2 (BackBufferWidth * 0.05f, StrUp) + level.CameraPosition,
					V2 = new Vector2 (BackBufferWidth * 0.21f, StrUp) + level.CameraPosition,
					V3 = new Vector2 (BackBufferWidth * 0.50f, StrUp) + level.CameraPosition,
					V4 = new Vector2 (BackBufferWidth * 0.80f, StrUp) + level.CameraPosition,
					V5 = new Vector2 (BackBufferWidth * 0.05f, StrDown) + level.CameraPosition,
					V6 = new Vector2 (BackBufferWidth * 0.89f, StrDown) + level.CameraPosition,
					V7 = new Vector2 (BackBufferWidth * 0.92f, StrDown) + level.CameraPosition;

			DrawShadowedString (defFont, S1, V1, SnakeGameColors.LRed);
			DrawShadowedString (defFont, S2, V2, SnakeGameColors.Yellow);
			DrawShadowedString (defFont, S3, V3, SnakeGameColors.Green);
			DrawShadowedString (defFont, S4, V4, SnakeGameColors.LBlue);

			// ���� ���� ���, �������� ������ "�������� ������"
			if (isAlive)
				DrawShadowedString (defFont, S5, V5, SnakeGameColors.Silver);

			// ���� ���� ������ ��� ����, �������� ��������������� ����
			if (isMusic)
				DrawShadowedString (defFont, "[\x266B]", V6, SnakeGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266B]", V6, SnakeGameColors.Black);

			if (isSound)
				DrawShadowedString (defFont, "[\x266A]", V7, SnakeGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266A]", V7, SnakeGameColors.Black);

			// ������
			// ����� ����� ������� �������
			Color compasColor = SnakeGameColors.CompasRed;
			if (GameAuxFunctions.VDist (playerPosition[0], applePosition) < GameAuxFunctions.VDist (compasOffs, applePosition))
				compasColor = SnakeGameColors.CompasGreen;

			// ��������� ������� �������
			compasOffs = playerPosition[0];
			compasPosition.X = (int)playerPosition[0].X;
			compasPosition.Y = (int)playerPosition[0].Y;
			Vector2 V8 = applePosition - playerPosition[0];

			// ������� �������� ������� ������� � � �����������
			compasTurn = (float)Math.Acos (V8.X / GameAuxFunctions.VDist (V8, Vector2.Zero)) * GameAuxFunctions.NNSign (V8.Y, true);
			spriteBatch.Draw (compas, compasPosition, compasSize, compasColor, compasTurn,
				new Vector2 (compas.Width, compas.Height) / 2, SpriteEffects.None, 0.0f);
			}

		/// <summary>
		/// ����� ���������� ��������� �� ������
		/// </summary>
		private void ShowLevelMessage ()
			{
			string S1 = string.Format ("������� {0,2:D}", levelNumber + 1),
					S2 = string.Format ("���������� ������ {0,2:D} ��������", applesQuantity),
					S3 = "������� ������, ����� ������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 50) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 150) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, S1, V1, SnakeGameColors.LBlue);
			spriteBatch.DrawString (midFont, S2, V2, SnakeGameColors.Orange);
			spriteBatch.DrawString (defFont, S3, V3, SnakeGameColors.DBlue);
			}

		/// <summary>
		/// ����� ���������� ��������� � ������
		/// </summary>
		private void ShowWinMessage ()
			{
			string S1 = "������� �������!",
					S2 = "������� ������ ��� �����������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S2).X) / 2,
						(BackBufferHeight + 150) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, S1, V1, SnakeGameColors.Green);
			spriteBatch.DrawString (defFont, S2, V2, SnakeGameColors.DBlue);
			}

		/// <summary>
		/// ����� ���������� ��������� � ���������
		/// </summary>
		private void ShowLoseMessage ()
			{
			string S1 = "�������",
					S2 = "�� �������!",
					S3 = "������� ������, ����� ����������� �����";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 150) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 150) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, S1, V1, SnakeGameColors.Red);
			spriteBatch.DrawString (bigFont, S2, V2, SnakeGameColors.Red);
			spriteBatch.DrawString (defFont, S3, V3, SnakeGameColors.DBlue);
			}

		/// <summary>
		/// ����� ���������� ��������� � ������ ����
		/// </summary>
		private void ShowStartMessage ()
			{
			string S1 = ProgramDescription.AssemblyTitle,
					S2 = ProgramDescription.AssemblyCopyright,
					S3 = ProgramDescription.AssemblyLastUpdate,
					S4 = "������� ������, ����� ������ ����,\n" +
						 "      F1 ��� ������ �������,      \n" +
						 "        ��� Esc ��� ������        ";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 300) / 2),
					V2 = new Vector2 (BackBufferWidth - midFont.MeasureString (S3).X - 20,
						BackBufferHeight - 70),
					V3 = new Vector2 (BackBufferWidth - midFont.MeasureString (S3).X - 20,
						BackBufferHeight - 40),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						BackBufferHeight - 180);

			spriteBatch.Draw (startBack, Vector2.Zero, SnakeGameColors.White);
			spriteBatch.Draw (snakeImg, startSnakeVector, SnakeGameColors.White);
			spriteBatch.DrawString (bigFont, S1, V1, SnakeGameColors.Gold);
			spriteBatch.DrawString (midFont, S2, V2, SnakeGameColors.Silver);
			spriteBatch.DrawString (midFont, S3, V3, SnakeGameColors.Silver);
			spriteBatch.DrawString (defFont, S4, V4, SnakeGameColors.DBlue);
			}

		/// <summary>
		/// ����� ���������� ��������� �� ��������� ����
		/// </summary>
		private void ShowFinishMessage ()
			{
			string S1 = "�� ��������!!!",
					S2 = "���� ����������:",
					S3 = string.Format ("����� �����:    {0,6:D}\n����� �������:   {1,5:D}",
						score, ateApples),
					S4 = "������� ������ ��� �����������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 250) / 2),
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 100) / 2),
					V3 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S3).X) / 2,
						(BackBufferHeight) / 2),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						(BackBufferHeight + 200) / 2);

			spriteBatch.Draw (startBack, Vector2.Zero, SnakeGameColors.White);
			spriteBatch.DrawString (bigFont, S1, V1, SnakeGameColors.Gold);
			spriteBatch.DrawString (midFont, S2, V2, SnakeGameColors.Brown);
			spriteBatch.DrawString (midFont, S3, V3, SnakeGameColors.Brown);
			spriteBatch.DrawString (defFont, S4, V4, SnakeGameColors.DBlue);
			}

		/// <summary>
		/// ����� ���������� ������ �� ������������� ������ �� ����
		/// </summary>
		private void ShowExitMessage ()
			{
			string S1 = "�� ������������� ������",
					S2 = "��������� ����?",
					S3 = "������� Y, ����� ����� �� ����,",
					S4 = "��� N, ����� ���������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 230) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 150) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 100) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						(BackBufferHeight + 140) / 2) + level.CameraPosition;

			spriteBatch.DrawString (bigFont, S1, V1, SnakeGameColors.Yellow);
			spriteBatch.DrawString (bigFont, S2, V2, SnakeGameColors.Yellow);
			spriteBatch.DrawString (defFont, S3, V3, SnakeGameColors.DBlue);
			spriteBatch.DrawString (defFont, S4, V4, SnakeGameColors.DBlue);
			}

		/// <summary>
		/// ����� ���������� ������� �� ����
		/// </summary>
		private void ShowHelpMessage ()
			{
			string S1 = "������� ����",
					S2 = "   ������ ���������� ������ ��� ������� �� ������ ������. ����� ��������\n" +
						 "� �������� ������ ����� ����� � ������ ����� �������. ����� ������ ���� �\n" +
						 "������������ ����������� �����, �� ���������� �� ������������ �� ��������\n" +
						 "������ � � ����� �����. �� ������ ��������� ������ ����� 10 �����, ��\n" +
						 "������������ ���������� ���������� ����������� �������� ������. ������\n" +
						 "����, ����� ������ � ������ � ��� ������������� ������",
					S3 = "�����!!!",
					S4 = "����������",
					S5 = "������ - ����� / ������������� / ������ ����     S - ��������� / ���������� �����\n" +
						 "������� - ���������� �������                     M - ��������� / ���������� ������\n" +
						 "Esc - ����� �� ���� / �� �������";

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
			}

		/// <summary>
		/// ����� ������������ ������� ����
		/// </summary>
		/// <param name="VGameTime">����� ����</param>
		protected override void Draw (GameTime VGameTime)
			{
			// �������� ������� ���� � ������ ���������
			graphics.GraphicsDevice.Clear (SnakeGameColors.DGreen);
			spriteBatch.Begin ();

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					ShowStartMessage ();

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					ShowHelpMessage ();

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// ����������� ������ 
					level.Draw (VGameTime, spriteBatch, playerPosition[0]);

					// ����������� ������������� �����������
					// ������
					appleAnimator.Draw (VGameTime, spriteBatch, applePosition, SpriteEffects.None, SnakeGameColors.White, 0.0);

					// ���� ������
					for (int n = 1; n < playerPosition.Count; n++)
						bodyAnimator.Draw (VGameTime, spriteBatch, playerPosition[n], SpriteEffects.None, SnakeGameColors.White, 0.0);

					// ������ ������
					headAnimator.Draw (VGameTime, spriteBatch, playerPosition[0], SpriteEffects.None, SnakeGameColors.White,
						// ��������� ���� �������� ��������
						Math.Acos (Math.Sign (playerTo.X)) * GameAuxFunctions.NNSign (playerTo.Y, true));


					// ����������� ����������
					DrawInfo ();

					// ����������� ���������
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

			// ���������� ���������� ���������
			spriteBatch.End ();

			// �����������
			base.Draw (VGameTime);
			}

		/// <summary>
		/// �������� ���������� ������ ����
		/// </summary>
		private void LoadNextLevel ()
			{
			// ������ ������� �������
			MediaPlayer.Stop ();
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));

			// ����� ��������� ��������� ������
			while (true)
				{
				// ����� � ������������� �� ��������� �������
				++levelNumber;
				if (levelNumber < LevelData.LevelsQuantity)
					break;

				// ���������� � �������� ������ � ����� ����
				levelNumber = -1;
				gameStatus = GameStatus.Finish;
				if (isMusic)
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// �������� ����������� ������ � �������� ������
			if (level != null)
				level.Dispose ();
			level = new SnakeLevel (Services, levelNumber);

			// ��������� ��������� ����
			// ������ ���������� ������
			applesQuantity = level.ApplesQuantity;
			speed = level.Speed;
			playerTo = level.PlayerTo * speed;

			// ��������� ������ ������
			NewApple ();

			// ��������� ����������
			headAnimator.PlayAnimation (headAnimation);
			isAlive = true;

			// ����� ���������
			showWinMsg = showLoseMsg = false;
			showLevelMsg = true;

			// ������������ ���� ������
			// �������
			playerPosition.Clear ();

			// ������
			Vector2 pp2 = Tile.Size / 2 + Tile.Size * level.PlayerStartPosition;
			playerPosition.Add (pp2);

			// ������ ����� ���� (������������� �� �������)
			pp2.X -= Tile.Width * Math.Sign (playerTo.X);
			pp2.Y -= Tile.Height * Math.Sign (playerTo.Y);
			playerPosition.Add (pp2);

			// ������ �������� � ����������� ���� (� ����������� �� ����, ���� ��� ��� ���)
			GameSettings (true);
			}

		/////////////////////////////////////////////////////////////////////////////////
		// �������� ������������

		/// <summary>
		/// ����� ��������� ������������ ������ ����� � ����� ������ ������ ��� ���� ������
		/// </summary>
		/// <param name="Collaptor">����������� �����</param>
		/// <param name="WithBody">�������� ������������ � �����</param>
		private bool IsCollapted (Vector2 Collaptor, bool WithBody)
			{
			// ����� �� ����������� � ������ ����������� ��� ����� ������,
			// ����� ����� ��������� ������ �� ������ ����, ������� ���������
			// �� ����������� ���������� �� ������. ����� ������ ���� �����
			// ������: �� StoneOffs �����, �����, ������ � ����. �� �����������
			// ����� �������� ����� ��������� ���������� ������ ������� ���
			// ��������� ������� � ��������� � collision
			Vector2[] V1 = new Vector2[]    {
					new Vector2(Collaptor.X / Tile.Width + StoneOffs, Collaptor.Y / Tile.Height + StoneOffs),
					new Vector2(Collaptor.X / Tile.Width + StoneOffs, Collaptor.Y / Tile.Height - StoneOffs),
					new Vector2(Collaptor.X / Tile.Width - StoneOffs, Collaptor.Y / Tile.Height + StoneOffs),
					new Vector2(Collaptor.X / Tile.Width - StoneOffs, Collaptor.Y / Tile.Height - StoneOffs)
											};

			// �������� �� ������������ �� �������
			for (int i = 0; i < V1.Length; i++)
				// ����������� ����������� ������� (���������� ����� ����������
				// ��� ��������� ������ ������)
				if (level.Tiles[(int)V1[i].X % (int)level.LevelSize.X,
								(int)V1[i].Y % (int)level.LevelSize.Y].Collision == TileCollision.Stone)
					{
					collaptedOn.X = (int)V1[i].X * Tile.Width + Tile.Width / 2;
					collaptedOn.Y = (int)V1[i].Y * Tile.Height + Tile.Width / 2;
					return true;
					}

			// �������� �� ������������ � ����� (���� ���������)
			if (WithBody)
				for (int i = 1; i < playerPosition.Count; i++)
					if (GameAuxFunctions.VDist (Collaptor, playerPosition[i]) < BodyOffs)
						return true;

			// �� ���� ������������
			return false;
			}

		/// <summary>
		/// ����� ��������� ������������ � �������
		/// </summary>
		/// <returns></returns>
		private bool IsAte ()
			{
			if (GameAuxFunctions.VDist (applePosition, playerPosition[0]) < AppleOffs)
				return true;

			return false;
			}

		/// <summary>
		/// ����� ���������� ����� ������
		/// </summary>
		private void NewApple ()
			{
			Vector2 NewV;

			// ��������� ����� �������, �� ���������� �� ����� � ������
			do
				{
				NewV.X = rnd.Next ((int)(level.LevelSize.X * Tile.Width));
				NewV.Y = rnd.Next ((int)(level.LevelSize.Y * Tile.Height));
				} while (IsCollapted (NewV, true));

			applePosition = NewV;

			// ����� ����� ��������
			appleAnimator.PlayAnimation (appleAnimation[rnd.Next (appleAnimation.Length)]);
			}

		/// <summary>
		/// ����� ������������ ��������� ������
		/// </summary>
		/// <param name="VFont"></param>
		/// <param name="VString"></param>
		/// <param name="VPosition"></param>
		/// <param name="VColor"></param>
		private void DrawShadowedString (SpriteFont VFont, string VString, Vector2 VPosition, Color VColor)
			{
			// ������ ��������� ������ ������, ��� � ������ ���� ������ ���
			// �������� �� �������������. � ���������� �� ��� �����������
			// ���������� �� default, ������� � ��� �� ������ ��������� ���
			// ������-�������������
			string SubStr = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";

			spriteBatch.DrawString (VFont, SubStr.Remove (VString.Length), VPosition, SnakeGameColors.DGreen);
			spriteBatch.DrawString (VFont, VString, VPosition, VColor);
			}

		/// <summary>
		/// ����� ��������� / ��������� ��������� ����
		/// </summary>
		/// <param name="Write">���� ��������� �� ����� ������</param>
		private void GameSettings (bool Write)
			{
			string FN = "C:\\Docume~1\\Alluse~1\\Applic~1\\Microsoft\\Windows\\SnakeGame.sav";
			string S = FN.Substring (0, FN.Length - 14);

			if (Write)
				{
				Directory.CreateDirectory (FN.Substring (0, FN.Length - 14));
				StreamWriter FL = new StreamWriter (FN, false);

				FL.Write ("{0:D}\n{1:D}\n{2:D}\n{3:D}\n{4:D}",
					levelNumber, score, ateApples, isMusic, isSound);

				FL.Close ();
				}
			else if (File.Exists (FN))
				{
				StreamReader FL = new StreamReader (FN);

				levelNumber = int.Parse (FL.ReadLine ());
				score = int.Parse (FL.ReadLine ());
				ateApples = int.Parse (FL.ReadLine ());
				isMusic = bool.Parse (FL.ReadLine ());
				isSound = bool.Parse (FL.ReadLine ());

				FL.Close ();
				}
			}
		}
	}
