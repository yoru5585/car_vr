using System;
using UnityEngine;
using Random = UnityEngine.Random;
namespace UnityStandardAssets.Utility
{
    public class WaypointProgressTracker : MonoBehaviour
    {
        // This script can be used with any object that is supposed to follow a
        // route marked out by waypoints.

        // This script manages the amount to look ahead along the route,
        // and keeps track of progress and laps.

        [SerializeField] public WaypointCircuit circuit; // A reference to the waypoint-based route we should follow
        // private -> public に KG
        [SerializeField] private float lookAheadForTargetOffset = 5;
        // The offset ahead along the route that the we will aim for

        [SerializeField] private float lookAheadForTargetFactor = .1f;
        // A multiplier adding distance ahead along the route to aim for, based on current speed

        [SerializeField] private float lookAheadForSpeedOffset = 10;
        // The offset ahead only the route for speed adjustments (applied as the rotation of the waypoint target transform)

        [SerializeField] private float lookAheadForSpeedFactor = .2f;
        // A multiplier adding distance ahead along the route for speed adjustments

        [SerializeField] private ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
        // whether to update the position smoothly along the route (good for curved paths) or just when we reach each waypoint.

        [SerializeField] private float pointToPointThreshold = 4;
        // proximity to waypoint which must be reached to switch target to next waypoint : only used in PointToPoint mode.

        public enum ProgressStyle
        {
            SmoothAlongRoute,
            PointToPoint,
        }

        // these are public, readable by other objects - i.e. for an AI to know where to head!
        public WaypointCircuit.RoutePoint targetPoint { get; private set; }
        public WaypointCircuit.RoutePoint speedPoint { get; private set; }
        public WaypointCircuit.RoutePoint progressPoint { get; private set; }

        public Transform target;

        public float progressDistance; // The progress round the route, used in smooth mode.
        // private -> public に KG
        public int progressNum; // the current waypoint number, used in point-to-point mode.
        public Vector3 lastPosition; // Used to calculate current speed (since we may not have a rigidbody component)
        public float speed; // current speed of this object (calculated from delta since last frame)


        public Vector3 initpos;
        public Quaternion initqua;

        public enum CarType
        {
            GO,
            AROUNDMYCAR,
            MYCARLANECARBEHIND,
            MYCARLANECARINFRONT,
        }

        public CarType type;

        public GameObject mycar;
        private BlockFinder blockFinder;
        public string debug;

        public bool active;

        int frontDisappear = 190;
        int frontAppear = 170;
        int backAppear = 130;
        int backDisappear = 150;
        int randomAppear = 20;



        // setup script properties
        private void Start()
        {
                this.mycar = GameObject.Find("Mycar");

                blockFinder = mycar.GetComponent<BlockFinder>();//使ってない

            if (circuit == null) {

                Cars cars = transform.parent.gameObject.GetComponent<Cars>();
                //Debug.Log("Cars "+cars);

                GameObject rgo = GameObject.Find(cars.roadname).gameObject;
                GameObject wgo = rgo.transform.Find("Waypoint").gameObject;
                GameObject wpgo = wgo.transform.Find(cars.waypointname).gameObject;
                circuit = wpgo.GetComponent<WaypointCircuit>();

                //Debug.Log(transform.name+" "+wpgo);

                //circuit = cars.circuit;
            }


            // we use a transform to represent the point to aim for, and the point which
            // is considered for upcoming changes-of-speed. This allows this component
            // to communicate this information to the AI without requiring further dependencies.

            // You can manually create a transform and assign it to this component *and* the AI,
            // then this component will update it, and the AI can read it.
            if (target == null)
            {
                target = new GameObject(name + " Waypoint Target").transform;
            }
            Reset();
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//            if (type == CarType.GO) {
//                Reset();
//            } else if (type == CarType.AROUNDMYCAR) {
//                ResetAroundMyCar();
//            }
        }

