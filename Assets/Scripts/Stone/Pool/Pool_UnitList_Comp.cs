using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Stone.Pool
{
	public class Pool_UnitList_Comp : Pool_UnitList<Pooled_BehaviourUnit>
	{
		protected Pool_Comp m_pool;
		public void setPool(Pool_Comp pool)
		{
			m_pool = pool;
		}
		protected override Pooled_BehaviourUnit createNewUnit<UT>() 
		{
			GameObject result_go = null;
			if (m_template != null && m_template is GameObject)
			{
				result_go = GameObject.Instantiate((GameObject)m_template);
			}
			else
			{
				result_go = new GameObject();
				result_go.name = typeof(Pooled_BehaviourUnit).Name;
			}
			result_go.name =result_go.name + "_"+m_createdNum;
			Pooled_BehaviourUnit comp = result_go.GetComponent<Pooled_BehaviourUnit>();
			if (comp == null)
			{
				comp = result_go.AddComponent<Pooled_BehaviourUnit>();
			}
			comp.DoInit();
			return comp;
		}

		protected override void OnUnitChangePool(Pooled_BehaviourUnit unit)
		{
			if (m_pool != null)
			{
				m_pool.OnUnitChangePool(unit);
			}
		}
	}
}