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
    public GameObject panelToClose;

    private bool waitingForInput = false;

    public GameObject textBlockToShow; // ��������� ������ �� ��������� ����


    void Start()
    {
        // ��������� ������������������
        StartCoroutine(ControlSequence());
    }

    IEnumerator ControlSequence()
    {
        // ��������� �� null ������ �� �������
        if (firstGameObject == null || secondGameObject == null || thirdGameObject == null || panelToClose == null)
        {
            Debug.LogError("���������, ��� ��� ������� ��������� � ����������!");
            yield break; // ���������� ���������� ��������
        }

        // ��������� ��� ������� � ������
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);
        thirdGameObject.SetActive(false);

        // �������� ������ ������ ����� �������� �����
        yield return WaitForInputOrTime(1.5f);
        firstGameObject.SetActive(true);

        // �������� ������ ������ ����� �������� ����� ����� ��������� �������
        yield return WaitForInputOrTime(3.5f);
        secondGameObject.SetActive(true);

        // ���� �������, ����� ��������� ������ � ������ �������
        yield return WaitForInputOrTime(17.0f);
        firstGameObject.SetActive(false);
        secondGameObject.SetActive(false);

        // �������� ������ ������
        thirdGameObject.SetActive(true);

        // �������� ������� ������� ����� ������ ����
        //waitingForInput = true;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); // ����, ���� �� ����� ������ ����� ������ ����

        // ����� ������ ������, ��������� ������
        panelToClose.SetActive(false);

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

    // ������������� ����� �������� ������� ������ ��� �������
    IEnumerator WaitForInputOrTime(float time)
    {
        float timer = 0f;

        while (timer < time)
        {
            if (Input.GetMouseButtonDown(0))    //|| timer==time)
            {
                yield return null; // ���� Unity ����� ���������� �������
                yield break; // ������� �� ��������, ���� ������ ������
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
    //void Update()
    //{
    //    // ��������� ������� ������ ������ ���� �� ��������� � ��������� �������� �����
    //    if (waitingForInput && Input.GetMouseButtonDown(0))
    //    {
    //        // ��� ��� �������� ������ ��� ��������� � �������� ControlSequence, ������� ����� ������ �� ����� ������.
    //        // �������� WaitUntil ���� ��������� ������� ������ � ��������� ����������.
    //    }
    //}
}
