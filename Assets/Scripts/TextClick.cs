using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stone.Comp;
using Stone.Util;
using UnityEngine.SceneManagement;

public class TextClick : BaseBehaviour {

	protected override void OnInitFirst()  
	{  
		Debuger.LogAtFrame("OnInitFirst");
	}  

	protected override void OnInitSecond()
	{  

	}

	protected override void OnUpdate()  
	{  
		
	} 

	public void Click()  
	{  
		Debuger.LogAtFrame("btn click");  
		SceneManager.LoadScene ("HelloScene");
	} 
}
