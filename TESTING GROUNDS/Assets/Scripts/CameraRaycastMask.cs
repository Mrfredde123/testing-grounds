using UnityEngine;

public class CameraRaycastMask : MonoBehaviour
{
    public Transform player; // Твой Capsule игрока
    public float maskRadius = 1.5f;

    void Start()
    {
        // Передаем радиус в шейдер один раз при старте
        Shader.SetGlobalFloat("_Radius", maskRadius);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Точка в центре тела игрока (на уровне груди)
        Vector3 targetPoint = player.position + Vector3.up * 1f;

        // Направление луча строго ОТ ГЛАЗА КАМЕРЫ до игрока
        Vector3 direction = (targetPoint - transform.position).normalized;

        // Полное расстояние от камеры до центра игрока
        float distanceToPlayer = Vector3.Distance(transform.position, targetPoint);

        // Даем лазеру запас длины в 1 метр, чтобы он прошивал игрока насквозь и не моргал на границе контакта
        float maxRayDistance = distanceToPlayer + 1f;

        // Рисуем лазер в окне Scene
        Debug.DrawLine(transform.position, targetPoint, Color.red);

        // Пускаем сквозной луч-пулю, который собирает ВСЕ препятствия на линии обзора
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, maxRayDistance, ~0);

        // Создаем флаг (проверку): закрывает ли нам что-то обзор? По умолчанию — нет (false)
        bool isPlayerHidden = false;

        // Перебираем все объекты, которые пробил наш сквозной луч
        foreach (RaycastHit hit in hits)
        {
            // Если луч наткнулся на стену/платформу, и точка удара находится БЛИЖЕ к камере,
            // чем сам игрок (с запасом безопасности в 10 сантиметров, чтобы не считать стены ЗА игроком)
            if (hit.distance < distanceToPlayer - 0.1f)
            {
                // Мы нашли препятствие, которое стоит МЕЖДУ камерой и игроком!
                isPlayerHidden = true;
                break; // Нашли хотя бы одно — этого достаточно, выходим из цикла
            }
        }

        // Применяем логику на основе проверки
        if (isPlayerHidden)
        {
            // Если игрока закрывает объект: прожигаем дыру строго в координатах самого игрока.
            // Так как координаты игрока плавные, дыра больше никогда не будет мигать!
            Shader.SetGlobalVector("_CutoutPosition", targetPoint);
        }
        else
        {
            // Если между камерой и игроком чисто (игрок стоит ПЕРЕД стенами) — 
            // убираем дыру далеко в небо, чтобы ничего не исчезало.
            Shader.SetGlobalVector("_CutoutPosition", new Vector3(0, 9999, 0));
        }
    }
}