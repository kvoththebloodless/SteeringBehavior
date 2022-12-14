using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private static float slowdownradius = 0f;

    public static Vector3 Seek(Vector3 currentVelocity, Vector3 currentposition, Vector3 finalposition, float mass, float maxforce)
    {

        var desiredVector = (finalposition - currentposition);
        Vector3 desiredVelocity;
        if (desiredVector.magnitude < slowdownradius)
        {
            desiredVelocity = desiredVector * (desiredVector.magnitude / (slowdownradius));
        }
        else
        {
            desiredVelocity = desiredVector;
        }

        //the farther, the more the steering force for seeking
        var steering = (desiredVelocity - currentVelocity) / mass;

        return Vector3.ClampMagnitude(ConstrainVectorToWorldXAndZ(steering), maxforce);
    }

    public static Vector3 Flee(Vector3 currentVelocity, Vector3 currentposition, Vector3 finalposition, float mass, float maxforce)
    {
        return -1 * Seek(currentVelocity, currentposition, finalposition, mass, maxforce);
    }

    public static Vector3 Wander(Transform t, float CIRCLE_DISTANCE, float CIRCLE_RADIUS, ref float anglerot, Vector3 velocity, float maxforce)
    {
        //https://www.youtube.com/watch?v=ujsR2vcJlLk
        //https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-wander--gamedev-1624

        //calculate the circle center
        var center = velocity.normalized;
        center.Scale(Vector3.one * CIRCLE_DISTANCE);

        DrawEllipse(t.position + t.forward.normalized * CIRCLE_DISTANCE, t.up, t.forward, CIRCLE_RADIUS, CIRCLE_RADIUS, 100, Color.red, 0);

        // Keeping this small means, that the deviation will be contained to one segment at a time almost, and gradually will move to other segments and hence gradual change in direction
        anglerot = Random.Range(anglerot - 10, anglerot + 10);
        //displacement
        var dispvector = Vector3.forward;
        var rotatedvector = Quaternion.AngleAxis(anglerot, Vector3.up) * dispvector.normalized;
        rotatedvector.Scale(Vector3.one * CIRCLE_RADIUS);
        //Debug.DrawLine(t.position + t.forward.normalized * CIRCLE_DISTANCE, t.position + t.forward.normalized * CIRCLE_DISTANCE + rotatedvector, Color.green);
        //change angle of rot by a bit for next frame. This part can be customized for different behavior


        return Vector3.ClampMagnitude(ConstrainVectorToWorldXAndZ(center + rotatedvector), maxforce);
    }

    public static Vector3 Pursuit(Vector3 velocityprey, Vector3 currentpositionprey, Vector3 velocityhunter, Vector3 currentpositionhunter, float mass, float maxforce)
    {
        if (velocityprey.magnitude == 0)
            return velocityprey;

        // how many updates the pursuer needs to move from its current position to the target position
        var improvedframesahead = (currentpositionprey - currentpositionhunter).magnitude / 0.5f;

        // If the distance is more then number of frames to lookahead increases, otherwise they reduce.
        var lookaheadposition = currentpositionprey + velocityprey * improvedframesahead;
        return Seek(velocityhunter, currentpositionhunter, lookaheadposition, mass, maxforce);

    }

    public static Vector3 Evade(Vector3 velocityprey, Vector3 currentpositionprey, Vector3 velocityhunter, Vector3 currentpositionhunter, float mass, float maxforce)
    {
        if (velocityprey.magnitude == 0)
            return velocityprey;

        // time to cover the distance between the hunter and prey = distance to cover/relative velocity at current velocities
        var improvedframesahead = (currentpositionprey - currentpositionhunter).magnitude / (velocityprey - velocityhunter).magnitude;

        // If the distance is more then number of frames to lookahead increases, otherwise they reduce.
        var lookaheadposition = currentpositionprey + velocityprey * improvedframesahead;

        //Debug.DrawLine(currentpositionhunter, lookaheadposition, Color.red);
        return Flee(velocityhunter, currentpositionhunter, lookaheadposition, mass, maxforce);

    }

    public static bool ObstacleOffset(out Vector3 offset, Vector3 position, Vector3 forward, Vector3 velocity, float max_speed, float maxdistance, float mindistance, float maxforce)
    {
        RaycastHit hitstraight;
        RaycastHit hitleft;
        RaycastHit hitright;
        var localmindistance = Mathf.Min(maxdistance, mindistance);
        var localmaxdistance = Mathf.Max(maxdistance, mindistance);
        var distance = Mathf.Clamp((velocity.magnitude / max_speed) * localmaxdistance + localmindistance, localmindistance, localmaxdistance);
        /*    Debug.DrawRay(position, forward * distance, Color.red, Time.deltaTime, false);
            Debug.DrawRay(position, Quaternion.AngleAxis(-45, Vector3.up) * forward * distance, Color.gray, Time.deltaTime, false);
            Debug.DrawRay(position, Quaternion.AngleAxis(+45, Vector3.up) * forward * distance, Color.gray, Time.deltaTime, false);*/
        offset = Vector3.zero;

        var hitst = Physics.Raycast(new Ray(position, velocity.normalized), out hitstraight, distance, LayerMask.GetMask("Obstacles", "Enemy"));
        if (hitst)
        { //THE FOLLOWING COMMENTED CODE IS NOT THE SAME AS HIT.NORMAL CAUSE IT WILL BE FROMT THE CENTER OF THE OBJECT RATHER THAN THE NORMAL WE WANT AT THE POINT
            //WHICH LEADS TO SITUATIONS WHERE SOMETIMES THE OBSTACLE OFFSET VECTOR ENCOURAGES GOING INTO THE WALL RATHER THAN AWAY FROM IT. ESPECIALLY AT CORNERS.
            /*offset += (hitstraight.point - hitstraight.transform.position);*/

            // The more the distance, the less the maxforce
            offset += ((position - hitstraight.point).magnitude) / maxdistance * (1 / maxforce) * hitstraight.normal;
            /*Debug.DrawRay(hitstraight.point, offset, Color.green, Time.deltaTime, false);*/
        }
        var hitlef = Physics.Raycast(new Ray(position, Quaternion.AngleAxis(-45, Vector3.up) * velocity.normalized), out hitleft, distance, LayerMask.GetMask("Obstacles", "Enemy"));
        if (hitlef)
        {
            offset += ((position - hitleft.point).magnitude) / maxdistance * (1 / maxforce) * hitleft.normal;
            /* Debug.DrawRay(hitleft.point, offset, Color.cyan, Time.deltaTime, false);*/
        }
        var hitri = Physics.Raycast(new Ray(position, Quaternion.AngleAxis(45, Vector3.up) * velocity.normalized), out hitright, distance, LayerMask.GetMask("Obstacles", "Enemy"));
        if (hitri)
        {
            offset += ((position - hitright.point).magnitude) / maxdistance * (1 / maxforce) * hitright.normal;
            /*Debug.DrawRay(hitright.point, offset, Color.blue, Time.deltaTime, false);*/
        }

        offset = Vector3.ClampMagnitude(ConstrainVectorToWorldXAndZ(offset), maxforce);

        return hitst || hitri || hitlef;

    }

    public static Vector3 FollowPath(Vector3 velocity, Vector3 currentposition, float mass, float maxforce, GameObject pathparent, ref int waypointindex, ref int direction, float radius)
    {
        if ((currentposition - pathparent.transform.GetChild(waypointindex).position).magnitude <= radius)
        {
            //should be going from the 0th way point to last or go back.
            if (waypointindex == pathparent.transform.childCount - 1)
            {
                direction = -1;
            }
            else if (waypointindex == 0)
            {
                direction = 1;
            }

            //Next waypoint position
            waypointindex += direction;

        }
        // seek the next waypoint
        return Seek(velocity, currentposition, pathparent.transform.GetChild(waypointindex).position, mass, maxforce);

    }

    //Taken from some forum
    private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0)
            {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    public static Vector3 HoardMode(Transform currentboid, Transform parent, PreyBehavior leader, Vector3 backpositiontoreach, Vector3 currentvelocity,
        Vector3 currentposition, float mass, float maxforceseek, float separationradius, float maxforceseparation, float maxforcevade)
    {
        Vector3 steering = Vector3.zero;

        steering += Seek(currentvelocity, currentposition, backpositiontoreach, mass, maxforceseek);
        steering += SeparationForce(parent, currentboid, separationradius, maxforceseparation);
        Debug.DrawLine(currentposition, backpositiontoreach, Color.red);
        //Evade the leader boid so we don't come in its way
        steering += Evade(leader.velocity, leader.transform.position, currentvelocity, currentposition, mass, (1 / (leader.transform.position - currentposition).magnitude) * maxforcevade);

        return ConstrainVectorToWorldXAndZ(steering);
    }

    private static Vector3 SeparationForce(Transform parent, Transform currentboid, float separationradius, float maxforce)
    {

        //Keep the current boid away form all the other boids
        Vector3 steering = Vector3.zero;
        for (int i = 0; i < parent.childCount; i++)
        {
            var boid = parent.GetChild(i);

            if (boid != currentboid && Vector3.Distance(currentboid.position, boid.position) <= separationradius)
            {
                // more force fromt he neighbour the closer you are
                steering += (currentboid.position - boid.position).normalized * separationradius / Mathf.Clamp(Vector3.Distance(currentboid.position, boid.position), 0.01f, Vector3.Distance(currentboid.position, boid.position));

            }
        }
        steering = Vector3.ClampMagnitude(steering, maxforce);
        return ConstrainVectorToWorldXAndZ(steering);

    }
    private static Vector3 ConstrainVectorToWorldXAndZ(Vector3 offset)
    {
        var x = Vector3.Dot(offset, Vector3.right) * Vector3.right;
        var z = Vector3.Dot(offset, Vector3.forward) * Vector3.forward;
        return x + z;
    }
}
