using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stone.Hex  
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

		public void Dump(string msg="")
		{
			Debug.Log (msg);
			Debug.Log ("zoneHex q = " + zoneHex.q + " r = " + zoneHex.r + " s = " + zoneHex.s);
			Debug.Log ("hex q = " + hex.q + " r = " + hex.r + " s = " + hex.s);
			Debug.Log ("walkable " + walkable.ToString());
			Debug.Log ("pfName " + pfName);
			Debug.Log ("triggerPfName " + triggerPfName);
		}
	}
}