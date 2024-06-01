using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFinder : MonoBehaviour {

    public GameObject wayPoint;
    public GameObject current;
    public int index;
    public int type;
    public int count;
    public Transform [] transforms;
    float onesec;
    public float interval = 0.5f;
    public List<int> types;


    public string roadname = "Road";
    public string waypointname = "WaypointUP";



	// Use this for initialization
	void Start () {
            if (wayPoint == null) {
                GameObject rgo = GameObject.Find(roadname).gameObject;
                Debug.Log(rgo.name);
                GameObject wgo = rgo.transform.Find("Waypoint").gameObject;
                wayPoint = wgo.transform.Find(waypointname).gameObject;
            types = rgo.GetComponent<RoadLoader>().types;
            Debug.Log("types "+types.Count);
            }
        transforms = wayPoint.transform.GetComponentsInChildren<Transform>();
        count = transforms.Length-1;
        Debug.Log(transforms.Length);
        //transforms.Remove(wayPoint);
        //Debug.Log(transforms.Count);
        GameObject go = FindBlockALL();
        if (go != null) {
            Debug.Log(go.name);
        }
	}
	
	// Update is called once per frame
	void Update () {
        onesec += Time.deltaTime;
        if (onesec > interval) {
            FindBlock();  
            if (0 <= index && index < types.Count) {
                type = types[index];
            } else {
                type = -1;
            }
            onesec -=interval;
        }
		
	}

//    public static T[] GetComponentsInChildrenWithoutSelf<T>(this GameObject self) where T : Component
//    {
//        return self.GetComponentsInChildren<T>().Where(c => self != c.gameObject).ToArray();
//    }

    GameObject FindBlockALL(){
        //var transforms = wayPoint.transform.GetComponentsInChildren<Transform>();
        Debug.Log("FindBlockALL");
        for(int i = 1; i < transforms.Length; i++) {
            Transform child = transforms[i];
            if (child == wayPoint.transform) {
                continue;
            }
            Debug.Log(child.gameObject.name+" "+child.position+" "+child.InverseTransformPoint(transform.position));
            float z = child.InverseTransformPoint(transform.position).z;
            if (0 <= z && z < 10) {
                current = child.gameObject;
                index = i-1;
                return child.gameObject;
            }
        }
        current = null;
        index = 0;
        return null;

//        foreach (Transform child in transforms) {
//            if (child == wayPoint.transform) {
//                continue;
//            }
//            Debug.Log(child.gameObject.name+" "+child.position+" "+child.InverseTransformPoint(transform.position));
//            float z = child.InverseTransformPoint(transform.position).z;
//            if (0 <= z && z < 10) {
//                current = child.gameObject;
//                return child.gameObject;
//            }
//        }


    }

    GameObject FindBlockNext(){
        //var transforms = wayPoint.transform.GetComponentsInChildren<Transform>();
        Debug.Log("FindBlockNext");
        for(int i = (index>0 ? index-1 :index); i < transforms.Length; i++) {
            Transform child = transforms[i];
            if (child == wayPoint.transform) {
                continue;
            }
            Debug.Log(child.gameObject.name+" "+child.position+" "+child.InverseTransformPoint(transform.position));
            float z = child.InverseTransformPoint(transform.position).z;
            if (0 <= z && z < 10) {
                current = child.gameObject;
                index = i-1;
                return child.gameObject;
            }
        }
        current = null;
        index = 0;
        return null;

    }
    void FindBlock(){
        if (current != null ){
            float z = current.transform.InverseTransformPoint(transform.position).z;
            if (!(0 <= z && z < 10)) {
                // 場所が変わったので探しなおす
                current = FindBlockNext();
                if (current == null) {
                    FindBlockALL();
                }
            }
        } else {
            current = FindBlockALL();
        }





    }
}
