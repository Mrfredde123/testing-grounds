using UnityEngine;

public class PlayerPosAssigner : MonoBehaviour
{
    // Сюда мы вручную перетащим материал в инспекторе
    public Material wallMaterial;

    void Update()
    {
        if (wallMaterial != null)
        {
            // Передаем позицию напрямую в материал
            wallMaterial.SetVector("_GlobalPlayerPosition", transform.position);
        }
    }
}
