using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;

public class Unit : BaseBehaviour {

	private Cell _cell;
	public Cell cell {
		get { return _cell; } 
		set { 
			if(_cell != value){
				if (_cell != null) {
					_cell.unit = null;
				}
				_cell = value;
				if (_cell != null) {
					_cell.unit = this;
					transform.position = new Vector3 ((float)_cell.point.x, transform.position.y, (float)_cell.point.y);
				}
				_cellChange ();
			}
		} 
	}

	private bool _isCellInit;

	private Role _data;
	public Role data {
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

	private void _cellChange()
	{
		// 第一次初始化不发事件
		if (!_isCellInit) {
			_isCellInit = true;
			return;
		}
		if (OnCellChange != null) {
			OnCellChange (this, EventArgs.Empty);
		}
	}

	protected override void OnUpdate()
	{  

	}
}
