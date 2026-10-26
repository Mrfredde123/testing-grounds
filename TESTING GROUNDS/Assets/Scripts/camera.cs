using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform cam_copy;
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.position = cam_copy.position+ new Vector3(4.53000021f, 5.05000019f, -4.01999998f);
    }
}
