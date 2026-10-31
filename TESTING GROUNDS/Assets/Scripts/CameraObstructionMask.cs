using UnityEngine;

public class CameraObstructionMask : MonoBehaviour
{
    [Header("Настройки игрока")]
    public Transform player;
    public float targetHeightOffset = 1.0f;

    [Header("Настройки дыры")]
    public float maxRadius = 2.5f;
    public float smoothSpeed = 15f;
    public float penetrationDepth = 4.0f;

    [Header("Луч сзади")]
    public float backOffset = 500.0f; // Ставь хоть 500, хоть 1000. Ограничений больше нет.

    [Header("Слои (LayerMask)")]
    public LayerMask wallLayer;

    private float currentRadius;
    private Vector3 currentCutoutPos;

    void Start()
    {
        if (player == null)
        {
            Player p = FindAnyObjectByType<Player>();
            if (p != null) player = p.transform;
        }
        currentRadius = 0f;
        currentCutoutPos = player != null ? player.position : Vector3.zero;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 playerChest = player.position + Vector3.up * targetHeightOffset;
        Vector3 direction = (playerChest - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerChest);

        // Стартовая точка луча далеко позади
        Vector3 rayStart = transform.position - (direction * backOffset);
        float maxDistance = distanceToPlayer + backOffset;

        // Рисуем синюю линию
        Debug.DrawLine(rayStart, transform.position, Color.blue);

        // Пробиваем ВСЁ на своем пути
        RaycastHit[] hits = Physics.RaycastAll(rayStart, direction, maxDistance, wallLayer);

        bool isBlocked = false;
        Vector3 bestHitPoint = Vector3.zero;

        // Ищем самую ПЕРВУЮ попавшуюся стену (ближайшую к началу луча)
        float closestDistance = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            // Единственный фильтр: игнорируем стены, которые находятся ЗА игроком
            // (hit.distance - это расстояние от rayStart. Если оно больше, чем путь до игрока, значит стена за игроком)
            if (hit.distance > backOffset + distanceToPlayer) continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                bestHitPoint = hit.point;
                isBlocked = true;
            }
        }

        if (isBlocked)
        {
            currentRadius = Mathf.Lerp(currentRadius, maxRadius, smoothSpeed * Time.deltaTime);

            // Берем найденную стену и сдвигаем дыру на penetrationDepth в сторону игрока
            Vector3 dirToPlayer = (playerChest - bestHitPoint).normalized;
            Vector3 targetPos = bestHitPoint + dirToPlayer * penetrationDepth;

            currentCutoutPos = Vector3.Lerp(currentCutoutPos, targetPos, smoothSpeed * Time.deltaTime);

            // Рисуем красную линию от камеры до найденной стены
            Debug.DrawLine(transform.position, bestHitPoint, Color.red);
        }
        else
        {
            currentRadius = Mathf.Lerp(currentRadius, 0f, smoothSpeed * Time.deltaTime);
            Debug.DrawLine(transform.position, playerChest, Color.green);
        }

        Shader.SetGlobalVector("_CutoutPosition", currentCutoutPos);
        Shader.SetGlobalFloat("_Radius", currentRadius);
    }
}