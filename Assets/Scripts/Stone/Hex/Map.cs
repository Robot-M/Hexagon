using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Stone.Core  
{
	public class Map
	{
		public int radius = 3;

		public Layout layout;

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

		private Dictionary<Hex, Zone> _showZoneDict;
		private Dictionary<Hex, Cell> _showCellDict;

		public Map()
		{
			init (3, new Layout (Layout.pointy, new Point(1, 1), new Point(0,0)));
		}

		public Map(int radius, Layout layout)
		{
			init (radius, layout);
		}

		private void init(int radius, Layout layout)
		{
			this.radius = radius;
			this.layout = layout;
			this.zoneNameDict = new SerializableDictionary<string, string> ();
			this._zoneDict = new Dictionary<string, Zone> ();

			this._showZoneDict = new Dictionary<Hex, Zone> ();
			this._showCellDict = new Dictionary<Hex, Cell> ();
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

		/// <summary>
		/// 保存地图
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="map">Map.</param>
		public static void SaveMapToXml(Map map, string fileName=null)
		{
			string path;
			if (fileName != null) {
				path = Res.GetMapPath (fileName);
				FileUtilEx.CreateDirectory (path);
				map.path = path;
			} else {
				path = map.path;
			}

			Debug.Assert (path != null);

			string mapName = Res.GetMapConfPath (path);
			string dataStr = XmlUtil.SerializeObject (map, typeof(Map));
			XmlUtil.CreateXml (mapName, dataStr);

			foreach (KeyValuePair<string, Zone> kv in map.zoneDict) {
				Debug.Log ("SaveMapToXml zone " + kv.Key);
				if (kv.Value.isDirty) {
					string zonePath = Res.GetMapZonePath(path, kv.Key);
					Zone.SaveZoneToXml(zonePath, kv.Value);
				}
			}
		}

		/// <summary>
		/// 保存单个有改变的zone
		/// </summary>
		/// <param name="map">Map.</param>
		/// <param name="zone">Zone.</param>
		public static void SaveMapZoneToXml(Map map, Zone zone)
		{
			Debug.Log ("SaveMapZoneToXml zone " + zone.name);
			string zonePath = Res.GetMapZonePath(map.path, zone.name);
			Zone.SaveZoneToXml(zonePath, zone);
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

		public Dictionary<Hex, Zone> GetShowZones()
		{
			if (_showZoneDict.Count == 0) {
				Zone zone = GetZone (curHex);
				if (zone != null) {
					addShowZone (zone);
					for (int i = 0; i < 6; i++) {
						Hex nearHex = Hex.Neighbor (curHex, i);
						zone = GetZone (nearHex);
						if (zone != null) {
							addShowZone (zone);
						}
					}
				}
			}
			return _showZoneDict;
		}

		public List<Cell> GetCellNeighbors(Cell cell)
		{
			List<Cell> results = new List<Cell> ();
			Hex hex = cell.realHex;
			for (int i = 0; i < 6; i++) {
				Hex nearHex = Hex.Neighbor (hex, i);
				Cell nearCell;
				_showCellDict.TryGetValue (nearHex, out nearCell);
				if (nearCell != null) {
					results.Add (nearCell);
				}
			}
			return results;
		}

		private void addShowZone(Zone zone)
		{
			_showZoneDict.Add (zone.hex, zone);
			for (int i = 0; i < zone.count; i++) {
				Cell cell = zone.cells [i];
				cell.realHex = cell.hex + zone.centerHex;
				cell.point = Layout.HexToPixel (layout, cell.realHex);
				_showCellDict.Add (cell.realHex, cell);
			}
		}

		private void removeShowZone(Zone zone)
		{
			_showZoneDict.Remove (zone.hex);
			for (int i = 0; i < zone.count; i++) {
				Cell cell = zone.cells [i];
				_showCellDict.Remove (cell.realHex);
			}
		}

		public void ChangeCurHex(Hex newHex, out List<Hex> addHexs, out List<Hex> removeHexs)
		{
			Hex direct = newHex - curHex;
			Hex direct_1 = Hex.RotateLeft (direct);
			Hex direct_2 = Hex.RotateRight (direct);

			addHexs = new List<Hex> ();
			addHexs.Add (newHex + direct);
			addHexs.Add (curHex + direct_1 + direct);
			addHexs.Add (curHex + direct_2 + direct);

			removeHexs = new List<Hex> ();
			foreach (Hex hex in addHexs) {
				removeHexs.Add(hex - direct * 2);
			}

			curHex = newHex;

			foreach (Hex hex in addHexs) {
				var zone = GetZone (hex);
				if (zone != null) {
					removeShowZone (zone);
				}
			}

			foreach (Hex hex in addHexs) {
				var zone = GetZone (hex);
				if (zone != null) {
					addShowZone (zone);
				}
			}
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

			// 位置在半径内，addShowZone
			if (Hex.Distance (hex, curHex) <= radius) {
				addShowZone (zone);
			}
		}
	}
}

