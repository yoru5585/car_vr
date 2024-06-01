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
        // 右手のコントローラの位置と向いている方向からRayを作成
        Ray laserPointer = new Ray(rightHandAnchor.position, rightHandAnchor.forward);

        // 作成したRay上にColliderがあるか判定
        RaycastHit hit;
        if (Physics.Raycast(laserPointer, out hit, maxRayDistance))
        {
            // Colliderがあれば、衝突箇所までレーザーを描画
            renderLaserToHit(laserPointer, hit);
            //案②
            //ポインターの色を変える
            pointerRect.gameObject.GetComponent<Image>().color = Color.green;
            rayLastpos = hit.point;
            GameObject.FindGameObjectWithTag("log").GetComponent<TextMeshProUGUI>().text = hit.point.x + "," + hit.point.y + "," + hit.point.z;
        }
        else
        {
            // Colliderがなければ、最大長のレーザーを描画
            renderLaserFullLength(laserPointer);
            //案②
            //ポインターの色を変える
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
        // Line Rendererの1点目と2点目の位置を指定する
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

    //ボールの投げ先用
    public Vector3 GetRayLastpos()
    {
        return rayLastpos;
    }

    //ボタン確認用
    public void buttonClicked()
    {
        GameObject.Find("Text").GetComponent<TextMeshProUGUI>().text = "OK";
    }
}
