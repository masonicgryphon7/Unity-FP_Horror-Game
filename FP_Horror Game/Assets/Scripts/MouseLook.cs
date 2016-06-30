using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    [HideInInspector]
    public bool useRotation = false;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float rotationX = 0F;
    public float rotationY = 0F;

    Quaternion originalRotation;

    [HideInInspector]
    public float startSensitivityX, startSensitivityY;

    private Transform rootTransform;

    void Start()
    {
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        originalRotation = transform.localRotation;

        #region Save Sensitivity options

        this.sensitivityX = PlayerPrefs.GetFloat("$Sensitivity");
        this.sensitivityY = PlayerPrefs.GetFloat("$Sensitivity");

        if (sensitivityX == 0)
            PlayerPrefs.SetFloat("$Sensitivity", 4);
        if (sensitivityY == 0)
            PlayerPrefs.SetFloat("$Sensitivity", 4);

        this.sensitivityX = PlayerPrefs.GetFloat("$Sensitivity");
        this.sensitivityY = PlayerPrefs.GetFloat("$Sensitivity");

        #endregion

        rootTransform = this.transform.root;

        startSensitivityX = sensitivityX;
        startSensitivityY = sensitivityY;
    }

    void Update()
    {
        if (Input.GetButton("Fire2") && Input.GetAxis("Fire2") > 0)
        {
            sensitivityX = startSensitivityX / 2;
            sensitivityY = startSensitivityY / 2;
        }
        else
        {
            sensitivityX = startSensitivityX;
            sensitivityY = startSensitivityY;
        }

        if (!useRotation && Cursor.lockState == CursorLockMode.Locked)
        {
            rotationX = Input.GetAxisRaw("Mouse X") * sensitivityX;
            rotationY = Input.GetAxisRaw("Mouse Y") * sensitivityY;
        }
        else if (Cursor.lockState != CursorLockMode.Locked)
            rotationX = rotationY = 0;

        if (axes == RotationAxes.MouseX)
        {
            if (!useRotation)
                originalRotation *= Quaternion.Euler(0f, rotationX, 0f);
        }
        else if (axes == RotationAxes.MouseY)
        {
            if (!useRotation)
                originalRotation *= Quaternion.Euler(-rotationY, 0f, 0f);
            originalRotation = ClampRotationAroundXAxis(originalRotation);
        }

        transform.localRotation = originalRotation;
    }

    public void ChangeRotation(Vector3 vector)
    {
        originalRotation = Quaternion.Euler(vector);
    }

    public void ChangeRotation(Quaternion quat)
    {
        originalRotation = quat;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, minimumY, maximumY);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}