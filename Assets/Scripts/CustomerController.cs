using UnityEngine;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 2f;          // Скорость ходьбы
    public Transform startPoint;          // Точка появления
    public Transform targetPoint;         // Точка у стола
    public GameObject[] drinkPrefabs;     // Префабы напитков (для заказа)

    private Animator animator;
    private bool isAtBar = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        transform.position = startPoint.position; // Начинаем вне экрана
    }

    // Вызывается при клике на стол
    public void WalkToBar()
    {
        if (!isAtBar)
        {
            StartCoroutine(MoveToBar());
        }
    }

    IEnumerator MoveToBar()
    {
        // Идём к стойке
        animator.Play("WalkIn");

        while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Останавливаемся
        isAtBar = true;
        animator.Play("Idle");

        // Создаём заказ (появляется напиток над головой)
        if (drinkPrefabs.Length > 0)
        {
            int randomDrink = Random.Range(0, drinkPrefabs.Length);
            Instantiate(drinkPrefabs[randomDrink], transform.position + Vector3.up * 2, Quaternion.identity);
        }

        // Ждём 5 секунд и уходим
        yield return new WaitForSeconds(5f);

        animator.Play("WalkOut");
        while (Vector3.Distance(transform.position, startPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isAtBar = false;
    }
}