        public void Init(Vector3 pos,Quaternion qua) {
            initpos = pos;
            initqua = qua;
            //Reset(pos,qua);
        }
        public void ResetInitPos(){

            Transform newtrans = circuit.Waypoints[0].transform;

            // 出現位置い物体があるかどうか？

            RaycastHit hit; 
//            Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 
            Vector3 pdiff = new Vector3(0f, 0, 0f);  //Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 

            int distance = 30;  
            float radius = 0.2f;// 0.5f; 
            //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
            //Debug.DrawRay(transform.position,(newtrans.position-transform.position).normalized*distance,Color.blue,0,false);
            //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
            //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
            if (Physics.SphereCast (newtrans.position + pdiff + newtrans.forward*-15,radius,newtrans.forward, out hit, distance)) { 
                Debug.DrawRay(newtrans.position + pdiff+ newtrans.forward*-15, newtrans.forward*distance, Color.red,0,false);
                Debug.Log("Obstacle exists!!!!!"+type);
                active = false;
//                Debug.Break();
            }  else {
                Debug.DrawRay(newtrans.position + pdiff+ newtrans.forward*-15, newtrans.forward*distance, Color.blue,0,false);

                //transform.position = initpos;
                Reset(newtrans.position,newtrans.localRotation);
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                progressDistance = 0;  
            }
        }
        public void ResetAroundMyCar()//対向車線
        {
            Vector3 v3f = mycar.transform.position;
            v3f.z += (frontAppear + Random.Range(0,randomAppear));// 60; // mycarのzの位置より ? m 先の位置に出現

            // 一番近いWaypointを探す
            float mindistance = float.MaxValue;
            int index = -1;
            for (int i = 0; i < circuit.Waypoints.Length; i++)
            {
                Vector3 v3fwp = circuit.Waypoints[i].transform.position;
                float distancewp = Vector3.Distance(v3f, v3fwp);
                if (distancewp < mindistance)
                {
                    index = i;
                    mindistance = distancewp;
                }
            }
            Transform newtrans = circuit.Waypoints[index].transform;
            Debug.Log("Lok"+index+","+circuit.Waypoints.Length);
            if (index + 1 < circuit.Waypoints.Length)
            {
                Debug.Log("LookAt"+newtrans.rotation);
                newtrans.LookAt(circuit.Waypoints[index + 1].transform);
                Debug.Log("After lookAt"+newtrans.rotation);
            }
            // active = false;
            //     transform.position = newtrans.position;
            //     //transform.localRotation = initqua;
            //     transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            RaycastHit hit;
//            Vector3 pdiff = new Vector3(0f, 1.1f, 0f); // 少し上　から前に飛ばす
            Vector3 pdiff = new Vector3(0f, 0, 0f);  //Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 

            int distance = 30;
            float radius = 0.2f;// 0.5f; 
            //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
            //Debug.DrawRay(transform.position,(newtrans.position-transform.position).normalized*distance,Color.blue,0,false);
            //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
            //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
            if (Physics.SphereCast(newtrans.position + pdiff + newtrans.forward * -15, radius, newtrans.forward, out hit, distance))
            {
                Debug.DrawRay(newtrans.position + pdiff + newtrans.forward * -15, newtrans.forward * distance, Color.red, 0, false);
                //Debug.Log("Obstacle exists!!!!!");
                //active = false;
                //                Debug.Break();
            }
            else
            {
                Debug.DrawRay(newtrans.position + pdiff + newtrans.forward * -15, newtrans.forward * distance, Color.blue, 0, false);
                //Debug.Log("Obstacle exists else");

                // progressDistanceの設定
                Vector3 pos = newtrans.position;
                float min = Vector3.Distance(new Vector3(), pos);
                if (circuit.GetDistanceLength() > 20)
                {
                    for (int i = 20; i < circuit.GetDistanceLength(); i += 5)
                    {
                        Vector3 targetposition =
                            circuit.GetRoutePoint(i).position;
                        //Debug.Log(i+" "+targetposition); ///////////////////////////////////////////////////////////////////////////
                        if (Vector3.Distance(targetposition, pos) < min)
                        {
                            progressDistance = i;
                            min = Vector3.Distance(targetposition, pos);

                        }
                    }
                }
                Vector3 psdiff = new Vector3(0f, -0.5f, 0f);  
                transform.position = newtrans.position+psdiff;

                transform.rotation = newtrans.rotation;
                //transform.localRotation = newtrans.localRotation;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                lastPosition = transform.position;
                // progressNum = index;
                // if (index +1 < circuit.Waypoints.Length) {
                //     progressNum = index +1;
                //     target.position = circuit.Waypoints[index +1].position;
                // }
            }
        }

