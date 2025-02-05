using System;
using UnityEngine;

namespace HumanFactory.Util
{
	public static class DebugServer
	{
		public static void Log(string message)
		{
			Debug.Log(message);
			Console.WriteLine(message);
		}
	}
}