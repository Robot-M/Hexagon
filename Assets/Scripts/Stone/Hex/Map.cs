using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stone.Hex  
{
	public class Map
	{
		private Dictionary<string, Zone> _zones;
		public Dictionary<string, string> zoneNames;

		private List<Hex> _spiralHexs;

		public Map()
		{
			this.zoneNames = new Dictionary<string, string> ();
			this._zones = new Dictionary<string, Zone> ();
		}

		public Map (Dictionary<string, string> zoneNames)
		{
			this.zoneNames = zoneNames;
			this._zones = new Dictionary<string, Zone> ();
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
				if (!HasZone (_spiralHexs [i])) {
					result = _spiralHexs [i];
					return true;
				}
			}
			result = new Hex ();
			return false;
		}

		public bool HasZone(Hex hex)
		{
			string key = "zone_" + hex.q + "_" + hex.r;
			return zoneNames.ContainsKey (key);
		}

		public bool HasZone(string key)
		{
			return zoneNames.ContainsKey (key);
		}

		public void AddZone(Hex hex, Zone zone)
		{
			string key = "zone_" + hex.q + "_" + hex.r;
			zone.hex = hex;
			_zones.Add (key, zone);
		}
	}
}

