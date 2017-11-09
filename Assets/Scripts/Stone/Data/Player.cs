using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Stone.Core;

namespace Stone.Core
{
	public class Player:Role
	{
		public Player ()
		{
		}

		public Hex centerHex;

		[XmlIgnore]
		public bool isDirty = false;

		public static Player CreatePlayer(string name, int uid)
		{
			Player player = new Player ();
			player.uid = uid;
			player.name = name;
			player.version = 1;
			player.level = 1;
			player.exp = 0;
			player.hp = 100;
			player.hpmax = 100;
			player.atk = 10;
			player.atkRange = 1;

			SavePlayerToXml (player);
			return player;
		}

		public static Player LoadPlayerFromXml(string path)
		{
			Player player = null;
			if (FileUtilEx.HasFile (path)) {
				string dataStr = XmlUtil.LoadXml (path);
				player = XmlUtil.DeserializeObject (dataStr, typeof(Player)) as Player;
			} else {
				Debug.Log ("LoadPlayerFromXml can't find");
			}
			return player;
		}

		public static void SavePlayerToXml(Player player)
		{
			Debug.Log ("SavePlayerToXml player " + player.name);
			if (player.isDirty) {
				string path = Res.GetPlayerPath (player.uid);
				string dataStr = XmlUtil.SerializeObject (player, typeof(Player));
				XmlUtil.CreateXml (path, dataStr);
			} else {
				Debug.Log ("SavePlayerToXml player has nothing to save");
			}
		}
	}
}

