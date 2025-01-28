using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalController : MonoBehaviour
{
    public List<GameObject> carfrontgreen;
    public List<GameObject> carfrontred;
    public List<GameObject> carfrontyellow;
    public List<GameObject> carsidegreen;
    public List<GameObject> carsidered;
    public List<GameObject> carsideyellow;
    public List<GameObject> personfrontgreen;
    public List<GameObject> personfrontred;
    public List<GameObject> personsidegreen;
    public List<GameObject> personsidered;    
    public Material materialgreen;
    public Material materialred;    
    public Material materialyellow;
    public Material materialgreen2;
    public Material materialred2;    
    public Material materialyellow2;
    public bool active;
    public int mode;
    public float elapsedTime;
    public int currentTime;
    public int localtime;
    public float offsetTime;
    public static float startTime;
    public int mode0timeBBRR=10;
    public int mode1timeBLRR=3;
    public int mode2timeBRRR=3;
    public int mode3timeYRRR=1;
    public int mode4timeRRRR=1;
    public int mode5timeRRBB=10;
    public int mode6timeRRBL=3;
    public int mode7timeRRBR=3;
    public int mode8timeRRYR=1;
    public int mode9timeRRRR=1;

    public int [] times = new int [10];
    public int [] timesSum = new int [10];

    public int crossNumber;
    public int color = 0;
    string text;
    // Start is called before the first frame update
    void Start()
    {
        times[0] =  mode0timeBBRR;
        times[1] =  mode1timeBLRR;
        times[2] =  mode2timeBRRR;
        times[3] =  mode3timeYRRR;
        times[4] =  mode4timeRRRR;
        times[5] =  mode5timeRRBB;
        times[6] =  mode6timeRRBL;
        times[7] =  mode7timeRRBR;
        times[8] =  mode8timeRRYR;
        times[9] =  mode9timeRRRR;

        int sum = 0;
        for(int i = 0; i < 10; i++) {
            sum += times[i];
            timesSum[i] = sum;
        
        }        
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time;
        if (active) {
            currentTime = (int)(elapsedTime - startTime - offsetTime + timesSum[9]);
            localtime = currentTime % timesSum[9];
            for(int i = 0; i < 10; i++) {
                //Debug.Log("i "+i+ " "+localtime+" "+timesSum[i]);
                if (localtime < timesSum[i]) {
                    mode = i;
                    //Debug.Log("Mode "+mode);
                    break;
                }
            }
        }


        if (mode == 0) { // 車:青/赤 人:青/赤
            SetColor(true,false,false, false,true,false, true,false, false,true);
            color = 3;
        }
        if (mode == 1) { // 車:青/赤 人:点滅/赤
            if ((int)elapsedTime % 2 == 0){
                SetColor(true,false,false, false,true,false, true,false,false,true);
            }
            else{
                SetColor(true,false,false, false,true,false, false,false,false,true);
            }
            color = 3;
        }
        if (mode == 2) { // 車:青/赤 人:赤/赤
            SetColor(true,false,false, false,true,false, false,true, false,true);
            color = 3;
        }
        if (mode == 3) { // 車:黄/赤 人:赤/赤
            SetColor(false,false,true, false,true,false, false,true,false,true);
            color = 2;
        }
        if (mode == 4) { // 車:赤/赤 人:赤/赤
            SetColor(false,true,false, false,true,false, false,true,false,true);
            color = 1;
        }

        if (mode == 5) { // 車:赤/青 人:赤/青
            SetColor(false,true,false, true,false,false, false,true,true,false);
            color = 1;
        }
        if (mode == 6) { // 車:赤/青 人:赤/点滅
            if ((int)elapsedTime % 2 == 0){
                SetColor(false,true,false,true,false,false,false,true,true,false);
            }
            else{
                SetColor(false,true,false,true,false,false,false,true,false,false);
            }
            color = 1;
        }
        if (mode == 7) { // 車:赤/青 人:赤/赤
            SetColor(false,true,false,true,false,false,false,true,false,true);
            color = 1;
        }
        if (mode == 8) { // 車:赤/黄 人:赤/赤
            SetColor(false,true,false,false,false,true,false,true,false,true);
            color = 1;
        }
        if (mode == 9) { // 車:赤/赤 人:赤/赤
            SetColor(false,true,false,false,true,false,false,true,false,true);
            color = 1;
        }        
    }

    void SetColor(bool cfg, bool cfr, bool cfy, bool csg, bool csr, bool csy, bool pfg, bool pfr, bool psg, bool psr ){

        //gameObject.GetComponent<Renderer>().material = material;//.color = Color.green;
        for(int i = 0; i < carfrontgreen.Count; i++){
            // carfrontgreen[i].transform.GetComponent<Renderer>().material.color = Color.green;
            carfrontgreen[i].GetComponent<Renderer>().material = cfg ? materialgreen :materialgreen2;
            // Debug.Log(i);
        }
        for(int i = 0; i < carfrontred.Count; i++){
            carfrontred[i].GetComponent<Renderer>().material = cfr ? materialred : materialred2;
            // Debug.Log(i);
        }
        for(int i = 0; i < carfrontyellow.Count; i++){
            carfrontyellow[i].GetComponent<Renderer>().material = cfy ? materialyellow : materialyellow2;
            // Debug.Log(i);
        }
        for(int i = 0; i < carsidegreen.Count; i++){
            // carfrontgreen[i].transform.GetComponent<Renderer>().material.color = Color.green;
            carsidegreen[i].GetComponent<Renderer>().material = csg ? materialgreen : materialgreen2 ;
            // Debug.Log(i);
        }
        for(int i = 0; i < carsidered.Count; i++){
            carsidered[i].GetComponent<Renderer>().material = csr ? materialred : materialred2;
            // Debug.Log(i);
        }
        for(int i = 0; i < carsideyellow.Count; i++){
            carsideyellow[i].GetComponent<Renderer>().material = csy ? materialyellow : materialyellow2;
            // Debug.Log(i);
        }
        for(int i = 0; i < personfrontgreen.Count; i++){
            // carfrontgreen[i].transform.GetComponent<Renderer>().material.color = Color.green;
            personfrontgreen[i].GetComponent<Renderer>().material = pfg ? materialgreen : materialgreen2;
            // Debug.Log(i);
        }
        for(int i = 0; i < personfrontred.Count; i++){
            personfrontred[i].GetComponent<Renderer>().material = pfr ? materialred: materialred2;
            // Debug.Log(i);
        }
        for(int i = 0; i < personsidegreen.Count; i++){
            personsidegreen[i].GetComponent<Renderer>().material = psg ? materialgreen : materialgreen2;
            // Debug.Log(i);
        }
        for(int i = 0; i < personsidered.Count; i++){
            personsidered[i].GetComponent<Renderer>().material = psr ? materialred: materialred2;
            // Debug.Log(i);
        }
    }

    [ContextMenu("Load ")]
    private void Load()
    {
        Vector3 v3fsum = new Vector3();
        float count = 0;

        string crossNumberStr = ""+crossNumber;
        Debug.Log("Test"+ crossNumberStr);
        text = (Resources.Load("signal", typeof(TextAsset)) as TextAsset).text;
        Debug.Log(text);
        carfrontgreen.Clear();
        carfrontred.Clear();
        carfrontyellow.Clear();
        carsidegreen.Clear();
        carsidered.Clear();
        carsideyellow.Clear();
        personfrontgreen.Clear();
        personfrontred.Clear();
        personsidegreen.Clear();
        personsidered.Clear();
        string[] lines = text.Split("\r\n");
        for (int i = 0; i < lines.Length; i++) {
            string[] csv = lines[i].Split(",");
            if (csv.Length >=5 && csv[0] == crossNumberStr) {
                if(csv[4] == "青" && csv[1] == "進行方向" && csv[3] == "車両"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    carfrontgreen.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                }
                if(csv[4] == "赤" && csv[1] == "進行方向" && csv[3] == "車両"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    carfrontred.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                }
                if(csv[4] == "黄" && csv[1] == "進行方向" && csv[3] == "車両"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    carfrontyellow.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "青" && csv[1] == "交差方向" && csv[3] == "車両"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    carsidegreen.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "赤" && csv[1] == "交差方向" && csv[3] == "車両"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    carsidered.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "黄" && csv[1] == "交差方向" && csv[3] == "車両"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    carsideyellow.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "青" && csv[1] == "進行方向" && csv[3] == "人"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    personfrontgreen.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "赤" && csv[1] == "進行方向" && csv[3] == "人"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    personfrontred.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "青" && csv[1] == "交差方向" && csv[3] == "人"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    personsidegreen.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                } 
                if(csv[4] == "赤" && csv[1] == "交差方向" && csv[3] == "人"){
                    Debug.Log(csv[5]);
                    GameObject go = GameObject.Find(csv[5]);
                    personsidered.Add(go);
                    v3fsum += go.transform.position;
                    count++;
                }                 
            }
        }
        transform.position = v3fsum / count;

    }
}
