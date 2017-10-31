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
	public string m_pfName;
	public string m_triggerPfName;

	private Cell _data;
	public Cell data { 
		get { return _data; } 
		set { 
			_data = value;
			m_walkable = _data.walkable;
			m_pfName = _data.pfName;
			m_triggerPfName = _data.triggerPfName;
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

	private void _refreshWalkable()
	{
		Debug.Log("_refreshWalkable childcount " + gameObject.transform.childCount);
		GameObject ground = gameObject.transform.GetChild (0).gameObject;
		ground.GetComponent<MeshRenderer> ().material.color = m_walkable ? Color.white : Color.red;
	}

	private void _refreshPfName()
	{
		Debug.Log("_refreshPfName childcount " + gameObject.transform.childCount);
		if(gameObject.transform.childCount > 0){
			GameObject oldGround = gameObject.transform.GetChild (0).gameObject;
			Destroy (oldGround);
		}

		GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(m_pfName, typeof(GameObject));
		GameObject ground = Instantiate (prefab);
		ground.transform.parent = gameObject.transform;
		ground.GetComponent<MeshRenderer> ().material.color = m_walkable ? Color.white : Color.red;
		ground.transform.localPosition = Vector3.zero;
	}

	private void _refreshTriggerPfName()
	{

	}

#if UNITY_EDITOR

	void Reset()
	{
		Debug.Log("脚本添加事件");
	}


	void OnValidate()
	{
		Debug.Log("脚本对象数据发生改变事件");
		if (_data != null) {
			if (m_walkable != _data.walkable) {
				Debug.Log ("m_walkable " + m_walkable.ToString ());
				_data.walkable = m_walkable;
				_refreshWalkable ();
			}
				
			if (m_pfName != _data.pfName) {
				if (FileUtilEx.HasFile (m_pfName)) {
					Debug.Log ("m_pfName " + m_pfName);
					Debug.Log ("_data.pfName " + _data.pfName);
					_data.pfName = m_pfName;
					_refreshPfName ();
				}
			}

			if (m_triggerPfName != _data.triggerPfName) {
				if (FileUtilEx.HasFile (m_triggerPfName)) {
					Debug.Log ("m_triggerPfName " + m_triggerPfName);
					Debug.Log ("_data.triggerPfName " + _data.triggerPfName);
					_data.triggerPfName = m_triggerPfName;
					_refreshTriggerPfName ();
				}
			}
		}
	}

#endif
}

