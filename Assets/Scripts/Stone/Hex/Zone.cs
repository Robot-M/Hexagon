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

		public Zone ()
		{
			
		}

		public Zone(Hex hex, int radius=3, List<Cell> cells=null)
		{
			this._hex = hex;
			this.radius = radius;

			// 如果没有cells，创建默认的cells
			if (cells == null) {
				this.cells = new List<Cell> ();
				for (int q = -radius; q <= radius; q++) {
					int r1 = Math.Max (-radius, -q - radius);
					int r2 = Math.Min (radius, -q + radius);
					for (int r = r1; r <= r2; r++) {
						Hex cHex = new Hex (q, r, -q - r);
						Cell cell = new Cell (hex, cHex, true);
						this.cells.Add (cell);
					}
				}
			} else {
				this.cells = cells;
			}
			this.count = this.cells.Count;
		}

		public static Zone LoadZoneFromXml(string path)
		{
			Zone zone = null;
			if (FileUtilEx.HasFile (path)) {
				string dataStr = XmlUtil.LoadXml (path);
				zone = XmlUtil.DeserializeObject (dataStr, typeof(Zone)) as Zone;
			} else {
				Debug.Log ("LoadZoneFromXml can't find");
			}
			return zone;
		}

		public static void SaveZoneToXml(string path, Zone zone)
		{
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
