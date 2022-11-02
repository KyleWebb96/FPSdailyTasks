using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [SerializeField] int sensHorz;
    [SerializeField] int sensVert;

    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool inverty;

    float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHorz;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;

        if (inverty)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
