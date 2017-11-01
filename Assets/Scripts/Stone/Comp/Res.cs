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
		static public string ObstaclePath = "Assets/Prefabs/obstacle/obstacle_1.prefab";

		static public string GetZonePath(string fileName)
		{
			return ZonePath + fileName + ConfExt;
		}

		static public string GetMapPath(string name)
		{
			return MapPath + name + "/";
		}

		static public string GetMapConfPath(string path)
		{
			return path + "map" + ConfExt;
		}

		static public string GetMapZonePath(string path, string name)
		{
			return path + name + ConfExt;
		}
	}
}

