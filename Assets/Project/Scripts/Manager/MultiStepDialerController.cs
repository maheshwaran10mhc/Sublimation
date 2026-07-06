using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiStepDialerController : MonoBehaviour
{
    [Header("Manual Input Fields")]
    public TMP_InputField[] manualFields;

    [Header("Auto Fill Fields")]
    public TMP_InputField[] autoFields;

    [Header("Correct Answers (Manual Fields)")]
    public float[] manualAnswers;

    [Header("Auto Fill Values")]
    public float[] autoValues;

    [Header("Success Icons")]
    public GameObject[] successIcons;

    [Header("Buttons")]
    public Button validateButton;

    [Header("Events")]
    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnWrongAnswer;
    public UnityEvent OnAllAnswersVerified;

    const float TOL = 0.001f;

    int activeIndex = 0;
    bool allSolved = false;

    SlideController slideController;

    void Start()
    {
        slideController = FindObjectOfType<SlideController>();

        for (int i = 0; i < manualFields.Length; i++)
        {
            manualFields[i].text = "";
            manualFields[i].interactable = (i == 0);

            if (autoFields[i] != null)
            {
                autoFields[i].text = "";
                autoFields[i].interactable = false;
            }

            if (successIcons[i] != null)
                successIcons[i].SetActive(false);
        }

        validateButton.onClick.AddListener(ValidateInput);
    }

    // ================= VALIDATION =================

    void ValidateInput()
    {
        if (allSolved) return;

        if (!float.TryParse(manualFields[activeIndex].text, out float val))
            return;

        if (Mathf.Abs(val - manualAnswers[activeIndex]) > TOL)
        {
            manualFields[activeIndex].text = "";
            OnWrongAnswer?.Invoke();
            return;
        }

        // ✅ Correct
        manualFields[activeIndex].interactable = false;

        if (successIcons[activeIndex] != null)
            successIcons[activeIndex].SetActive(true);

        OnCorrectAnswer?.Invoke();

        // 🔥 Auto fill paired field
        autoFields[activeIndex].text = autoValues[activeIndex].ToString();

        activeIndex++;

        if (activeIndex < manualFields.Length)
        {
            manualFields[activeIndex].interactable = true;
        }
        else
        {
            FinishPuzzle();
        }
    }

    // ================= FINISH =================

    void FinishPuzzle()
    {
        if (allSolved) return;

        allSolved = true;

        validateButton.interactable = false;

        if (slideController != null)
            slideController.MarkPageCompleted();

        OnAllAnswersVerified?.Invoke();
    }

    // ================= RESET =================

    public void ResetAll()
    {
        activeIndex = 0;
        allSolved = false;

        validateButton.interactable = true;

        for (int i = 0; i < manualFields.Length; i++)
        {
            manualFields[i].text = "";
            manualFields[i].interactable = (i == 0);

            autoFields[i].text = "";

            if (successIcons[i] != null)
                successIcons[i].SetActive(false);
        }
    }
}
