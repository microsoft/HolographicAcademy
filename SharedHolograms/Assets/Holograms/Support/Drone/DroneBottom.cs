using UnityEngine;
using System.Collections;

public class DroneBottom : MonoBehaviour
{

    public Vector3 RotationVector = new Vector3(1, 0, 0);
    public int RotationSpeed = 300;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(RotationVector * Time.deltaTime * RotationSpeed);
    }
}