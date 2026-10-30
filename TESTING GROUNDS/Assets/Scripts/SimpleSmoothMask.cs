using UnityEngine;

public class SimpleSmoothMask : MonoBehaviour
{
    public float radius = 2.0f; // Радиус мягкой дыры
    public Transform mainCamera; // Перетащи сюда Main Camera из иерархии

    void Start()
    {
        // Передаем радиус в шейдер один раз при старте
        Shader.SetGlobalFloat("_Radius", radius);
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Точка груди игрока
        Vector3 playerChest = transform.position + Vector3.up * 1f;

        // Вектор направления от камеры к игроку
        Vector3 direction = (playerChest - mainCamera.position).normalized;
        float distanceToPlayer = Vector3.Distance(mainCamera.position, playerChest);

        // Пускаем невидимый луч строго от камеры до игрока
        Ray ray = new Ray(mainCamera.position, direction);

        // Магия: если на линии взгляда камеры до игрока встало ХОТЬ ЧТО-ТО (стена/платформа)
        if (Physics.Raycast(ray, distanceToPlayer - 0.2f))
        {
            // Игрока загораживает объект спереди -> Включаем дыру ровно на игроке!
            Shader.SetGlobalVector("_CutoutPosition", playerChest);
        }
        else
        {
            // Между камерой и игроком чисто (объекты сзади) -> Уводим дыру в небо
            Shader.SetGlobalVector("_CutoutPosition", new Vector3(0, 9999, 0));
        }
    }
}