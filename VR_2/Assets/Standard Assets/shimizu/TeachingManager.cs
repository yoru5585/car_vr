using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeachingManager : MonoBehaviour
{
    //デモ映像をもう一度見れるようにする。
    GameObject ExampleCanvas;
    UnityStandardAssets.Vehicles.Car.CarAIControl targetCar;
    UnityStandardAssets.Vehicles.Car.CarAIControl playerCar;
    Rigidbody targetRG;
    Rigidbody playerRG;

    Vector3 targetPos = new Vector3(605.5f, 2.36f, 1361.70f);
    Vector3 playerPos = new Vector3(607.90f, 2.38f, 1354.81f);

    int state = 0;
    [SerializeField] Text TimeText;
    [SerializeField] GameObject bg2Text;
    [SerializeField] GameObject bg2Button;
    [SerializeField] GameObject bg2Button1;
    [SerializeField] GameObject cube;
    float timeCount;
    bool IsStart, IsEnd, isAllFinished;
    // Start is called before the first frame update
    void Start()
    {
        ExampleCanvas = GameObject.Find("ExampleCanvas");
        GameObject target = GameObject.Find("CarWaypointBased_t");
        GameObject player = GameObject.Find("CarWaypointBased_p");
        targetCar = target.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();
        playerCar = player.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();
        targetRG = target.GetComponent<Rigidbody>();
        playerRG = player.GetComponent<Rigidbody>();
        bg2Button.GetComponent<Button>().onClick.AddListener(OnNextButtonClicked);
        //target.transform.localPosition = targetPos;
        //target.transform.Rotate(new Vector3(0, -2, 0));
        //player.transform.localPosition = playerPos;
        //player.transform.Rotate(new Vector3(0, -2, 0));
        cube.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                targetCar.move = false;
                playerCar.move = false;
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.DownArrow))  {
                    OnNextButtonClicked();
                }

                break;
            case 1:
                demo();
                break;
            default:
                break;
        }
    }

    public void OnNextButtonClicked()
    {
        //ExampleCanvas.transform.GetChild(1).gameObject.SetActive(false);
        //ExampleCanvas.transform.GetChild(3).gameObject.SetActive(true);
        bg2Button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "終了";
        bg2Button.GetComponent<Button>().onClick.RemoveAllListeners();
        bg2Button.GetComponent<Button>().onClick.AddListener(OnEndButtonClicked);
        bg2Button.SetActive(false);

        targetRG.constraints = RigidbodyConstraints.None;
        targetRG.freezeRotation = false;
        playerRG.constraints = RigidbodyConstraints.None;
        playerRG.freezeRotation = false;
        targetCar.move = true;
        playerCar.move = true;
        state++;
    }

    public void OnRetryButtonClicked()
    {
        //シーンリセット
            bg2Button1.SetActive(false);        
        SceneManager.LoadScene("input4Screen");
        state = 0;
    }

    public void OnEndButtonClicked()
    {
        Debug.Log("別のシーンへ");
                SceneManager.LoadScene("Start");
    }

    void demo()
    {
        //他車動作
        if (IsStart)
        {
            cube.SetActive(true);
            timeCount += Time.deltaTime;
            //Debug.Log(timeCount);
            if (timeCount > 0)
            {
                TimeText.text = ((int)timeCount).ToString();
            }
        }

        if (IsEnd)
        {
            cube.SetActive(false);
            Debug.Log("他車停止");
            IsStart = false;
            IsEnd = false;
            isAllFinished = true;
            //他車停止
            targetRG.constraints = RigidbodyConstraints.FreezePosition;
            targetRG.freezeRotation = true;
            playerRG.constraints = RigidbodyConstraints.FreezePosition;
            playerRG.freezeRotation = true;
            //ボタン追加
            bg2Button.SetActive(true);
            bg2Button1.SetActive(true);
            //テキスト追加
            bg2Text.GetComponent<Text>().text += 
                "前の車が横断歩道を" +
                "\n通過し３秒程度経過して" + 
                "\n自車が通過すればOKです。";
        }

        if (isAllFinished) {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.DownArrow))  {
                OnEndButtonClicked();
            } 
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))  {
                OnRetryButtonClicked();
            }                        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.name == "Colliders_t")
        {
            IsStart = true;
            
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent.name == "Colliders_p")
        {
            IsEnd = true;

        }
    }
}
