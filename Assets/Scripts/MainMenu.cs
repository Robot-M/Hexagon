using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Core;
using System.Xml.Serialization;

public class MainMenu : BaseBehaviour {
	
	private enum EditType
	{
		MAP,
		ZONE,
		PIECE
	}

	public GameObject m_mapMng;

	public Dropdown m_typeDropDown;
	public Button m_addButton;

	public Dropdown m_fileDropDown;
	public InputField m_inputField;

	public Dropdown m_pieceDropDown;

	private List<string> _mapNames;
	private List<string> _zoneNames;
	private List<string> _pieceNames;

	private EditMapMng _editMapMng;

	private EditType _editTp = EditType.MAP;
	private string _zoneCreateName = "zone_";
	private string _mapCreateName = "map_";
	private string _pieceCreateName = "piece_";

	protected override void OnInitFirst()  
	{  
		_mapNames = FileUtilEx.GetForldNames(Res.MapPath);
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);
		_pieceNames = FileUtilEx.GetFileNames(Res.PiecePath, Res.ConfExt);
	}  


	protected override void OnInitSecond()
	{  
		m_addButton.gameObject.SetActive(_editTp == EditType.MAP);
		m_pieceDropDown.gameObject.SetActive (_editTp != EditType.PIECE);
		if (m_pieceDropDown.gameObject.activeSelf) {
			_refreshPieceDropDown ();
		}
		_editMapMng = m_mapMng.GetComponent<EditMapMng> ();
		_refreshFileDropDown ();
	}

	private void _refreshFileDropDown()
	{
		List<string> names = null;
		switch (_editTp) {
		case EditType.MAP:
			names = _mapNames;
			break;
		case EditType.ZONE:
			names = _zoneNames;
			break;
		case EditType.PIECE:
			names = _pieceNames;
			break;
		}
		m_fileDropDown.options.Clear ();
		
		for (int i = 0; i < names.Count; i++)
		{
			_addDropDown (m_fileDropDown, names [i]);
		}

		_addDropDown (m_fileDropDown, "__create__");

		m_fileDropDown.value = 0;
		m_fileDropDown.RefreshShownValue ();
		m_inputField.text = m_fileDropDown.captionText.text;
	}

	private void _addDropDown(Dropdown dropDown, string name, bool isFirst=false)
	{
		Dropdown.OptionData tempData = new Dropdown.OptionData();
		tempData.text = name;
		if (isFirst) {
			dropDown.options.Insert (0, tempData);
		} else {
			dropDown.options.Add (tempData);
		}
	}

	private void _refreshPieceDropDown()
	{
		m_pieceDropDown.ClearOptions ();
		_addDropDown (m_pieceDropDown, "__null__");
		for (int i = 0; i < _pieceNames.Count; i++)
		{
			_addDropDown (m_pieceDropDown, _pieceNames [i]);
		}
		m_pieceDropDown.value = 0;
		m_pieceDropDown.RefreshShownValue ();
	}

	protected override void OnUpdate()
	{

	}

	public void OnTypeDropChange()
	{
		Debug.Log ("OnClickSave " + m_typeDropDown.value.ToString());

		_editTp = (EditType) m_typeDropDown.value;
		m_addButton.gameObject.SetActive(_editTp == EditType.MAP);
		m_pieceDropDown.gameObject.SetActive (_editTp != EditType.PIECE);
		if (m_pieceDropDown.gameObject.activeSelf) {
			_refreshPieceDropDown ();
		}
		_refreshFileDropDown ();
		_editMapMng.ClearGameObject ();
	}

	public void OnFileDropChange()
	{
		m_inputField.text = m_fileDropDown.captionText.text;
	}

	public void OnPieceDropChange()
	{
		
	}
		
	public void OnClickOpen()
	{
		Debug.Log ("OnClickOpen");

		if (_editTp == EditType.MAP) {
			if (m_inputField.text != "__create__") {
				_editMapMng.OpenMap (m_inputField.text);
			} else {
				string fileName = _mapCreateName + (_mapNames.Count + 1);
				_editMapMng.CreateMap (fileName);
				m_inputField.text = fileName;
				_addDropDown (m_fileDropDown, fileName, true);

				m_fileDropDown.value = 0;
				m_fileDropDown.RefreshShownValue ();

				_mapNames.Insert (0, fileName);
			}
		}else if(_editTp == EditType.ZONE){
			if (m_inputField.text != "__create__") {
				_editMapMng.OpenZone (m_inputField.text);
			} else {
				string fileName = _zoneCreateName + (_zoneNames.Count + 1);
				_editMapMng.CreateZone (fileName);
				m_inputField.text = fileName;
				_addDropDown (m_fileDropDown, fileName, true);

				m_fileDropDown.value = 0;
				m_fileDropDown.RefreshShownValue ();

				_zoneNames.Insert (0, fileName);
			}
		} else {
			if (m_inputField.text != "__create__") {
				_editMapMng.OpenPiece (m_inputField.text);
			} else {
				string fileName = _pieceCreateName + (_pieceNames.Count + 1);
				_editMapMng.CreatePiece (fileName);
				m_inputField.text = fileName;
				_addDropDown (m_fileDropDown, fileName, true);

				m_fileDropDown.value = 0;
				m_fileDropDown.RefreshShownValue ();

				_pieceNames.Insert (0, fileName);
			}
		}
	}

	public void OnClickSave()
	{
		Debug.Log ("OnClickSave " + _editTp.ToString());
		if (_editTp == EditType.MAP) {
			_editMapMng.SaveMap (m_inputField.text);
		}else if(_editTp == EditType.ZONE){
			_editMapMng.SaveZone (m_inputField.text);
		} else {
			_editMapMng.SavePiece (m_inputField.text);
		}
	}

	public void OnClickAdd()
	{
		Debug.Log ("OnClickAdd");
		_editMapMng.AddZone ();
	}

}
