using System.Collections.Generic;
using UnityEngine;

public class SelectArmyScript : MonoBehaviour
{
    public List<GameObject> Players;
    private Vector3 lefttoppos;
    public List<GameObject> highlighted = new List<GameObject>();
    public float thresholdtoselect = 0.1f;
    public static Dictionary<GameObject, List<GameObject>> mapLeaderToArmy = new Dictionary<GameObject, List<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        Players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {

            var rightbottompos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            SetAllInactive(highlighted);
            highlighted = GetHighlighted(Players, lefttoppos.x, lefttoppos.y, rightbottompos.x, rightbottompos.y);
            SetAllActive(highlighted);
        }
        else
        {
            lefttoppos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

    }

    private List<GameObject> GetHighlighted(List<GameObject> Players, float leftupx, float leftupy, float rightbottomx, float rightbottomy)
    {
        List<GameObject> newlist = new List<GameObject>();

        var minx = Mathf.Min(leftupx, rightbottomx);
        var maxx = Mathf.Max(leftupx, rightbottomx);
        var miny = Mathf.Min(leftupy, rightbottomy);
        var maxy = Mathf.Max(leftupy, rightbottomy);
        foreach (GameObject obj in Players)
        {
            var screepos = Camera.main.WorldToViewportPoint(obj.transform.position);
            if (screepos.x >= minx && screepos.x <= maxx && screepos.y <= maxy && screepos.y >= miny)
            {
                newlist.Add(obj);
            }

        }
        return newlist;
    }

    private void SetAllActive(List<GameObject> Players)
    {
        foreach (GameObject obj in Players)
        {
            var objselect = obj.transform.Find("SelectionMarker").gameObject;
            if (!objselect.activeInHierarchy)
                objselect.SetActive(true);
        }
    }
    private void SetAllInactive(List<GameObject> Players)
    {
        foreach (GameObject obj in Players)
        {
            var objselect = obj.transform.Find("SelectionMarker").gameObject;
            if (objselect.activeInHierarchy)
                objselect.SetActive(false);
        }
    }

    private void MakeArmyUnit(List<GameObject> highlighted)
    {
        foreach (GameObject obj in highlighted)
        {
            if (mapLeaderToArmy.ContainsKey(obj))
            {
                /*  mapLeaderToArmy*/
            }
        }
    }
}
