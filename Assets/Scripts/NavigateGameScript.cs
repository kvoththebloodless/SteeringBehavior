using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateGameScript : MonoBehaviour
{
    public float anglerotdegrees = 60;
    public float goforward = 0.5f;
    public float speeduponshift = 3;
    private Camera maincamera;
    private Vector3 currentAimDirection;
    private Vector3 smoothdampvelo;
    // Start is called before the first frame update
    void Start()
    {
        maincamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {

            var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            var xdiff = pos.x - currentAimDirection.x;
            var ydiff = pos.y - currentAimDirection.y;

            // When you put the quaternion part first, it'll do the rotation based on the coordinate axes of the quaternion part, which is the world coordinate axes
            // if we did maincamera.transform.rotation*Quaternion.Euler(0, xdiff * anglerotdegrees, 0), it'll rotate based on the xdiff * anglerotdegrees in the rotated coordinate of the transform.
            // so if the transform is weirdly rotated it'll take that rotated axes and then rotate xdiff * anglerotdegrees in that rotated coordinate space. 
            // whereas we want the world coordinate axes and rotate in that space. 
            maincamera.transform.rotation = Quaternion.Euler(0, xdiff * anglerotdegrees, 0) * maincamera.transform.rotation;
            maincamera.transform.localRotation *= Quaternion.Euler(-1 * ydiff * anglerotdegrees, 0, 0);

            //Following takes care of camera translation

            Vector3 disp = Vector3.zero;
            if (Input.GetKey(KeyCode.D))
            {
                disp += maincamera.transform.right * goforward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                disp -= maincamera.transform.right * goforward;
            }
            if (Input.GetKey(KeyCode.W))
            {   // The translation forward is always horizontally at the center and vertically varies as to where the y axis is
                var pos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                pos1.x = 0.5f;
                var ray = Camera.main.ViewportPointToRay(pos1).direction;

                disp += ray;
            }
            if (Input.GetKey(KeyCode.S))
            {
                var pos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                pos1.x = 0.5f;
                var ray = Camera.main.ViewportPointToRay(pos1).direction;
                disp -= ray;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                disp *= speeduponshift;
            }
            maincamera.transform.position = Vector3.SmoothDamp(maincamera.transform.position, maincamera.transform.position + disp, ref smoothdampvelo, 0.2f);

        }
        currentAimDirection = Camera.main.ScreenToViewportPoint(Input.mousePosition);

    }

}
