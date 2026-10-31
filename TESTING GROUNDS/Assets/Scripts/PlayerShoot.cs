using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;

    private InputAction shootAction;

    void Awake()
    {
        shootAction = new InputAction("Shoot", binding: "<Mouse>/leftButton");
        shootAction.Enable();
    }

    void Update()
    {
        if (shootAction.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Создаем снаряд
        GameObject bullet = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Берем скрипт Projectile с созданного снаряда
        Projectile projScript = bullet.GetComponent<Projectile>();

        // Передаем ему направление, куда смотрит SpawnPoint
        if (projScript != null)
        {
            // spawnPoint.forward — это синяя ось (Z) объекта SpawnPoint
            projScript.Setup(spawnPoint.forward);
        }
    }
}