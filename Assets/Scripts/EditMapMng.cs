using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Comp;
using Stone.Hex;

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

	private System.Random _random;
	private List<string> _pfNameList;

	private List<string> _zoneNames;

	protected override void OnInitFirst()  
	{  
		_layout = new Layout (Layout.pointy, new Point(m_sizeX, m_sizeY), new Point(0,0));

		_pfNameList = FileUtilEx.GetDirs (Res.GroundPath, ".prefab");
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);

		_random = new System.Random (unchecked((int)DateTime.Now.Ticks));
	}  

	protected override void OnInitSecond()
	{  
		
	}

	private string _getRandomPf()
	{
		int index = _random.Next (_pfNameList.Count);
		return _pfNameList [index];
	}

	//===================== zone =====================

	private void _openZone()
	{
		for (int i = 0; i < m_transform.childCount; i++) {  
			Destroy (m_transform.GetChild (i).gameObject);  
		}

		for (int i = 0; i < _zone.count; i++) {
			Cell cell = _zone.cells [i];

			Point pt = Layout.HexToPixel (_layout, cell.hex);

			GameObject go = Instantiate (m_cellPf);
			go.transform.parent = m_transform;
			go.transform.position = new Vector3((float)pt.x, 0.0f, (float)pt.y);
			go.GetComponent<CellMng> ().data = cell;
			
			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(cell.pfName, typeof(GameObject));
			GameObject ground = Instantiate (prefab);
			ground.transform.parent = go.transform;
			ground.GetComponent<MeshRenderer> ().material.color = cell.walkable ? Color.white : Color.red;
			ground.transform.localPosition = Vector3.zero;
		}
	}

	public void CreateZone(string fileName)
	{
		Debug.Log ("CreateZone");

		_zone = _getRandomZone (true);
		_openZone ();
	}
	
	public void OpenZone(string fileName)
	{
		string path = Res.GetZonePath (fileName);
		Debug.Log ("OpenZone path " + path);

		_zone = Zone.LoadZoneFromXml (path);
		if (_zone != null) {
			_openZone ();
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
		if (!_map.IsEmpty ()) {

		}
	}

	public void CreateMap(string fileName)
	{
		_map = new Map();
		_openMap ();
	}

	public void OpenMap(string fileName)
	{

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
			for (int i = 0; i < _zone.count; i++) {
				Cell cell = _zone.cells [i];
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
		}
	}

	public void SaveMap(string fileName)
	{
		Debug.Log ("SaveMap fileName " + fileName);

		if (_map != null) { 
			string dataStr = XmlUtil.SerializeObject (_map, typeof(Map));
			XmlUtil.CreateXml (fileName, dataStr);
		} else {
			Debug.Log ("SaveMap fail");
		}
	}

	protected override void OnUpdate()  
	{  
		//		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//
		//		// Ignore any input while the mouse is over a UI element
		//		if (_eventSystem.IsPointerOverGameObject()) {
		//			return;
		//		}


	}

}
