using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrow : MonoBehaviour
{
    [SerializeField] Transform rightHandAnchor;
    ShowPointer showPointer;
    GameObject ball;

    /// <summary>
    /// 標的の座標
    /// </summary>
    private Vector3 TargetPosition;

    /// <summary>
    /// 射出角度
    /// </summary>
    [SerializeField, Range(0F, 90F), Tooltip("射出する角度")]
    private float ThrowingAngle;

    // Start is called before the first frame update
    void Start()
    {
        showPointer = GetComponent<ShowPointer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            //ボールを投げる
            TargetPosition = showPointer.GetRayLastpos();
            ThrowingBall();
            Invoke("destroy", 2);
            //参考サイト↓
            //https://qiita.com/_udonba/items/a71e11c8dd039171f86c
            //試しにtargetの位置にボールを出現
            //ball = Instantiate(Resources.Load<GameObject>("ball"));
            //ball.transform.position = showPointer.GetRayLastpos();
            //Invoke("destroy", 2);
        }        
    }

    private void destroy()
    {
        Destroy(ball);
    }

    /// <summary>
    /// ボールを射出する
    /// </summary>
    private void ThrowingBall()
    {
        // Ballオブジェクトの生成
        GameObject ball = Instantiate(Resources.Load<GameObject>("ball"), rightHandAnchor.transform.position, Quaternion.identity);

        // 射出角度
        float angle = ThrowingAngle;

        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(rightHandAnchor.transform.position, TargetPosition, angle);

        // 射出
        Rigidbody rid = ball.GetComponent<Rigidbody>();
        rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

}
