using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;
using UnityEngine.SceneManagement;

public class NovelTextSystem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text dialogueText; // ����� � TextPanel2
    public TMP_Text nameText;     // ����� � Character (���)
    public GameObject choiceButtonPanel; // ������ ��� ������ ������
    public Button choiceButtonPrefab;    // ������ ������ ������

    [Header("Background Settings")]
    public GameObject[] backgroundImages; // Массив фоновых изображений
    private CanvasGroup[] backgroundCanvasGroups;
    private int currentBackgroundIndex = 0;
    public float backgroundFadeDuration = 1f;

    [Header("INK File")]
    public TextAsset inkJSONAsset;

    [Header("Settings")]
    public float typeSpeed = 0.05f;

    private Story currentStory;
    private bool isTyping = false;
    private bool isChoosing = false;

    void Start()
    {

        InitializeBackgrounds();

        currentStory = new Story(inkJSONAsset.text);

        choiceButtonPanel.SetActive(false);

        ContinueStory();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentStory.currentText;
                isTyping = false;
            }
            else if (!isChoosing)
            {
                ContinueStory();
            }
        }
    }

    void InitializeBackgrounds()
    {
        backgroundCanvasGroups = new CanvasGroup[backgroundImages.Length];

        for (int i = 0; i < backgroundImages.Length; i++)
        {
            // Добавляем CanvasGroup если нет
            var canvasGroup = backgroundImages[i].GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = backgroundImages[i].AddComponent<CanvasGroup>();
            }

            backgroundCanvasGroups[i] = canvasGroup;
            backgroundImages[i].SetActive(i == 0); // Активируем только первый фон
            canvasGroup.alpha = i == 0 ? 1 : 0;    // Делаем видимым только первый фон
        }
    }

    IEnumerator ChangeBackground(int newIndex)
    {
        if (newIndex < 0 || newIndex >= backgroundImages.Length)
        {
            Debug.LogError("Invalid background index!");
            yield break;
        }

        // Активируем новый фон перед анимацией
        backgroundImages[newIndex].SetActive(true);

        float timer = 0;
        CanvasGroup currentBG = backgroundCanvasGroups[currentBackgroundIndex];
        CanvasGroup nextBG = backgroundCanvasGroups[newIndex];

        while (timer < backgroundFadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / backgroundFadeDuration;

            currentBG.alpha = 1 - progress;
            nextBG.alpha = progress;

            yield return null;
        }

        // Деактивируем старый фон после анимации
        backgroundImages[currentBackgroundIndex].SetActive(false);
        currentBackgroundIndex = newIndex;
    }

    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            string text = currentStory.Continue();

            // Проверяем изменение фона через теги INK
            if (currentStory.currentTags.Count > 0)
            {
                foreach (string tag in currentStory.currentTags)
                {
                    if (tag.StartsWith("BG_"))
                    {
                        string bgIndexStr = tag.Replace("BG_", "");
                        if (int.TryParse(bgIndexStr, out int newBgIndex))
                        {
                            StartCoroutine(ChangeBackground(newBgIndex));
                        }
                    }
                }
            }

            // Остальной код ContinueStory()
            if (currentStory.variablesState["characterName"] != null)
            {
                nameText.text = (string)currentStory.variablesState["characterName"];
            }

            StartCoroutine(TypeText(text));

            if (currentStory.currentChoices.Count > 0)
            {
                StartCoroutine(ShowChoices());
            }
        }
        else
        {
            Debug.Log("End of story");
            SceneManager.LoadScene(2);
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    IEnumerator ShowChoices()
    {
        isChoosing = true;
        choiceButtonPanel.SetActive(true);

        // Очистка старых кнопок
        foreach (Transform child in choiceButtonPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Создание новых кнопок
        for (int i = 0; i < currentStory.currentChoices.Count; i++)
        {
            Choice choice = currentStory.currentChoices[i];
            Button button = Instantiate(choiceButtonPrefab, choiceButtonPanel.transform);

            // Настройка текста
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = choice.text;
            }
            else
            {
                Debug.LogError("No TMP_Text component in button prefab!");
            }

            // Настройка обработчика клика
            int choiceIndex = i;
            button.onClick.RemoveAllListeners(); // Очищаем старые обработчики
            button.onClick.AddListener(() => StartCoroutine(MakeChoiceCoroutine(choiceIndex)));
        }

        yield return null;
    }

    IEnumerator MakeChoiceCoroutine(int choiceIndex)
    {
        // Выбираем вариант
        currentStory.ChooseChoiceIndex(choiceIndex);

        // Скрываем панель выбора
        choiceButtonPanel.SetActive(false);
        isChoosing = false;

        // Продолжаем историю
        ContinueStory();
        yield return null;
    }
}