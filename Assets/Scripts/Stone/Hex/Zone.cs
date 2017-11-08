using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Stone.Comp;

namespace Stone.Hex  
{
	public class Zone
	{
		private Hex _hex; 
		public Hex hex { 
			get { return _hex; } 
			set { 
				if (!Hex.Equals (_hex, value)){
					_hex = value;
					_name = "zone_" + _hex.q + "_" + _hex.r;

					for (int i = 0; i < cells.Count; i++) {
						Cell cell = cells [i];
						cell.zoneHex = value;
					}
				}
			} 
		}

		// 中心点真实hex
		public Hex centerHex;

		public int radius;
		public List<Cell> cells;

		[XmlIgnore]
		public int count;

		[XmlIgnore]
		public bool isDirty = false;

		//zone 名字
		private string _name = "zone_0_0";
		[XmlIgnore]
		public string name { 
			get { return _name; }  
		}

		public Zone ()
		{
			
		}

		public Zone(Hex hex, int radius, List<Cell> cells)
		{
			this.hex = hex;
			this.radius = radius;
			this.cells = cells;
			this.count = cells.Count;
		}

		public static Zone GetRandomZone(Hex hex, int radius)
		{
			List<Cell> cells = new List<Cell> ();

			for (int q = -radius; q <= radius; q++) {
				int r1 = Math.Max (-radius, -q - radius);
				int r2 = Math.Min (radius, -q + radius);
				for (int r = r1; r <= r2; r++) {
					Hex cHex = new Hex (q, r, -q - r);
					Cell cell = new Cell (hex, cHex, true);
					cells.Add (cell);
				}
			}

			return new Zone (hex, radius, cells);
		}

		public static Zone LoadZoneFromXml(string path)
		{
			Debug.Log ("LoadZoneFromXml path " + path);
			Zone zone = null;
			if (FileUtilEx.HasFile (path)) {
				string dataStr = XmlUtil.LoadXml (path);
				zone = XmlUtil.DeserializeObject (dataStr, typeof(Zone)) as Zone;
				zone.count = zone.cells.Count;
			} else {
				Debug.Log ("LoadZoneFromXml can't find");
			}
			return zone;
		}

		public static void SaveZoneToXml(string path, Zone zone)
		{
			Debug.Log ("SaveZoneToXml path " + path);
			if (zone.isDirty) {
				zone.isDirty = false;
				string dataStr = XmlUtil.SerializeObject (zone, typeof(Zone));
				XmlUtil.CreateXml (path, dataStr);
			} else {
				Debug.Log ("SaveZoneToXml zone has nothing to save");
			}
		}
	}
}