        public void ResetAroundMyCar2(){
            int mycarnumber = 0;

            /*int mycarnumber = (blockFinder.count - blockFinder.index);
             mycarnumber =mycarnumber - 20;
            if (mycarnumber > blockFinder.count -2) {
                mycarnumber = blockFinder.count -2;
            }
            if (mycarnumber < 2) {
                mycarnumber = 2;
            }
            Debug.Log("ResetAroundMyCar"+ mycarnumber+" "+ blockFinder.count+" "+blockFinder.index);*/
            Transform newtrans = circuit.Waypoints[mycarnumber].transform;
            // 出現位置い物体があるかどうか？

            RaycastHit hit; 
            Vector3 pdiff = new Vector3(0f, 0, 0f);  //Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 
            int distance = 30; 
            float radius = 0.2f;// 0.5f; 
            //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
            //Debug.DrawRay(transform.position,(newtrans.position-transform.position).normalized*distance,Color.blue,0,false);
            //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
            //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
            if (Physics.SphereCast (newtrans.position + pdiff + newtrans.forward*-15,radius,newtrans.forward, out hit, distance)) { 
                Debug.DrawRay(newtrans.position + pdiff+ newtrans.forward*-15, newtrans.forward*distance, Color.red,0,false);
                Debug.Log("Obstacle exists!!!!!");
                active = false;
//                Debug.Break();
            }  else {
                Debug.DrawRay(newtrans.position + pdiff+ newtrans.forward*-15, newtrans.forward*distance, Color.blue,0,false);
                Debug.Log("Obstacle exists else");
                Vector3 pos = newtrans.position;
                float min = Vector3.Distance(new Vector3(),pos) ;
                if (circuit.GetDistanceLength() > 20) {
                    for(int i = 20; i < circuit.GetDistanceLength(); i+=5) {
                        Vector3 targetposition =
                            circuit.GetRoutePoint(i).position;
                        //Debug.Log(i+" "+targetposition); ///////////////////////////////////////////////////////////////////////////
                        if (Vector3.Distance(targetposition,pos) < min) {
                            progressDistance = i;
                            min = Vector3.Distance(targetposition,pos) ;

                        }
                    }
                }
            Reset(newtrans.position+new Vector3(0,1,0),newtrans.localRotation);
            transform.position = newtrans.position;
            transform.localRotation = newtrans.localRotation;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                lastPosition = transform.position;
            }

        }

