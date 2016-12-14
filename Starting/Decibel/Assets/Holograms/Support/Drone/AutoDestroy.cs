using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
    public GameObject Poly;

    void Start()
    {
        Invoke("DestroyMe", 2.0f);
    }

    void DestroyMe()
    {
        Destroy(this.gameObject);
    }

    void Update()
    {
        transform.position = Poly.transform.position;
    }
}