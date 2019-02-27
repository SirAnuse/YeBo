/*
 * Created by SharpDevelop.
 * User: DEV0003
 * Date: 27/02/2019
 * Time: 12:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace YeBo
{
	public class GameLoop : Loop
	{
		public GameLoop() : base("GameLoop")
		{
		}
		
		protected override void OnStart()
		{
			throw new NotImplementedException();
		}
		
		protected override void OnStop()
		{
			throw new NotImplementedException();
		}
		
		protected override void OnTick()
		{
			Log.Debug("got tick");
		}
		
		protected override void OnInitialize()
		{
		}
	}
}
