using System;

namespace FalconNet.Sim
{
	/***************************************************************************\
		This class is the master loop for the simulation and graphics.
		It starts and stops each as appropriate during transitions between
		the SIM and UI.
	\***************************************************************************/
	public class SimulationLoopControl {
		public static void StartSim( );
		public static void StopSim( );
	
		public static void StartGraphics( );
		public static void StopGraphics( );
	
		public static void Loop( );
		public static void StartLoop( );
	
		public static int  InSim( )			{ return currentMode == RunningGraphics; }
		public static int  GetSimTick( )	{ return sim_tick; }
	
		public static HANDLE wait_for_sim_cleanup;
		public static HANDLE wait_for_graphics_cleanup;
	
	  
		protected enum SimLoopControlMode { 
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
	
		protected static HANDLE wait_for_start_graphics;
		protected static HANDLE wait_for_stop_graphics;
		protected static int sim_tick;
	}
}

