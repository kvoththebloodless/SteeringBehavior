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


        SteerWander();

    }

    private void SteerAroundObstacles()
    {
        Vector3 obstaclepush;
        //MovementManager.ObstacleOffset(out obstaclepush, transform.position, transform.forward, (velocity.magnitude / max_speed) * 1f + 0.1f, 0.02f);
        //SetVelocity(velocity + obstaclepush);
    }

    private void SteerPursuit(Vector3 preyvelocity, Vector3 preyposition)
    {
        if (!seek)
        {
            var steering = MovementManager.Pursuit(preyvelocity, preyposition, velocity, transform.position, mass, 0.015f);
            SetVelocity(velocity + steering);
        }
        else
        {
            var steering = MovementManager.Seek(velocity, transform.position, preyposition, mass, 0.01f);
            SetVelocity(velocity + steering);
        }
    }
    private void SteerWander()
    {

        SetVelocity(velocity + MovementManager.Wander(transform, CIRCLE_DISTANCE, CIRCLE_RADIUS, ref anglerot, velocity, 0.001f)); ;

    }
    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            sawPlayer = true;
            SteerPursuit(other.GetComponent<PreyBehavior>().velocity, other.transform.position);
            max_speed = 1f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        sawPlayer = false;
        max_speed = 0.8f;
    }
    private void SetVelocity(Vector3 velocity)
    {
        this.velocity = Vector3.ClampMagnitude(velocity, max_speed);
        transform.forward = velocity.normalized;
        transform.position += velocity * Time.deltaTime;

    }


}
