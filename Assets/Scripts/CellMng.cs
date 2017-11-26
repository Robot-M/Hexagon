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

	private GameObject _preGroundPf;
	private GameObject _preTraggerPf;

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
		_refreshState ();
		_refreshGroundPf ();
	}

	private void _refreshState()
	{
		if (m_state != Cell.State.WALK) {
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

	public void AddObstacle(GameObject go, Vector3 pos)
	{
		string pfName = AssetDatabase.GetAssetPath (go);
		data.AddObstacle (pfName, pos);
		_onDataChange ();
	}

	public void RemoveObstacle(GameObject go, Vector3 pos)
	{
		string pfName = AssetDatabase.GetAssetPath (go);
		data.RemoveObstacle (pfName, pos);
		_onDataChange ();
	}

	public void UpdateObstacle(GameObject go, Vector3 pos)
	{

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

