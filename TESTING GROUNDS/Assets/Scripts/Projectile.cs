using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public float maxDistance = 50f;
    public LayerMask wallLayer;

    [Header("Настройки визуала")]
    // Сюда мы впишем поворот, чтобы меч смотрел острием вперед
    public Vector3 rotationOffset = new Vector3(0f, 0f, 0f);
    public float wallOffset = 0.1f; // Отступ от стены

    private float distanceTraveled = 0f;
    private bool isStuck = false;
    private Vector3 moveDirection;

    public void Setup(Vector3 direction)
    {
        moveDirection = direction.normalized;

        if (moveDirection != Vector3.zero)
        {
            // Разворачиваем в сторону полета + применяем смещение для меча
            transform.rotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(rotationOffset);
        }
    }

    void Update()
    {
        if (isStuck) return;

        float moveStep = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, moveStep + 0.1f, wallLayer))
        {
            // Ставим в точку удара с небольшим отступом
            transform.position = hit.point - (moveDirection * wallOffset);

            // УДАЛЕНО: transform.SetParent(hit.transform); // Больше не плющит!

            // Отключаем коллайдер, чтобы он не мешал
            //Collider col = GetComponent<Collider>();
            //if (col != null) col.enabled = false;

            isStuck = true;
            Debug.Log("Снаряд застрял!");
        }
        else
        {
            transform.position += moveDirection * moveStep;
            distanceTraveled += moveStep;

            if (distanceTraveled >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}