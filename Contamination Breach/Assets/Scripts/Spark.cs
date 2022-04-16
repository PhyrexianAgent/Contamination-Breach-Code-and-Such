using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour
{
    const float SPARK_TIME = 0.04f;
    void Start()
    {
        Invoke("EndSelf", SPARK_TIME);
    }

    void EndSelf()
    {
        Destroy(gameObject);
    }
}
