using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;

public class EditMapMng : BaseBehaviour {

	public GameObject m_cellPf;

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

	// 是否是map编辑／zone编辑
	private bool _isMap;

	private System.Random _random;
	private List<string> _pfNames;
	private List<string> _zoneNames;

	EventSystem _eventSystem;

	protected override void OnInitFirst()  
	{  
		_eventSystem = FindObjectOfType<EventSystem> ();

		_layout = new Layout (Layout.pointy, new Point(m_sizeX, m_sizeY), new Point(0,0));

		_pfNames = FileUtilEx.GetDirs (Res.GroundPath, ".prefab");
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);

		_random = new System.Random (unchecked((int)DateTime.Now.Ticks));
	}  

	protected override void OnInitSecond()
	{  
		
	}

	public void ClearGameObject()
	{
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
			var zoneDict = _map.GetShowZones ();
			foreach (var zone in zoneDict.Values) {
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
			isRandom = _random.Next (10) < 5;
		}

		Zone zone;
		if (isRandom) {
			string name = _zoneNames [_random.Next (_zoneNames.Count)];
			string path = Res.GetZonePath (name);
			zone = Zone.LoadZoneFromXml (path);

			Debug.Log ("_getRandomZone Random zone " + name);
		} else {
			zone = Zone.GetRandomZone (new Hex(), m_zoneRadius);
			for (int i = 0; i < zone.count; i++) {
				Cell cell = zone.cells [i];
				cell.pfName = _getRandomPf ();
				cell.walkable = _random.Next (10) < 8;
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
		
		Map.SaveMapToXml (_map, fileName);
	}

	protected override void OnUpdate()  
	{  
		
	}

	public void FixedUpdate()
	{
		
	}

}
