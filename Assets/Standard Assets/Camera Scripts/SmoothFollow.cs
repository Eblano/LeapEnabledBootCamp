using UnityEngine;
using System.Collections;

[System.Serializable]
/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
// The target we are following
// The distance in the x-z plane to the target
// the height we want the camera to be above the target
// How much we 
// Place the script in the Camera-Control group in the component menu
[UnityEngine.AddComponentMenu("Camera-Control/Smooth Follow")]
public partial class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float height;
    public float heightDamping;
    public float rotationDamping;
    public virtual void Update()
    {
        // Early out if we don't have a target
        if (!this.target)
        {
            return;
        }
        // Calculate the current rotation angles
        var wantedRotationAngle = this.target.eulerAngles.y;
        var wantedHeight = this.target.position.y + this.height;
        var currentRotationAngle = this.transform.eulerAngles.y;
        var currentHeight = this.transform.position.y;
        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, this.rotationDamping * Time.deltaTime);
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, this.heightDamping * Time.deltaTime);
        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        this.transform.position = this.target.position;
        this.transform.position = this.transform.position - ((currentRotation * Vector3.forward) * this.distance);

        {
            float _153 = // Set the height of the camera
            currentHeight;
            Vector3 _154 = this.transform.position;
            _154.y = _153;
            this.transform.position = _154;
        }
        // Always look at the target
        this.transform.LookAt(this.target);
    }

    public SmoothFollow()
    {
        this.distance = 10f;
        this.height = 5f;
        this.heightDamping = 2f;
        this.rotationDamping = 3f;
    }

}