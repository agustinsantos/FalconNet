using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Gwen.Control;
using Gwen;
using FalconNet.Ui95;
using GwenUI;

using FalconNet.Common;

namespace FalconNet.Main
{
	public class MainGwenGui: GameWindow
	{
		private Gwen.Input.OpenTK input;
		private Gwen.Renderer.OpenTK renderer;
		private Gwen.Skin.Base skin;
		private Gwen.Control.Canvas canvas;
		const int fps_frames = 50;
		private readonly List<long> ftime;
		private readonly Stopwatch stopwatch;
		private long lastTime;
		private bool altDown = false;
		private GuiConfiguration conf;
		public static string FalconDirectory = @"../../../data/";
		private readonly string skinsPath = FalconDirectory + "skins/";
		
		public MainGwenGui (GuiConfiguration guiConf)
            : base(1024, 768)
		{
			Keyboard.KeyDown += Keyboard_KeyDown;
			Keyboard.KeyUp += Keyboard_KeyUp;

			Mouse.ButtonDown += Mouse_ButtonDown;
			Mouse.ButtonUp += Mouse_ButtonUp;
			Mouse.Move += Mouse_Move;
			Mouse.WheelChanged += Mouse_Wheel;

			ftime = new List<long> (fps_frames);
			stopwatch = new Stopwatch ();
			
			conf = guiConf;
		}

		public override void Dispose ()
		{
			canvas.Dispose ();
			skin.Dispose ();
			renderer.Dispose ();
			base.Dispose ();
		}

		/// <summary>
		/// Occurs when a key is pressed.
		/// </summary>
		/// <param name="sender">The KeyboardDevice which generated this event.</param>
		/// <param name="e">The key that was pressed.</param>
		void Keyboard_KeyDown (object sender, KeyboardKeyEventArgs e)
		{
			if (e.Key == global::OpenTK.Input.Key.Escape)
				Exit ();
			else if (e.Key == global::OpenTK.Input.Key.AltLeft)
				altDown = true;
			else if (altDown && e.Key == global::OpenTK.Input.Key.Enter)
			if (WindowState == WindowState.Fullscreen)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Fullscreen;

			input.ProcessKeyDown (e);
		}

		void Keyboard_KeyUp (object sender, KeyboardKeyEventArgs e)
		{
			altDown = false;
			input.ProcessKeyUp (e);
		}

		void Mouse_ButtonDown (object sender, MouseButtonEventArgs args)
		{
			input.ProcessMouseMessage (args);
		}

		void Mouse_ButtonUp (object sender, MouseButtonEventArgs args)
		{
			input.ProcessMouseMessage (args);
		}

		void Mouse_Move (object sender, MouseMoveEventArgs args)
		{
			input.ProcessMouseMessage (args);
		}

		void Mouse_Wheel (object sender, MouseWheelEventArgs args)
		{
			input.ProcessMouseMessage (args);
		}

		/// <summary>
		/// Setup OpenGL and load resources here.
		/// </summary>
		/// <param name="e">Not used.</param>
		protected override void OnLoad (EventArgs e)
		{
			GL.ClearColor (Color.MidnightBlue);

			renderer = new Gwen.Renderer.OpenTK ();
			skin = new Gwen.Skin.TexturedBase (renderer, skinsPath+"DefaultSkin.png");
			canvas = new Canvas (skin);

			input = new Gwen.Input.OpenTK (this);
			input.Initialize (canvas);

			canvas.SetSize (Width, Height);
			canvas.ShouldDrawBackground = true;
			canvas.BackgroundColor = Color.FromArgb (255, 150, 170, 170);
			//canvas.KeyboardInputEnabled = true;
			
			ProcessConfiguration ();
			
			stopwatch.Restart ();
			lastTime = 0;
		}

		/// <summary>
		/// Respond to resize events here.
		/// </summary>
		/// <param name="e">Contains information on the new GameWindow size.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnResize (EventArgs e)
		{
			GL.Viewport (0, 0, Width, Height);
			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadIdentity ();
			GL.Ortho (0, Width, Height, 0, -1, 1);

			canvas.SetSize (Width, Height);
		}

		/// <summary>
		/// Add your game logic here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			if (ftime.Count == fps_frames)
				ftime.RemoveAt (0);

