using System;

namespace Stone.Core
{
	public class Res
	{
		public Res ()
		{
		}

		static public string ConfExt = ".xml";
		static public string MapPath = "Assets/Configs/maps/";
		static public string ZonePath = "Assets/Configs/zones/";
		static public string PiecePath = "Assets/Configs/pieces/";
		static public string PlayerPath = "Assets/Configs/players/";
		static public string GroundPath = "Assets/Prefabs/dizhuan/";
		static public string ObstaclePath = "Assets/Prefabs/obstacle/obstacle_1.prefab";

		static public string GetPiecePath(string fileName)
		{
			return PiecePath + fileName + ConfExt;
		}

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

		static public string GetPlayerPath(int uid)
		{
			return PlayerPath + "player_" + uid + ConfExt;
		}
	}
}

