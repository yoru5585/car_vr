using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class WayPointsToArea : MonoBehaviour
    {

        public WaypointCircuit circuit; 
        public Material material;
        public float areawidth = 4;
        public float areaoffset = 2; // widthの半分
        public float areaheight = 2;
        public float arealengthmag = 1.1f;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        [ContextMenu("Create")]
        private void Create()
        {

            Vector3 v3f = circuit.Waypoints[0].transform.position; //起点を設定
            for (int i = 1; i < circuit.Waypoints.Length; i++)
            {

                Vector3 v3fwp = circuit.Waypoints[i].transform.position;
                float distancewp = Vector3.Distance(v3f, v3fwp);
                if (distancewp < 5) { // 5m未満なら次のWayPointへ
                    continue;
                }

                GameObject go   = GameObject.CreatePrimitive (PrimitiveType.Cube);
                go.name = "OCube";
                go.layer = LayerMask.NameToLayer("Ignore Raycast");
                go.GetComponent<BoxCollider>().isTrigger = true;
                go.GetComponent<MeshRenderer>().material = material;

                go.transform.parent = transform;
                go.transform.position = v3f;//一度位置を決めて向きを設定して位置を決め直す
                go.transform.LookAt(circuit.Waypoints[i].transform);
                go.transform.position = (v3f+v3fwp)/2-go.transform.right*areaoffset;
                go.transform.localScale = new Vector3(areawidth, areaheight, distancewp*arealengthmag); //距離はギャップを消すためmag倍


                //次の起点を設定
                v3f = v3fwp;
            }



        }    
        [ContextMenu("Delete")]
        private void Delete(){
            var parent = transform;
            var children = new Transform[parent.childCount];
            var childIndex = 0;

            foreach (Transform child in parent)
            {
                children[childIndex++] = child;
            }            
            foreach (Transform child in children)
            {
                DestroyImmediate(child.gameObject);
            }    

        }            
    }
}