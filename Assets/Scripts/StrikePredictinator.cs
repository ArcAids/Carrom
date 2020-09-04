using UnityEngine;

public class StrikePredictinator : MonoBehaviour  //Property of Doofinshmertz evil inc.
{
    [SerializeField] PredictionArrow arrow = null;
    [SerializeField] LayerMask layerMask = new LayerMask();

    [SerializeField]
    [Tooltip("Radius of striker collider")]
    float strikerRadius = 0.1971429f;

    [SerializeField]
    [Tooltip("Radius of coin collider")]
    float coinRadius = 0.135f;

    float minStrikerCoinDistance = 0.33f;
    Vector3 aim90DegreesTurned = Vector3.one;
    Vector3 predictedStrikerPositionWhenHit = Vector3.one;

    private void Awake()
    {
        minStrikerCoinDistance = coinRadius + strikerRadius;
    }
    public void Enable(bool state)
    {
        arrow.Enable(state);
    }

    public void Predict(Vector2 aimDirection)
    {

        aim90DegreesTurned = new Vector3(aimDirection.y, -aimDirection.x);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 20, layerMask);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position + aim90DegreesTurned * strikerRadius, aimDirection, 20, layerMask);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position - aim90DegreesTurned * strikerRadius, aimDirection, 20, layerMask);

#if UNITY_EDITOR
        Debug.DrawRay(transform.position + aim90DegreesTurned * strikerRadius, aimDirection, Color.red, Time.deltaTime);
        Debug.DrawRay(transform.position - aim90DegreesTurned * strikerRadius, aimDirection, Color.red, Time.deltaTime);
        Debug.DrawRay(transform.position, aimDirection, Color.red, Time.deltaTime);
#endif


        hit = CalculateClosest(hit, CalculateClosest(rightHit, leftHit));

        if (hit)
        {
            arrow.Enable(true);
            arrow.MoveTo(hit.transform.position);
            arrow.RotateTo(PredictedTargetDirectionAngle(hit.transform.position, aimDirection));

            //Old simple test code.
            //Vector2 direction = (new Vector2(hit.transform.position.x,hit.transform.position.y)- hit.point).normalized;
            //arrow.RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        else
            arrow.Enable(false);
    }

    private RaycastHit2D CalculateClosest(RaycastHit2D hit, RaycastHit2D hit2)
    {
        if (!hit)
            return hit2;
        if (!hit2)
            return hit;

        if (hit.distance > hit2.distance)
            return hit2;
        else
            return hit;
    }

    float PredictedTargetDirectionAngle(Vector3 targetPosition, Vector3 aimDirection)
    {
        Vector3 intersection;

        LineLineIntersection(out intersection, transform.position, aimDirection, targetPosition, aim90DegreesTurned);

        Debug.DrawRay(intersection, aim90DegreesTurned, Color.green, Time.deltaTime);

        float intersectionTargetDistanceSqrd = (intersection - targetPosition).sqrMagnitude;

        float distanceFromIntersection = Mathf.Sqrt(minStrikerCoinDistance * minStrikerCoinDistance - intersectionTargetDistanceSqrd);

        Ray2D ray = new Ray2D(intersection, (transform.position - intersection).normalized);
        predictedStrikerPositionWhenHit = ray.GetPoint(distanceFromIntersection);

        Debug.DrawLine(predictedStrikerPositionWhenHit, targetPosition, Color.blue, Time.deltaTime);
        Vector2 predictedDirection = targetPosition - predictedStrikerPositionWhenHit;
        return (Mathf.Atan2(predictedDirection.y, predictedDirection.x)) * Mathf.Rad2Deg;
    }

    //Code from Unity Wiki to find intersection point
    bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(predictedStrikerPositionWhenHit, strikerRadius);
    }
}
