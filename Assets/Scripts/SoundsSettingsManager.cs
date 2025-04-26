using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsManager : MonoBehaviour
{
    public GameObject settingsPanel; // Ссылка на панель настроек
    public Slider volumeSlider; // Ссылка на слайдер громкости

    void Start()
    {
        // Сначала скрываем панель
        settingsPanel.SetActive(false);

        // Устанавливаем начальное значение громкости
        volumeSlider.value = AudioListener.volume;

        // Добавляем обработчик изменения значения слайдера
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Метод для открытия/закрытия панели
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // Метод для установки громкости
    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}