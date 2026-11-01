using UnityEngine;
using UnityEngine.InputSystem;
public class Projectile : MonoBehaviour
{
    public Transform player;
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
    private bool isReturning = false;

    // В файле Projectile.cs
    public void Setup(Vector3 direction, Transform playerTransform)
    {
        moveDirection = direction.normalized;
        player = playerTransform; // <--- Вот тут мы сохраняем игрока

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(rotationOffset);
        }
    }

    void Update()
    {
        if (isStuck && Keyboard.current.tKey.wasPressedThisFrame)
        {
            isReturning = true;
            // (опционально) здесь можно отключить коллайдер меча, чтобы он не задевал стены на пути назад
        }
        if (isStuck && !isReturning) return;
        
        float moveStep = speed * Time.deltaTime;
        if (isReturning) 
        {
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.position) < 1.0f)
            {
                // 1. Достаем скрипт игрока
                PlayerShoot playerScript = player.GetComponent<PlayerShoot>();

                // 2. Если скрипт найден — вызываем возврат
                if (playerScript != null)
                {
                    playerScript.ReturnSword();
                }

                // 3. И ТОЛЬКО ПОСЛЕ ЭТОГО уничтожаем меч на сцене
                Destroy(gameObject);
            }
        }
        else if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, moveStep + 0.1f, wallLayer))
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