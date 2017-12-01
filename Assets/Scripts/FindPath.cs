using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Stone.Core;

public class FindPath : BaseBehaviour {
	
	public NavMeshAgent agent;  
	Vector3 point;  
	Ray aray;  
	RaycastHit ahit;

	bool mDoFindPath = false;

	protected override void OnInitFirst()  
	{  
		Debuger.LogAtFrame("OnInitFirst");
	}  

	protected override void OnInitSecond()  
	{  
		
	} 

	protected override void OnUpdate()
	{
		if (agent == null)  
		{  
			return;  
		}  

		if (agent.isOnNavMesh == false && agent.isOnOffMeshLink == false)  
		{  
			return;  
		}

		if (Input.GetMouseButtonDown (0)) {
			aray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (aray, out ahit)) {
				point = ahit.point;
			}
			agent.SetDestination (point);
			mDoFindPath = true;
		}

		if (mDoFindPath && !agent.pathPending && agent.remainingDistance != Mathf.Infinity
		   && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= agent.stoppingDistance) {
			Debug.Log("NavMeshAgent Pathfinding End"); 
			mDoFindPath = false;
			agent.ResetPath ();
		}
	}

	void OnGUI()  
	{  
		if (GUILayout.Button("Button"))  
		{  
			//寻找导航网格上，距离玩家最近的点,然后把玩家设置为点的坐标，就脱离卡死了。  
			NavMeshHit tmpNavMeshHit;  
			if (NavMesh.SamplePosition(transform.position, out tmpNavMeshHit, 10000f, NavMesh.AllAreas))  
			{  
				transform.position = tmpNavMeshHit.position;  

				agent.enabled = false;  
				agent.enabled = true;  
			}  
		}  
	}  
}