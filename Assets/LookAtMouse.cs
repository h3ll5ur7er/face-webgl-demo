using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast( Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            transform.LookAt(hit.point);
        }
    }
}
