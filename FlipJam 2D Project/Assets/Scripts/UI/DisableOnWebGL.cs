using UnityEngine;

public class DisableOnWebGL : MonoBehaviour
{
    void Start()
    {
        #if UNITY_WEBGL
        gameObject.SetActive(false);
        #endif
    }
}