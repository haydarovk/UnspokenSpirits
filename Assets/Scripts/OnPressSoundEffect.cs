using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Button))]
public class PlaySoundOnPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public AudioClip clickSound;
    public AudioSource audioSource;
    public AudioMixerGroup mixerGroup;
    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}