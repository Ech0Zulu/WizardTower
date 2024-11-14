using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 5f;
    public float rotateSpeed = 5f;
    private float yRotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.Move(move*Time.deltaTime*speed);

        if (Input.GetKeyDown(KeyCode.A)) yRotation -= rotateSpeed*Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E)) yRotation += rotateSpeed * Time.deltaTime;

        transform.rotation = new Quaternion(0, yRotation,0,0);
    }
}
