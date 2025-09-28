using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 20f;
    [SerializeField] public float currentHealth;

    [Header("World-Space UI")]
    public GameObject healthBarPrefab;
    public Vector3 offset = new Vector3(0, 2.2f, 0);
    public Vector2 barSize = new Vector2(160f, 18f);
    [Range(0.001f, 0.2f)] public float worldScale = 0.03f;
    public Color bgColor = new Color(0, 0, 0, 0.5f);
    public Color fillColor = new Color(0.2f, 0.85f, 0.2f, 1f);

    private Transform barRoot;    // HealthBarRoot
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
        if (!mainCam) Debug.LogWarning("[EnemyHealthBar] No Camera.main found. Please tag your main camera as 'MainCamera'.");

        BuildOrRebuild();
    }

    void LateUpdate()
    {
        // ????????????
        if (fillImage && maxHealth > 0f)
            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);

        // ???????????????
        if (barRoot)
        {
            if (!mainCam) mainCam = Camera.main;
            if (mainCam)
            {
                Vector3 dir = (barRoot.position - mainCam.transform.position).normalized;
                barRoot.forward = dir;
            }
        }
    }

    // ---------- Public API ----------
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - Mathf.Abs(amount));
        if (currentHealth <= 0f)
        {
            Debug.Log($"{name} died");

            if (barRoot != null)
            {
                Destroy(barRoot.gameObject);
            }
        }
    }


    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Abs(amount));
    }

    [ContextMenu("Rebuild Now")]
    public void BuildOrRebuild()
    {
        // ??????????????
        if (barRoot) DestroyImmediate(barRoot.gameObject);

        // ????????
        var root = new GameObject("HealthBarRoot");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = offset;
        barRoot = root.transform;

        GameObject canvasGO;

        if (healthBarPrefab)
        {
            // ?????????
            canvasGO = Instantiate(healthBarPrefab, barRoot).gameObject;
            canvasGO.name = "Canvas";
            canvas = canvasGO.GetComponent<Canvas>();
            if (!canvas) canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rect = canvasGO.GetComponent<RectTransform>();
            rect.sizeDelta = barSize;
            rect.localScale = Vector3.one * worldScale;

            // ?? BarFill
            var fillTf = canvasGO.transform.Find("BarFill");
            if (!fillTf)
            {
                Debug.LogWarning("[EnemyHealthBar] Could not find child 'BarFill' in prefab. Creating one for you.");
                CreateFill(canvasGO.transform);
            }
            else
            {
                fillImage = fillTf.GetComponent<Image>();
                if (!fillImage) fillImage = fillTf.gameObject.AddComponent<Image>();
                if (fillImage.type != Image.Type.Filled)
                {
                    fillImage.type = Image.Type.Filled;
                    fillImage.fillMethod = Image.FillMethod.Horizontal;
                    fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                }
            }

            // ???????? BG ????????
            if (!canvasGO.transform.Find("BarBG"))
                CreateBG(canvasGO.transform);
        }
        else
        {
            // ????????? Canvas/BG/Fill ???
            canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasGroup));
            canvasGO.transform.SetParent(barRoot, false);
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rect = canvasGO.GetComponent<RectTransform>();
            rect.sizeDelta = barSize;
            rect.localScale = Vector3.one * worldScale;

            CreateBG(canvasGO.transform);
            CreateFill(canvasGO.transform);
        }
    }

    // ---------- Builders ----------
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
