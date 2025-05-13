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

    // ��� ��������� ����� (����� 4)
    private string[] textBlocks = {
        "� ���������, ��� �� ��� ��������. �� ������ ������, ����� ���� ���������� ����, ������������ �� ����, ��-�� �������� ���� ������ ���� �������, � ��� ��� �� ���������. ������ �������, � ����� ������� ������ � �������� �����, ��� ����� ���������� � ����� � �������� ��������, ���� ��� ����������� ������. �� ��� �� �������� ���������� � ������ ������������ �����, ����� ��� �� ���������� ��, ��� � ������ ��� ����, ���� � ��� ����� ����������� ����.\n",
        "������������ �������� � ������� ����������, � ��������� � ������ � �������� ����. ������ �������, ������� �� �����, � �������� ������ ������������ �� ����� �������, ����������� � ������������� ���������, ������� � �������� � ���������� ��� ������: ����� �������� ��������� ���� ���������� ������������� ����� � ����������� ��������� ����� � �����-�� �������� �� ����.\n",
        "�� ���� ���������� �� ������������ ������ ����� ����, � ����� ������� ������. ���������� �� ������� ������, � �������� ��������� ���������� ������� �� �������� ����������. ������� � ���� � ����������� \n",
        "�������� � ������, �� ���� ��������� ����������� � ���������� � ���. ����������, ��� ����� ������� ���������� ��� ������� ����, � ������ ���� � ������ ��� ��� �������� ������� ������ ����� ��� ����� ������.\n��� ���, ����� � ������\n"
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
        // ���������� 1-� ����
        yield return TypeText(textBlocks[0]);

        yield return WaitForClick();

        // ���������� ����� ����� 1-�� �����
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
        Debug.Log($"������ �������: {choice1Button.gameObject.activeSelf}, {choice2Button.gameObject.activeSelf}");

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
            // ��� ������ 1: 2-� � 3-� �����
            currentBlockIndex=1;
            yield return TypeText(textBlocks[currentBlockIndex]);
            yield return WaitForClick();

            currentBlockIndex=2;
            yield return TypeText(textBlocks[currentBlockIndex]);
            yield return WaitForClick();
        }

        // ��� ����� �������: 4-� ����
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
                // ���������� ����� ����� 1-�� �����
                ShowChoicePanel();
            }
        }
    }
}