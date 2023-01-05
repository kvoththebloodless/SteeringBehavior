using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static Action InputLeftMouseClicked = () => { };
    public static Action InputRightMouseClicked = () => { };
    public static Action InputLeftMousePressedDown = () => { };
    public static Action InputRightMousePressedDown = () => { };
    public static Action InputLeftMouseDragged = () => { };
    public static Action InputRightMouseDragged = () => { };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InputLeftMouseClicked.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            InputRightMouseClicked.Invoke();
        }
        if (Input.GetMouseButton(0))
        {
            InputLeftMousePressedDown.Invoke();
        }
        if (Input.GetMouseButton(1))
        {
            InputRightMousePressedDown.Invoke();
        }


    }
}
