using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLoader : MonoBehaviour {

    public RoadCreator2      road_creator;

//    public string courseText = "0,0,0,0,0";
//    public string [] courseTextArray;
//    public string [] courseTypeArray;

    List <string> courseTextList = new List<string>();
    List <string> courseTypeList = new List<string>();
    List <string> courseBuiLList = new List<string>();
    List <string> courseBuiRList = new List<string>();



    private List<Vector3>    positions = new List<Vector3>();
    public List<int> types  = new List<int>();
    private List<int> buildingL= new List<int>();
    private List<int> buildingR= new List<int>();

    public Material         material = null;
    public Material         road_material = null;
    public Material         wall_material = null;
    public PhysicMaterial   physic_material = null;

    public GameObject goBase;



	// Use this for initialization
	void Start () {
        
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

        courseTextList = new List<string>();
        courseTypeList = new List<string>();
        courseBuiLList = new List<string>();
        courseBuiRList = new List<string>();

        positions = new List<Vector3>();
        types  = new List<int>();
        buildingL= new List<int>();
        buildingR= new List<int>();
        
        TextAsset textasset = Resources.Load<TextAsset>("Road01"); //new TextAsset();
        string [] lines;
        //textasset = Resources.Load("Road01.csv",typeof(TextAsset))as TextAsset;

        //Debug.Log(textasset.text);
        lines = textasset.text.Split('\n');
        Debug.Log(lines.Length);
        for(int i = 0; i < lines.Length; i++) {
            string [] csv = lines[i].Split(',');
            if (csv.Length >= 1 && csv[0].Length>0) {
                courseTextList.Add(csv[0]);
                if (csv.Length >= 2 && csv[1].Trim().Length>0) {
                    
                    //Debug.Log(i+" csv1len"+csv[1].Length);
                    courseTypeList.Add(csv[1]);
                } else {
                    courseTypeList.Add("0");
                    //Debug.Log("add0");
                }
                if (csv.Length >= 3 && csv[2].Trim().Length>0) {
                    courseBuiLList.Add(csv[2]);
                } else {
                    courseBuiLList.Add("0");
                }
                if (csv.Length >= 4 && csv[3].Trim().Length>0) {
                    courseBuiRList.Add(csv[3]);
                } else {
                    courseBuiRList.Add("0");
                }
            }

        }
    }



    public  void Create(){
        DeleteALL();
//        for( int i=0; i < transform.childCount; ++i ){
//            GameObject.DestroyImmediate( transform.GetChild( i ).gameObject );
//            Debug.Log(transform.GetChild( i ).gameObject);
//        }

//        Transform[] transformList = transform.GetComponentsInChildren<Transform>();
//
//        foreach( Transform trans in transformList ){
//
//            if( trans != transform ){
//
//                trans.parent = null;
//
//                GameObject.DestroyImmediate(trans.gameObject);
//
//            }
//
//        }
//        foreach(Transform child in transform) {
//            Debug.Log(child);
//            GameObject.DestroyImmediate( child.gameObject);
//
//        }
        LoadRoadData();

        goBase = this.transform.gameObject;


            
        this.road_creator   = new RoadCreator2();
        road_creator.goBase = goBase;
        GameObject flag = new GameObject("Created");
        flag.transform.parent = goBase.transform;
        GameObject waypointsBase = new GameObject("Waypoint");
        waypointsBase.transform.parent = goBase.transform;
        road_creator.waypointsBase = waypointsBase;

        positions = new List<Vector3>();
        types = new List<int>();

        //string [] courseTextArray = courseText.Split(',');

        Vector3 vbase = new Vector3(0,0,10);
        Vector3 vi = transform.position;
        positions.Add(vi);
        float deg = transform.eulerAngles.y;
        //Debug.Log(deg);
        for(int i = 0; i < courseTextList.Count; i++) {
            //Debug.Log(i+ " "+ courseTextList[i]+" "+ courseTypeList[i]);
            float degdiff = float.Parse(courseTextList[i]);
            int type = int.Parse(courseTypeList[i]);

            deg += degdiff;
            Debug.Log(i+" "+degdiff+ " "+deg+" "+ courseBuiLList[i]+ " "+ courseBuiRList[i]);
            Vector3 v2 = Quaternion.AngleAxis(deg, Vector3.up)*vbase;
            vi = vi + v2;

            positions.Add(vi);
            types.Add(type);
            buildingL.Add(int.Parse(courseBuiLList[i]));
            buildingR.Add(int.Parse(courseBuiRList[i]));
        }
        types.Add(0);
        buildingL.Add(0);
        buildingR.Add(0);
//
//        for(int i = 0; 
//
//
//        positions.Add(new Vector3(0,0,0));
//        positions.Add(new Vector3(0,0,10));
//
//        Vector3 vb = new Vector3(0,0,10);
//        Vector3 v1 = new Vector3(0,0,10);
//        Vector3 v2 = Quaternion.AngleAxis(90, Vector3.up)*vb;
//        Vector3 v3 = v1 + v2;
//        Vector3 v4 = Quaternion.AngleAxis(180, Vector3.up)*vb;
//        Vector3 v5 = v3 + v4;
//
//
//        positions.Add(v3);
//        positions.Add(v5);


        List<int>       junction_points = new List<int>();
        junction_points.Add(0);
        junction_points.Add(this.positions.Count - 1);
        junction_points.Sort();

        List<int>   split_points = new List<int>();

        split_points.Add(junction_points[0]);

        for(int i = 0;i < junction_points.Count - 1;i++) {

            split_points.Add((int)Mathf.Lerp((float)junction_points[i], (float)junction_points[i + 1], 1.0f/3.0f));
            split_points.Add((int)Mathf.Lerp((float)junction_points[i], (float)junction_points[i + 1], 2.0f/3.0f));
        }

        split_points.Add(junction_points[junction_points.Count - 1]);

        //

        this.road_creator.split_points = split_points.ToArray();


        List<RoadCreator2.HeightPeg>     pegs = new List<RoadCreator2.HeightPeg>();
        pegs.Add(new RoadCreator2.HeightPeg(0, 0.0f));
        this.road_creator.height_pegs = pegs.ToArray();
		
        //this.road_creator.positions       = this.line_drawer.positions;
        this.road_creator.positions       = this.positions;
        this.road_creator.types       = this.types;
        this.road_creator.buildingL       = this.buildingL;
        this.road_creator.buildingR       = this.buildingR;

        this.road_creator.material        = this.material;
        this.road_creator.road_material   = this.road_material;
        this.road_creator.wall_material   = this.wall_material;
        this.road_creator.physic_material = this.physic_material;
        //this.road_creator.peak_position   = this.junction_finder.junction.i0;
        this.road_creator.peak_position   = 0;

        this.road_creator.createRoad();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
