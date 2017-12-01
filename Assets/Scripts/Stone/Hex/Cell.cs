using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Stone.Core  
{
	public class Cell
	{
		public Cell()
		{
			
		}

		public enum State
		{
			WALK,   //可行走
			REMOVE,	//可移除的障碍物
			STATIC  //不可移除的障碍物
		}

		public enum ObstState
		{
			SINGLE,   	//单格障碍物
			PARENT,		//多格障碍物的主体
			CHILD		//多格障碍物的子体
		}

		public Cell(Hex zoneHex, Hex hex)
		{
			this.zoneHex = zoneHex;
			this.hex = hex;
		}
		// 区域Hex
		public Hex zoneHex;
		// 区域内的Hex
		public Hex hex;

		// 可行走／不可行走
		public State state = State.WALK;

		// 地板pf
		public string groundPfName;
		// 触发器pf
		public string triggerPfName;
		// 触发效果 id
		public int triggerId;

		// 是否障碍物已经满了，不能通过地图编辑再插入
		public bool isFullObst = false;
		// 障碍物列表（多个）
		public List<string> obstList;
		// 障碍物位置列表（多个）
		public List<Vector3> obstPosList;

		// 处理占多格的障碍物
		public ObstState obstState = ObstState.SINGLE;
		// 该格的障碍物是其他格的一部分，主障碍物的相对位置
		public Hex mainHex;
		// 该格的障碍物是主障碍物，其他部分在的相对位置
		public List<Hex> partHexs;

		private Hex _realHex;	//真实地图的hex
		[XmlIgnore]
		public Hex realHex { 
			get { return _realHex; } 
			set { 
				if (!Hex.Equals (_realHex, value)) {
					_realHex = value;
					_realName = "hex_" + realHex.q + "_" + realHex.r;
				}
			} 
		}

		//真实地图的hex的名字
		private string _realName="hex_0_0";
		public string realName { 
			get { return _realName; }  
		}

		private Point _point;	//真实地图的hex
		[XmlIgnore]
		public Point point { 
			get { return _point; } 
			set { 
				_point = value;
			} 
		}

		private Unit _unit;
		[XmlIgnore]
		public Unit unit { 
			get { return _unit; } 
			set { 
				_unit = value;
			} 
		}

		// 是否可以行走，障碍物／有单位 都不能行走
		public bool IsWalkable()
		{
			return this.state == State.WALK && _unit == null;
		}

		public bool IsObstacle()
		{
			return this.state != State.WALK;
		}

		public bool IsFullObst()
		{
			if (IsWalkable ()) {
				return false;
			} else {
				return isFullObst;
			}
		}

		public void AddObstacle(string pfName, Vector3 pos)
		{
			state = State.REMOVE;

		}

		public void RemoveObstacle(string pfName, Vector3 pos)
		{

		}

		public void UpdateObstacle(string pfName, Vector3 pos)
		{

		}

		public void Dump(string msg="")
		{
			Debug.Log (msg);
			Debug.Log ("zoneHex q = " + zoneHex.q + " r = " + zoneHex.r + " s = " + zoneHex.s);
			Debug.Log ("hex q = " + hex.q + " r = " + hex.r + " s = " + hex.s);
			Debug.Log ("state " + state);
			Debug.Log ("groundPfName " + groundPfName);
			Debug.Log ("triggerPfName " + triggerPfName);
		}

		public int Distance(Cell cell)
		{
			return Hex.Distance (this.realHex, cell.realHex);
		}
	}
}