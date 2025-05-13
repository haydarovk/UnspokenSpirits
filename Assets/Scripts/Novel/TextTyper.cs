using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTyper : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float startDelay = 1.0f; // �������� ����� ������� ������
    [SerializeField] private float fadeDuration = 1.0f; // ����������������� ���������

    private string[] textBlocks;
    public GameObject[] backgroundImages;
    public GameObject initialImage;
    [SerializeField] private TMP_Text displayText;

    // ��������� ������ �� ������
    public GameObject textPanel; // ������ � ������� (TextMeshPro)
    public GameObject phonePanel; // ������ ��������

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
            Debug.LogError("��������� ���� �� ������� �� ���� GameObject!");
            enabled = false;
            return;
        }

        // ��������� ������� ������� � ������ ������, ���� ��� �� ���������
        if (textPanel == null)
        {
            Debug.LogError("������ � ������� (textPanel) �� ���������! ���������� ������ TextPanel � ���� Text Panel � Inspector.");
            enabled = false;
            return;
        }

        if (phonePanel == null)
        {
            Debug.LogError("������ �������� (phonePanel) �� ���������! ���������� ������ PhonePanel � ���� Phone Panel � Inspector.");
            enabled = false;
            return;
        }

        textBlocks = new string[]
        {
            "���, ��������� ��� �������� ������� ������. " +
            "� �� ����� ����������, ��� ������ ������ �� ��������� �� ��������. " +
            "���������� ��������� ���� �� ������ ����������, �� ������ ������� ��� ����������� � �������� ��������.",

            "�����������, ��� ������ ����� ��� ����������. ����������� ����� �����������. ������ ����� ����� �������������� ����������� �� ����� �� �����, �������� ���, " +
            "���������� �������, ����������� ���� ������. ����� ����, �������� ������� ������ �������� �� ������� � ���������. " +
            "��� ��� ���� ����, ����� ������ 4 ���� (� ������ ������) �������� ������, ��������� � ���, ��� � � �����������.  " +
            "�������, �� ���� �������, �������, � �� �������� � ������� �������������. ��� � 17 ��� ����� ������� ���� ���� ����� �����",

            "[���������: ] -900-��.\n[�: ] -���?\n[���������: ] -������ ���� � 900-�� �������.\n �����,- � ��� � ���� ���� ����������� ��������� ���������� ����. " +
            "�� ������� ������� ����, ��������� ���� � ����������.",

            "��������� �������� ���� ��������� ���-�� ������������, �������, ��� ���� ������� ���������� � ���������. " +
            "������ �����, ��� ���� ���-�� �� ����: �� ������, �� ��������� ����������� �������, �� ������ ����� 11, ������������ �������. " +
            "����� ����� ��� ��������� ��� ������ � �����. ������ ������������ �� ��������� �������, � ���������� � ������ ������ ����, ������� ��������� �������� " +
            "�� ��������� ����������� �, ������������, ����� �� ��������.",

            "��� �� ��������� ������� �������, � ������� � ������� ��������� 4 ���� ����� �����, � ����� ������������� � ������� ��� ���� �����. �������, ��� ����� �����������. ",

            "���������� �� ����� �� 9 ����, � ����� �� ��������, � ��� ������� �������� ���� ����� ������, �� �� �� ����� �� ���� ������� � ������� 900. " +
            "� ��� ����� ���� ������������ � ����� � ������ �����, ��� ��� ������ ��������� �� �������� ����� � ����� � ������ ��� �������.",

            "��� ��- ��� ����. ������� ���� � ��������� �������",

            "� � ����� � ��������� �������, ���������� � ����� ����.",

            " �� ������� ����� � ������ ���� �������� ����� �������, ����� ��� ������ ��� �� ������ ���. �� ����� ������ �������: ������ ���-> qwerty.123."
        };
    }

    void Start()
    {
        if (initialImage == null)
        {
            Debug.LogError("�� �������� initialImage! ���������� ������ Image ������� ����� � ���� Initial Image � Inspector.");
        }

        if (backgroundImages == null || backgroundImages.Length == 0)
        {
            Debug.LogError("�� ��������� backgroundImages! ���������� ������� Image ������� ���� � ���� Background Images � Inspector.");
        }
        else
        {
            // ��������� Canvas Group � ������� ���� � ������������� ����� �� 0
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                CanvasGroup cg = backgroundImages[i].AddComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;

                // ���� ��� ������ ���, ��������� ������ �� CanvasGroup
                if (i == 0)
                {
                    currentCanvasGroup = cg;
                }

                // ��������� ��� ����
                backgroundImages[i].SetActive(false);
            }

            // ���������� ������ ���
            backgroundImages[0].SetActive(true);
        }

        // ��������� Canvas Group � initialImage � ������������� ����� �� 0
        initialImageCanvasGroup = initialImage.GetComponent<CanvasGroup>();
        if (initialImageCanvasGroup == null)
        {
            initialImageCanvasGroup = initialImage.AddComponent<CanvasGroup>();
        }
        initialImageCanvasGroup.alpha = 0;
        initialImage.SetActive(true);

        // ��������� �������� ��� �������� ��������� initialImage � ������� ����
        StartCoroutine(FadeInFirstElements());
        StartCoroutine(StartTypeText(textBlocks[currentBlockIndex]));

        // ���������, ��� ������ �������� ���������� ���������
        phonePanel.SetActive(false);
    }

    void Update()
    {
        if (allTextFinished && Input.GetMouseButtonDown(0))
        {
            Debug.Log("��� ����� ������ ���� ���������.");
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
            Debug.Log("��� ��������� ����� ���������!");
            displayText.text = "";
            allTextFinished = true;

            // ����� ���������� ���� ��������� ������, �������� ������ ������ � ���������� ������ ��������
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
            // �������� CanvasGroup ��� ���������� ����
            nextCanvasGroup = backgroundImages[index].GetComponent<CanvasGroup>();

            // ��������� �������� ��� �������� ��������
            StartCoroutine(FadeBackground(currentCanvasGroup, nextCanvasGroup, fadeDuration));

            // ��������� ������� ������ ����
            currentBackgroundIndex = index;
        }
        else
        {
            Debug.LogError("�������� ������ ����: " + index);
        }
    }

    IEnumerator FadeBackground(CanvasGroup current, CanvasGroup next, float duration)
    {
        // ������������� ��������� ��� �������� � � ������ 0
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

        // ��������� ���������� ��� ����� ���������� ��������
        if (current != null)
        {
            backgroundImages[currentBackgroundIndex == 0 ? backgroundImages.Length - 1 : currentBackgroundIndex - 1].SetActive(false);
        }

        // ��������� ������� CanvasGroup
        currentCanvasGroup = next;
    }

    IEnumerator FadeInFirstElements()
    {
        // ���� ���� ����
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

    // ����� ��� ������� ������ ������ � ����������� ������ ��������
    void HideTextPanelAndShowPhonePanel()
    {
        phonePanel.SetActive(true);
        textPanel.SetActive(false);
    }

}