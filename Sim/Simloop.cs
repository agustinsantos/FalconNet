using System;
using FalconNet.Common;

namespace FalconNet.Sim
{
	/***************************************************************************\
		This class is the master loop for the simulation and graphics.
		It starts and stops each as appropriate during transitions between
		the SIM and UI.
	\***************************************************************************/
	public class SimulationLoopControl
	{

		public static void StartSim ()
		{
			throw new NotImplementedException ();
		}

		public static void StopSim ()
		{
			throw new NotImplementedException ();
		}
	
		public static void StartGraphics ()
		{
			throw new NotImplementedException ();
		}

		public static void StopGraphics ()
		{
			throw new NotImplementedException ();
		}
	
		public static void Loop ()
		{
			throw new NotImplementedException ();
		}

		public static void StartLoop ()
		{
			throw new NotImplementedException ();
		}
	
		public static bool  InSim ()
		{
			return currentMode == SimLoopControlMode.RunningGraphics;
		}

		public static int  GetSimTick ()
		{
			return sim_tick;
		}
	
		public static HANDLE wait_for_sim_cleanup;
		public static HANDLE wait_for_graphics_cleanup;
	
	  
		protected enum SimLoopControlMode
		{ 
			Stopped,
			StartingSim, 
			RunningSim, 
			StartingGraphics, 
			Step2, 
			StartRunningGraphics, 
			RunningGraphics, 
			StoppingGraphics,
			Step5,
			StoppingSim,
		};
		
		protected static SimLoopControlMode currentMode;
		protected static HANDLE wait_for_start_graphics;
		protected static HANDLE wait_for_stop_graphics;
		protected static int sim_tick;
	}
}

