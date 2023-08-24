using UnityEngine;
using System.Collections;

[System.Serializable]
public class BendingSegment : object
{
    public Transform firstTransform;
    public Transform lastTransform;
    public float thresholdAngleDifference;
    public float bendingMultiplier;
    public float maxAngleDifference;
    public float maxBendingAngle;
    public float responsiveness;
    internal float angleH;
    internal float angleV;
    internal Vector3 dirUp;
    internal Vector3 referenceLookDir;
    internal Vector3 referenceUpDir;
    internal int chainLength;
    internal Quaternion[] origRotations;
    public BendingSegment()
    {
        this.bendingMultiplier = 0.6f;
        this.maxAngleDifference = 30f;
        this.maxBendingAngle = 80f;
        this.responsiveness = 5f;
    }

}
[System.Serializable]
public class NonAffectedJoints : object
{
    public Transform joint;
    public float effect;
}
[System.Serializable]
public partial class HeadLookController : MonoBehaviour
{
    public Transform rootNode;
    public BendingSegment[] segments;
    public NonAffectedJoints[] nonAffectedJoints;
    public Vector3 headLookVector;
    public Vector3 headUpVector;
    public Vector3 target;
    public Transform targetTransform;
    public float effect;
    public bool overrideAnimation;
    public virtual void Start()
    {
        if (this.rootNode == null)
        {
            this.rootNode = this.transform;
        }
        // Setup segments
        foreach (BendingSegment segment in this.segments)
        {
            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
            segment.referenceLookDir = (parentRotInv * this.rootNode.rotation) * this.headLookVector.normalized;
            segment.referenceUpDir = (parentRotInv * this.rootNode.rotation) * this.headUpVector.normalized;
            segment.angleH = 0f;
            segment.angleV = 0f;
            segment.dirUp = segment.referenceUpDir;
            segment.chainLength = 1;
            Transform t = segment.lastTransform;
            while ((t != segment.firstTransform) && (t != t.root))
            {
                segment.chainLength++;
                t = t.parent;
            }
            segment.origRotations = new Quaternion[segment.chainLength];
            t = segment.lastTransform;
            int i = segment.chainLength - 1;
            while (i >= 0)
            {
                segment.origRotations[i] = t.localRotation;
                t = t.parent;
                i--;
            }
        }
    }

