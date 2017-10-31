using System;

namespace Stone.Comp
{
	public class Res
	{
		public Res ()
		{
		}

		static public string ConfExt = ".xml";
		static public string MapPath = "Assets/Configs/maps/";
		static public string ZonePath = "Assets/Configs/zones/";
		static public string GroundPath = "Assets/Prefabs/dizhuan/";

		static public string GetZonePath(string fileName)
		{
			return ZonePath + fileName + ConfExt;
		}
	}
}

