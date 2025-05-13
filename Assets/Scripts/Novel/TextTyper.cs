using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTyper : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float startDelay = 1.0f; // Задержка перед началом печати
    [SerializeField] private float fadeDuration = 1.0f; // Продолжительность затухания

    private string[] textBlocks;
    public GameObject[] backgroundImages;
    public GameObject initialImage;
    [SerializeField] private TMP_Text displayText;

    // Добавляем ссылки на панели
    public GameObject textPanel; // Панель с текстом (TextMeshPro)
    public GameObject phonePanel; // Панель телефона

    private int currentBlockIndex = 0;
    private int blocksDisplayed = 0;
    private bool isTyping = false;
    private bool allTextFinished = false;
    private int backgroundChangeCounter = 0;
    private int currentBackgroundIndex = 0;
    private bool isNewBackground = true;
    private CanvasGroup currentCanvasGroup;
    private CanvasGroup nextCanvasGroup;
    private CanvasGroup initialImageCanvasGroup;

    void Awake()
    {
        if (displayText == null)
        {
            Debug.LogError("Текстовое поле не найдено на этом GameObject!");
            enabled = false;
            return;
        }

        // Проверяем наличие панелей и выдаем ошибку, если они не назначены
        if (textPanel == null)
        {
            Debug.LogError("Панель с текстом (textPanel) не назначена! Перетащите объект TextPanel в поле Text Panel в Inspector.");
            enabled = false;
            return;
        }

        if (phonePanel == null)
        {
            Debug.LogError("Панель телефона (phonePanel) не назначена! Перетащите объект PhonePanel в поле Phone Panel в Inspector.");
            enabled = false;
            return;
        }

        textBlocks = new string[]
        {
            "Мда, последний год пролетел слишком быстро. " +
            "Я не успел опомниться, как сменил статус со школьника на студента. " +
            "Результаты экзаменов меня не сильно порадовали, но баллов хватило для поступления в желаемый институт.",

            "Удивительно, как быстро может все измениться. Беззаботная жизнь закончилась. Теперь нужно будет самостоятельно просыпаться по утрам на учебу, готовить еду, " +
            "заниматься стиркой, планировать свой бюджет. Кроме того, придется сменить уютную квартиру на комнату в общежитии. " +
            "Все это ради того, чтобы спустя 4 года (в лучшем случае) получить диплом, говорящий о том, что я — программист.  " +
            "Кажется, за ними будущее, надеюсь, я не прогадал с выбором специальности. Как в 17 лет можно выбрать дело всей своей жизни…",

            "[Комендант: ] -900-ая.\n[Я: ] -Что?\n[Комендант: ] -Будешь жить в 900-ой комнате.\n Держи,- и вот в моей руке оказывается небольшой английский ключ. " +
            "Он приятно холодит кожу, возвращая меня в реальность.",

            "Комендант напротив меня закончила что-то рассказывать, кажется, это были правила проживания в общежитии. " +
            "Скорее всего, там было что-то по типу: не курить, не распивать алкогольные напитки, не шуметь после 11, поддерживать чистоту. " +
            "После этого она протянула мне листок и ручку. Быстро пробежавшись по документу глазами, я расписался в правом нижнем углу, получил несколько указаний " +
            "по временной регистрации и, попрощавшись, вышел из кабинета.",

            "Мне не терпелось увидеть комнату, в которой я проведу следующие 4 года своей жизни, а также познакомиться с соседом или даже двумя. Надеюсь, они будут адекватными. ",

            "Поднявшись на лифте на 9 этаж, я пошел по коридору, в обе стороны которого были двери блоков, но ни на одном не было комнаты с номером 900. " +
            "Я уже хотел было развернуться и пойти в другое крыло, как мой взгляд зацепился за одинокую дверь в конце с нужным мне номером.",

            "Вот он- мой “дом”. Вставив ключ и провернув дважды…",

            "… я вошел в небольшую комнату, окрашенную в серые тона.",

            " По наличию вещей и только двух кроватей стало понятно, сосед мой учится уже не первый год. На столе лежала записка: напиши мне-> qwerty.123."
        };
    }

    void Start()
    {
        if (initialImage == null)
        {
            Debug.LogError("Не назначен initialImage! Перетащите объект Image первого кадра в поле Initial Image в Inspector.");
        }

        if (backgroundImages == null || backgroundImages.Length == 0)
        {
            Debug.LogError("Не назначены backgroundImages! Перетащите объекты Image заднего фона в поле Background Images в Inspector.");
        }
        else
        {
            // Добавляем Canvas Group к каждому фону и устанавливаем альфу на 0
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                CanvasGroup cg = backgroundImages[i].AddComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;

                // Если это первый фон, сохраняем ссылку на CanvasGroup
                if (i == 0)
                {
                    currentCanvasGroup = cg;
                }

                // Выключаем все фоны
                backgroundImages[i].SetActive(false);
            }

            // Активируем первый фон
            backgroundImages[0].SetActive(true);
        }

        // Добавляем Canvas Group к initialImage и устанавливаем альфу на 0
        initialImageCanvasGroup = initialImage.GetComponent<CanvasGroup>();
        if (initialImageCanvasGroup == null)
        {
            initialImageCanvasGroup = initialImage.AddComponent<CanvasGroup>();
        }
        initialImageCanvasGroup.alpha = 0;
        initialImage.SetActive(true);

        // Запускаем корутину для плавного появления initialImage и первого фона
        StartCoroutine(FadeInFirstElements());
        StartCoroutine(StartTypeText(textBlocks[currentBlockIndex]));

        // Убедитесь, что панель телефона изначально выключена
        phonePanel.SetActive(false);
    }

    void Update()
    {
        if (allTextFinished && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Все блоки текста были завершены.");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                displayText.text = textBlocks[currentBlockIndex];
                isTyping = false;
            }
            else
            {
                ShowNextTextBlock();
            }
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        displayText.text = "";
        foreach (char letter in text)
        {
            displayText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    IEnumerator StartTypeText(string text)
    {
        if (isNewBackground)
        {
            yield return new WaitForSecondsRealtime(startDelay);
            isNewBackground = false;
        }
        StartCoroutine(TypeText(text));
    }

    void ShowNextTextBlock()
    {
        blocksDisplayed++;
        currentBlockIndex++;

        if (blocksDisplayed == 4 && backgroundChangeCounter == 0)
        {
            ChangeBackgroundAndHideImage();
            backgroundChangeCounter++;
        }
        else if (blocksDisplayed == 5 && backgroundChangeCounter == 1)
        {
            ChangeBackground();
            backgroundChangeCounter++;
        }
        else if (blocksDisplayed == 6 && backgroundChangeCounter == 2)
        {
            ChangeBackground();
            backgroundChangeCounter++;
        }
        else if (blocksDisplayed == 7 && backgroundChangeCounter == 3)
        {
            ChangeBackground();
            backgroundChangeCounter++;
        }
        else if (blocksDisplayed == 8 && backgroundChangeCounter == 4)
        {
            ChangeBackground();
            backgroundChangeCounter++;
        }

        if (currentBlockIndex < textBlocks.Length)
        {
            displayText.text = "";
            StartCoroutine(StartTypeText(textBlocks[currentBlockIndex]));
        }
        else
        {
            Debug.Log("Все текстовые блоки завершены!");
            displayText.text = "";
            allTextFinished = true;

            // После завершения всех текстовых блоков, скрываем панель текста и показываем панель телефона
            HideTextPanelAndShowPhonePanel();
        }
    }

    void ChangeBackgroundAndHideImage()
    {
        StartCoroutine(FadeOutInitialImage(initialImageCanvasGroup, fadeDuration));
        ChangeBackground();
    }

    void ChangeBackground()
    {
        currentBackgroundIndex++;
        if (currentBackgroundIndex < backgroundImages.Length)
        {
            SetBackground(currentBackgroundIndex);
            isNewBackground = true;
        }
    }

    void SetBackground(int index)
    {
        if (index >= 0 && index < backgroundImages.Length)
        {
            // Получаем CanvasGroup для следующего фона
            nextCanvasGroup = backgroundImages[index].GetComponent<CanvasGroup>();

            // Запускаем корутину для плавного перехода
            StartCoroutine(FadeBackground(currentCanvasGroup, nextCanvasGroup, fadeDuration));

            // Обновляем текущий индекс фона
            currentBackgroundIndex = index;
        }
        else
        {
            Debug.LogError("Неверный индекс фона: " + index);
        }
    }

    IEnumerator FadeBackground(CanvasGroup current, CanvasGroup next, float duration)
    {
        // Устанавливаем следующий фон активным и с альфой 0
        if (next != null)
        {
            backgroundImages[currentBackgroundIndex].SetActive(true);
            next.alpha = 0;
        }

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / duration);

            if (current != null)
            {
                current.alpha = 1 - alpha;
            }

            if (next != null)
            {
                next.alpha = alpha;
            }

            yield return null;
        }

        // Отключаем предыдущий фон после завершения перехода
        if (current != null)
        {
            backgroundImages[currentBackgroundIndex == 0 ? backgroundImages.Length - 1 : currentBackgroundIndex - 1].SetActive(false);
        }

        // Обновляем текущий CanvasGroup
        currentCanvasGroup = next;
    }

    IEnumerator FadeInFirstElements()
    {
        // Ждем один кадр
        yield return null;

        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);

            if (initialImageCanvasGroup != null)
            {
                initialImageCanvasGroup.alpha = alpha;
            }

            if (currentCanvasGroup != null)
            {
                currentCanvasGroup.alpha = alpha;
            }

            yield return null;
        }
    }
    IEnumerator FadeOutInitialImage(CanvasGroup cg, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = 1 - Mathf.Clamp01(time / duration);
            yield return null;
        }
        initialImage.SetActive(false);
    }

    // Метод для скрытия панели текста и отображения панели телефона
    void HideTextPanelAndShowPhonePanel()
    {
        phonePanel.SetActive(true);
        textPanel.SetActive(false);
    }

}