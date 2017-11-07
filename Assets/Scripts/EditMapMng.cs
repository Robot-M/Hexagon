using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Comp;
using Stone.Hex;

public class EditMapMng : BaseBehaviour {

	private enum Turn
	{
		SELECT,
		MOVE,
		ATTACK
	}

	public GameObject m_cellPf;
	public Transform m_playerTf;

	private PlayerMng _playerMng;

	// hex size
	public int m_sizeX = 1;
	public int m_sizeY = 1;

	// zone radius
	public int m_zoneRadius = 3;

	private Layout _layout;

	private Zone _zone;
	public Zone zone { 
		get { return _zone; } 
	}

	private Map _map;
	public Map map { 
		get { return _map; } 
	}

	// 地图上所有显示的元素集合
	private Dictionary<Cell, GameObject> _mapGoDict;

	// 是否是map编辑／zone编辑
	private bool _isMap;

	private System.Random _random;
	private List<string> _pfNames;
	private List<string> _zoneNames;

	// 循环，移动
	private Turn _turn;
	private List<Cell> _path;
	private int _pathIdx;
	private int _speed = 5;
	private float _minDistance = 0.1f;

	EventSystem _eventSystem;

	protected override void OnInitFirst()  
	{  
		_turn = Turn.SELECT;
		_eventSystem = FindObjectOfType<EventSystem> ();

		_layout = new Layout (Layout.pointy, new Point(m_sizeX, m_sizeY), new Point(0,0));

		_pfNames = FileUtilEx.GetDirs (Res.GroundPath, ".prefab");
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);

		_random = new System.Random (unchecked((int)DateTime.Now.Ticks));

		_mapGoDict = new Dictionary<Cell, GameObject> ();
	}  

	protected override void OnInitSecond()
	{  
		_playerMng = m_playerTf.GetComponent<PlayerMng> ();
	}

	public void ClearGameObject()
	{
		_mapGoDict.Clear ();
		for (int i = 0; i < m_transform.childCount; i++) {  
			Destroy (m_transform.GetChild (i).gameObject);  
		}
	}

	//===================== zone =====================

	private void _openZone(Zone zone)
	{
		for (int i = 0; i < zone.count; i++) {
			Cell cell = zone.cells [i];

			Point pt;
			if (_isMap) {
				pt = cell.point;
			} else {
				cell.realHex = zone.centerHex + cell.hex;
				pt = Layout.HexToPixel (_layout, cell.realHex);
			}

			GameObject go = Instantiate (m_cellPf);
			go.name = cell.realName;
			go.transform.parent = m_transform;
			go.transform.position = new Vector3((float)pt.x, 0.0f, (float)pt.y);

			CellMng mng = go.GetComponent<CellMng> ();
			mng.data = cell;
			mng.OnDataChange += _handleCellDataChange;

			_mapGoDict.Add (cell, go);

			if (_playerMng.cell == null && cell.walkable) {
				Debug.Log ("_playerMng init");
				_playerMng.cell = cell;
				cell.mng = _playerMng;
			}
		}
	}

	private void _handleCellDataChange (object sender, EventArgs e)
	{
		CellMng mng = (CellMng)sender;
		Cell cell = mng.data;
		if (_isMap) {
			Zone zone = _map.GetZone (cell.zoneHex);
			zone.isDirty = true;
		} else {
			_zone.isDirty = true;
		}
	}

	public void CreateZone(string fileName)
	{
		Debug.Log ("CreateZone");
		
		_isMap = false;
		ClearGameObject ();
		_zone = _getRandomZone (true);
		_openZone (_zone);
	}
	
	public void OpenZone(string fileName)
	{
		string path = Res.GetZonePath (fileName);
		Debug.Log ("OpenZone path " + path);
		
		_isMap = false;
		ClearGameObject ();
		_zone = Zone.LoadZoneFromXml (path);
		if (_zone != null) {
			_openZone (_zone);
		} else {
			Debug.Log ("OpenZone fail");
		}
	}

	public void SaveZone(string fileName)
	{
		string path = Res.GetZonePath (fileName);
		Debug.Log ("SaveZone path " + path);

		if (_zone != null) { 
			Zone.SaveZoneToXml (path, _zone);
			_zoneNames.Insert (0, fileName);
		} else {
			Debug.Log ("SaveZone fail");
		}
	}

	//===================== map =====================

	private void _openMap()
	{
		_isMap = true;
		ClearGameObject ();
		if (!_map.IsEmpty ()) {
			List<Zone> zones = _map.GetShowZones ();
			foreach (Zone zone in zones) {
				_openZone (zone);
			}
		}
	}

	public void CreateMap(string fileName)
	{
		_map = new Map(m_zoneRadius, _layout);
		_openMap ();
	}

	public void OpenMap(string fileName)
	{
		_map = Map.LoadMapFromXml (fileName);
		_openMap ();
	}
	
	private string _getRandomPf()
	{
		int index = _random.Next (_pfNames.Count);
		return _pfNames [index];
	}

	private Zone _getRandomZone(bool isCreate=false)
	{
		bool isRandom = false;
		if (!isCreate) {
			isRandom = _random.Next (10) > 7;
		}

		Zone zone;
		if (isRandom) {
			string name = _zoneNames [_random.Next (_zoneNames.Count)];
			string path = Res.GetZonePath (name);
			zone = Zone.LoadZoneFromXml (path);

			Debug.Log ("_getRandomZone Random zone " + name);
		} else {
			zone = new Zone (new Hex(), m_zoneRadius);
			for (int i = 0; i < zone.count; i++) {
				Cell cell = zone.cells [i];
				cell.pfName = _getRandomPf ();
				cell.walkable = _random.Next (10) < 6;
			}
		}
		zone.isDirty = true;
		return zone;
	}

	public void AddZone()
	{
		Hex hex;
		if (_map.GetNextHex (out hex)) {
			Zone zone = _getRandomZone ();
			_map.AddZone (hex, zone);
			_openZone (zone);
		}
	}

	public void SaveMap(string fileName)
	{
		Debug.Log ("SaveMap fileName " + fileName);
		
		Map.SaveMapToXml (fileName, _map);
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
				_pathIdx = 0;
				_turn = Turn.SELECT;

				_playerMng.cell.mng = null;
				_playerMng.cell = _path[_pathIdx];
				_playerMng.cell.mng = _playerMng;
			}
			return;  
		}  
	}

}
