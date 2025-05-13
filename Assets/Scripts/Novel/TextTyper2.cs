using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NovelTextSystem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text textComponent;
    public Button choice1Button;
    public Button choice2Button;

    [Header("Settings")]
    public float typeSpeed = 0.05f;

    // Все текстовые блоки (всего 4)
    private string[] textBlocks = {
        "Я задумался, как на это ответить. На первый взгляд, здесь явно замечалась пыль, накопившаяся за лето, из-за закрытых окон воздух стал затхлым, и мне это не нравилось. Наведя порядок, я смогу создать уютное и приятное место, где смогу готовиться к учебе и спокойно отдыхать, пока жду возвращения соседа. Но мне не хотелось вторгаться в личное пространство Влада, вдруг ему не понравится то, что я трогал его вещи, хоть и для такой благородной цели.\n",
        "Вооружившись тряпками и моющими средствами, я приступил к уборке в кухонной зоне. Открыв шкафчик, висящий на стене, я удивился обилию всевозможных по вкусу сиропов, контейнеров с разноцветными пюрешками, травами и специями и незнакомой мне посуды: самым странным предметом была сплюснутая металлическая ложка с отверстиями необычной формы и какой-то пружиной по краю.\n",
        "На этом странности не закончились… Набрав ведро воды, я начал двигать мебель. Добравшись до кровати соседа, я удивился огормному количеству коробок со звенящим содержимым. Кажется я живу с алкоголиком… \n",
        "Уставший и потный, но явно довольный результатом я направился в душ. Представив, как будет приятно понежиться под струями воды, я открыл кран и… Супер… Вот они прелести верхних этажей… Напор был очень слабый.\nВот она, жизнь в общаге…\n"
    };

    private int currentBlockIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        choice1Button.gameObject.SetActive(false);
        choice2Button.gameObject.SetActive(false);
        StartCoroutine(StartDialogue());
    }

    IEnumerator StartDialogue()
    {
        // Показываем 1-й блок
        yield return TypeText(textBlocks[0]);

        yield return WaitForClick();

        // Показываем выбор после 1-го блока
        ShowChoicePanel();
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        textComponent.text = "";

        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    IEnumerator WaitForClick()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;
    }

    void ShowChoicePanel()
    {
        textComponent.text = "";

        choice1Button.gameObject.SetActive(true);
        choice2Button.gameObject.SetActive(true);
        Debug.Log($"Кнопки активны: {choice1Button.gameObject.activeSelf}, {choice2Button.gameObject.activeSelf}");

        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();

        choice1Button.onClick.AddListener(() => StartCoroutine(ProcessChoice(0)));
        choice2Button.onClick.AddListener(() => StartCoroutine(ProcessChoice(1)));
    }

    IEnumerator ProcessChoice(int choice)
    {
        choice1Button.gameObject.SetActive(false);
        choice2Button.gameObject.SetActive(false);

        if (choice == 0)
        {
            // Для выбора 1: 2-й и 3-й блоки
            currentBlockIndex=1;
            yield return TypeText(textBlocks[currentBlockIndex]);
            yield return WaitForClick();

            currentBlockIndex=2;
            yield return TypeText(textBlocks[currentBlockIndex]);
            yield return WaitForClick();
        }

        // Для обоих выборов: 4-й блок
        currentBlockIndex = 3;
        yield return TypeText(textBlocks[currentBlockIndex]);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                textComponent.text = textBlocks[currentBlockIndex];
                isTyping = false;
            }
            else if(!isTyping&&currentBlockIndex==0)
            {
                // Показываем выбор после 1-го блока
                ShowChoicePanel();
            }
        }
    }
}