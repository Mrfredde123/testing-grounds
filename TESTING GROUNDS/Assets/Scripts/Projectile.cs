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

    [Header("Леска")]

    public float sagAmount = 0.4f;   // насколько провисает, в метрах
    public int leashSegments = 16;   // чем больше, тем плавнее
    public LineRenderer leash;
    public Transform leashAnchor;       // пустышка на рукояти меча
    public float maxLeashDistance = 12f;


    private float distanceTraveled = 0f;
    private bool isStuck = false;
    private Vector3 moveDirection;
    private bool isReturning = false;

    // В файле Projectile.cs



    public Transform handPoint;   // новое поле

    void Awake()
    {

        // Если в инспекторе пусто, ищем компоненты автоматически
        if (leash == null) leash = GetComponent<LineRenderer>();

        if (leashAnchor == null)
        {
            // Ищем дочерний объект с именем Anchor
            Transform foundAnchor = transform.Find("Anchor");
            if (foundAnchor != null) leashAnchor = foundAnchor;
        }
    }
    public void Setup(Vector3 direction, Transform playerTransform, Transform spawn)
    {
        moveDirection = direction.normalized;
        player = playerTransform;
        handPoint = spawn;              // <-- сохраняем руку

        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(rotationOffset);
    }
    void UpdateLeash()
    {
        if (leash == null) return;

        Transform from = handPoint != null ? handPoint : player;
        if (from == null) return;

        Vector3 start = from.position;
        Vector3 end = leashAnchor != null ? leashAnchor.position : transform.position;

        leash.positionCount = 2;
        leash.SetPosition(0, start);
        leash.SetPosition(1, end);
        float dist = Vector3.Distance(start, end);
        float percent = Mathf.InverseLerp(0f, maxLeashDistance, dist);
        Color darkRed = new Color(0.4f, 0f, 0f, 1f); // Темно-красный
        Color currentColor = Color.Lerp(Color.white, darkRed, percent);
        leash.startColor = currentColor;
        leash.endColor = currentColor;
    }

    void Update()
    {
        if (player == null) return;
        if (isStuck && Keyboard.current.tKey.wasPressedThisFrame)
        {
            isReturning = true;
            // (опционально) здесь можно отключить коллайдер меча, чтобы он не задевал стены на пути назад
        }

        UpdateLeash();

        if (player != null && Vector3.Distance(transform.position, player.position) > maxLeashDistance)
            isReturning = true;

        if (isStuck && !isReturning) return;

        float moveStep = speed * Time.deltaTime;
        if (isReturning)
        {
            // Берем руку (handPoint), а если её нет — игрока (player)
            Transform target = handPoint != null ? handPoint : player;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
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
                isReturning = true;
            }
        }
    }
}