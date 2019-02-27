using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace YeBo
{
    /* Function documentation:
     * - Constructor (Inputs: none): Instantiate a logger, set the start time, and set Initialized to true.
     * - Initialize (Inputs: none): Instantiate a new Loop instance, and start it. Sets Instance to the new one.
     */
	
    public class MasterLoop : Loop
    {
    	public MasterLoop() : base("MasterLoop")
    	{
    		
    	}
    	
		protected override void OnStart()
		{
		}
		
		protected override void OnStop()
		{

		}
		
		protected override void OnTick()
		{
			try
			{
				while (active)
				{
					foreach (var i in Loops)
					{
						if (this == i)
							continue;
						
						i.Tick();
					}
					Thread.Sleep(restMs);
				}
			}
			catch
			{
				
			}
		}
		
		protected override void OnInitialize()
		{
			Start();
		}
    }
    
    public abstract class Loop
	{
    	public static List<Loop> Loops;
    	
    	public static Loop Instance;
    	
        protected readonly Log Log;
        protected readonly string name;
        protected readonly int restMs;
        protected readonly Thread loopThread;
        protected readonly DateTime loopStart;
        
        public bool active;

        public Loop(string name)
		{
			Log = new Log(name);
			loopStart = DateTime.Now;
            loopThread = new Thread(Tick) { Name = name };
            this.name = name;
            restMs = 1000 / ValidateTPS();
            Log.Debug("Adding loop: " + name + ".");
            Loops.Add(this);
		}
        
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnTick();
        protected abstract void OnInitialize();
        
        public int ValidateTPS()
        {
        	var baseTps = Config.Settings.TPS;
        	if (baseTps > 1000 || baseTps < 0)
        	{
        		Log.Warning("TPS was invalid (over 1000 or under 0). Defaulting to 5.");
        		return 5;
        	}
        	else
        		return baseTps;
        }
        
        public void Start()
		{
        	OnStart();
		}
        
		public void Stop()
		{
			OnStop();
			active = false;
			loopThread.Abort();
			Log.Info("Loop has been stopped.");
			
		}

        public void Tick()
        {
        	OnTick(); 
        }
        
        public void Initialize()
        {
        	OnInitialize();
        }
        
        public static void Setup()
        {
        	Loops = new List<Loop>();
        }
        
        public static void TickLoops()
        {
        	foreach (var i in Loops)
        		i.Tick();
        }
        
        public static void InitializeLoops()
        {
        	foreach (var i in Loops)
        		i.Initialize();
        }
		
		
	}
}
