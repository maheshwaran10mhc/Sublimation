using UnityEngine;
using UnityEngine.Events;

public class DragAndCheckCollision : MonoBehaviour
{
    private Camera cam;
    private bool isDragging;

    private Vector3 startPosition;
    private Vector3 offset;

    [Header("Target To Check")]
    public Collider targetCollider;

    [Header("Return Settings")]
    public float returnSpeed = 5f;

    [Header("Drag Material")]
    public Material dragMaterial;

    private Renderer[] objectRenderers;
    private Material[][] originalMaterials;

    [Header("Events")]
    public UnityEvent onCorrectCollision;
    public UnityEvent onWrongCollision;

    private bool isReturning = false;

    void Start()
    {
        cam = Camera.main;
        startPosition = transform.position;

        // Get all renderers on this object and its children
        objectRenderers = GetComponentsInChildren<Renderer>(true);

        originalMaterials = new Material[objectRenderers.Length][];

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            originalMaterials[i] = objectRenderers[i].materials;

            if (dragMaterial != null)
            {
                Material[] mats = objectRenderers[i].materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j] = dragMaterial;
                }

                objectRenderers[i].materials = mats;
            }
        }
    }


    void OnMouseDown()
    {
        // Restore original material while dragging
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            objectRenderers[i].materials = originalMaterials[i];
        }
        isDragging = true;
        isReturning = false;

        Vector3 mousePos = GetMouseWorldPos();
        offset = transform.position - mousePos;

    }


    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPos();

            // Follow mouse/finger
            transform.position = mousePos + offset;
        }

        if (isReturning)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                startPosition,
                returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
            {
                transform.position = startPosition;
                isReturning = false;

                // Restore drag material after returning
                if (dragMaterial != null)
                {
                    for (int i = 0; i < objectRenderers.Length; i++)
                    {
                        Material[] mats = objectRenderers[i].materials;

                        for (int j = 0; j < mats.Length; j++)
                        {
                            mats[j] = dragMaterial;
                        }

                        objectRenderers[i].materials = mats;
                    }
                }
            }

        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        isReturning = true;
    }



    Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Plane facing the camera through the object's start position
        Plane plane = new Plane(-cam.transform.forward, startPosition);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == targetCollider)
        {
            Debug.Log("Correct Object Hit!");
            onCorrectCollision?.Invoke();
        }
        else
        {
            Debug.Log("Wrong Object Hit!");
            onWrongCollision?.Invoke();
        }
    }
}
