using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UIDropZone : MonoBehaviour
{
    [Header("Correct Item ID")]
    public string correctItemID;

    [Header("Event")]
    public UnityEvent OnCorrectDrop;

    private bool completed;

    public bool TryDrop(UIDragItemSimple item)
    {
        if (completed)
            return false;

        if (item.itemID != correctItemID)
            return false;

        completed = true;

        OnCorrectDrop?.Invoke();

        return true;
    }
}
