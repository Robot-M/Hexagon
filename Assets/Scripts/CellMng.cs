using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;

public class CellMng : BaseBehaviour {

	public Cell.State m_state;
	public GameObject m_groundPf;
	public GameObject m_triggerPf;
	public int m_triggerId;

	// 障碍物
	public List<GameObject> m_obstGoList = new List<GameObject>();
	public GameObject m_multObstGo;
	public List<GameObject> m_partGoList = new List<GameObject>();

	private GameObject _preGroundPf;
	private GameObject _preTraggerPf;

	private GameObject _groundGO;

	private Cell _data;
	public Cell data { 
		get { return _data; } 
		set { 
			_data = value;
			m_state = _data.state;

			m_groundPf = (GameObject) AssetDatabase.LoadAssetAtPath(_data.groundPfName, typeof(GameObject));
			_preGroundPf = m_groundPf;
			
			m_triggerPf = (GameObject) AssetDatabase.LoadAssetAtPath(_data.triggerPfName, typeof(GameObject));
			_preTraggerPf = m_triggerPf;

			refreshCell ();
		} 
	}
	
	public event EventHandler OnDataChange;

	protected override void OnInitFirst()  
	{  
		
	}  

	protected override void OnInitSecond()
	{  
		
	}

	protected override void OnUpdate()
	{
		
	}

	void OnMouseEnter()
	{
//		print ("鼠标进入区域");
	}

	void OnMouseOver(){
//		print ("鼠标停留在区域内");
	}
	void OnMouseExit(){
//		print ("鼠标离开区域");
	}
	void OnMouseDown(){
//		print ("鼠标按下");
	}
	void OnMouseUp(){
//		print ("鼠标松开");
	}
	void OnMouseDrag(){
//		print ("鼠标拖拽");
	}

	public void refreshCell()
	{
		_refreshGroundPf ();
		_refreshState ();
	}

	private void _refreshState()
	{
		if (m_state != Cell.State.WALK) {
			if (_data.obstList.Count > 0) {
				int num = _data.obstList.Count;
				for (int i = 0; i < num; i++) {
					GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath (_data.obstList[i], typeof(GameObject));
					GameObject go = Instantiate (prefab);
					go.transform.parent = gameObject.transform;
					go.transform.localPosition = _data.obstPosList [i];
					m_obstGoList.Add (go);
				}
			}

			if (_data.IsMainObst()) {
				GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath (_data.multObst, typeof(GameObject));
				GameObject go = Instantiate (prefab);
				go.transform.parent = gameObject.transform;
				go.transform.localPosition = _data.multObstPos;
				m_multObstGo = go;
			}
		} else {
			
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

	private void _refreshTriggerId()
	{

	}

	private void _onDataChange()
	{
		if (OnDataChange != null) {
			OnDataChange (this, EventArgs.Empty);
		}
	}

#if UNITY_EDITOR

	public void AddObstacle(GameObject go)
	{
		m_obstGoList.Add (go);
		Vector3 pos = go.transform.localPosition;
		string pfName = AssetDatabase.GetAssetPath (go);
		_data.AddObstacle (pfName, pos);
		_onDataChange ();
	}

	public void RemoveObstacle(GameObject go)
	{
		int index = m_obstGoList.FindIndex ((GameObject input) => {
			return input == go;
		});
		if (index != -1) {
			_data.RemoveObstacle (index);
			_onDataChange ();
		}
	}

	public bool HaveObstacle(GameObject go)
	{
		return m_obstGoList.Contains (go);
	}

	public void UpdateObstacle(GameObject go)
	{
		Vector3 pos = go.transform.localPosition;
		string pfName = AssetDatabase.GetAssetPath (go);
		_data.UpdateObstacle (pfName, pos);
		_onDataChange ();
	}

	public void ClearObstacle()
	{
		if (m_obstGoList.Count > 0) {
			m_obstGoList.Clear ();
			_data.ClearObstacle ();
		}
	}

	public void ClearMultObstacle()
	{
		// 多格障碍
		if (m_multObstGo != null) {
			if (m_partGoList.Count > 0) {
				// 主障碍，附带其他的也要移除
				for (int i = 0; i < m_partGoList.Count; i++) {
					GameObject go = m_partGoList [i];
					CellMng mng = go.GetComponent<CellMng> ();
					mng.ClearMultObstacle ();
				}
				Destroy(m_multObstGo);
			} else {
				// 多格障碍的一部分
				m_multObstGo = null;
				m_partGoList.Clear ();
			}
			_data.clearMultObstacle ();
		}
	}

	void Reset()
	{
		Debug.Log("脚本添加事件");
	}


	void OnValidate()
	{
		if (_data != null) {
			Debug.Log("脚本对象数据发生改变事件");

			if (m_state != _data.state) {
				Debug.Log ("m_state " + m_state.ToString ());
				_data.state = m_state;
				if (m_state == Cell.State.WALK) {
					ClearObstacle ();
					ClearMultObstacle ();
				}
				_refreshState ();
				_onDataChange ();
			}
				
			if (m_groundPf != _preGroundPf) {
				_preGroundPf = m_groundPf;
				string pfName = AssetDatabase.GetAssetPath (m_groundPf);
				_data.groundPfName = pfName;

				Debug.Log ("groundPfName change " + pfName);
				_refreshGroundPf ();
				_onDataChange ();
			}

			if (m_triggerPf != _preTraggerPf) {
				_preTraggerPf = m_triggerPf;
				string pfName = AssetDatabase.GetAssetPath (m_triggerPf);
				_data.triggerPfName = pfName;

				Debug.Log ("triggerPfName change " + pfName);
				_refreshTriggerPf ();
				_onDataChange ();
			}

			if (m_triggerId != _data.triggerId) {
				Debug.Log ("m_triggerId " + m_triggerId);
				_data.triggerId = m_triggerId;
				_refreshTriggerId ();
				_onDataChange ();
			}
		}
	}

#endif
}

