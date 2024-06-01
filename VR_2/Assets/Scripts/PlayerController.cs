using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody player_rg;
    Vector3 movingDirecion;
    Vector3 movingVelocity;
    Vector3 originPos;
    [SerializeField] float speedMagnification = 10.0f;
    [SerializeField] float origin_walk_speed = 6;

    bool IsStop = false;
    [SerializeField] bool IsCameraReverse = false;

    Vector2 newAngle = Vector2.zero;
    Vector2 lastMousePosition = Vector2.zero;
    [SerializeField] float rotationSpeed;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        player_rg = GetComponent<Rigidbody>();
        originPos = transform.position;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStop) return;

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            //初期地点へワープ
            transform.position = originPos;
        }

        Move();
        CameraControll();
    }

    void Move()
    {
        Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        movingDirecion = new Vector3(axis.x, 0, axis.y);
        movingDirecion.Normalize();
        movingVelocity = movingDirecion * speedMagnification;
        Vector3 newPos = mainCamera.transform.TransformVector(movingVelocity);
        //Debug.Log(tmp);

        //移動
        player_rg.velocity = new Vector3(newPos.x, player_rg.velocity.y, newPos.z);
    }

    void CameraControll()
    {
        //ここでゲームパッドのカメラ操作
        newAngle = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        Vector3 euler = Vector3.zero;
        euler.y += newAngle.x * rotationSpeed;
        transform.RotateAround(mainCamera.transform.position, Vector3.up, euler.y);
    }
}
