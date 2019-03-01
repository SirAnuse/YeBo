using System;

namespace YeBo
{
	public class GameLoop : Loop
	{
        public bool Reading = false;
        public bool Processing = false;
		public GameLoop() : base("GameLoop")
		{
		}
		
		protected override void OnStart()
		{
            active = true;
            Start();
		}
		
		protected override void OnStop()
		{
        }
		
		protected override void OnTick()
		{
            try
            {
                Log.TickLogs();
                if (Main.QueuedCommands.Count <= 0)
                    return;
                var cmd = string.Empty;
                if (!Main.QueuedCommands.TryDequeue(out cmd))
                    return;
                CommandHandler.Instance.ProcessCommand(cmd);
            }
            catch
            {

            }
		}
		
		protected override void OnInitialize()
		{
		}
	}
}
