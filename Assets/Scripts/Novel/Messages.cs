using System.Collections;
using TMPro;
using UnityEngine;

public class SequenceController : MonoBehaviour
{
    public GameObject firstGameObject;
    public GameObject secondGameObject;
    public GameObject thirdGameObject;
    public GameObject panelToClose;

    private bool waitingForInput = false;

    public GameObject textBlockToShow; // Добавляем ссылку на текстовый блок


    void Start()
    {
        // Запускаем последовательность
        StartCoroutine(ControlSequence());
    }

    IEnumerator ControlSequence()
    {
        // Проверяем на null ссылки на объекты
        if (firstGameObject == null || secondGameObject == null || thirdGameObject == null || panelToClose == null)
        {
            Debug.LogError("Убедитесь, что все объекты назначены в инспекторе!");
            yield break; // Прекращаем выполнение корутины
        }

        // Отключаем все объекты в начале
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);
        thirdGameObject.SetActive(false);

        // Включаем первый объект через заданное время
        yield return WaitForInputOrTime(1.5f);
        firstGameObject.SetActive(true);

        // Включаем второй объект через заданное время после включения первого
        yield return WaitForInputOrTime(3.5f);
        secondGameObject.SetActive(true);

        // Ждем немного, затем выключаем первый и второй объекты
        yield return WaitForInputOrTime(17.0f);
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);

        // Включаем третий объект
        thirdGameObject.SetActive(true);

        // Начинаем ожидать нажатия левой кнопки мыши
        //waitingForInput = true;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); // Ждем, пока не будет нажата левая кнопка мыши

        // Когда кнопка нажата, закрываем панель
        panelToClose.SetActive(false);
        textBlockToShow.SetActive(true);

        waitingForInput = false;
        Debug.Log("Последовательность завершена.");
    }

    // Универсальный метод ожидания нажатия кнопки или времени
    IEnumerator WaitForInputOrTime(float time)
    {
        float timer = 0f;

        while (timer < time)
        {
            if (Input.GetMouseButtonDown(0))    //|| timer==time)
            {
                yield return null; // Даем Unity время обработать событие
                yield break; // Выходим из корутины, если нажата кнопка
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
    //void Update()
    //{
    //    // Проверяем нажатие кнопки только если мы находимся в состоянии ожидания ввода
    //    if (waitingForInput && Input.GetMouseButtonDown(0))
    //    {
    //        // Код для закрытия панели уже находится в корутине ControlSequence, поэтому здесь ничего не нужно делать.
    //        // Корутина WaitUntil сама обнаружит нажатие кнопки и продолжит выполнение.
    //    }
    //}
}
