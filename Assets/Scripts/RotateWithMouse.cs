using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    [SerializeField] float sens;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            float verticalInput = Input.GetAxis("Mouse Y");

            // Rotate the camera based on input
            transform.Rotate(Vector3.up, horizontalInput * sens * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -verticalInput * sens * Time.deltaTime, Space.Self);
        }
    }
}