        public void ResetMyCarLineCarBehind()//後ろに出現する処理
        {
            Vector3 v3f = mycar.transform.position;
//            v3f.z -= (backAppear + Random.Range(0,randomAppear)); // mycarのzの位置より ? m 先の位置に出現
            v3f.z -= (backAppear); // mycarのzの位置より ? m 先の位置に出現
            Debug.Log("ResetMyCarLineCarBehind0 "+v3f.z);

            // 一番近いWaypointを探す
            float mindistance = float.MaxValue;
            int index = -1;
            for (int i = 0; i < circuit.Waypoints.Length; i++)
            {
                Vector3 v3fwp = circuit.Waypoints[i].transform.position;
                float distancewp = Vector3.Distance(v3f, v3fwp);
                if (distancewp < mindistance)
                {
                    index = i;
                    mindistance = distancewp;
                }
            }
            Debug.Log("ResetMyCarLineCarBehind1 "+index+" "+mindistance);
            Transform newtrans = circuit.Waypoints[index].transform;
            Debug.Log("Lok"+index+","+circuit.Waypoints.Length);
            if (index + 1 < circuit.Waypoints.Length)
            {
                Debug.Log("LookAt"+newtrans.rotation);
                newtrans.LookAt(circuit.Waypoints[index + 1].transform);
                Debug.Log("After lookAt"+newtrans.rotation);
            }            
            // active = false;
            //     transform.position = newtrans.position;
            //     //transform.localRotation = initqua;
            //     transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            RaycastHit hit;
//            Vector3 pdiff = new Vector3(0f, 1.1f, 0f); // 少し上　から前に飛ばす
            Vector3 pdiff = new Vector3(0f, 0, 0f);  //Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 

            int distance = 20;
            float radius = 0.2f;// 0.5f; 
            //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
            //Debug.DrawRay(transform.position,(newtrans.position-transform.position).normalized*distance,Color.blue,0,false);
            //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
            //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
            bool ok = false;
            if (Physics.SphereCast(newtrans.position + pdiff + newtrans.forward * distance * -0.5f, radius, newtrans.forward, out hit, distance))
            {
                Debug.Log("Obstacle exists!!!!!"+ hit.collider.name);
                Debug.DrawRay(newtrans.position + pdiff + newtrans.forward *distance * -0.5f, newtrans.forward * distance, Color.red, 10, false);
                //active = false;
                //                Debug.Break();
                // if (hit.collider.name.Substring(0,3)!="Car") {
                //     ok = true;
                //     Debug.Log("Obstacle exists!!!!! not car"+ hit.collider.name);
                // }
            }
            else
            {
                Debug.Log("Obstacle exists else");
                Debug.DrawRay(newtrans.position + pdiff + newtrans.forward * distance * -0.5f, newtrans.forward * distance, Color.blue, 10, false);
                ok = true;
            }
            if (ok) {
                // progressDistanceの設定

                Vector3 pos = newtrans.position;
                float min = Vector3.Distance(new Vector3(), pos);
                if (circuit.GetDistanceLength() > 20)
                {
                    for (int i = 20; i < circuit.GetDistanceLength(); i += 5)
                    {
                        Vector3 targetposition =
                            circuit.GetRoutePoint(i).position;
                        //Debug.Log(i+" "+targetposition); ///////////////////////////////////////////////////////////////////////////
                        if (Vector3.Distance(targetposition, pos) < min)
                        {
                            progressDistance = i;
                            min = Vector3.Distance(targetposition, pos);

                        }
                    }
                }
                Vector3 psdiff = new Vector3(0f, -0.5f, 0f);  
                transform.position = newtrans.position+psdiff;

                //float y = transform.position.y;
                //y += 30;
                transform.rotation = newtrans.rotation;


                //キューブを出現させる
                //GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //Cube.transform.position = newtrans.position;

                //transform.localRotation = newtrans.localRotation;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                lastPosition = transform.position;
                // progressNum = index;
                // if (index +1 < circuit.Waypoints.Length) {
                //     progressNum = index +1;
                //     target.position = circuit.Waypoints[index +1].position;
                // }
            }

            /*Vector3 v3f = mycar.transform.position;//maycarの座標
            v3f.z = 10; // mycarのzの位置より ? m 先の位置

            // 一番近いWaypointを探す 最少値を探す処理
            float mindistance = float.MaxValue;
            int index = -1;
            for (int i = 0; i < circuit.Waypoints.Length; i++)//waypoints繰り返す
            {
                Vector3 v3fwp = circuit.Waypoints[i].transform.position;//i番目のwaypointのposition
                float distancewp = Vector3.Distance(v3f, v3fwp);//maycarとi番目のwaypointの距離
                if (distancewp < mindistance)
                {
                    index = i;
                    mindistance = distancewp;
                }
            }

            

            Transform newtrans = circuit.Waypoints[index].transform;
            if (index + 1 < circuit.Waypoints.Length)
            {
                newtrans.LookAt(circuit.Waypoints[index + 1].transform);
            }
            // active = false;
            //     transform.position = newtrans.position;
            //     //transform.localRotation = initqua;
            //     transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            RaycastHit hit;
            Vector3 pdiff = new Vector3(0f, 1.1f, 0f); // 少し上　から前に飛ばす
            int distance = 20;
            float radius = 1f;// 0.5f; 
            //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
            //Debug.DrawRay(transform.position,(newtrans.position-transform.position).normalized*distance,Color.blue,0,false);
            //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
            Debug.DrawRay(newtrans.position + pdiff + newtrans.forward * -10, newtrans.forward * distance, Color.red, 0, false);
            //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
            if (Physics.SphereCast(newtrans.position + pdiff + newtrans.forward * -5, radius, newtrans.forward, out hit, distance))
            {
                Debug.Log("Obstacle exists!!!!!");
                //active = false;
                //                Debug.Break();
            }
            else
            {
                Debug.Log("Obstacle exists else");

                // progressDistanceの設定
                Vector3 pos = newtrans.position;
                float min = Vector3.Distance(new Vector3(), pos);
                if (circuit.GetDistanceLength() > 20)
                {
                    for (int i = 20; i < circuit.GetDistanceLength(); i += 5)
                    {
                        Vector3 targetposition =
                            circuit.GetRoutePoint(i).position;
                        //Debug.Log(i+" "+targetposition); ///////////////////////////////////////////////////////////////////////////
                        if (Vector3.Distance(targetposition, pos) < min)
                        {
                            progressDistance = i;
                            min = Vector3.Distance(targetposition, pos);

                            

                        }
                    }
                }

                transform.position = newtrans.position;
                transform.rotation = newtrans.rotation;
                //transform.localRotation = newtrans.localRotation;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                lastPosition = transform.position;
                // progressNum = index;
                // if (index +1 < circuit.Waypoints.Length) {
                //     progressNum = index +1;
                //     target.position = circuit.Waypoints[index +1].position;
                // }
            }*/
        }
        public void ResetMyCarLineCarInFront()
        {
            Vector3 v3f = mycar.transform.position;
            v3f.z += frontAppear + Random.Range(0,randomAppear); // mycarのzの位置より ? m 先の位置

            // 一番近いWaypointを探す
            float mindistance = float.MaxValue;
            int index = -1;
            for (int i = 0; i < circuit.Waypoints.Length; i++)
            {
                Vector3 v3fwp = circuit.Waypoints[i].transform.position;
                float distancewp = Vector3.Distance(v3f, v3fwp);
                if (distancewp < mindistance)
                {
                    index = i;
                    mindistance = distancewp;
                }
            }
            Transform newtrans = circuit.Waypoints[index].transform;
            if (index + 1 < circuit.Waypoints.Length)
            {
                newtrans.LookAt(circuit.Waypoints[index + 1].transform);
            }
            // active = false;
            //     transform.position = newtrans.position;
            //     //transform.localRotation = initqua;
            //     transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            RaycastHit hit;
            // Vector3 pdiff = new Vector3(0f, 1.1f, 0f); // 少し上　から前に飛ばす
            Vector3 pdiff = new Vector3(0f, 0, 0f);  //Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 

            int distance = 30;
            float radius = 0.2f;// 0.5f; 
            //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
            //Debug.DrawRay(transform.position,(newtrans.position-transform.position).normalized*distance,Color.blue,0,false);
            //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
            //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
            if (Physics.SphereCast(newtrans.position + pdiff + newtrans.forward * -15, radius, newtrans.forward, out hit, distance))
            {
                Debug.DrawRay(newtrans.position + pdiff + newtrans.forward * -15, newtrans.forward * distance, Color.red, 0, false);
                Debug.Log("Obstacle exists!!!!!");
                //active = false;
                //                Debug.Break();
            }
            else
            {
                Debug.DrawRay(newtrans.position + pdiff + newtrans.forward * -15, newtrans.forward * distance, Color.blue, 0, false);
                Debug.Log("Obstacle exists else");

                // progressDistanceの設定
                Vector3 pos = newtrans.position;
                float min = Vector3.Distance(new Vector3(), pos);
                if (circuit.GetDistanceLength() > 20)
                {
                    for (int i = 20; i < circuit.GetDistanceLength(); i += 5)
                    {
                        Vector3 targetposition =
                            circuit.GetRoutePoint(i).position;
                        //Debug.Log(i+" "+targetposition); ///////////////////////////////////////////////////////////////////////////
                        if (Vector3.Distance(targetposition, pos) < min)
                        {
                            progressDistance = i;
                            min = Vector3.Distance(targetposition, pos);

                        }
                    }
                }

                Vector3 psdiff = new Vector3(0f, -0.5f, 0f);  
                transform.position = newtrans.position+psdiff;
                //float y = transform.position.y;
                //y += 30;
                transform.rotation = newtrans.rotation;

                //キューブを出現させる
                //GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //Cube.transform.position = newtrans.position;

                //transform.localRotation = newtrans.localRotation;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                lastPosition = transform.position;
                // progressNum = index;
                // if (index +1 < circuit.Waypoints.Length) {
                //     progressNum = index +1;
                //     target.position = circuit.Waypoints[index +1].position;
                // }
            }
        }
        public void Reset(Vector3 pos,Quaternion qua)
        {
            transform.localRotation = qua;
            Debug.Log("Reset"+ circuit.GetDistanceLength());
            //           progressDistance = 0;
            float min = Vector3.Distance(new Vector3(),pos) ;
            if (circuit.GetDistanceLength() > 20) {
                for(int i = 20; i < circuit.GetDistanceLength(); i+=5) {
                    Vector3 targetposition =
                    circuit.GetRoutePoint(i).position;
                    //Debug.Log(i+" "+targetposition); ///////////////////////////////////////////////////////////////////////////
                    if (Vector3.Distance(targetposition,pos) < min) {
                        progressDistance = i;
                        min = Vector3.Distance(targetposition,pos) ;
                    }
                }
            }

            progressNum = 0;
            //waypointsの行き先を変える
            if (progressStyle == ProgressStyle.PointToPoint)
            {
                target.position = circuit.Waypoints[progressNum].position;

                //もし～～なら
                //もしゴールに着いたら座標を0にして輪をつくる
                //target.position = new Vector3(0,0,0);


                target.rotation = circuit.Waypoints[progressNum].rotation;
            }
        }



