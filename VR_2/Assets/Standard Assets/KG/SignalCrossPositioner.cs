using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalCrossPositioner : MonoBehaviour {

    public GameObject wayPoint;
    public GameObject current;
    public int index;
    public int type;
    public int count;
    public Transform [] transforms;
    float onesec;
    public float interval = 0.5f;
    List<int> types;


    public string roadname = "Road";
    public string waypointname = "CENTER";


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
        // Create();
    }

    public void DeleteALL(){
        Transform[] transformList = transform.GetComponentsInChildren<Transform>();

        foreach( Transform trans in transformList ){
            if ( trans != null) 
            if( trans != transform ){

                trans.parent = null;

                GameObject.DestroyImmediate(trans.gameObject);

            }

        }
    }

    void LoadRoadData(){

    }

    public void GetPositions(){

        Start();
        Debug.Log("GetPosition");
        List <Transform> transList = new List<Transform>();
        for(int i = 0; i < types.Count; i++) {
            if (types[i] == 2) {
                transList.Add(transforms[i+1]);
                Debug.Log(i);
            }                  
        }
        transforms = new Transform[transList.Count];
        transforms = transList.ToArray();
    }

    public void SetPosition(){
        for(int i = 0; i < transforms.Length; i++) {
            string signalcrossname = "SignalCross "+string.Format("{0:D3}",i);
            Debug.Log(signalcrossname);
            Transform tr = transform.Find(signalcrossname);
            tr.gameObject.transform.position = transforms[i].transform.position;;
            tr.gameObject.transform.localRotation = transforms[i].transform.localRotation;;



            //go.transform.localRotation = Quaternion.FromToRotation(Vector3.left,this.sections[i].direction);
        }



    }
    public void Create(){
        GetPositions();

        for(int i = 0; i < transforms.Length; i++) {
            
        // プレハブを取得
            GameObject prefab = (GameObject)Resources.Load ("Prefabs/SignalCross");
        // プレハブからインスタンスを生成
            var go = GameObject.Instantiate (prefab, transforms[i].position, transforms[i].transform.rotation,transform);

            go.name = "SignalCross "+string.Format("{0:D3}",i);

            //go.transform.localRotation = Quaternion.FromToRotation(Vector3.left,this.sections[i].direction);
        }

    }
    // Update is called once per frame
    void Update () {

    }
}
