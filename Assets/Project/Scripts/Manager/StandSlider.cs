using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class StandSlider : MonoBehaviour
{
    [Header("Movement Limits")]
    public float minX = -2f;
    public float maxX = 2f;

    [Header("Target Lock Position")]
    public float lockPositionX = 1f;
    public float lockTolerance = 0.05f;

    [Header("Attempts Required")]
    public int requiredAttempts = 3;

    [Header("Enable Attempt System")]
    public bool useAttemptSystem = true;

    [Header("Blur Canvas Group")]
    public CanvasGroup blurCanvasGroup;

    [Header("Objects To Enable After Lock")]
    public List<GameObject> objectsToEnable = new List<GameObject>();

    [Header("Lock Event (Attempt System)")]
    public UnityEvent onLocked;

    [Header("Event When Moved Twice (Attempt System OFF)")]
    public UnityEvent onMovedTwice;

    private int attempts = 0;
    private bool locked = false;

    private bool dragging = false;
    private Vector3 lastMouseWorld;
    private Camera cam;

    private int moveCount = 0;

    void Start()
    {
        cam = Camera.main;

        if (blurCanvasGroup != null)
            blurCanvasGroup.alpha = 1f;
    }

    void Update()
    {
        if (locked)
            return;

        HandleDrag();
        UpdateBlurAlpha();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    dragging = true;

                    // COUNT MOVES ONLY IF ATTEMPT SYSTEM IS OFF
                    if (!useAttemptSystem)
                    {
                        moveCount++;

                        if (moveCount == 2)
                        {
                            onMovedTwice?.Invoke();
                        }
                    }

                    lastMouseWorld = cam.ScreenToWorldPoint(
                        new Vector3(
                            Input.mousePosition.x,
                            Input.mousePosition.y,
                            cam.WorldToScreenPoint(transform.position).z
                        ));
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;

            if (useAttemptSystem)
                CheckTarget();
        }

        if (!dragging)
            return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                cam.WorldToScreenPoint(transform.position).z
            ));

        float deltaX = mouseWorld.x - lastMouseWorld.x;

        Vector3 pos = transform.position;
        pos.x += deltaX;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;

        lastMouseWorld = mouseWorld;
    }

    void UpdateBlurAlpha()
    {
        if (blurCanvasGroup == null)
            return;

        float t = Mathf.InverseLerp(minX, maxX, transform.position.x);

        blurCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
    }

    void CheckTarget()
    {
        if (Mathf.Abs(transform.position.x - lockPositionX) <= lockTolerance)
        {
            attempts++;

            if (attempts >= requiredAttempts)
            {
                LockStand();
            }
        }
    }

    void LockStand()
    {
        locked = true;

        Vector3 pos = transform.position;
        pos.x = lockPositionX;
        transform.position = pos;

        if (blurCanvasGroup != null)
            blurCanvasGroup.alpha = 1f;

        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        onLocked?.Invoke();
    }

    public void EnableAttemptSystem(bool value)
    {
        useAttemptSystem = value;
    }
}
