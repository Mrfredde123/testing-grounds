using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{

    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public bool hasSword = false;
    private InputAction shootAction;
    public Sword sword;
    public GameObject swordObject;

    void Awake()
    {
        shootAction = new InputAction("Shoot", binding: "<Mouse>/leftButton");
        shootAction.Enable();
    }

    void Update()
    {
        if (sword.isActiveAndEnabled)
        {
            hasSword = true;
        }
        if (shootAction.WasPressedThisFrame())
        {
            if (hasSword)
            {
                Shoot();

            }

        }
    }

    void Shoot()
    {
        hasSword = false;
        swordObject.SetActive(false);
        // Создаем снаряд
        GameObject bullet = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Берем скрипт Projectile с созданного снаряда
        Projectile projScript = bullet.GetComponent<Projectile>();

        // Передаем ему направление, куда смотрит SpawnPoint
        // В файле PlayerShoot.cs, внутри метода Shoot()
        if (projScript != null)
        {
            // Передаем направление (spawnPoint.forward) И ссылку на игрока (transform)
            projScript.Setup(spawnPoint.forward, transform, spawnPoint);

        }
    }
    public void ReturnSword()
    {
        hasSword = true;
        swordObject.SetActive(true);

    }
}