    public virtual void LateUpdate()
    {
        if (Time.deltaTime == 0)
        {
            return;
        }
        this.target = this.targetTransform.position;
        // Remember initial directions of joints that should not be affected
        Vector3[] jointDirections = new Vector3[this.nonAffectedJoints.Length];
        int i = 0;
        while (i < this.nonAffectedJoints.Length)
        {
            foreach (Transform child in this.nonAffectedJoints[i].joint)
            {
                jointDirections[i] = child.position - this.nonAffectedJoints[i].joint.position;
                break;
            }
            i++;
        }
        // Handle each segment
        foreach (BendingSegment segment in this.segments)
        {
            Transform t = segment.lastTransform;
            if (this.overrideAnimation)
            {
                i = segment.chainLength - 1;
                while (i >= 0)
                {
                    t.localRotation = segment.origRotations[i];
                    t = t.parent;
                    i--;
                }
            }
            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
            // Desired look direction in world space
            Vector3 lookDirWorld = (this.target - segment.lastTransform.position).normalized;
            // Desired look directions in neck parent space
            Vector3 lookDirGoal = parentRotInv * lookDirWorld;
            // Get the horizontal and vertical rotation angle to look at the target
            float hAngle = HeadLookController.AngleAroundAxis(segment.referenceLookDir, lookDirGoal, segment.referenceUpDir);
            Vector3 rightOfTarget = Vector3.Cross(segment.referenceUpDir, lookDirGoal);
            Vector3 lookDirGoalinHPlane = lookDirGoal - Vector3.Project(lookDirGoal, segment.referenceUpDir);
            float vAngle = HeadLookController.AngleAroundAxis(lookDirGoalinHPlane, lookDirGoal, rightOfTarget);
            // Handle threshold angle difference, bending multiplier,
            // and max angle difference here
            float hAngleThr = Mathf.Max(0, Mathf.Abs(hAngle) - segment.thresholdAngleDifference) * Mathf.Sign(hAngle);
            float vAngleThr = Mathf.Max(0, Mathf.Abs(vAngle) - segment.thresholdAngleDifference) * Mathf.Sign(vAngle);
            hAngle = (Mathf.Max(Mathf.Abs(hAngleThr) * Mathf.Abs(segment.bendingMultiplier), Mathf.Abs(hAngle) - segment.maxAngleDifference) * Mathf.Sign(hAngle)) * Mathf.Sign(segment.bendingMultiplier);
            vAngle = (Mathf.Max(Mathf.Abs(vAngleThr) * Mathf.Abs(segment.bendingMultiplier), Mathf.Abs(vAngle) - segment.maxAngleDifference) * Mathf.Sign(vAngle)) * Mathf.Sign(segment.bendingMultiplier);
            // Handle max bending angle here
            hAngle = Mathf.Clamp(hAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
            vAngle = Mathf.Clamp(vAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
            Vector3 referenceRightDir = Vector3.Cross(segment.referenceUpDir, segment.referenceLookDir);
            // Lerp angles
            segment.angleH = Mathf.Lerp(segment.angleH, hAngle, Time.deltaTime * segment.responsiveness);
            segment.angleV = Mathf.Lerp(segment.angleV, vAngle, Time.deltaTime * segment.responsiveness);
            // Get direction
            lookDirGoal = (Quaternion.AngleAxis(segment.angleH, segment.referenceUpDir) * Quaternion.AngleAxis(segment.angleV, referenceRightDir)) * segment.referenceLookDir;
            // Make look and up perpendicular
            Vector3 upDirGoal = segment.referenceUpDir;
            Vector3.OrthoNormalize(ref lookDirGoal, ref upDirGoal);
            // Interpolated look and up directions in neck parent space
            Vector3 lookDir = lookDirGoal;
            segment.dirUp = Vector3.Slerp(segment.dirUp, upDirGoal, Time.deltaTime * 5);
            Vector3.OrthoNormalize(ref lookDir, ref segment.dirUp);
            // Look rotation in world space
            Quaternion lookRot = (parentRot * Quaternion.LookRotation(lookDir, segment.dirUp)) * Quaternion.Inverse(parentRot * Quaternion.LookRotation(segment.referenceLookDir, segment.referenceUpDir));
            // Distribute rotation over all joints in segment
            Quaternion dividedRotation = Quaternion.Slerp(Quaternion.identity, lookRot, this.effect / segment.chainLength);
            t = segment.lastTransform;
            i = 0;
            while (i < segment.chainLength)
            {
                t.rotation = dividedRotation * t.rotation;
                t = t.parent;
                i++;
            }
        }
        // Handle non affected joints
        i = 0;
        while (i < this.nonAffectedJoints.Length)
        {
            Vector3 newJointDirection = Vector3.zero;
            foreach (Transform child in this.nonAffectedJoints[i].joint)
            {
                newJointDirection = child.position - this.nonAffectedJoints[i].joint.position;
                break;
            }
            Vector3 combinedJointDirection = Vector3.Slerp(jointDirections[i], newJointDirection, this.nonAffectedJoints[i].effect);
            this.nonAffectedJoints[i].joint.rotation = Quaternion.FromToRotation(newJointDirection, combinedJointDirection) * this.nonAffectedJoints[i].joint.rotation;
            i++;
        }
    }

    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
         // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);
        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);
        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }

    public HeadLookController()
    {
        this.headLookVector = Vector3.forward;
        this.headUpVector = Vector3.up;
        this.target = Vector3.zero;
        this.effect = 1f;
    }

}