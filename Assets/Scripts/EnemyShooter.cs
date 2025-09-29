using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovementCircleOnly : MonoBehaviour
{
    [Header("Circle Patrol (XZ)")]
    public Transform circleCenter;        // ถ้าเว้นว่าง = ใช้ตำแหน่งเริ่มต้นเป็นศูนย์กลาง
    public Vector3 centerOffset = Vector3.zero;
    public float circleRadius = 4f;       // รัศมีวงกลม
    public bool clockwise = false;        // ทิศทางเดิน
    public float linearSpeed = 3f;        // ความเร็วเชิงเส้นรอบวง (m/s)
    public bool faceAlongMove = true;     // หมุนหันตามทิศทางวิ่งตอนลาดตระเวน

    [Header("Detect / Engage")]
    public Transform target;              // ใส่ผู้เล่น (หรือปล่อยว่างแล้วให้ autoFindByTag)
    public bool autoFindTargetByTag = true;
    public string playerTag = "Player";
    public float detectRange = 10f;       // ระยะตรวจจับ

    [Header("Strafe (เมื่อพบเป้าหมาย)")]
    public float strafeSpeed = 2.5f;      // ความเร็วสไลด์ซ้าย/ขวา
    public float strafeSwitchSeconds = 2f;// เวลาสลับทิศ (ตามที่ขอ = 2 วิ)
    public bool faceTargetWhileStrafing = true; // หันหน้าเข้าหาเป้าหมายขณะสไลด์

    [Header("Y Handling (พื้นราบ / XZ เท่านั้น)")]
    public bool useGroundSnap = false;    // Raycast หา Y ของพื้น
    public LayerMask groundMask = ~0;
    public float snapRayLength = 2f;
    public float snapOffset = 0f;
    public bool lockYToStart = true;      // ล็อก Y คงที่ (ปิดถ้าใช้ GroundSnap)

    [Header("Debug")]
    public bool drawCircleGizmo = true;

    private CharacterController cc;
    private Vector3 centerPos;
    private float startY;
    private float theta;                  // มุมรอบวง (เรเดียน)
    private int strafeDir = 1;            // +1 ขวา, -1 ซ้าย
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

        // ตั้งศูนย์กลางวงกลม
        centerPos = (circleCenter ? circleCenter.position : transform.position) + centerOffset;

        // คำนวณมุมเริ่มต้นจากตำแหน่งปัจจุบัน → center
        Vector3 flatToSelf = Flat(transform.position) - Flat(centerPos);
        if (flatToSelf.sqrMagnitude < 0.0001f)
        {
            // ถ้าตรงศูนย์กลางพอดี ให้วางบนวงกลมทิศ +X
            theta = 0f;
            Vector3 startOnCircle = Flat(centerPos) + new Vector3(circleRadius, 0f, 0f);
            SetXZ(startOnCircle);
        }
        else
        {
            theta = Mathf.Atan2(flatToSelf.z, flatToSelf.x); // atan2(z,x)
            // บังคับให้อยู่บนรัศมีที่กำหนด
            Vector3 onCircle = Flat(centerPos) + new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta)) * circleRadius;
            SetXZ(onCircle);
        }
    }

    void Update()
    {
        // ----- ตัดสินสถานะ -----
        bool inRange = target && Vector3.Distance(Flat(transform.position), Flat(target.position)) <= detectRange;
        state = inRange ? State.Engage : State.Patrol;

        if (state == State.Patrol)
        {
            PatrolCircleXZ();
        }
        else
        {
            EngageStrafe();
        }

        // ----- จัดการแกน Y -----
        if (useGroundSnap)
        {
            GroundSnap();
        }
        else if (lockYToStart)
        {
            var p = transform.position; p.y = startY; transform.position = p;
        }
    }

    // ===== PATROL: เดินเป็นวงกลมบน XZ =====
    private void PatrolCircleXZ()
    {
        // ระยะทางต่อวินาที = linearSpeed → ความเร็วเชิงมุม rad/s = v / r
        float angularSpeed = (circleRadius > 0.0001f) ? (linearSpeed / circleRadius) : 0f;
        if (clockwise) angularSpeed = -angularSpeed;

        // อัปเดตมุม
        theta += angularSpeed * Time.deltaTime;

        // เป้าหมายตำแหน่งถัดไปบนวงกลม
        Vector3 desired = Flat(centerPos) + new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta)) * circleRadius;

        // เวคเตอร์ก้าว
        Vector3 step = (desired - Flat(transform.position));
        // จำกัดก้าวให้ความยาวเท่า v*dt (กันอาการวาร์ปถ้าค่าเฟรมตก)
        float maxStep = linearSpeed * Time.deltaTime;
        if (step.magnitude > maxStep) step = step.normalized * maxStep;

        cc.Move(step);

        // หมุนหันตามทิศทางการเคลื่อนที่
        if (faceAlongMove && step.sqrMagnitude > 0.000001f)
        {
            Vector3 dir = step.normalized;
            var look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, look.eulerAngles.y, 0f);
        }
    }

    // ===== ENGAGE: หยุดเดินวงกลม แล้วสลับสไลด์ซ้าย/ขวา ทีละ N วิ =====
    private void EngageStrafe()
    {
        // สลับทิศทุก strafeSwitchSeconds วินาที
        strafeTimer += Time.deltaTime;
        if (strafeTimer >= strafeSwitchSeconds)
        {
            strafeTimer = 0f;
            strafeDir *= -1; // สลับซ้าย/ขวา
        }

        // ทิศทางสไลด์บนระนาบ XZ (อิงท้องถิ่นของศัตรู)
        Vector3 right = transform.right; right.y = 0f; right.Normalize();
        Vector3 step = right * (strafeDir * strafeSpeed * Time.deltaTime);
        cc.Move(step);

        // หันหน้าเข้าหาเป้าหมายระหว่างสไลด์ (ตามที่ต้องการ)
        if (faceTargetWhileStrafing && target)
        {
            Vector3 to = Flat(target.position) - Flat(transform.position);
            if (to.sqrMagnitude > 0.000001f)
            {
                var look = Quaternion.LookRotation(to.normalized, Vector3.up);
                transform.rotation = Quaternion.Euler(0f, look.eulerAngles.y, 0f);
            }
        }
    }

    // ===== Utils =====
    private void GroundSnap()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, snapRayLength, groundMask))
        {
            var p = transform.position; p.y = hit.point.y + snapOffset; transform.position = p;
        }
    }

    private static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);

    private void SetXZ(Vector3 flatPos)
    {
        var p = transform.position;
        p.x = flatPos.x; p.z = flatPos.z;
        transform.position = p;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawCircleGizmo) return;

        // วาดรัศมีตรวจจับ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // วาดวงกลมลาดตระเวน
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
