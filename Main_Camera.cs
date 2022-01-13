using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Camera : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f")){
            Vector3 temp = gameObject.transform.eulerAngles;
            temp.z += 180.0f;
            gameObject.transform.eulerAngles = temp;
        }
    }
}
