using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stone.Util;
using Stone.Comp;

public class CompTest : BaseBehaviour {

	protected override void OnInitFirst()  
	{  
		Debuger.LogAtFrame("OnInitFirst");  
	}  

	protected override void OnInitSecond()  
	{  
		Debuger.LogAtFrame("OnInitSecond");  
		Debuger.LogAtFrame ("transform.childCount = " + m_transform.childCount);
		Debuger.LogAtFrame ("transform.hierarchyCount = " + m_transform.hierarchyCount);
		Debuger.LogAtFrame ("transform.hierarchyCapacity = " + m_transform.hierarchyCapacity);

		Debuger.LogAtFrame ("transform.parent = " + m_transform.parent);

		Debuger.LogAtFrame ("transform.GetSiblingIndex = " + m_transform.GetSiblingIndex());
	}  

	protected override void OnUpdate()  
	{  
		Debuger.LogAtFrame("OnUpdate");  
	}  

	protected void OnEnable()  
	{  
		Debuger.LogAtFrame("OnEnable");  
	}  
	protected void OnDisable()  
	{  
		Debuger.LogAtFrame("OnDisable");  
	}  
	protected void OnDestroy()  
	{  
		Debuger.LogAtFrame("OnDestroy");  
	}  
}
