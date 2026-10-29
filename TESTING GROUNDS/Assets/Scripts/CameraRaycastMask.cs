using UnityEngine;

public class CameraRaycastMask : MonoBehaviour
{
    public Transform player;
    public float maskRadius = 1.5f;

    void Start()
    {
        Shader.SetGlobalFloat("_Radius", maskRadius);
    }

    void Update()
    {
        if (player == null) return;

        Vector3 targetPoint = player.position + Vector3.up * 1f;
        Vector3 direction = targetPoint - transform.position;
        float distanceToPlayer = direction.magnitude;

        Debug.DrawLine(transform.position, targetPoint, Color.red);

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        // Магия: "~LayerMask.GetMask("Ignore Raycast")" заставляет луч игнорировать всё, 
        // что находится на слое Ignore Raycast (туда мы сейчас отправим игрока)
        if (Physics.Raycast(ray, out hit, distanceToPlayer, ~LayerMask.GetMask("Ignore Raycast")))
        {
            Shader.SetGlobalVector("_CutoutPosition", hit.point);
        }
        else
        {
            Shader.SetGlobalVector("_CutoutPosition", new Vector3(0, 9999, 0));
        }
    }
}
