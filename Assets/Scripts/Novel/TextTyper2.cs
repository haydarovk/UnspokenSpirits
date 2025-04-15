using System.Collections;
using UnityEngine;
using TMPro;

public class TextTyperTMP : MonoBehaviour
{
    public TMP_Text textComponent; // Ссылка на компонент TextMeshPro Text
    public string fullText; // Текст, который нужно вывести посимвольно
    public float typeSpeed = 0.05f; // Задержка между выводом символов

    private string currentText = ""; // Текущий отображаемый текст
    private bool isTyping = false; // Флаг, показывающий, идет ли сейчас вывод текста

    void Start()
    {
        // Проверяем, что textComponent назначен
        if (textComponent == null)
        {
            Debug.LogError("TextMeshPro Text Component не назначен! Пожалуйста, перетащите компонент TextMeshPro Text в поле Text Component в Inspector.");
            enabled = false; // Отключаем скрипт, чтобы не было ошибок
            return;
        }

        // Запускаем корутину для вывода текста посимвольно
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        currentText = ""; // Начинаем с пустого текста
        textComponent.text = currentText; // Обновляем TextMeshPro Text

        // Проходимся по каждому символу в fullText
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText += fullText[i]; // Добавляем текущий символ к отображаемому тексту
            textComponent.text = currentText; // Обновляем TextMeshPro Text
            yield return new WaitForSeconds(typeSpeed); // Ждем указанное время
        }

        isTyping = false; // Устанавливаем флаг, что вывод текста завершен
    }

    // (Необязательно) Функция для мгновенного завершения вывода текста при нажатии кнопки
    public void SkipTyping()
    {
        if (isTyping)
        {
            StopCoroutine(TypeText()); // Останавливаем корутину TypeText
            textComponent.text = fullText; // Отображаем весь текст сразу
            isTyping = false;
        }
    }
}
