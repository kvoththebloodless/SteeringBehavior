using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehavior : MonoBehaviour
{
    private Vector3 positionToGoTo;
    private Camera cam;
    public float max_speed = 0.005f;
    public float mass = 100;
    public float slowdownradius = 0.2f;
    public Vector3 velocity;
    private bool sawPlayer;
    public float CIRCLE_DISTANCE = 1;
    public float CIRCLE_RADIUS = 0.3f;
    private float angle = 10;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        positionToGoTo = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))

            {
                positionToGoTo = hit.point;
            }
        }
        SteerSeekAndArrive();
        SteerAroundObstacles();

    }
    private void SteerAroundObstacles()
    {
        Vector3 offset;

        MovementManager.ObstacleOffset(out offset, transform.position, transform.forward, 0.4f, Color.red);

        //taking only the x and z components
        var obstaclepushx = Vector3.Dot(offset, Vector3.right) * Vector3.right;
        var obstaclepushz = Vector3.Dot(offset, Vector3.forward) * Vector3.forward;
        var finalobstaclepush = obstaclepushx + obstaclepushz;
        SetVelocity(velocity + finalobstaclepush.normalized * 0.002f);
        transform.position += velocity;

        /*  var obstaclepushx = Vector3.Dot(offset, transform.right) * Vector3.right;
          var obstaclepushz = Vector3.Dot(offset, transform.forward) * Vector3.forward;
          var finalobstaclepush = obstaclepushx + obstaclepushz;*/
        /*
                SetVelocity(velocity + finalobstaclepush / mass);
                transform.position += velocity;*/
    }

    private void SteerFlee(Vector3 positiontoflee)
    {
        SetVelocity(velocity + MovementManager.Flee(velocity, transform.position, positiontoflee, mass, max_speed * 5));
        transform.position += velocity;
    }
    private void SteerSeekAndArrive()
    {
        SetVelocity(velocity + MovementManager.Seek(velocity, transform.position, positionToGoTo, mass, max_speed));
        transform.position += velocity;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Enemy")
        {
            sawPlayer = true;
            SteerFlee(other.transform.position);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        sawPlayer = false;
    }

    private void SetVelocity(Vector3 velocity)
    {
        this.velocity = Vector3.ClampMagnitude(velocity, max_speed);
        transform.forward = velocity.normalized;

    }
}
