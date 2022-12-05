using UnityEngine;

public class HunterBehavior : MonoBehaviour
{
    private Camera cam;
    public float max_speed = 0.005f;
    public float mass = 100;
    public float slowdownradius = 0.2f;
    private Vector3 velocity;
    public float CIRCLE_DISTANCE = 1;
    public float CIRCLE_RADIUS = 0.3f;
    public float anglerot = 30;
    private Vector3 positionToGoTo;
    private bool sawPlayer;
    public bool seek;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        velocity = transform.forward * max_speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!sawPlayer)
            SteerSeekArriveAndWander();
        SteerAroundObstacles();


    }

    private void SteerSeekArriveAndWander()
    {


        // The wander steering. Think of it as an additional velocity or force. the object wants to wander off of its path so it provides an additional velocity. 
        var steering = MovementManager.Wander(transform, CIRCLE_DISTANCE, CIRCLE_RADIUS, ref anglerot, velocity);

        //This velocity is added to the existing velocity in a small amount of 0.001f. so that the deviation is little overtime. 
        // Overtime the small deviations add up to a smooth wander motion. 
        // tinkering with the circle distance and radius will change how effective the wander is. 
        // usually increasing distance of circle helps in this setup that I've done. 
        SetVelocity(velocity + (steering).normalized * 0.0001f);

        transform.position += velocity;

    }

    private void SteerAroundObstacles()
    {
        /*    var obstaclepush = MovementManager.ObstacleOffset(transform.position, transform.forward, 6);
            var obstaclepushx = Vector3.Dot(obstaclepush, transform.right) * transform.right;
            var obstaclepushz = Vector3.Dot(obstaclepush, transform.forward) * transform.forward;
            var finalobstaclepush = obstaclepushx + obstaclepushz;
            velocity += finalobstaclepush;
            transform.position += velocity;*/
    }

    private void SteerPursuit(Vector3 preyvelocity, Vector3 preyposition)
    {
        if (!seek)
        {
            var steering = MovementManager.Pursuit(preyvelocity, preyposition, velocity, transform.position, mass, max_speed * 3);
            velocity += steering;
            transform.position += velocity;
        }
        else
        {
            var steering = MovementManager.Seek(velocity, transform.position, preyposition, mass, max_speed);
            velocity += steering;
            transform.position += velocity;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            sawPlayer = true;
            SteerPursuit(other.GetComponent<PreyBehavior>().velocity, other.transform.position);
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
