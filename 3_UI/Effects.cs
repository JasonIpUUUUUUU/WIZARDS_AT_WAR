using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    [SerializeField]
    private float effectTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, effectTime);
    }
}
