using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;

public class EditMapMng : BaseBehaviour {

	public enum EditType
	{
		PIECE,   //块
		ZONE,	//区域
		MAP,	//地图
	}

	public GameObject m_cellPf;
	public GameObject m_placePf;

	// hex size
	public int m_sizeX = 1;
	public int m_sizeY = 1;

	// zone radius
	public int m_zoneRadius = 3;
	private int _preZoneRadius = 3;

	private Layout _layout;

	private Piece _piece;
	public Piece piece { 
		get { return _piece; } 
	}

	private Zone _zone;
	public Zone zone { 
		get { return _zone; } 
	}

	private Map _map;
	public Map map { 
		get { return _map; } 
	}

	// 是否是map编辑／zone编辑／piece编辑
	private EditType _type;

	// 块名字
	private Piece _curPiece;

	private Dictionary<Hex, GameObject> _goDict;

	private System.Random _random;
	private List<string> _pfNames;
	private List<string> _zoneNames;
	private List<string> _pieceNames;

//	EventSystem _eventSystem;

	protected override void OnInitFirst()  
	{  
//		_eventSystem = FindObjectOfType<EventSystem> ();

		_layout = new Layout (Layout.pointy, new Point(m_sizeX, m_sizeY), new Point(0,0));

		_pfNames = FileUtilEx.GetDirs (Res.GroundPath, ".prefab");
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);
		_pieceNames = FileUtilEx.GetFileNames(Res.PiecePath, Res.ConfExt);

		_goDict = new Dictionary<Hex, GameObject> ();

