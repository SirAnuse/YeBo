using System;
using System.Diagnostics;
using System.Threading;

namespace YeBo
{
    /* Function documentation:
     * - Constructor (Inputs: none): Instantiate a logger, set the start time, and set Initialized to true.
     * - Initialize (Inputs: none): Instantiate a new Loop instance, and start it. Sets Instance to the new one.
     */

    public class Loop
	{
		public static Loop Instance;
		public static bool Initialized;
        public Log Log;

        private Thread loopThread;
        private int msRest;
        private DateTime start;

        private bool active;

        public Loop()
		{
			Log = new Log(GetType().Name);
			start = DateTime.Now;
			Initialized = true;
            loopThread = new Thread(Tick) { Name = "LoopThread" };
            msRest = 1000 / Config.Settings.TPS;
		}
		
		public static void Initialize()
		{
			Instance = new Loop();
            Instance.StartLoop();
        }

        public void Tick()
        {
            try
            {
                while (active)
                {
                    Log.TickLogs();
                }
                Thread.Sleep(msRest);
            }
            catch
            {

            }
        }
		
		public void StartLoop()
		{
            active = true;
            loopThread.Start();
		}
	}
}
