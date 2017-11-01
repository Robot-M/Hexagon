using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Comp;
using Stone.Hex;

public class CellMng : BaseBehaviour {

	public bool m_walkable;
	public GameObject m_groundPf;
//	public GameObject m_triggerPf;

	private GameObject _preGroundPf;

	private Cell _data;
	public Cell data { 
		get { return _data; } 
		set { 
			_data = value;
			m_walkable = _data.walkable;

			m_groundPf = (GameObject) AssetDatabase.LoadAssetAtPath(_data.pfName, typeof(GameObject));
			_preGroundPf = m_groundPf;

			refreshCell ();
		} 
	}
	
	public event EventHandler OnDataChange;

	private GameObject _groundGO;
	private GameObject _obstacleGO;

	protected override void OnInitFirst()  
	{  
		
	}  

	protected override void OnInitSecond()
	{  
		
	}

	protected override void OnUpdate()
	{
		
	}

	public void refreshCell()
	{
		_refreshWalkable ();
		_refreshGroundPf ();
	}

	private void _refreshWalkable()
	{
		if (!m_walkable) {
			if (_obstacleGO == null) {
				GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath (Res.ObstaclePath, typeof(GameObject));
				_obstacleGO = Instantiate (prefab);
				_obstacleGO.transform.parent = gameObject.transform;
				_obstacleGO.transform.localPosition = new Vector3 (0, 0.3f, 0);
			}
			_obstacleGO.SetActive (true);
		} else if (_obstacleGO != null) {
			_obstacleGO.SetActive(false);
		}
	}

	private void _refreshGroundPf()
	{
		if (_groundGO != null) {
			Destroy (_groundGO);
		}
		
		_groundGO = Instantiate (m_groundPf);
		_groundGO.transform.parent = gameObject.transform;
		_groundGO.transform.localPosition = Vector3.zero;
	}

	private void _refreshTriggerPf()
	{

	}

	private void _onDataChange()
	{
		if (OnDataChange != null) {
			OnDataChange (this, EventArgs.Empty);
		}
	}

#if UNITY_EDITOR

	void Reset()
	{
		Debug.Log("脚本添加事件");
	}


	void OnValidate()
	{
		if (_data != null) {
			Debug.Log("脚本对象数据发生改变事件");

			if (m_walkable != _data.walkable) {
				Debug.Log ("m_walkable " + m_walkable.ToString ());
				_data.walkable = m_walkable;
				_refreshWalkable ();
				_onDataChange ();
			}
				
			if (m_groundPf != _preGroundPf) {
				_preGroundPf = m_groundPf;
				string pfName = AssetDatabase.GetAssetPath (m_groundPf);
				_data.pfName = pfName;

				Debug.Log ("pfName change " + pfName);
				_refreshGroundPf ();
				_onDataChange ();
			}
		}
	}

#endif
}

