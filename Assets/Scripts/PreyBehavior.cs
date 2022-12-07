using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehavior : MonoBehaviour
{
    private Vector3 positionToGoTo;
    private Camera cam;
    public float max_speed = 0.5f;
    public float max_force = 0.001f;
    public float mass = 100;
    public float slowdownradius = 0.2f;
    public Vector3 velocity;
    private bool sawPlayer;
    public float CIRCLE_DISTANCE = 1;
    public float CIRCLE_RADIUS = 0.3f;
    private float angle = 10;
    private int waypointindex = 0;
    private int waypointdirection = 1;
    [SerializeField]
    private GameObject path;
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

        MovementManager.ObstacleOffset(out offset, transform.position, transform.forward, (velocity.magnitude / max_speed) * 1f + 0.1f, 0.02f);

        SetVelocity(velocity + offset);


    }

    private void SteerFlee(Vector3 positiontoflee)
    {
        SetVelocity(velocity + MovementManager.Flee(velocity, transform.position, positiontoflee, mass, 0.015f));

    }
    private void SteerSeekAndArrive()
    {
        //SetVelocity(velocity + MovementManager.Seek(velocity, transform.position, positionToGoTo, mass, 0.01f));
        //SetVelocity(velocity + MovementManager.Wander(transform, CIRCLE_DISTANCE, CIRCLE_RADIUS, ref angle, velocity, 0.001f)); ;
        SetVelocity(velocity + MovementManager.FollowPath(velocity, transform.position, mass, 0.005f, path, ref waypointindex, ref waypointdirection, 1.5f));


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
        transform.position += velocity * Time.deltaTime;
    }
}
