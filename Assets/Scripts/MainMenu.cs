using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using Stone.Comp;
using Stone.Hex;
using System.Xml.Serialization;

public class MainMenu : BaseBehaviour {

	public GameObject m_mapMng;

	public Button m_addButton;

	public Dropdown m_dropDown;
	public InputField m_inputField;
	
	public Toggle m_tgZone;
	
	private string _zoneCreateName = "zone_";
	private string _mapCreateName = "map_";

	private List<string> _mapNames;
	private List<string> _zoneNames;

	private EditMapMng _editMapMng;

	protected override void OnInitFirst()  
	{  
		_mapNames = FileUtilEx.GetForldNames(Res.MapPath);
		_zoneNames = FileUtilEx.GetFileNames(Res.ZonePath, Res.ConfExt);
	}  


	protected override void OnInitSecond()
	{  
		m_addButton.interactable = !m_tgZone.isOn;
		_editMapMng = m_mapMng.GetComponent<EditMapMng> ();
		_refreshDropDown ();
	}

	private void _refreshDropDown()
	{
		List<string> names = m_tgZone.isOn ? _zoneNames : _mapNames;
		m_dropDown.options.Clear ();
		
		for (int i = 0; i < names.Count; i++)
		{
			_addDropDown (names [i]);
		}

		_addDropDown ("__create__");

		m_dropDown.value = 0;
		m_dropDown.RefreshShownValue ();
		m_inputField.text = m_dropDown.captionText.text;
	}

	private void _addDropDown(string name, bool isFirst=false)
	{
		Dropdown.OptionData tempData = new Dropdown.OptionData();
		tempData.text = name;
		if (isFirst) {
			m_dropDown.options.Insert (0, tempData);
		} else {
			m_dropDown.options.Add (tempData);
		}
	}

	protected override void OnUpdate()
	{

	}

	public void OnClickOpen()
	{
		Debug.Log ("OnClickOpen");

		if (m_tgZone.isOn) {
			if (m_inputField.text != "__create__") {
				_editMapMng.OpenZone (m_inputField.text);
			} else {
				string fileName = _zoneCreateName + (_zoneNames.Count + 1);
				_editMapMng.CreateZone (fileName);
				m_inputField.text = fileName;
				_addDropDown (fileName, true);

				m_dropDown.value = 0;
				m_dropDown.RefreshShownValue ();

				_zoneNames.Insert (0, fileName);
			}
		} else {
			if (m_inputField.text != "__create__") {
				_editMapMng.OpenMap (m_inputField.text);
			} else {
				string fileName = _mapCreateName + (_mapNames.Count + 1);
				_editMapMng.CreateMap (fileName);
				m_inputField.text = fileName;
				_addDropDown (fileName, true);

				m_dropDown.value = 0;
				m_dropDown.RefreshShownValue ();

				_mapNames.Insert (0, fileName);
			}
		}
	}

	public void OnClickSave()
	{
		Debug.Log ("OnClickSave " + m_tgZone.isOn.ToString());
		if (m_tgZone.isOn) {
			_editMapMng.SaveZone (m_inputField.text);
		} else {
			_editMapMng.SaveMap (m_inputField.text);
		}
	}

	public void OnClickAdd()
	{
		Debug.Log ("OnClickAdd");
		_editMapMng.AddZone ();
	}

	public void OnDropChange()
	{
		m_inputField.text = m_dropDown.captionText.text;
	}
	
	public void OnToggleZone(bool b)
	{
		Debug.Log ("OnToggleZone " + b.ToString());
		m_addButton.interactable = !m_tgZone.isOn;
		_refreshDropDown ();
		_editMapMng.ClearGameObject ();
	}
}
