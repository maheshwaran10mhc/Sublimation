using UnityEngine;
public class DragAndCollideAnimation : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private LayerMask draggableLayer = ~0;
    [SerializeField] private float dragHeight = 0f;

    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoord;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        // Called automatically by Unity when this object's collider is clicked
        zCoord = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        if (rb != null)
            rb.isKinematic = true; // avoid physics fighting the drag
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 newPosition = GetMouseWorldPosition() + offset;

        if (dragHeight != 0f)
            newPosition.y = dragHeight;

        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (rb != null)
            rb.isKinematic = false; // re-enable physics so it can register collisions
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}