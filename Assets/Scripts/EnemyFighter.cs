using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovementFighter : MonoBehaviour
{
    [Header("Circle Patrol (XZ)")]
    public Transform circleCenter;
    public Vector3 centerOffset = Vector3.zero;
    public float circleRadius = 4f;
    public bool clockwise = false;
    public float patrolSpeed = 3f;

    [Header("Chase / Engage")]
    public Transform target;
    public bool autoFindTargetByTag = true;
    public string playerTag = "Player";
    public float detectRange = 10f;
    public float chaseSpeed = 4f;

    [Header("Y Handling")]
    public bool useGroundSnap = false;
    public LayerMask groundMask = ~0;
    public float snapRayLength = 2f;
    public float snapOffset = 0f;
    public bool lockYToStart = true;

    [Header("Debug")]
    public bool drawCircleGizmo = true;

    private CharacterController cc;
    private Vector3 centerPos;
    private float startY;
    private float theta;

    private enum State { Patrol, Chase }
    private State state = State.Patrol;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        startY = transform.position.y;

        if (autoFindTargetByTag && target == null)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) target = p.transform;
        }

        centerPos = (circleCenter ? circleCenter.position : transform.position) + centerOffset;

        Vector3 flatToSelf = Flat(transform.position) - Flat(centerPos);
        theta = Mathf.Atan2(flatToSelf.z, flatToSelf.x);
    }

    void Update()
    {
        bool inRange = target && Vector3.Distance(Flat(transform.position), Flat(target.position)) <= detectRange;
        state = inRange ? State.Chase : State.Patrol;

        if (state == State.Patrol)
        {
            PatrolCircleXZ();
        }
        else
        {
            ChaseTarget();
        }

        if (useGroundSnap)
            GroundSnap();
        else if (lockYToStart)
        {
            var p = transform.position;
            p.y = startY;
            transform.position = p;
        }
    }

    private void PatrolCircleXZ()
    {
        float angularSpeed = (circleRadius > 0.0001f) ? (patrolSpeed / circleRadius) : 0f;
        if (clockwise) angularSpeed = -angularSpeed;

        theta += angularSpeed * Time.deltaTime;
        Vector3 desired = Flat(centerPos) + new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta)) * circleRadius;
        Vector3 step = desired - Flat(transform.position);
        float maxStep = patrolSpeed * Time.deltaTime;
        if (step.magnitude > maxStep) step = step.normalized * maxStep;
        cc.Move(step);
    }

    private void ChaseTarget()
    {
        if (!target) return;

        Vector3 toTarget = Flat(target.position) - Flat(transform.position);
        if (toTarget.sqrMagnitude < 0.01f) return;

        Vector3 dir = toTarget.normalized;
        cc.Move(dir * chaseSpeed * Time.deltaTime);

        
        var look = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, look.eulerAngles.y, 0f);
    }

    private void GroundSnap()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, snapRayLength, groundMask))
        {
            var p = transform.position;
            p.y = hit.point.y + snapOffset;
            transform.position = p;
        }
    }

    private static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);

    void OnDrawGizmosSelected()
    {
        if (!drawCircleGizmo) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Vector3 c = (circleCenter ? circleCenter.position : transform.position) + centerOffset;
        Gizmos.color = new Color(0f, 1f, 1f, 0.7f);
        const int seg = 64;
        Vector3 prev = c + new Vector3(circleRadius, 0f, 0f);
        for (int i = 1; i <= seg; i++)
        {
            float a = (i / (float)seg) * Mathf.PI * 2f;
            Vector3 p = c + new Vector3(Mathf.Cos(a) * circleRadius, 0f, Mathf.Sin(a) * circleRadius);
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }
}
