using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Stone.Core
{
	public class Role
	{
		public Role ()
		{
		}

		// 唯一性id
		public int uid;

		// 名字
		public string name;
		// 第几代
		public int version;
		// 等级
		public int level;
		// 经验
		public int exp;

		// ======战斗属性======
		// 血量
		public int hp;
		// 血量上限
		public int hpmax;
		// 攻击
		public int atk;
		// 攻击距离
		public int atkRange;
		// 行动优先值
		public int priority;
		// 移动距离
		public int moveRange;

	}
}

