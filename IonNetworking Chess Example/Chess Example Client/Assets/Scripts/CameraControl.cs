using UnityEngine;

public class CameraControl : MonoBehaviour {

    //The GameObject the script is controlling
    public GameObject target = null;

    //Settings

    public float slowSpeed = 0.12f;
    public float fastSpeed = 0.20f;
    public float mouseSensitivity = 3.5f;

    private float moveSpeed = 0.12f;

    private void Start()
    {
        //If there is no target selected, use parent.
        if(target == null)
        {
            target = gameObject;
        }
    }

    // Update is called once per frame
    private void Update ()
    {
        CheckKeyInputs();
        CheckMouseInputs();

        ApplyBoundaries();
	}
    
    //Check key inputs
    private void CheckKeyInputs()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = fastSpeed;
        else
            moveSpeed = slowSpeed;

        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }

        if (Input.GetKey(KeyCode.S))
        {
            MoveBackward();
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            MoveDown();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            MoveUp();
        }
    }

    //Check mouse inputs
    private Vector3 panRight = new Vector3(0, 0, 1);
    private Vector3 panUp = new Vector3(1, 0, 0);

    private void CheckMouseInputs()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * mouseSensitivity, Input.GetAxis("Mouse X") * mouseSensitivity, 0));
            float x = transform.rotation.eulerAngles.x;
            float y = transform.rotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(x, y, 0);
        }
        if (Input.GetMouseButton(2))
        {
            float x = -Input.GetAxis("Mouse X");
            float z = -Input.GetAxis("Mouse Y");

            Vector3 flat = GetFlattenedDirection();
            target.transform.localPosition += new Vector3(flat.z, 0, -flat.x) * x; //x axis
            target.transform.localPosition += new Vector3(flat.x, 0, flat.z) * z; //z axis
        }
    }

    //Apply boundaries to keep inside play space
    private void ApplyBoundaries()
    {

    }

    //Move forward
    private void MoveForward()
    {
        target.transform.localPosition += GetFlattenedDirection() * moveSpeed;
    }

    //Move backward
    private void MoveBackward()
    {
        Vector3 delta = -1 * GetFlattenedDirection() * moveSpeed;
        target.transform.localPosition += delta;
    }

    //Move right
    private void MoveRight()
    {
        Vector3 flat = GetFlattenedDirection();
        Vector3 delta = new Vector3(flat.z, 0, -flat.x) * moveSpeed;
        
        target.transform.localPosition += delta;
    }

    //Move left
    private void MoveLeft()
    {
        Vector3 flat = GetFlattenedDirection();
        Vector3 delta = new Vector3(-flat.z, 0, flat.x) * moveSpeed;

        target.transform.localPosition += delta;
    }

    //Move up
    private void MoveUp()
    {
        target.transform.localPosition += new Vector3(0, moveSpeed, 0);
    }

    //Move down
    private void MoveDown()
    {
        target.transform.localPosition += new Vector3(0, -moveSpeed, 0);
    }

    //Returns the targets transform.forward without the y axis
    private Vector3 GetFlattenedDirection()
    {
        return (new Vector3(target.transform.forward.x, 0, target.transform.forward.z));
    }
}
