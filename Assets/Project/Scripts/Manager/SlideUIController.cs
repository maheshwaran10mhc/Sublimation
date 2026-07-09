using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class SlideUIController : MonoBehaviour
{
    [System.Serializable]
    public class PageUIData
    {
        [TextArea(3, 6)]
        public string pageText;

        public float width = 700f;
        public float height = 150f;

        // -------- NEW PER PAGE RECT SETTINGS --------
        public Vector2 anchoredPosition;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        // --------------------------------------------
        public bool preserveAspect = true;
    }

    [Header("References")]
    public SlideController slideController;

    [Header("UI")]
    public TMP_Text pageTextUI;
    public RectTransform commonImageRect;

    [Header("Default Rect Settings")]
    public Vector2 defaultAnchoredPosition;
    public Vector2 defaultAnchorMin;
    public Vector2 defaultAnchorMax;
    public Vector2 defaultPivot;

    [Header("Page UI Data")]
    public List<PageUIData> pageUIData = new List<PageUIData>();

    private int lastPage = -1;
    private Image commonImage;



    void Start()
    {
        if (commonImageRect != null)
            commonImage = commonImageRect.GetComponent<Image>();

        ApplyDefaultRectSettings();
        UpdateUI();
    }

    void Update()
    {
        if (slideController == null)
            return;

        int currentPage = slideController.GetCurrentPage();

        if (currentPage != lastPage)
        {
            UpdateUI();
            lastPage = currentPage;
        }
    }

    void ApplyDefaultRectSettings()
    {
        if (commonImageRect == null) return;

        commonImageRect.anchorMin = defaultAnchorMin;
        commonImageRect.anchorMax = defaultAnchorMax;
        commonImageRect.pivot = defaultPivot;
        commonImageRect.anchoredPosition = defaultAnchoredPosition;
        commonImageRect.localScale = Vector3.one;
        commonImageRect.localRotation = Quaternion.identity;
    }

    void UpdateUI()
    {
        if (slideController == null) return;

        int page = slideController.GetCurrentPage();

        if (page < 0 || page >= pageUIData.Count)
            return;

        PageUIData data = pageUIData[page];

        if (pageTextUI != null)
            pageTextUI.text = data.pageText;

        if (commonImageRect != null)
        {
            // Apply page rect settings
            commonImageRect.anchorMin = data.anchorMin;
            commonImageRect.anchorMax = data.anchorMax;
            commonImageRect.pivot = data.pivot;
            commonImageRect.anchoredPosition = data.anchoredPosition;

            commonImageRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, data.width);

            commonImageRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, data.height);
        }
        if (commonImage != null)
        {
            commonImage.preserveAspect = data.preserveAspect;
        }
    }
}
