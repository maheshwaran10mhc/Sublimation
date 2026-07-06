using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SlideController : MonoBehaviour
{
    [System.Serializable]
    public class PageData
    {
        [Header("Events")]
        public UnityEvent onNextClick;
        public UnityEvent onNextCompleted;
        public UnityEvent onBackClick;

        [Header("Lock Two Buttons On This Page?")]
        public bool lockTwoButtons;
    }

    [Header("Total Pages")]
    public int totalPages = 1;

    [Header("Buttons")]
    public Button nextButton;
    public Button secondButton;

    [Header("Page Number UI")]
    public TMP_Text pageNumberText;

    [Header("Page Settings")]
    public List<PageData> pages = new List<PageData>();

    [Header("Testing Cheat")]
    public bool cheatButtons = false; // 🔥 Cheat checkbox

    protected int currentPage = 0;
    protected HashSet<int> completedPages = new HashSet<int>();

    private HashSet<int> nextClickTriggeredPages = new HashSet<int>();

    protected virtual void Start()
    {
        UpdatePage();
        UpdateButtonStates();
    }

    public int GetCurrentPage()
    {
        return currentPage;
    }

    public void NextPage()
    {
        if (currentPage >= totalPages - 1)
            return;

        if (!cheatButtons && !completedPages.Contains(currentPage))
            return;

        if (!nextClickTriggeredPages.Contains(currentPage))
        {
            pages[currentPage]?.onNextClick?.Invoke();
            nextClickTriggeredPages.Add(currentPage);
        }

        int previous = currentPage;
        currentPage++;

        UpdatePage();
        UpdateButtonStates();

        pages[previous]?.onNextCompleted?.Invoke();
    }

    public void BackPage()
    {
        if (!cheatButtons && currentPage <= 0)
            return;

        if (currentPage <= 0)
            return;

        pages[currentPage]?.onBackClick?.Invoke();

        currentPage--;

        

        UpdatePage();
        UpdateButtonStates();
    }

    public void MarkPageCompleted()
    {
        if (!completedPages.Contains(currentPage))
            completedPages.Add(currentPage);

        UpdateButtonStates();
    }

    protected virtual void UpdatePage()
    {
        UpdatePageNumber();
    }

    void UpdateButtonStates()
    {
        bool isCompleted = completedPages.Contains(currentPage);

        if (nextButton != null)
        {
            nextButton.interactable =
                (cheatButtons || isCompleted) && currentPage < totalPages - 1;
        }

        if (secondButton != null && currentPage < pages.Count)
        {
            if (pages[currentPage].lockTwoButtons)
            {
                secondButton.interactable = isCompleted;
            }
            else
            {
                secondButton.interactable = true;
            }
        }
    }

    void UpdatePageNumber()
    {
        if (pageNumberText == null) return;

        pageNumberText.text =
            (currentPage + 1).ToString("D2") + " / " +
            totalPages.ToString("D2");
    }
}
