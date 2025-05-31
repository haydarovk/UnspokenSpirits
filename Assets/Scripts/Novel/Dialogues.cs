using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using Zenject;
using System.Linq;

public class Dialogues : MonoBehaviour
{
    private Story _currentStory;
    private TextAsset _inkJson;

    private GameObject _dialoguePanel;
    private TextMeshProUGUI _dialogueText;
    private TextMeshProUGUI _nameText;

    [HideInInspector] public GameObject _choiceButtonPanel;
    [SerializeField] private GameObject _choiceButton;
    private List<TextMeshProUGUI> _choicesText = new();

    public bool DialogPlay {  get; private set; }

    [Inject]
    public void construct(DialoguesInstaller dialoguesinstaller)
    {
        _inkJson = dialoguesinstaller.inkJson;
        _dialoguePanel = dialoguesinstaller.dialoguePanel;
        _dialogueText = dialoguesinstaller.dialogueText;
        _nameText = dialoguesinstaller.nameText;
        _choiceButtonPanel = dialoguesinstaller.choiceButtonPanel;
        _choiceButton = dialoguesinstaller.choiceButton;
        ;
    }

    public GameObject[] backgroundImages;
    private int currentBackgroundIndex = 0;

    private void Awake()
    {
        _currentStory = new Story(_inkJson.text);
    }

    void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        DialogPlay = true;
        _dialoguePanel.SetActive(true);
        ContinueStory();
    }

    public void ContinueStory(bool choiceBefore = false)
    {
        if (_currentStory.canContinue)
        {
            ShowDialogue();
            ShowChoiceButton();
        }
        else if(!choiceBefore)
        {
            ExitDialogue();
        }
    }

    private void ShowDialogue()
    {
        _dialogueText.text = _currentStory.Continue();
        _nameText.text = (string)_currentStory.variablesState["characterName"];
        //ChangeBackground();
    }

    private void ShowChoiceButton()
    {
        if (_choiceButton == null || _choiceButtonPanel == null)
        {
            Debug.LogError("Choice button or panel not assigned!");
            return;
        }

        List<Choice> currentChoices = _currentStory.currentChoices;
        if (_choiceButtonPanel != null)
        {
            _choiceButtonPanel.SetActive(currentChoices.Count != 0);
        }

        if (currentChoices.Count <= 0) return;

        // Очистка предыдущих кнопок с проверкой
        if (_choiceButtonPanel != null)
        {
            foreach (Transform child in _choiceButtonPanel.transform)
            {
                if (child != null) Destroy(child.gameObject);
            }
        }

        _choicesText.Clear();

        for (int i = 0; i < currentChoices.Count; i++)
        {
            if (_choiceButton == null) continue;

            GameObject choice = Instantiate(_choiceButton);
            if (choice == null) continue;

            var buttonAction = choice.GetComponent<ButtonAction>();
            if (buttonAction != null)
            {
                buttonAction.index = i;
            }

            if (_choiceButtonPanel != null)
            {
                choice.transform.SetParent(_choiceButtonPanel.transform, false);
            }

            TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
            if (choiceText != null)
            {
                choiceText.text = currentChoices[i].text;
                _choicesText.Add(choiceText);
            }
        }
    }

    public void ChoiceButtonAction(int choiceIndex)
    {
        _currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory(true);
    }

    private void ExitDialogue()
    {
        Debug.Log("End Story");
        DialogPlay = false;
        _dialoguePanel.SetActive(false);
    }

    //private void ChangeBackground()
    //{
    //    if (backgroundImages == null || backgroundImages.Length == 0) return;

    //    // Выключаем все фоны
    //    foreach (var bg in backgroundImages)
    //    {
    //        bg.SetActive(false);
    //    }

    //    // Включаем текущий фон
    //    backgroundImages[currentBackgroundIndex].SetActive(true);

    //    // Переходим к следующему фону (или зацикливаем)
    //    currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundImages.Length;
    //}
}
