using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;

public class PlayerMng : BaseBehaviour {

	private Cell _cell;
	public Cell cell {
		get { return _cell; } 
		set { 
			if(_cell != value){
				if (_cell != null) {
					_cell.mng = null;
				}
				_cell = value;
				if (_cell != null) {
					_cell.mng = this;
					transform.position = new Vector3 ((float)_cell.point.x, transform.position.y, (float)_cell.point.y);
				}
				_onCellChange ();
			}
		} 
	}

	private Player _data;
	public Player data {
		get { return _data; } 
		set { 
			_data = value;


		} 
	}

	public event EventHandler OnCellChange;
	public event EventHandler OnDataChange;

	protected override void OnInitFirst()  
	{  

	}  

	protected override void OnInitSecond()
	{  

	}

	private void _onCellChange()
	{
		if (OnCellChange != null) {
			OnCellChange (this, EventArgs.Empty);
		}
	}

	protected override void OnUpdate()
	{

	}
}
