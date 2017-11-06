using UnityEngine;
//放置物体可以使用这个脚本，比如建设养成类 可以使用这个脚本来进行物品放置

	/** Small sample script for placing obstacles */
	
	public class ObjectPlacer : MonoBehaviour {
        //放置道具的预制,可以做些变化，随机赋值之类
        public GameObject go;

		/** Flush Graph Updates directly after placing. Slower, but updates are applied immidiately */
		public bool direct = false;

		/** Issue a graph update object after placement */
		public bool issueGUOs = true;

		/** 检测添加或者移除物体,可以做修改进行物品的添加是释放 */
		void Update () {
        //鼠标 左键添加 右键移除
        if (Input.GetMouseButton(0))
        {
            PlaceObject();
        }
        if(Input.GetMouseButton(1)) {
            RemoveObject();
        }

        /* 按键移除
         * if (Input.GetKeyDown("r")) {
                 RemoveObject();
             }

         */
    }
        //设置障碍物
		public void PlaceObject () {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
        
        // Figure out where the ground is
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            
            if (hit.transform.gameObject.tag != "Ground") return;
            //获取放置面的坐标
            Vector3 p = hit.point;
            //获取 传入的坐标点 获取最近的 可行走路径点坐标
            // var node = AstarPath.active.GetNearest(p, NNConstraint.None).node;
       
            GameObject obj = GameObject.Instantiate(go, p, go.transform.rotation) as GameObject;
            obj.transform.SetParent(hit.collider.gameObject.transform.parent.gameObject.transform, true);
            /** 
                if (issueGUOs) {
                    Bounds b = obj.GetComponent<Collider>().bounds;
                    GraphUpdateObject guo = new GraphUpdateObject(b);
                    AstarPath.active.UpdateGraphs(guo);
                    if (direct) {
                        AstarPath.active.FlushGraphUpdates();
                    
                    } 
                }*/
        }
    }

		public void RemoveObject () {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// Check what object is under the mouse cursor
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				// Ignore ground and triggers
				if (hit.collider.isTrigger || hit.transform.gameObject.tag != "Obstacle") return;
                //TODO  清除物体的父级
				
				Destroy(hit.collider.gameObject.transform.parent.gameObject);

            /** 
              if (issueGUOs) {
                GraphUpdateObject guo = new GraphUpdateObject(b);
                AstarPath.active.UpdateGraphs(guo);
                if (direct) {
                    AstarPath.active.FlushGraphUpdates();
                } 
        }*/
        }
    }
	}