			ftime.Add (stopwatch.ElapsedMilliseconds - lastTime);
			lastTime = stopwatch.ElapsedMilliseconds;

			if (stopwatch.ElapsedMilliseconds > 1000) {
				//test.Note = String.Format ("String Cache size: {0} Draw Calls: {1} Vertex Count: {2}", renderer.TextCacheSize, renderer.DrawCallCount, renderer.VertexCount);
				//test.Fps = 1000f * ftime.Count / ftime.Sum ();
				stopwatch.Restart ();

				if (renderer.TextCacheSize > 1000) // each cached string is an allocated texture, flush the cache once in a while in your real project
					renderer.FlushTextCache ();
			}
		}

		/// <summary>
		/// Add your game rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnRenderFrame (FrameEventArgs e)
		{
			GL.Clear (ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			canvas.RenderCanvas ();

			SwapBuffers ();
		}
		
		List<GuiWindow> wins = new List<GuiWindow> ();

		private void ProcessConfiguration ()
		{
			if (conf == null)
				return;
			foreach (ScfNode node in conf.tableWins.Values) {
				Debug.WriteLine ("GUI Node: " + node.winType);
				switch (node.winType) {
				case "[WINDOW]":
					GuiWindow win = new GuiWindow (canvas);
					WindowParser.ProcessConf (node, win);
					wins.Add (win);
					break;
				case "[BUTTON]":
				case "[TEXT]":
				case "[EDITBOX]":
				case "[LISTBOX]":
				case "[SCROLLBAR]":
				case "[TREELIST]":
				case "[MAKEFONT]":
				case "[IMAGE]":
				case "[BITMAP]":
				case "[LINE]":
				case "[BOX]":
				case "[MARQUE]":
				case "[ANIMATION]":
				case "[LOCATOR]":
				case "[SOUND]":
				case "[SLIDER]":
				case "[POPUPMENU]":
				case "[PANNER]":
				case "[ANIM]":
				case "[STRINGLIST]":
				case "[MOVIE]":
				case "[FILL]":
				case "[CLOCK]":
				case "[TILE]":
					throw new NotImplementedException();
					break;
				default:
					Debug.WriteLine ("Unknown GUI type: " + node.winType);
					break;
				}
				
			}
		}
		
	}
		
	public class WindowParser
	{
		public static void ProcessConf (ScfNode node, GuiWindow win)
		{
			foreach (string prop in node.properties) {
				Debug.WriteLine ("GUI property: " + prop);
				List<string> tokens = prop.SplitWords ();
				switch (tokens [0]) {
				case "[SETUP]":
					Debug.WriteLine ("Win setup found");
					node.id = tokens [1];
					win.SetSize (int.Parse (tokens [3]), int.Parse (tokens [4]));
					break;
				case "[X]":
					win.X = int.Parse (tokens [1]);
					break;
				case "[Y]":
					win.Y = int.Parse (tokens [1]);
					break;
				case "[W]":
					win.Width = int.Parse (tokens [1]);
					break;
				case "[H]":
					win.Height = int.Parse (tokens [1]);
					break;
				case "[XY]":
					win.Position (Pos.Top | Pos.Left, int.Parse (tokens [1]), int.Parse (tokens [2]));
					break;
				case "[RANGES]":
					//Point pt1 = new Point (int.Parse (tokens [1]), int.Parse (tokens [2]));
					//Point pt2 = new Point (int.Parse (tokens [3]), int.Parse (tokens [4]));
					Point sz = new Point (int.Parse (tokens [5]), int.Parse (tokens [6]));
					win.MinimumSize = sz;
					// Min Pos = pt1
					// Max Pos = pt2
					break;
				case "[CLIENTAREA]":
				case "[FONT]":
				case "[GROUP]":
				case "[FLAGBITON]":
				case "[FLAGBITOFF]":
				case "[DEPTH]":
				case "[OPENMENU]":
				case "[OPENCLIENTMENU]":
				case "[CURSOR]":	
				case "[DRAGH]":
				case "[CLIENTFLAG]":	
					Debug.WriteLine ("GUI parameter not applyed to Gwen: " + prop);
					break;
				default:
					Debug.WriteLine ("Unknown GUI parameter: " + tokens [0]);
					break;
				}
					
			}
		}
	}
	
}