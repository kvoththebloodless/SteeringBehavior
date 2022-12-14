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
    public bool iamleader;
    [Header("Seek")]
    float seekforce = 0.03f;
    [Header("Obstacle Avoidance")]
    [SerializeField]
    float obstacleavoidanceforce = 0.03f;
    [SerializeField]
    float obstaclemaxcheckdistance = 0.5f;
    [SerializeField]
    float obstaclemincheckdistance = 0.1f;
    [SerializeField]
    private GameObject path;
    [Header("Hoard properties")]
    [SerializeField]
    float seekforcehorde = 0.02f;
    [SerializeField]
    float separationhorderradius = 0.5f;
    [SerializeField]
    float separationhordeforce = 0.05f;
    [SerializeField]
    float evadehordeforce = 0.05f;

    [SerializeField]
    float distancebehind = 0.1f;
    private PreyBehavior leader;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        positionToGoTo = transform.position;
        leader = transform.parent.GetChild(0).GetComponent<PreyBehavior>();
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

        MovementManager.ObstacleOffset(out offset, transform.position, transform.forward, velocity, max_speed, obstaclemaxcheckdistance, obstaclemincheckdistance, obstacleavoidanceforce);

        SetVelocity(velocity + offset);


    }

    private void SteerFlee(Vector3 positiontoflee)
    {
        SetVelocity(velocity + MovementManager.Flee(velocity, transform.position, positiontoflee, mass, 0.015f));

    }
    private void SteerSeekAndArrive()
    {
        //
        //SetVelocity(velocity + MovementManager.Wander(transform, CIRCLE_DISTANCE, CIRCLE_RADIUS, ref angle, velocity, 0.005f)); ;
        //SetVelocity(velocity + MovementManager.FollowPath(velocity, transform.position, mass, 0.008f, path, ref waypointindex, ref waypointdirection, 1f));
        if (iamleader)
        {
            SetVelocity(velocity + MovementManager.Seek(velocity, transform.position, positionToGoTo, mass, seekforce));
        }
        else
        {


            // Find a point howfarbehind distance behind the leader in the inverse direction of leader's velocity.
            //That's what we need to steer towards primarily.
            var backpositiontoreach = leader.transform.position - (1 * leader.transform.forward * leader.distancebehind);

            //To slowly decrease the speed of the hoard as the leader slows down. At 0 speed, the deacceration will be max.
            var deaccelerationfactor = -(1 - leader.velocity.magnitude / max_speed);

            //Stop steering, evading and separating based on the following vector based on the velocity of the leader.
            // as the leader slows, so do all these forces
            var hoardvel = MovementManager.HoardMode(
                this.transform,
                this.transform.parent,
                this.transform.parent.GetChild(0).GetComponent<PreyBehavior>(),
                backpositiontoreach,
                velocity,
                transform.position,
                mass,
               //seek
               leader.seekforcehorde,
               //radius separation
               leader.separationhorderradius
               ,
               //separation force
               leader.separationhordeforce,
               //evade force
               leader.evadehordeforce

               );

            //adding the deacceleration factor to the hoard vel component every frame
            SetVelocity(velocity + deaccelerationfactor * hoardvel + hoardvel);

            //adding deacceleration factor to the overall velocity. Multiplying by 0.05f for a bit of a gradual decline.
            SetVelocity(velocity + deaccelerationfactor * velocity * 0.05f);
        }

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

        if (this.velocity.magnitude > 0.0001f)
            transform.forward = Vector3.RotateTowards(transform.forward, velocity.normalized, Time.deltaTime, 0.1f);

        // an unclean check so that past a certain magnitude it's considered stopped
        if (this.velocity.magnitude < 0.001f)
        {
            this.velocity = Vector3.zero;
        }
        transform.position += velocity * Time.deltaTime;
    }
}
