using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Stone.Util
{
	public static class Debuger
	{
		public static void LogAtFrame(string infor)
		{
			Debug.Log("["+Time.frameCount+"]"+infor);
		}
	}
}