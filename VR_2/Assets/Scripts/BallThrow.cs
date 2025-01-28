using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrow : MonoBehaviour
{
    [SerializeField] Transform rightHandAnchor;
    ShowPointer showPointer;
    GameObject ball;

    /// <summary>
    /// �W�I�̍��W
    /// </summary>
    private Vector3 TargetPosition;

    /// <summary>
    /// �ˏo�p�x
    /// </summary>
    [SerializeField, Range(0F, 90F), Tooltip("�ˏo����p�x")]
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
            //�{�[���𓊂���
            TargetPosition = showPointer.GetRayLastpos();
            ThrowingBall();
            Invoke("destroy", 2);
            //�Q�l�T�C�g��
            //https://qiita.com/_udonba/items/a71e11c8dd039171f86c
            //������target�̈ʒu�Ƀ{�[�����o��
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
    /// �{�[�����ˏo����
    /// </summary>
    private void ThrowingBall()
    {
        // Ball�I�u�W�F�N�g�̐���
        GameObject ball = Instantiate(Resources.Load<GameObject>("ball"), rightHandAnchor.transform.position, Quaternion.identity);

        // �ˏo�p�x
        float angle = ThrowingAngle;

        // �ˏo���x���Z�o
        Vector3 velocity = CalculateVelocity(rightHandAnchor.transform.position, TargetPosition, angle);

        // �ˏo
        Rigidbody rid = ball.GetComponent<Rigidbody>();
        rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
    }

    /// <summary>
    /// �W�I�ɖ�������ˏo���x�̌v�Z
    /// </summary>
    /// <param name="pointA">�ˏo�J�n���W</param>
    /// <param name="pointB">�W�I�̍��W</param>
    /// <returns>�ˏo���x</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // �ˏo�p�����W�A���ɕϊ�
        float rad = angle * Mathf.PI / 180;

        // ���������̋���x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // ���������̋���y
        float y = pointA.y - pointB.y;

        // �Ε����˂̌����������x�ɂ��ĉ���
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // �����𖞂����������Z�o�ł��Ȃ����Vector3.zero��Ԃ�
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

}
