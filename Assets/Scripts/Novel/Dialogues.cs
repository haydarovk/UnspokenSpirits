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
    public void Construct(DialoguesInstaller dialoguesInstaller)
    {
        _inkJson = dialoguesInstaller.inkJson;
        _dialoguePanel = dialoguesInstaller.dialoguePanel;
        _dialogueText = dialoguesInstaller.dialogueText;
        _nameText = dialoguesInstaller.nameText;
        _choiceButtonPanel = dialoguesInstaller.choiceButtonPanel;
        _choiceButton = dialoguesInstaller.choiceButton;
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
        ChangeBackground();
    }

    private void ShowChoiceButton()
    {
        List<Choice> currentChoices = _currentStory.currentChoices;
        _choiceButtonPanel.SetActive(currentChoices.Count != 0);
        if(currentChoices.Count <= 0 )
        {
            return;
        }
        _choiceButtonPanel.transform.Cast<Transform>().ToList().ForEach(child => Destroy(child.gameObject));
        _choicesText.Clear();
        for (int i = 0; i < currentChoices.Count; i++)
        {
            GameObject choice = Instantiate(_choiceButton);
            choice.GetComponent<ButtonAction>().index = i;
            choice.transform.SetParent(_choiceButtonPanel.transform);

            TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
            choiceText.text = currentChoices[i].text;
            _choicesText.Add(choiceText);
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

    private void ChangeBackground()
    {
        if (backgroundImages == null || backgroundImages.Length == 0) return;

        // Выключаем все фоны
        foreach (var bg in backgroundImages)
        {
            bg.SetActive(false);
        }

        // Включаем текущий фон
        backgroundImages[currentBackgroundIndex].SetActive(true);

        // Переходим к следующему фону (или зацикливаем)
        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundImages.Length;
    }
}
