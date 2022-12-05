using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private static float slowdownradius = 0.5f;

    public static Vector3 Seek(Vector3 currentVelocity, Vector3 currentposition, Vector3 finalposition, float mass, float speed)
    {
        var desiredVector = (finalposition - currentposition);
        Vector3 desiredVelocity;
        if (desiredVector.magnitude < slowdownradius)
        {
            desiredVelocity = desiredVector * speed * (desiredVector.magnitude / (slowdownradius));
        }
        else
        {
            desiredVelocity = desiredVector * speed;
        }
        var steering = (desiredVelocity - currentVelocity) / mass;

        return Vector3.ClampMagnitude(steering, speed);
    }

    public static Vector3 Flee(Vector3 currentVelocity, Vector3 currentposition, Vector3 finalposition, float mass, float speed)
    {
        return -1 * Seek(currentVelocity, currentposition, finalposition, mass, speed);
    }

    public static Vector3 Wander(Transform t, float CIRCLE_DISTANCE, float CIRCLE_RADIUS, ref float anglerot, Vector3 velocity)
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
        Debug.DrawLine(t.position + t.forward.normalized * CIRCLE_DISTANCE, t.position + t.forward.normalized * CIRCLE_DISTANCE + rotatedvector, Color.green);
        //change angle of rot by a bit for next frame. This part can be customized for different behavior


        return center + rotatedvector;
    }

    public static Vector3 Pursuit(Vector3 velocityprey, Vector3 currentpositionprey, Vector3 velocityhunter, Vector3 currentpositionhunter, float mass, float speed)
    {
        var improvedframesahead = (currentpositionhunter - currentpositionhunter).magnitude / speed;
        var lookaheadposition = currentpositionprey + velocityprey * improvedframesahead;
        return Seek(velocityhunter, currentpositionhunter, lookaheadposition, mass, speed);

    }

    public Vector3 Evade(Vector3 velocityprey, Vector3 currentpositionprey, Vector3 velocityhunter, Vector3 currentpositionhunter, float mass, float speed)
    {
        return -1 * Pursuit(velocityprey, currentpositionprey, velocityhunter, currentpositionhunter, mass, speed);
    }

    public static bool ObstacleOffset(out Vector3 offset, Vector3 position, Vector3 forward, float maxdistance, Color color)
    {
        RaycastHit hit;
        Debug.DrawRay(position, forward, color, Time.deltaTime, false);
        if (Physics.Raycast(new Ray(position, forward), out hit, maxdistance, LayerMask.GetMask("Obstacles", "Enemy")))
        {
            Debug.DrawRay(hit.point, hit.point - hit.transform.position, Color.green, Time.deltaTime, false);
            //THIS IS ESSENTIALLY THE SAME AS HIT.NORMAL
            offset = (hit.point - hit.transform.position);
            return true;
        }
        offset = Vector3.zero;
        return false;
    }
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

}
