using System.Collections;
using TMPro;
using UnityEngine;

public class SequenceController : MonoBehaviour
{
    public GameObject dialoguePanel; // Панель диалогов (из Dialogues)
    public GameObject choiceButtonPanel; // Панель кнопок (из Dialogues)
    public GameObject firstGameObject;
    public GameObject secondGameObject;
    public GameObject thirdGameObject;
    public GameObject phonePanel;

    private bool waitingForInput = false;

    void Start()
    {
        // Запускаем последовательность
        StartCoroutine(ControlSequence());
    }

    IEnumerator ControlSequence()
    {
        // Проверяем на null ссылки на объекты
        if (firstGameObject == null || secondGameObject == null || thirdGameObject == null || phonePanel == null)
        {
            Debug.LogError("Убедитесь, что все объекты назначены в инспекторе!");
            yield break; // Прекращаем выполнение корутины
        }

        // Отключаем все объекты в начале
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);
        thirdGameObject.SetActive(false);

        // Включаем первый объект через заданное время
        yield return new WaitForSeconds(1.5f);
        firstGameObject.SetActive(true);

        // Включаем второй объект через заданное время после включения первого
        yield return new WaitForSeconds(3.5f);
        secondGameObject.SetActive(true);

        // Ждем немного, затем выключаем первый и второй объекты
        yield return new WaitForSeconds(7.0f);
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);

        // Включаем третий объект
        thirdGameObject.SetActive(true);

        // Начинаем ожидать нажатия левой кнопки мыши
        //waitingForInput = true;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); // Ждем, пока не будет нажата левая кнопка мыши

        // Когда кнопка нажата, закрываем панель
        phonePanel.SetActive(false);

        // Активируем панель диалогов и кнопок
        if (dialoguePanel != null && choiceButtonPanel != null)
        {
            dialoguePanel.SetActive(true);
            choiceButtonPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Не назначены dialoguePanel или choiceButtonPanel!");
        }

        waitingForInput = false;
        Debug.Log("Последовательность завершена.");
    }

    void Update()
    {
        // Проверяем нажатие кнопки только если мы находимся в состоянии ожидания ввода
        if (waitingForInput && Input.GetMouseButtonDown(0))
        {
            // Код для закрытия панели уже находится в корутине ControlSequence, поэтому здесь ничего не нужно делать.
            // Корутина WaitUntil сама обнаружит нажатие кнопки и продолжит выполнение.
        }
    }
}
