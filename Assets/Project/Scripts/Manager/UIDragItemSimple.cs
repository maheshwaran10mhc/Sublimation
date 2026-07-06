using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UIDragItemSimple : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemID;

    [Header("Drag Scale")]
    public float dragScale = 1.2f;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector3 startPosition;
    private Vector3 originalScale;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = rect.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rect.position;
        canvasGroup.blocksRaycasts = false;

        rect.SetAsLastSibling(); // bring to front

        rect.localScale = originalScale * dragScale; // 🔹 scale while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rect,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out worldPos))
        {
            rect.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        rect.localScale = originalScale; // 🔹 restore scale

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            UIDropZone zone = hit.collider.GetComponent<UIDropZone>();

            if (zone != null)
            {
                if (zone.TryDrop(this))
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
        }

        rect.position = startPosition;
    }
}
