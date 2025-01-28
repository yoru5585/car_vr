using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLaserRenderer : MonoBehaviour
{
    [SerializeField] Transform rightHandAnchor;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float maxRayDistance = 500.0f;

    void Update()
    {
        // �E��̃R���g���[���̈ʒu�ƌ����Ă����������Ray���쐬
        Ray laserPointer = new Ray(rightHandAnchor.position, rightHandAnchor.forward);

        // �쐬����Ray���Collider�����邩����
        RaycastHit hit;
        if (Physics.Raycast(laserPointer, out hit, maxRayDistance))
        {
            // Collider������΁A�Փˉӏ��܂Ń��[�U�[��`��
            renderLaserToHit(laserPointer, hit);
        }
        else
        {
            // Collider���Ȃ���΁A�ő咷�̃��[�U�[��`��
            renderLaserFullLength(laserPointer);
        }
    }

    private void renderLaserToHit(Ray ray, RaycastHit hit)
    {
        renderLaser(ray.origin, hit.point);
    }

    private void renderLaserFullLength(Ray ray)
    {
        renderLaser(ray.origin, ray.origin + ray.direction * maxRayDistance);
    }

    private void renderLaser(Vector3 from, Vector3 to)
    {
        // Line Renderer��1�_�ڂ�2�_�ڂ̈ʒu���w�肷��
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }

}

