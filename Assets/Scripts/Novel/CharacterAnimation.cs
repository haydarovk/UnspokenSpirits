using UnityEngine;
using UnityEngine.UI; // Обязательно добавьте это пространство имен

public class ImageSpriteChanger : MonoBehaviour
{
    public Sprite[] sprites; // Массив спрайтов для смены
    public float frameRate = 0.25f; // Скорость смены кадров (интервал в секундах)

    private Image image;
    private int currentSpriteIndex = 0;
    private float timer = 0f;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Компонент Image не найден!");
            enabled = false; // Отключаем скрипт, если нет компонента Image
            return;
        }
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("Не назначен массив спрайтов!");
            enabled = false; // Отключаем скрипт, если нет спрайтов
            return;
        }
        image.sprite = sprites[0]; // Устанавливаем первый спрайт
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            ChangeSprite();
        }
    }

    void ChangeSprite()
    {
        currentSpriteIndex++;
        if (currentSpriteIndex >= sprites.Length)
        {
            currentSpriteIndex = 0; // Возвращаемся к первому спрайту
        }
        image.sprite = sprites[currentSpriteIndex];
    }

    // Пример использования: Вызовите ChangeSprite() из другой части вашего кода,
    // например, по нажатию кнопки.
}
