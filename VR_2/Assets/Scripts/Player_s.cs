using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Player_s : MonoBehaviour
{
    Vector2 newAngle = Vector2.zero;
    Vector2 rotationSpeed = new Vector2(0.5f, 0.5f);
    
    float walk_speed;
    float rotateInfluence = 2;
    Vector3 originPos;
    //[SerializeField] CharacterController playerController;
    [SerializeField] Transform player_trans;
    [SerializeField] GameObject rightController;
    [SerializeField] Camera mainCamera;
    [SerializeField] float jump_value = 6;
    [SerializeField] float origin_walk_speed = 6;

    LineRenderer linerend;
    RaycastHit[] hits = new RaycastHit[10];
    Vector3 rightContPos_world;

    bool IsStop = false;
    // Start is called before the first frame update
    void Start()
    {
        walk_speed = origin_walk_speed;
        originPos = player_trans.position;
    }

    private void Update()
    {
        if (IsStop)
        {
            return;
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            walk_speed = origin_walk_speed * 1.5f;
        }
        else
        {
            walk_speed = origin_walk_speed;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            //初期地点へワープ
            player_trans.position = originPos;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            //logTextをクリア
            GetComponent<Receive_udp>().logClear();
        }

        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            //受け取る
            GetComponent<Receive_udp>().receive();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            //ジャンプ
            OnJump();
        }

        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f || Input.GetKeyDown(KeyCode.Space))
        {

            //hits = Physics.SphereCastAll(rightContPos_world, 10, Vector3.forward);
            //foreach (var hit in hits)
            //{
            //    if (hit.collider.tag == "Car")
            //    {
            //        Debug.Log("car");
            //        player_trans.parent = hit.collider.transform;
            //        player_trans.position = Vector3.zero;
            //        break;
            //    }
            //}

            // 結果は事前に確保したhitに前方から順番に書き込みされます
            //rightContPos_world = transform.TransformPoint(rightController.transform.position);
            //var hitCount = Physics.RaycastNonAlloc(rightContPos_world, Vector3.forward, hits, 3f);
            //DrawRayLine(rightContPos_world, Vector3.forward);
            //for (int i = 0; i < hitCount; i++)
            //{
            //    var hit = hits[i];
            //    // ここで当たり判定の結果をもとに処理を行います
            //    if (hit.collider.tag == "Car")
            //    {
            //        Debug.Log("car");
            //        player_trans.parent = hit.collider.transform;
            //        player_trans.position = Vector3.zero;
            //    }
            //}
            Debug.Log("car");
            player_trans.parent = GameObject.FindGameObjectWithTag("Car").transform;
            player_trans.localPosition = new Vector3(0.27f , 0.631f, 0);
            player_trans.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
            player_trans.gameObject.GetComponent<Rigidbody>().useGravity = false;
            IsStop = true;
        }



        OnMove();
        CameraControll_Gamepad();
    }

    void OnMove()
    {
        //移動
        Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 newPos = Vector3.zero;
        newPos.z += walk_speed * axis.y;
        newPos.x += walk_speed * axis.x;
        Vector3 tmp = mainCamera.transform.TransformVector(newPos);
        player_trans.position += new Vector3(tmp.x, 0, tmp.z) * Time.deltaTime;
        //playerController.Move(new Vector3(tmp.x, 0, tmp.z));
    }

    void OnJump()
    {
        //ジャンプ
        //Debug.Log("a");
        Vector3 vector = new Vector3(0, jump_value, 0);
        player_trans.gameObject.GetComponent<Rigidbody>().AddForce(vector, ForceMode.Impulse);
    }

    void CameraControll_Gamepad()
    {
        //ここでゲームパッドのカメラ操作
        newAngle = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        //mainCamera.transform.localEulerAngles -= new Vector3(newAngle.y * rotationSpeed.y, -newAngle.x * rotationSpeed.x, 0) * 10;
        Vector3 euler = Vector3.zero;
        euler.y += newAngle.x * rotateInfluence;
        player_trans.RotateAround(mainCamera.transform.position, Vector3.up, euler.y);
    }

    public void SetRotateInfluence(float value)
    {
        rotateInfluence += value;
    }

    //引数はorigin（始点）と方向（direction）
    private void DrawRayLine(Vector3 start, Vector3 direction)
    {
        //LineRendererコンポーネントの取得
        linerend = this.GetComponent<LineRenderer>();

        //線の太さを設定
        linerend.startWidth = 0.04f;
        linerend.endWidth = 0.04f;

        //始点, 終点を設定し, 描画
        linerend.SetPosition(0, start);
        linerend.SetPosition(1, start + direction);
    }
}
