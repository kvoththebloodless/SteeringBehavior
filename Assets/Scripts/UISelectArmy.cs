using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectArmy : MonoBehaviour
{
    Vector3 lefttoppos;

    LineRenderer lineren;

    private void Start()
    {
        lineren = GetComponent<LineRenderer>();
        lineren.useWorldSpace = false;
        lineren.positionCount = 4;
        lineren.loop = true;
        lineren.startWidth = 0.002f;
        lineren.endWidth = 0.002f;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            lineren.positionCount = 4;
            var rightbottompos = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f)));
            lineren.SetPosition(0, lefttoppos);
            lineren.SetPosition(1, new Vector3(rightbottompos.x, lefttoppos.y, lefttoppos.z));
            lineren.SetPosition(2, rightbottompos);
            lineren.SetPosition(3, new Vector3(lefttoppos.x, rightbottompos.y, lefttoppos.z));

        }
        else
        {
            lefttoppos = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f)));
            lineren.positionCount = 0;
        }

    }
}
