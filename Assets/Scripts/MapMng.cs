using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;

public class MapMng : BaseBehaviour {

	private enum Turn
	{
		SELECT,
		MOVE,
		ATTACK
	}

	// 循环，移动
	private Turn _turn;
	private List<Cell> _path;
	private int _pathIdx;
	private int _speed = 5;
	private float _minDistance = 0.1f;

	//========== 地图 ===========
	public GameObject m_cellPf;
	public Transform m_playerTf;

	private PlayerMng _playerMng;
	
	private Map _map;
	public Map map { 
		get { return _map; } 
	}

	// 地图上所有显示的元素集合,分区域的集合
	private Dictionary<Hex, Dictionary<Cell,GameObject>> _zoneGoDict;

	// 随机生成zone
	private System.Random _random;
	private List<string> _pfNames;
	private List<string> _zoneNames;

	EventSystem _eventSystem;

	protected override void OnInitFirst()  
	{  
		_turn = Turn.SELECT;
		_eventSystem = FindObjectOfType<EventSystem> ();

		_random = new System.Random (unchecked((int)DateTime.Now.Ticks));

		_pfNames = FileUtilEx.GetDirs (Res.GroundPath, ".prefab");
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);

		_zoneGoDict = new Dictionary<Hex, Dictionary<Cell,GameObject>> ();
	}  

	protected override void OnInitSecond()
	{  
		_playerMng = m_playerTf.GetComponent<PlayerMng> ();
		_playerMng.data = Player.CreatePlayer ("无尽骑士", 1);
		_playerMng.OnDataChange += _handlePlayerDataChange;
		_playerMng.OnCellChange += _handlePlayerCellChange;
	}

	// 清除map上的对象
	public void ClearGameObject()
	{
		foreach (Dictionary<Cell,GameObject> dict in _zoneGoDict.Values) {
			dict.Clear ();
		}
		_zoneGoDict.Clear ();

		for (int i = 0; i < m_transform.childCount; i++) {  
			Destroy (m_transform.GetChild (i).gameObject);  
		}
	}

	void _handlePlayerDataChange(object sender, EventArgs e)
	{

	}

	// player移动监听
	void _handlePlayerCellChange(object sender, EventArgs e)
	{
		PlayerMng mng = (PlayerMng)sender;
		Cell cell = mng.cell;
		// 区域改变，刷新新区域
		if (cell != null && cell.realHex != _map.curHex) {
			List<Hex> addHexs; 
			List<Hex> removeHexs; 

			_map.ChangeCurHex (cell.realHex, out addHexs, out removeHexs);

			// 添加距离为1的zone
			bool needSave = false;
			foreach (Hex hex in addHexs) {
				var zone = _map.GetZone (hex);
				if (zone == null) {
					zone = _getRandomZone ();
					_map.AddZone (hex, zone);
					needSave = true;
				}
				_showZone (zone);
			}

			if (needSave) {
				Map.SaveMapToXml (_map);
			}

			// 隐藏距离为2的zone
			foreach (Hex hex in removeHexs) {
				_hideZone (hex);
			}

			// 移除距离为3的zone
			foreach (Hex hex in _zoneGoDict.Keys) {
				if (hex.Distance (_map.curHex) >= 3) {
					_removeZone (hex);
				}
			}
		}
	}

	//===================== zone =====================
	// 显示区域
	private void _showZone(Zone zone)
	{
		if (zone == null) { return; }
		if (_zoneGoDict.ContainsKey (zone.hex)) {
			// 已存在，显示出来就行
			var goDict = _zoneGoDict[zone.hex];
			foreach (GameObject go in goDict.Values) {
				go.SetActive (true);
			}
		} else {
			// 不存在，添加
			Dictionary<Cell,GameObject> goDict = new Dictionary<Cell, GameObject> ();
			for (int i = 0; i < zone.count; i++) {
				Cell cell = zone.cells [i];
				
				Point pt = cell.point;
				
				GameObject go = Instantiate (m_cellPf);
				go.name = cell.realName;
				go.transform.parent = m_transform;
				go.transform.position = new Vector3((float)pt.x, 0.0f, (float)pt.y);
				
				CellMng mng = go.GetComponent<CellMng> ();
				mng.OnDataChange += _handleCellDataChange;
				mng.data = cell;
				
				goDict.Add (cell, go);
			}
			_zoneGoDict.Add (zone.hex, goDict);
		}
	}

	// 隐藏区域
	private void _hideZone(Hex hex)
	{
		if (_zoneGoDict.ContainsKey (hex)) {
			// 已存在，隐藏出来就行
			var goDict = _zoneGoDict [hex];
			foreach (GameObject go in goDict.Values) {
				go.SetActive (false);
			}
		}
	}

	// 移除区域
	private void _removeZone(Hex hex)
	{
		if (_zoneGoDict.ContainsKey (hex)) {
			// 已存在，隐藏出来就行
			var goDict = _zoneGoDict [hex];
			foreach (GameObject go in goDict.Values) {
				Destroy (go);
			}
			goDict.Clear ();
			_zoneGoDict.Remove (hex);
		}
	}

	// 监听cell数据改变，用于保存改变
	private void _handleCellDataChange (object sender, EventArgs e)
	{
		CellMng mng = (CellMng)sender;
		Cell cell = mng.data;
		Zone zone = _map.GetZone (cell.zoneHex);
		zone.isDirty = true;
		Map.SaveMapZoneToXml (_map, zone);
	}

	// 随机地板
	private string _getRandomPf()
	{
		int index = _random.Next (_pfNames.Count);
		return _pfNames [index];
	}

	// 随机或者从已有zone中取一个
	private Zone _getRandomZone()
	{
		bool isRandom = _random.Next (10) < 3;

		Zone zone;
		if (isRandom) {
			// 从已有的zone里面随机一个
			string name = _zoneNames [_random.Next (_zoneNames.Count)];
			string path = Res.GetZonePath (name);
			zone = Zone.LoadZoneFromXml (path);

			Debug.Log ("_getRandomZone Random zone " + name);
		} else {
			// 完全随机一个新zone出来
			zone = Zone.GetRandomZone (new Hex(), _map.radius);
			for (int i = 0; i < zone.count; i++) {
				Cell cell = zone.cells [i];
				cell.pfName = _getRandomPf ();
				cell.walkable = _random.Next (10) < 8;
			}
		}
		zone.isDirty = true;
		return zone;
	}

	//===================== map =====================
	/// <summary>
	/// 从文件里面读取地图。
	/// 默认情况时 显示当前区域，以及当前区域距离为1的六个区域。
	/// 移动跨区域时 
	/// 1.会加载或刷新出新的区域，保持显示7个区域；
	/// 2.距离为2的区域会隐藏（以便往回走又需要重新加载）；
	/// 3.清楚距离为3的区域
	/// </summary>
	/// <param name="fileName">File name.</param>
	public void OpenMap(string fileName)
	{
		ClearGameObject ();
		_map = Map.LoadMapFromXml (fileName);

		if (!_map.IsEmpty ()) {
			var zoneDict = _map.GetShowZones ();
			foreach (var zone in zoneDict.Values) {
				_showZone (zone);
			}
		}

		// test 选一个空位置初始化
		Zone curZone = _map.GetCurZone();
		for(int i = 0; i < curZone.count; i++){
			Cell cell = curZone.cells[i];
			if(cell.walkable){
				_playerMng.cell = cell;
				break;
			}
		}
	}

	public void SaveMap()
	{
		Map.SaveMapToXml (_map);
	}

	protected override void OnUpdate()  
	{  
		if (_turn == Turn.SELECT) {
			if (Input.GetMouseButtonUp (0)) {
				_handleInput ();
			}
		}
	}

	public void FixedUpdate()
	{
		if (_turn == Turn.MOVE) {
			_handleMove ();
		}
	}

	private void _handleInput()
	{
		// Ignore any input while the mouse is over a UI element
		if (_eventSystem.IsPointerOverGameObject()) {
			return;
		}

		Debug.Log ("_handleInput");

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			Transform trans = hit.transform;
			CellMng mng = trans.GetComponent<CellMng>();
			if (mng == null) {
				return;
			}
			Cell cell = mng.data;

			Debug.Log ("hit cell " + cell.realName);

			Debug.Log ("cell.walkable " + cell.walkable);
			Debug.Log ("cell.mng " + (cell.mng == null));

			if (cell.walkable && cell.mng == null) {
				Debug.Log ("find path ");
				_path = AStar.search (_map, _playerMng.cell, cell);
				if (_path.Count > 0) {
					Debug.Log ("_path Count " + _path.Count);
					_turn = Turn.MOVE;
				}
			}
		}
	}

	private void _handleMove()
	{
		if (_path == null || _path.Count == 0) {
			_turn = Turn.SELECT;
			return;
		}

		//Direction to the next waypoint  
		Point point = _path[_pathIdx].point;
		Vector3 tar = new Vector3 ((float)point.x, m_playerTf.position.y, (float)point.y);
		Vector3 dir = (tar - m_playerTf.position).normalized;  
		dir *= _speed * Time.fixedDeltaTime;  
		m_playerTf.position += dir;

		//Check if we are close enough to the next waypoint  
		//If we are, proceed to follow the next waypoint  
		if (Vector3.Distance(m_playerTf.position, tar) < _minDistance)  
		{  
			_pathIdx++;  
			if (_pathIdx >= _path.Count) {
				_playerMng.cell = _path[_pathIdx];

				_pathIdx = 0;
				_turn = Turn.SELECT;
				_path = null;
			}
			return;  
		}  
	}

}