        // reset the object to sensible values
        public void Reset()
        {
 //           progressDistance = 0;
            progressNum = 0;
            if (progressStyle == ProgressStyle.PointToPoint)
            {
                target.position = circuit.Waypoints[progressNum].position;
                target.rotation = circuit.Waypoints[progressNum].rotation;
            }
        }

        public void SetActive(){
            active = true;
            if (type ==CarType.AROUNDMYCAR) {
                ResetAroundMyCar();
            }
            if(type == CarType.MYCARLANECARBEHIND)
            {
                ResetMyCarLineCarBehind();
            }
            if (type == CarType.MYCARLANECARINFRONT)
            {
                ResetMyCarLineCarInFront();
            }
            if (type == CarType.GO)
            {
                //ResetInitPos();
            }
        }

        private void Goal(){
            active = false;

            if (type ==CarType.AROUNDMYCAR) {
                transform.position = initpos;
                transform.localRotation = initqua;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            } 
            if (type ==CarType.GO) {
                //ResetInitPos();
            }


        }
        private void Update()
        {
            if(!active ){
                return;
            }
            if (progressStyle == ProgressStyle.SmoothAlongRoute)
            {
                // determine the position we should currently be aiming for
                // (this is different to the current progress position, it is a a certain amount ahead along the route)
                // we use lerp as a simple way of smoothing out the speed over time.
                if (Time.deltaTime > 0)
                {
                    speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude/Time.deltaTime,
                                       Time.deltaTime);
                }
                target.position =
                    circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor*speed)
                           .position;
                target.rotation =
                    Quaternion.LookRotation(
                        circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor*speed)
                               .direction);
                progressNum = circuit.GetRouteNumber(progressDistance);

