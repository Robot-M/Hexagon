using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Stone.Comp;

namespace Stone.Hex  
{
	public class Map
	{
		public int radius = 3;

		public Hex curHex;

		public Dictionary<string, string> zoneNames;

		[XmlIgnore]
		public string path;

		private Dictionary<string, Zone> _zones;
		private List<Hex> _spiralHexs;

		public Map()
		{
			this.radius = 3;
			this.zoneNames = new Dictionary<string, string> ();
			this._zones = new Dictionary<string, Zone> ();
		}

		public Map(int radius)
		{
			this.radius = radius;
			this.zoneNames = new Dictionary<string, string> ();
			this._zones = new Dictionary<string, Zone> ();
		}

		public Map (int radius, Dictionary<string, string> zoneNames)
		{
			this.radius = radius;
			this.zoneNames = zoneNames;
			this._zones = new Dictionary<string, Zone> ();
		}

		public static Map LoadMapFromXml(string path)
		{
			Map map = null;
			string mapName = Res.GetMapConfPath (path);
			if (FileUtilEx.HasFile (mapName)) {
				map.path = path;
				string dataStr = XmlUtil.LoadXml (mapName);
				map = XmlUtil.DeserializeObject (dataStr, typeof(Map)) as Map;
				
				if (map.GetCurZone () != null) {
					for (int i = 0; i < 6; i++) {
						Hex nearHex = Hex.Neighbor (map.curHex, i);
						map.GetZone (nearHex);
					}
				}

			} else {
				Debug.Log ("LoadMapFromXml can't find");
			}
			return map;
		}

		public static void SaveMapToXml(string path, Map map)
		{
			string mapName = Res.GetMapConfPath (path);
			string dataStr = XmlUtil.SerializeObject (map, typeof(Map));
			XmlUtil.CreateXml (mapName, dataStr);
		}
		
		public bool IsEmpty()
		{
			return zoneNames.Count == 0;
		}

		/// <summary>
		/// 按螺旋方向获取空缺的位置
		/// </summary>
		/// <returns><c>true</c>, if next hex was gotten, <c>false</c> otherwise.</returns>
		/// <param name="result">Result.</param>
		public bool GetNextHex(out Hex result)
		{
			if (_spiralHexs == null) {
				_spiralHexs = Hex.Spiral (new Hex(), 3);
			}

			for (int i = 0; i < _spiralHexs.Count; i++) {
				if (!HasZoneName (_spiralHexs [i])) {
					result = _spiralHexs [i];
					return true;
				}
			}
			result = new Hex ();
			return false;
		}

		public bool HasZoneName(Hex hex)
		{
			string key = GetZoneKey (hex);
			return zoneNames.ContainsKey (key);
		}

		public string GetZoneKey(Hex hex)
		{
			return "zone_" + hex.q + "_" + hex.r;
		}

		public Zone GetCurZone()
		{
			return GetZone (curHex);
		}

		public Zone GetZone(Hex hex)
		{
			Zone zone = null;
			string key = GetZoneKey (hex);
			if (_zones.ContainsKey (key)) {
				zone = _zones [key];
			} else if (zoneNames.ContainsKey (key)) {
				zone = Zone.LoadZoneFromXml (Res.GetMapZonePath (path, key));
				_zones.Add (key, zone);
			}
			return zone;
		}

		public List<Zone> GetShowZones()
		{
			List<Zone> zones = new List<Zone> ();
			Zone zone = GetZone (curHex);
			if (zone != null) {
				zones.Add (zone);
				for (int i = 0; i < 6; i++) {
					Hex nearHex = Hex.Neighbor (curHex, i);
					zone = GetZone (nearHex);
					if (zone != null) {
						zones.Add (zone);
					}
				}
			}
			return zones;
		}

		public void AddZone(Hex hex, Zone zone)
		{
			string key = GetZoneKey (hex);
			zone.hex = hex;
			for (int i = 0; i < 6; i++) {
				Hex direct = Hex.Direction (i);
				Hex nearHex = Hex.Add (hex, direct);
				Zone nearZone = GetZone(nearHex);
				if(nearZone != null){
					Hex diagonal = Hex.Diagonal (i);
					zone.centerHex = nearZone.centerHex + diagonal * radius + direct;
					break;
				}
			}

			_zones.Add (key, zone);
			if (!zoneNames.ContainsKey (key)) {
				zoneNames.Add (key, key);
			}
		}
	}
}

