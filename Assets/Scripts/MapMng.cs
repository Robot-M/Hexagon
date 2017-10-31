using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Comp;
using Stone.Hex;

public class MapMng : BaseBehaviour {

	// hex size
	public int m_sizeX = 1;
	public int m_sizeY = 1;

	// map size
	public int m_width = 20;
	public int m_height = 20;

	// hex profab
	private List<string> _pfNameList;
	private LayerMask _layerMask;

	private Layout _layout;
	private Dictionary<string, Hex> _mapDatas;

	EventSystem _eventSystem;

	protected override void OnInitFirst()  
	{  
		_eventSystem = FindObjectOfType<EventSystem> ();

		_pfNameList = FileUtilEx.GetDirs ("Assets/Prefabs/ground", ".prefab");

		List<string> fileNames = FileUtilEx.GetFileNames("Assets/Prefabs/ground", ".prefab");

		_layerMask = LayerMask.NameToLayer ("Ground");

		_layout = new Layout (Layout.pointy, new Point(m_sizeX, m_sizeY), new Point(0,0));

		_mapDatas = new Dictionary<string, Hex> ();
		for (int q = -m_width; q <= m_width; q++) {
			int r1 = Math.Max(-m_width, -q - m_width);
			int r2 = Math.Min(m_width, -q + m_width);
			for (int r = r1; r <= r2; r++) {
				string key = "hex_" + q + "_" + r;
				_mapDatas.Add(key, new Hex(q, r, -q-r));
			}
		}


	}  

	protected override void OnInitSecond()
	{  

		System.Random rd = new System.Random (unchecked((int)DateTime.Now.Ticks));
		foreach (Hex value in _mapDatas.Values)
		{
			Point pt = Layout.HexToPixel (_layout, value);

			int index = rd.Next (9);
			string path = _pfNameList [index];
			Debug.Log ("path = " + path);

			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
			GameObject go = Instantiate (prefab);
			go.layer = LayerMask.NameToLayer ("Ground");
			go.transform.parent = m_transform;
			go.transform.position = new Vector3((float)pt.x, 0.0f, (float)pt.y);
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

	T GetByRay<T>(Ray ray) where T : class {
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, _layerMask)) {
			return hit.transform.GetComponentInParent<T>();
		}
		return null;
	}
}