                // get our current progress along the route
                progressPoint = circuit.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude*0.5f;
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (type == CarType.GO) {
                    if (progressDistance > circuit.GetDistanceLength()-10) { // ゴールしたら初期位置から


                        Goal();
                    }
                } 
                else if (type ==CarType.AROUNDMYCAR) {　// mycarの後ろならmycarの前に戻す
                    float resetdistance = 60;
                    float mycarz = mycar.transform.position.z;
                    float z = transform.position.z;
                    debug = "mycarz " + (int)mycarz + " z " + (int)z + "  " + (int)(z - (mycarz - resetdistance));
                    if (z - (mycarz - resetdistance) < 0)
                    {
                        //                        Goal();
                        ResetAroundMyCar();
                    }
                    /*int mycarnumber = (blockFinder.count - blockFinder.index);
                    if (progressNum > mycarnumber+3) {
                        Goal();
                    }
                    if (Vector3.Distance(transform.position,mycar.transform.position)> 200){
                        Goal();
                    }
                    if (progressNum >= blockFinder.count-2){
                        Goal();
                    }*/
                }else if (type == CarType.MYCARLANECARBEHIND)
                {　// mycarの前に行き過ぎならmycarの後ろに戻す
                    float resetdistance = 150;
                    float resetdistanceInFront = 30;
                    float mycarz = mycar.transform.position.z;//mycarz
                    float z = transform.position.z;//自車z
                    debug = "mycarz " + (int)mycarz + " z " + (int)z + "  " + (int)(z - mycarz);
                    //if (z - (mycarz - resetdistance) < 0)
                    if (z -mycarz > frontDisappear)
                    {
                        ResetMyCarLineCarBehind();
                    }
                    else if (mycarz - z > backDisappear)
                    {
                        ResetMyCarLineCarInFront();
                    }
                }
                /*else if (type == CarType.MYCARLANECARINFRONT)
                {　// mycarの後ろに行きすぎならmycarの先に戻す
                    float resetdistance = 30;
                    float mycarz = mycar.transform.position.z;//mycarz
                    float z = transform.position.z;//自車z
                    debug = "mycarz " + (int)mycarz + " z " + (int)z + "  " + (int)(z - mycarz);
                    //if (z - (mycarz - resetdistance) < 0)
                    if (mycarz-z > resetdistance)
                    {
                        ResetMyCarLineCarInFront();
                    }
                }*/
                debug = ""+blockFinder.index+ " "+blockFinder.count;

                lastPosition = transform.position;
            }
            else
            {
                // point to point mode. Just increase the waypoint if we're close enough:

                Vector3 targetDelta = target.position - transform.position;
                if (targetDelta.magnitude < pointToPointThreshold)
                {
                    progressNum = (progressNum + 1)%circuit.Waypoints.Length;
                }


                target.position = circuit.Waypoints[progressNum].position;
                target.rotation = circuit.Waypoints[progressNum].rotation;

                // get our current progress along the route
                progressPoint = circuit.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude;
                }
                lastPosition = transform.position;
            }
        }


        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }
    }
}
