using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Stone.Core  
{
	public class Cell
	{
		public Cell()
		{
			
		}

		public Cell(Hex zoneHex, Hex hex, bool walkable, string pfName="", string triggerPfName="")
		{
			this.zoneHex = zoneHex;
			this.hex = hex;
			this.walkable = walkable;
			this.pfName = pfName;
			this.triggerPfName = triggerPfName;
		}
		public Hex zoneHex;
		public Hex hex;
		public bool walkable;
		public string pfName;
		public string triggerPfName;

		private Hex _realHex;	//真实地图的hex
		[XmlIgnore]
		public Hex realHex { 
			get { return _realHex; } 
			set { 
				if (!Hex.Equals (_realHex, value)) {
					_realHex = value;
					_realName = "hex_" + realHex.q + "_" + realHex.r;
				}
			} 
		}

		//真实地图的hex的名字
		private string _realName="hex_0_0";
		public string realName { 
			get { return _realName; }  
		}

		private Point _point;	//真实地图的hex
		[XmlIgnore]
		public Point point { 
			get { return _point; } 
			set { 
				_point = value;
			} 
		}

		private BaseBehaviour _mng;
		[XmlIgnore]
		public BaseBehaviour mng { 
			get { return _mng; } 
			set { 
				_mng = value;
			} 
		}

		public void Dump(string msg="")
		{
			Debug.Log (msg);
			Debug.Log ("zoneHex q = " + zoneHex.q + " r = " + zoneHex.r + " s = " + zoneHex.s);
			Debug.Log ("hex q = " + hex.q + " r = " + hex.r + " s = " + hex.s);
			Debug.Log ("walkable " + walkable.ToString());
			Debug.Log ("pfName " + pfName);
			Debug.Log ("triggerPfName " + triggerPfName);
		}

		public int Distance(Cell cell)
		{
			return Hex.Distance (this.realHex, cell.realHex);
		}
	}
}