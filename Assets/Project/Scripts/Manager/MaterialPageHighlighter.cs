using UnityEngine;
using System.Collections.Generic;

public class MaterialPageHighlighter : MonoBehaviour
{
    [System.Serializable]
    public class PageHighlight
    {
        [Header("Target Page Number (Starts from 1)")]
        public int pageNumber = 1;

        [Header("Objects To Highlight")]
        public List<Renderer> targetRenderers = new List<Renderer>();

        [Header("Materials")]
        public Material originalMaterial;
        public Material highlightMaterial;

        [HideInInspector] public bool highlightActivated = false;
        [HideInInspector] public bool finished = false;
    }

    [Header("Slide Controller Reference")]
    public SlideController slideController;

    [Header("Page Highlight Setup")]
    public List<PageHighlight> pageHighlights = new List<PageHighlight>();

    void Update()
    {
        if (slideController == null)
            return;

        int currentPage = slideController.GetCurrentPage();

        foreach (var page in pageHighlights)
        {
            if (page.finished)
                continue;

            int pageIndex = page.pageNumber - 1;

            if (!page.highlightActivated && currentPage == pageIndex)
            {
                ApplyHighlight(page);
            }
        }
    }

    void ApplyHighlight(PageHighlight page)
    {
        foreach (var renderer in page.targetRenderers)
        {
            if (renderer == null) continue;

            renderer.material = page.highlightMaterial;
        }

        page.highlightActivated = true;
    }

    // Called from Unity Event
    public void RevertMaterial(int pageNumber)
    {
        foreach (var page in pageHighlights)
        {
            if (page.pageNumber != pageNumber)
                continue;

            foreach (var renderer in page.targetRenderers)
            {
                if (renderer == null) continue;

                renderer.material = page.originalMaterial;
            }

            // stop highlight system for this page
            page.highlightActivated = false;
            page.finished = true;
        }
    }
}
