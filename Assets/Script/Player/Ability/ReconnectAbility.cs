using UnityEngine;
using System.Collections;

public class ReconnectAbility : MonoBehaviour
{
    public static bool connectedHead = false;
    public static bool connectedRightArm = false;
    public static bool connectedLeftArm = false;
    public static bool connectedRightLeg = false;
    public static bool connectedLeftLeg = false;

    [Header("Connection Settings")]
    public float snapDistance;
    public float reconnectDelay;

    [Header("Body Part Transforms")]
    [SerializeField] private Transform head, body, rArm, lArm, rLeg, lLeg;

    [Header("Body Part Rigidbodies")]
    [SerializeField] private Rigidbody2D headRb, bodyRb, rArmRb, lArmRb, rLegRb, lLegRb;

    [Header("Spring Joints")]
    [SerializeField] private SpringJoint2D bodyToHeadSpringJoint, rArmToBodySpringJoint, lArmToBodySpringJoint, rLegToBodySpringJoint, lLegToBodySpringJoint;

    [Header("Movement Control")]
    public MovementStage movementStage;

    private bool canHeadReconnect = true, canRightArmReconnect = true, canLeftArmReconnect = true, canRightLegReconnect = true, canLeftLegReconnect = true;

    private void Awake()
    {
        // Reset static connection flags on scene load
        connectedHead = false;
        connectedRightArm = false;
        connectedLeftArm = false;
        connectedRightLeg = false;
        connectedLeftLeg = false;

        InitializeSpringJoints();
        FindOrAssignMovementStage();
        UpdateCurrentMovementStage();
    }

    private void FixedUpdate() => TryReconnectParts();

    private void InitializeSpringJoints()
    {
        var joints = new[] { bodyToHeadSpringJoint, rArmToBodySpringJoint, lArmToBodySpringJoint, rLegToBodySpringJoint, lLegToBodySpringJoint };
        foreach (var joint in joints)
        {
            if (joint)
            {
                joint.enabled = false;
                joint.connectedBody = null;
            }
        }
    }

    private void FindOrAssignMovementStage()
    {
        movementStage ??= GetComponent<MovementStage>() ?? FindObjectOfType<MovementStage>();
    }

    private void TryReconnectParts()
    {
        TryReconnectPart(ref connectedHead, canHeadReconnect, head, body, headRb, bodyToHeadSpringJoint, "Head");
        TryReconnectPart(ref connectedRightArm, canRightArmReconnect, rArm, body, bodyRb, rArmToBodySpringJoint, "Right Arm");
        TryReconnectPart(ref connectedLeftArm, canLeftArmReconnect, lArm, body, bodyRb, lArmToBodySpringJoint, "Left Arm");
        TryReconnectPart(ref connectedRightLeg, canRightLegReconnect, rLeg, body, bodyRb, rLegToBodySpringJoint, "Right Leg");
        TryReconnectPart(ref connectedLeftLeg, canLeftLegReconnect, lLeg, body, bodyRb, lLegToBodySpringJoint, "Left Leg");
    }

    private void TryReconnectPart(ref bool connectedFlag, bool canReconnect, Transform part, Transform body, Rigidbody2D rb, SpringJoint2D joint, string partName)
    {
        if (!connectedFlag && canReconnect && part && body && rb && joint && Vector2.Distance(part.position, body.position) < snapDistance)
        {
            ConnectPart(joint, rb, ref connectedFlag);
        }
    }

    private void ConnectPart(SpringJoint2D jointToEnable, Rigidbody2D targetConnectedBody, ref bool connectionFlag)
    {
        if (jointToEnable && targetConnectedBody)
        {
            jointToEnable.connectedBody = targetConnectedBody;
            jointToEnable.enabled = true;
            connectionFlag = true;
            UpdateCurrentMovementStage();
        }
    }

    // --- Disconnection Methods ---
    public void DisconnectHead() => DisconnectPart(bodyToHeadSpringJoint, ref connectedHead, "Head");
    public void DisconnectRightArm() => DisconnectPart(rArmToBodySpringJoint, ref connectedRightArm, "Right Arm");
    public void DisconnectLeftArm() => DisconnectPart(lArmToBodySpringJoint, ref connectedLeftArm, "Left Arm");
    public void DisconnectRightLeg() => DisconnectPart(rLegToBodySpringJoint, ref connectedRightLeg, "Right Leg");
    public void DisconnectLeftLeg() => DisconnectPart(lLegToBodySpringJoint, ref connectedLeftLeg, "Left Leg");

    private void DisconnectPart(SpringJoint2D joint, ref bool connectedFlag, string partName)
    {
        if (joint)
        {
            joint.enabled = false;
            joint.connectedBody = null;
            connectedFlag = false;
            StartCoroutine(ReconnectCooldown(partName));
            UpdateCurrentMovementStage();
        }
    }

    private IEnumerator ReconnectCooldown(string partName)
    {
        SetReconnectFlag(partName, false);
        yield return new WaitForSeconds(reconnectDelay);
        SetReconnectFlag(partName, true);
    }

    private void SetReconnectFlag(string partName, bool value)
    {
        switch (partName)
        {
            case "Head": canHeadReconnect = value; break;
            case "Right Arm": canRightArmReconnect = value; break;
            case "Left Arm": canLeftArmReconnect = value; break;
            case "Right Leg": canRightLegReconnect = value; break;
            case "Left Leg": canLeftLegReconnect = value; break;
        }
    }

    private void UpdateCurrentMovementStage()
    {
        if (!movementStage) return;

        BodyPartStage newStage;
        if (connectedHead && connectedRightArm && connectedLeftArm && connectedRightLeg && connectedLeftLeg)
            newStage = BodyPartStage.FullyConnected;
        else if (connectedHead && connectedRightArm && connectedLeftArm)
            newStage = BodyPartStage.TwoArmsConnected;
        else if (connectedHead && connectedRightArm)
            newStage = BodyPartStage.RightArmConnected;
        else if (connectedHead)
            newStage = BodyPartStage.BodyConnected;
        else
            newStage = BodyPartStage.HeadOnly;

        movementStage.CurrentStage = newStage;
    }

    // Public method to manually reset connection state
    public void Reset()
    {
        connectedHead = false;
        connectedRightArm = false;
        connectedLeftArm = false;
        connectedRightLeg = false;
        connectedLeftLeg = false;

        canHeadReconnect = true;
        canRightArmReconnect = true;
        canLeftArmReconnect = true;
        canRightLegReconnect = true;
        canLeftLegReconnect = true;

        InitializeSpringJoints();
        UpdateCurrentMovementStage();
    }
}