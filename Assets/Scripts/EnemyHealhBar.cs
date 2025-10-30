using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 75f;
    [SerializeField] public float currentHealth;

    [Header("World-Space UI")]
    public GameObject healthBarPrefab;
    public Vector3 offset = new Vector3(0, 2.2f, 0);
    public Vector2 barSize = new Vector2(160f, 18f);
    [Range(0.001f, 0.2f)] public float worldScale = 0.03f;
    public Color bgColor = new Color(0, 0, 0, 0.5f);
    public Color fillColor = new Color(0.2f, 0.85f, 0.2f, 1f);

    private Transform barRoot;
    private Canvas canvas;
    private Image fillImage;
    private Camera mainCam;

    void Awake()
    {
        if (currentHealth <= 0f) currentHealth = maxHealth;
    }

    void Start()
    {
        mainCam = Camera.main;
        if (!mainCam)
            Debug.LogWarning("[EnemyHealthBar] No Camera.main found.");

        // ลบ HealthBarRoot เดิมก่อนสร้างใหม่
        var existing = transform.Find("HealthBarRoot");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        BuildOrRebuild();
    }

    void LateUpdate()
    {
        if (fillImage && maxHealth > 0f)
            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);

        if (barRoot && mainCam)
        {
            barRoot.LookAt(mainCam.transform);
            barRoot.Rotate(0, 180f, 0); // แก้ bar กลับด้าน
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - Mathf.Abs(amount));
        if (currentHealth <= 0f)
        {
            if (barRoot != null)
                Destroy(barRoot.gameObject);

            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Abs(amount));
    }

    [ContextMenu("Rebuild Now")]
    public void BuildOrRebuild()
    {
        // ลบ HealthBarRoot เดิมอีกครั้งเพื่อความชัวร์
        if (barRoot != null)
            DestroyImmediate(barRoot.gameObject);

        // สร้าง HealthBarRoot ใหม่
        barRoot = new GameObject("HealthBarRoot").transform;
        barRoot.SetParent(transform, false);
        barRoot.localPosition = offset;

        GameObject canvasGO;

        if (healthBarPrefab)
        {
            canvasGO = Instantiate(healthBarPrefab, barRoot).gameObject;
            canvasGO.name = "Canvas";
            canvas = canvasGO.GetComponent<Canvas>() ?? canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var fillTf = canvasGO.transform.Find("BarFill");
            if (fillTf) fillImage = fillTf.GetComponent<Image>();
            else CreateFill(canvasGO.transform);

            if (!canvasGO.transform.Find("BarBG"))
                CreateBG(canvasGO.transform);
        }
        else
        {
            canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasGroup));
            canvasGO.transform.SetParent(barRoot, false);
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            CreateBG(canvasGO.transform);
            CreateFill(canvasGO.transform);
        }

        var rect = canvasGO.GetComponent<RectTransform>();
        rect.sizeDelta = barSize;
        rect.localScale = Vector3.one * worldScale;
    }

    void CreateBG(Transform parent)
    {
        var bgGO = new GameObject("BarBG", typeof(Image));
        bgGO.transform.SetParent(parent, false);
        var bgImg = bgGO.GetComponent<Image>();
        bgImg.sprite = MakeSolidSprite(bgColor);

        var r = bgGO.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0, 0.5f);
        r.anchorMax = new Vector2(1, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(0, barSize.y);
    }

    void CreateFill(Transform parent)
    {
        var fillGO = new GameObject("BarFill", typeof(Image));
        fillGO.transform.SetParent(parent, false);
        fillImage = fillGO.GetComponent<Image>();
        fillImage.sprite = MakeSolidSprite(fillColor);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;

        var r = fillGO.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0, 0.5f);
        r.anchorMax = new Vector2(1, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(0, barSize.y - 4f);
    }

    static Sprite MakeSolidSprite(Color c)
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, c);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var pos = transform.position + offset;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, pos);
        Gizmos.DrawWireCube(pos, new Vector3(0.2f, 0.2f, 0.01f));
    }
#endif
}