		_random = new System.Random (unchecked((int)DateTime.Now.Ticks));
	}  

	protected override void OnInitSecond()
	{  
		
	}

	public void ClearGameObject()
	{
		_goDict.Clear ();
		for (int i = 0; i < m_transform.childCount; i++) {  
			Destroy (m_transform.GetChild (i).gameObject);  
		}
	}

	private GameObject _getGoByHex(Hex hex)
	{
		GameObject go = null;
		_goDict.TryGetValue (hex, out go);
		return go;
	}

	//===================== create cells =====================

	private void _createCell(Cell cell)
	{
		Point pt;
		if (_type == EditType.MAP) {
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

		_goDict.Add (cell.realHex, go);
	}

	private void _handleCellDataChange (object sender, EventArgs e)
	{
		CellMng mng = (CellMng)sender;
		Cell cell = mng.data;
		if (_type == EditType.MAP) {
			Zone zone = _map.GetZone (cell.zoneHex);
			zone.isDirty = true;
		} else if(_type == EditType.ZONE){
			_zone.isDirty = true;
		} else if(_type == EditType.PIECE){
			_piece.isDirty = true;
		}
	}

	private void _refreshMultObst()
	{
		foreach (var item in _goDict){
			CellMng mng = item.Value.GetComponent<CellMng>();
			Cell cell = mng.data;
			if (cell.IsMainObst ()) {
				List<Hex> partHexs = cell.partHexs;
				for (var i = 0; i < partHexs.Count; i++) {
					Hex hex = cell.realHex + partHexs [i];
					GameObject go = _getGoByHex (hex);
					mng.AddPartGo (go);

					CellMng tarMng = go.GetComponent<CellMng>();
					tarMng.m_multObstGo = mng.m_multObstGo;
				}
			}
		}
	}

	private void _openPiece(Piece piece)
	{
		for (int i = 0; i < piece.count; i++) {
			Cell cell = piece.cells [i];
			_createCell (cell);
		}
		_refreshMultObst ();
	}

	private void _openZone(Zone zone)
	{
		for (int i = 0; i < zone.count; i++) {
			Cell cell = zone.cells [i];
			_createCell (cell);
		}

		if (_type == EditType.ZONE) {
			_refreshMultObst ();
		}
	}

	//===================== piece =====================

	public void CreatePiece(string fileName)
	{
		Debug.Log ("CreatePiece");

		_type = EditType.PIECE;
		ClearGameObject ();
		_piece = Piece.GetRandomPiece (m_zoneRadius);
		_openPiece (_piece);
	}

	public void OpenPiece(string fileName)
	{
		string path = Res.GetPiecePath (fileName);
		Debug.Log ("OpenPiece path " + path);

		_type = EditType.PIECE;
		ClearGameObject ();
		_piece = Piece.LoadPieceFromXml (path);
		if (_piece != null) {
			_openPiece (_piece);
		} else {
			Debug.Log ("OpenPiece fail");
		}
	}

	public void SavePiece(string fileName)
	{
		string path = Res.GetPiecePath (fileName);
		Debug.Log ("SavePiece path " + path);

		if (_piece != null) { 
			Piece.SavePieceToXml (path, _piece);
			_pieceNames.Insert (0, fileName);
		} else {
			Debug.Log ("SavePiece fail");
		}
	}

	//===================== zone =====================

	public void CreateZone(string fileName)
	{
		Debug.Log ("CreateZone");
		
		_type = EditType.ZONE;
		ClearGameObject ();
		_zone = _getRandomZone (true);
		_openZone (_zone);
	}
	
	public void OpenZone(string fileName)
	{
		string path = Res.GetZonePath (fileName);
		Debug.Log ("OpenZone path " + path);
		
		_type = EditType.ZONE;
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
		_type = EditType.MAP;
		ClearGameObject ();
		if (!_map.IsEmpty ()) {
			var zoneDict = _map.GetShowZones ();
			foreach (var zone in zoneDict.Values) {
				_openZone (zone);
			}
			_refreshMultObst ();
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
				cell.groundPfName = _getRandomPf ();
				cell.state = _random.Next (10) < 8 ? Cell.State.WALK : Cell.State.OBSTACLE;
				if (cell.state == Cell.State.OBSTACLE) {
					// 默认的障碍物
					cell.AddObstacle (Res.ObstaclePath, Vector3.zero);
				}
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

	public void SelectPiece(string fileName)
	{
		if (fileName == "none") {
			_curPiece = null;
		} else {
			string path = Res.GetPiecePath (fileName);
			_curPiece = Piece.LoadPieceFromXml (path);
		}
	}

	//===================== update =====================

	protected override void OnUpdate()  
	{  
		if (_curPiece != null) {
			//大物件（多格障碍物） 点击结束添加
			if (Input.GetMouseButtonUp (0)) {
				PlaceMultObject ();
			}
		} else if(m_placePf != null) {
			//小物件（单格障碍物） 鼠标 左键添加 右键移除
			if (Input.GetMouseButton (0)) {
				PlaceObject ();
			}
			if (Input.GetMouseButton (1)) {
				RemoveObject ();
			}
		}
	}

	public void FixedUpdate()
	{
		
	}

	public void PlaceMultObject()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		int layerMask = 1 << 9;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
			GameObject hitGo = hit.transform.gameObject;
			CellMng curMng = hitGo.GetComponent<CellMng>();
			Cell curCell = curMng.data;

			// 需要的格子都是空的才能插入
			if (curCell.IsWalkable ()) {
				bool isEmpty = true;
				for (int i = 1; i < piece.count; i++) {
					Cell cell = _curPiece.cells [i];
					GameObject go = _getGoByHex(curCell.realHex + cell.hex);
					if (go) {
						CellMng mng = go.GetComponent<CellMng>();
						if (!mng.data.IsWalkable ()) {
							isEmpty = false;
							break;
						}
					} else {
						break;
					}
				}

				if (isEmpty) {
					// 满足条件就替换刷新数据
					for (int i = 0; i < piece.count; i++) {
						Cell cell = _curPiece.cells [i];
						GameObject go = _getGoByHex(curCell.realHex + cell.hex);

						CellMng mng = go.GetComponent<CellMng> ();
						mng.data = cell;
					}
				}
			}
		}
	}

	//设置单格障碍物
	public void PlaceObject ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		// Figure out where the ground is
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

			if (hit.transform.gameObject.tag != "Ground")
				return;
			//获取放置面的坐标
			Vector3 p = hit.point;

			Transform parent = hit.collider.transform.parent;
			CellMng mng = parent.GetComponent<CellMng>();
			Cell cell = mng.data;
			Vector3 lpos = p - parent.position;

			GameObject go = GameObject.Instantiate (m_placePf, p, m_placePf.transform.rotation) as GameObject;
			go.transform.parent = parent;
			go.transform.localPosition = lpos;
			mng.AddObstacle (go);
		}
	}

	//移除单格障碍物
	public void RemoveObject ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		// Check what object is under the mouse cursor
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			// Ignore ground and triggers
			if (hit.collider.isTrigger || hit.transform.gameObject.tag != "Obstacle")
				return;

			Transform hitTrans = hit.collider.transform;
			GameObject go = hitTrans.gameObject;

			CellMng mng = hitTrans.parent.GetComponent<CellMng>();
			if (mng.HaveObstacle (go)) {
				mng.RemoveObstacle (go);
				Destroy (go);
			}
		}
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (_type == EditType.PIECE) {
			if (m_zoneRadius != _preZoneRadius) {
				_preZoneRadius = m_zoneRadius;
				CreatePiece ("");
			}
		}
	}
#endif

}
