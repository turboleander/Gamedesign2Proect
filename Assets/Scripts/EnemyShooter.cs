using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovementCircleOnly : MonoBehaviour
{
    [Header("Circle Patrol (XZ)")]
    public Transform circleCenter;
    public Vector3 centerOffset = Vector3.zero;
    public float circleRadius = 4f;
    public bool clockwise = false;
    public float linearSpeed = 3f;
    public bool faceAlongMove = true;

    [Header("Detect / Engage")]
    public Transform target;
    public bool autoFindTargetByTag = true;
    public string playerTag = "Player";
    public float detectRange = 10f;

    [Header("Attack (Projectile)")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireCooldown = 1.5f;
    private float nextFireTime = 0f;

    [Header("Strafe (Engage)")]
    public float strafeSpeed = 2.5f;
    public float strafeSwitchSeconds = 2f;

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
    private int strafeDir = 1;
    private float strafeTimer = 0f;

    private enum State { Patrol, Engage }
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
        if (flatToSelf.sqrMagnitude < 0.0001f)
        {
            theta = 0f;
            Vector3 startOnCircle = Flat(centerPos) + new Vector3(circleRadius, 0f, 0f);
            SetXZ(startOnCircle);
        }
        else
        {
            theta = Mathf.Atan2(flatToSelf.z, flatToSelf.x);
            Vector3 onCircle = Flat(centerPos) + new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta)) * circleRadius;
            SetXZ(onCircle);
        }
    }

    void Update()
    {
        bool inRange = target && Vector3.Distance(Flat(transform.position), Flat(target.position)) <= detectRange;
        state = inRange ? State.Engage : State.Patrol;

        if (state == State.Patrol)
        {
            PatrolCircleXZ();
        }
        else
        {
            EngageStrafe();
            TryShoot();
        }

        // 👇 หันหน้าเข้าหาผู้เล่นตลอดเวลา (ใช้ smooth rotation)
        if (target)
        {
            Vector3 toTarget = Flat(target.position) - Flat(transform.position);
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            }
        }

        // -------------------- จัดการแกน Y --------------------
        if (useGroundSnap) GroundSnap();
        else if (lockYToStart)
        {
            var p = transform.position;
            p.y = startY;
            transform.position = p;
        }
    }

    // ----------------- Patrol -----------------
    private void PatrolCircleXZ()
    {
        float angularSpeed = (circleRadius > 0.0001f) ? (linearSpeed / circleRadius) : 0f;
        if (clockwise) angularSpeed = -angularSpeed;

        theta += angularSpeed * Time.deltaTime;
        Vector3 desired = Flat(centerPos) + new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta)) * circleRadius;
        Vector3 step = (desired - Flat(transform.position));
        float maxStep = linearSpeed * Time.deltaTime;
        if (step.magnitude > maxStep) step = step.normalized * maxStep;

        cc.Move(step);
    }

    // ----------------- Engage -----------------
    private void EngageStrafe()
    {
        strafeTimer += Time.deltaTime;
        if (strafeTimer >= strafeSwitchSeconds)
        {
            strafeTimer = 0f;
            strafeDir *= -1;
        }

        Vector3 right = transform.right; right.y = 0f; right.Normalize();
        Vector3 step = right * (strafeDir * strafeSpeed * Time.deltaTime);
        cc.Move(step);
    }

    // ----------------- Shoot -----------------
    private void TryShoot()
    {
        if (!target || !projectilePrefab || !firePoint) return;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;

            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector3 dir = (target.position - firePoint.position).normalized;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.linearVelocity = dir * projectileSpeed;
            }

            Collider bulletCol = bullet.GetComponent<Collider>();
            Collider shooterCol = GetComponent<Collider>();
            if (bulletCol != null && shooterCol != null)
                Physics.IgnoreCollision(bulletCol, shooterCol);

            Destroy(bullet, 5f);
        }
    }

    // ----------------- Ground Snap -----------------
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

    // ----------------- Helpers -----------------
    private static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);

    private void SetXZ(Vector3 flatPos)
    {
        var p = transform.position;
        p.x = flatPos.x;
        p.z = flatPos.z;
        transform.position = p;
    }

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
