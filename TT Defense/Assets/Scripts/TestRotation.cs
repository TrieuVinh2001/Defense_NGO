using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.Rotate(transform.position - new Vector3(7, 5, 0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.right = transform.position - new Vector3(7, 5, 0);
        //gameObject.transform.Rotate(transform.position - new Vector3(7,5,0));
    }
}
