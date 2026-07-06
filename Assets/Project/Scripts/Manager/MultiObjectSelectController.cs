using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiObjectSelectController : MonoBehaviour
{
    [Header("Buttons (10 total)")]
    public Button[] objectButtons;

    [Header("Is Each Object Correct?")]
    public bool[] isCorrect; // same size as objectButtons, true/false per object

    [Header("Tick Icons (shown above object if correct)")]
    public GameObject[] tickIcons;

    [Header("Wrong Icons (shown above object if incorrect)")]
    public GameObject[] wrongIcons;

    [Header("Required Correct Count")]
    public int requiredCorrect = 8;

    [Header("Events")]
    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnWrongAnswer;
    public UnityEvent OnPuzzleCompleted;

    int correctCount = 0;
    bool completed = false;

    SlideController slideController;

    void Start()
    {
        slideController = FindObjectOfType<SlideController>();

        for (int i = 0; i < objectButtons.Length; i++)
        {
            if (tickIcons[i] != null) tickIcons[i].SetActive(false);
            if (wrongIcons[i] != null) wrongIcons[i].SetActive(false);

            int index = i; // capture for closure
            objectButtons[i].onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    void OnButtonClicked(int index)
    {
        if (completed) return;

        // Prevent re-clicking the same button after it's answered
        objectButtons[index].interactable = false;

        if (isCorrect[index])
        {
            if (tickIcons[index] != null) tickIcons[index].SetActive(true);
            correctCount++;
            OnCorrectAnswer?.Invoke();
        }
        else
        {
            if (wrongIcons[index] != null) wrongIcons[index].SetActive(true);
            OnWrongAnswer?.Invoke();
        }

        CheckCompletion();
    }

    void CheckCompletion()
    {
        if (completed) return;

        if (correctCount >= requiredCorrect)
        {
            completed = true;

            // Lock all remaining buttons once puzzle is completed
            for (int i = 0; i < objectButtons.Length; i++)
            {
                if (objectButtons[i] != null)
                    objectButtons[i].interactable = false;
            }

            if (slideController != null)
                slideController.MarkPageCompleted();

            OnPuzzleCompleted?.Invoke();
        }
    }

    // ================= RESET =================
    public void ResetAll()
    {
        completed = false;
        correctCount = 0;

        for (int i = 0; i < objectButtons.Length; i++)
        {
            objectButtons[i].interactable = true;
            if (tickIcons[i] != null) tickIcons[i].SetActive(false);
            if (wrongIcons[i] != null) wrongIcons[i].SetActive(false);
        }
    }
}