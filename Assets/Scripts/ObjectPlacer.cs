using UnityEngine;
//�����������ʹ������ű������罨�������� ����ʹ������ű���������Ʒ����

	/** Small sample script for placing obstacles */
	
	public class ObjectPlacer : MonoBehaviour {
        //���õ��ߵ�Ԥ��,������Щ�仯�������ֵ֮��
        public GameObject go;

		/** Flush Graph Updates directly after placing. Slower, but updates are applied immidiately */
		public bool direct = false;

		/** Issue a graph update object after placement */
		public bool issueGUOs = true;

		/** �����ӻ����Ƴ�����,�������޸Ľ�����Ʒ��������ͷ� */
		void Update () {
        //��� ������ �Ҽ��Ƴ�
        if (Input.GetMouseButton(0))
        {
            PlaceObject();
        }
        if(Input.GetMouseButton(1)) {
            RemoveObject();
        }

        /* �����Ƴ�
         * if (Input.GetKeyDown("r")) {
                 RemoveObject();
             }

         */
    }
        //�����ϰ���
		public void PlaceObject () {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
        
        // Figure out where the ground is
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            
            if (hit.transform.gameObject.tag != "Ground") return;
            //��ȡ�����������
            Vector3 p = hit.point;
            //��ȡ ���������� ��ȡ����� ������·��������
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
                //TODO  �������ĸ���
				
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
