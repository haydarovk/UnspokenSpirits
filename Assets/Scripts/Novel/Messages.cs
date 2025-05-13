using System.Collections;
using TMPro;
using UnityEngine;

public class SequenceController : MonoBehaviour
{
    public GameObject dialoguePanel; // ������ �������� (�� Dialogues)
    public GameObject choiceButtonPanel; // ������ ������ (�� Dialogues)
    public GameObject firstGameObject;
    public GameObject secondGameObject;
    public GameObject thirdGameObject;
    public GameObject phonePanel;

    private bool waitingForInput = false;

    void Start()
    {
        // ��������� ������������������
        StartCoroutine(ControlSequence());
    }

    IEnumerator ControlSequence()
    {
        // ��������� �� null ������ �� �������
        if (firstGameObject == null || secondGameObject == null || thirdGameObject == null || phonePanel == null)
        {
            Debug.LogError("���������, ��� ��� ������� ��������� � ����������!");
            yield break; // ���������� ���������� ��������
        }

        // ��������� ��� ������� � ������
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);
        thirdGameObject.SetActive(false);

        // �������� ������ ������ ����� �������� �����
        yield return new WaitForSeconds(1.5f);
        firstGameObject.SetActive(true);

        // �������� ������ ������ ����� �������� ����� ����� ��������� �������
        yield return new WaitForSeconds(3.5f);
        secondGameObject.SetActive(true);

        // ���� �������, ����� ��������� ������ � ������ �������
        yield return new WaitForSeconds(7.0f);
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);

        // �������� ������ ������
        thirdGameObject.SetActive(true);

        // �������� ������� ������� ����� ������ ����
        //waitingForInput = true;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); // ����, ���� �� ����� ������ ����� ������ ����

        // ����� ������ ������, ��������� ������
        phonePanel.SetActive(false);

        // ���������� ������ �������� � ������
        if (dialoguePanel != null && choiceButtonPanel != null)
        {
            dialoguePanel.SetActive(true);
            choiceButtonPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("�� ��������� dialoguePanel ��� choiceButtonPanel!");
        }

        waitingForInput = false;
        Debug.Log("������������������ ���������.");
    }

    void Update()
    {
        // ��������� ������� ������ ������ ���� �� ��������� � ��������� �������� �����
        if (waitingForInput && Input.GetMouseButtonDown(0))
        {
            // ��� ��� �������� ������ ��� ��������� � �������� ControlSequence, ������� ����� ������ �� ����� ������.
            // �������� WaitUntil ���� ��������� ������� ������ � ��������� ����������.
        }
    }
}
