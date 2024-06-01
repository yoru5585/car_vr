using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowPointer : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] RectTransform pointerRect;
    [SerializeField] Transform rightHandAnchor;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float maxRayDistance = 500.0f;

    Vector3 rayLastpos;
    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        showLaser();
        pointerRect.anchoredPosition = worldToViewportPoint(rayLastpos);
    }

    private void showLaser()
    {
        // �E��̃R���g���[���̈ʒu�ƌ����Ă����������Ray���쐬
        Ray laserPointer = new Ray(rightHandAnchor.position, rightHandAnchor.forward);

        // �쐬����Ray���Collider�����邩����
        RaycastHit hit;
        if (Physics.Raycast(laserPointer, out hit, maxRayDistance))
        {
            // Collider������΁A�Փˉӏ��܂Ń��[�U�[��`��
            renderLaserToHit(laserPointer, hit);
            //�ćA
            //�|�C���^�[�̐F��ς���
            pointerRect.gameObject.GetComponent<Image>().color = Color.green;
            rayLastpos = hit.point;
            GameObject.FindGameObjectWithTag("log").GetComponent<TextMeshProUGUI>().text = hit.point.x + "," + hit.point.y + "," + hit.point.z;
        }
        else
        {
            // Collider���Ȃ���΁A�ő咷�̃��[�U�[��`��
            renderLaserFullLength(laserPointer);
            //�ćA
            //�|�C���^�[�̐F��ς���
            pointerRect.gameObject.GetComponent<Image>().color = Color.red;
            rayLastpos = laserPointer.origin + laserPointer.direction * maxRayDistance;
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

    private Vector2 worldToViewportPoint(Vector3 worldPos)
    {
        Vector2 ViewportPosition = cam.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        return WorldObject_ScreenPosition;
    }

    //�{�[���̓�����p
    public Vector3 GetRayLastpos()
    {
        return rayLastpos;
    }

    //�{�^���m�F�p
    public void buttonClicked()
    {
        GameObject.Find("Text").GetComponent<TextMeshProUGUI>().text = "OK";
    }
}
