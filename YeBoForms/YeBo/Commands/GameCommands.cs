using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YeBo.Commands
{
	public static class StringArrayExtensions
	{
		public static string ListItems(this string[] array)
		{
			var sb = new StringBuilder(string.Empty);
            for (var i = 0; i < array.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append(array[i]);
            }
            return sb.ToString();
		}
	}
	
	internal class TestCommand : Command
	{
		public TestCommand() : base("test", "yote", "fote", "vote")
		{
			
		}
		
		protected override bool Process(string[] args)
		{
			CommandHandler.Log.Debug("TestCommand invoked with args: " + args.ListItems());
			return true;
		}
	}
	
	internal class SettingsCommand : Command
	{
		public SettingsCommand() : base("settings", "options")
		{
			
		}
		
		protected override bool Process(string[] args)
		{
			if (args[0] == null || args[1] == null)
			{
				CommandHandler.Log.Error("Not enough arguments!");
				return false;
			}
			var setting = args[0];
			var change = args[1];
			switch (setting)
			{
				case "font":
					var size = Convert.ToInt32(change);
					var font = Log.OutputBox.Font;
					Log.OutputForm.Invoke(new MethodInvoker(delegate {
					                                        	Log.OutputBox.Font = new System.Drawing.Font(font.FontFamily.Name, size);
					                                        }));
					CommandHandler.Log.Info("Font size changed to " + change + ".");
					break;
			}
			return true;
		}
	}
}
