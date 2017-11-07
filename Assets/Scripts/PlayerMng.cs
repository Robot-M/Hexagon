using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Comp;
using Stone.Hex;

public class PlayerMng : BaseBehaviour {

	private Cell _cell;
	public Cell cell {
		get { return _cell; } 
		set { 
			_cell = value;
			transform.position = new Vector3((float)_cell.point.x, 1.4f, (float)_cell.point.y);
		} 
	}

	protected override void OnInitFirst()  
	{  

	}  

	protected override void OnInitSecond()
	{  

	}

	protected override void OnUpdate()
	{

	}
}
