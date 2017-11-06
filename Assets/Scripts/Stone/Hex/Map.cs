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

		public SerializableDictionary<string, string> zoneNameDict;

		[XmlIgnore]
		public string path;

		private Dictionary<string, Zone> _zoneDict;

		[XmlIgnore]
		public Dictionary<string, Zone> zoneDict { 
			get { return _zoneDict; } 
		}

		private List<Hex> _spiralHexs;

		public Map()
		{
			this.radius = 3;
			this.zoneNameDict = new SerializableDictionary<string, string> ();
			this._zoneDict = new Dictionary<string, Zone> ();
		}

		public Map(int radius)
		{
			this.radius = radius;
			this.zoneNameDict = new SerializableDictionary<string, string> ();
			this._zoneDict = new Dictionary<string, Zone> ();
		}

		public Map (int radius, SerializableDictionary<string, string> zoneNameDict)
		{
			this.radius = radius;
			this.zoneNameDict = zoneNameDict;
			this._zoneDict = new Dictionary<string, Zone> ();
		}

		public static Map LoadMapFromXml(string fileName)
		{
			Map map = null;
			string path = Res.GetMapPath (fileName);
			if (FileUtilEx.HasDirectory (path)) {
				string mapName = Res.GetMapConfPath (path);
				Debug.Log ("LoadMapFromXml mapName " + mapName);
				string dataStr = XmlUtil.LoadXml (mapName);
				map = XmlUtil.DeserializeObject (dataStr, typeof(Map)) as Map;
				map.path = path;
				
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

		public static void SaveMapToXml(string fileName, Map map)
		{
			string path = Res.GetMapPath (fileName);
			FileUtilEx.CreateDirectory (path);

			string mapName = Res.GetMapConfPath (path);
			string dataStr = XmlUtil.SerializeObject (map, typeof(Map));
			XmlUtil.CreateXml (mapName, dataStr);

			foreach (KeyValuePair<string, Zone> kv in map.zoneDict) {
				if (kv.Value.isDirty) {
					string zonePath = Res.GetMapZonePath(path, kv.Key);
					Zone.SaveZoneToXml(zonePath, kv.Value);
				}
			}
		}
		
		public bool IsEmpty()
		{
			return zoneNameDict.Count == 0;
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
			return zoneNameDict.ContainsKey (key);
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
			if (_zoneDict.ContainsKey (key)) {
				zone = _zoneDict [key];
			} else if (zoneNameDict.ContainsKey (key)) {
				zone = Zone.LoadZoneFromXml (Res.GetMapZonePath (path, key));
				_zoneDict.Add (key, zone);
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

			_zoneDict.Add (key, zone);
			if (!zoneNameDict.ContainsKey (key)) {
				zoneNameDict.Add (key, key);
			}
		}
	}
}

