using UnityEngine;
using System.Collections.Generic;

public class PersistentAssetController : MonoBehaviour
{
    [System.Serializable]
    public class PageTransformData
    {
        public int pageIndex;

        [Header("Local Transform")]
        public Vector3 localPosition;
        public Vector3 localEulerRotation;
        public Vector3 localScale = Vector3.one;
    }

    [Header("Slide Controller")]
    public SlideController slideController;

    [SerializeField]
    private List<PageTransformData> pageTransforms =
        new List<PageTransformData>();

    private SortedDictionary<int, PageTransformData> lookup;

    private int lastPage = -1;

    private void Awake()
    {
        lookup = new SortedDictionary<int, PageTransformData>();

        PageTransformData baseData = new PageTransformData
        {
            pageIndex = 0,
            localPosition = transform.localPosition,
            localEulerRotation = transform.localEulerAngles,
            localScale = transform.localScale
        };

        lookup[0] = baseData;

        foreach (var data in pageTransforms)
        {
            if (data == null)
                continue;

            if (!lookup.ContainsKey(data.pageIndex))
                lookup.Add(data.pageIndex, data);
        }
    }

    private void Update()
    {
        if (slideController == null)
            return;

        int currentPage = slideController.GetCurrentPage();

        if (currentPage != lastPage)
        {
            lastPage = currentPage;
            ApplyForPage(currentPage);
        }
    }

    private void ApplyForPage(int pageIndex)
    {
        if (lookup == null || lookup.Count == 0)
            return;

        PageTransformData chosen = null;

        foreach (var pair in lookup)
        {
            if (pair.Key > pageIndex)
                break;

            chosen = pair.Value;
        }

        if (chosen == null)
            return;

        Transform t = transform;

        // Reset first to avoid precision offset
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        // Apply exact transform
        t.localPosition = chosen.localPosition;
        t.localRotation = Quaternion.Euler(chosen.localEulerRotation);
        t.localScale = chosen.localScale;
    }
}
