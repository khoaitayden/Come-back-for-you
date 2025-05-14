using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Transform rArm;
    [SerializeField] private Transform lArm;
    [SerializeField] private Transform rLeg;
    [SerializeField] private Transform lLeg;

    [Header("Body Part Rigidbodies")]
    [SerializeField] private Rigidbody2D headRb;
    [SerializeField] private Rigidbody2D rArmRb;
    [SerializeField] private Rigidbody2D lArmRb;
    [SerializeField] private Rigidbody2D rLegRb;
    [SerializeField] private Rigidbody2D lLegRb;
    [SerializeField] private Rigidbody2D bodyRb;

    [Header("Spring Joints")]
    [SerializeField] private SpringJoint2D bodyToHeadSpringJoint;
    [SerializeField] private SpringJoint2D rArmToBodySpringJoint;
    [SerializeField] private SpringJoint2D lArmToBodySpringJoint;
    [SerializeField] private SpringJoint2D rLegToBodySpringJoint;
    [SerializeField] private SpringJoint2D lLegToBodySpringJoint;

    private bool canHeadReconnect = true;
    private bool canRightArmReconnect = true;
    private bool canLeftArmReconnect = true;
    private bool canRightLegReconnect = true;
    private bool canLeftLegReconnect = true;

    private void Awake()
    {
        InitializeSpringJoints();
    }

    private void FixedUpdate()
    {
        TryReconnectParts();
    }

    private void InitializeSpringJoints()
    {
        SpringJoint2D[] joints = { bodyToHeadSpringJoint, rArmToBodySpringJoint, lArmToBodySpringJoint, rLegToBodySpringJoint, lLegToBodySpringJoint };
        foreach (var joint in joints)
        {
            if (joint != null)
            {
                joint.enabled = false;
                joint.connectedBody = null;
            }
        }
    }

    private void TryReconnectParts()
    {
        if (!connectedHead && canHeadReconnect && Vector2.Distance(head.position, body.position) < snapDistance)
            ConnectPart(bodyToHeadSpringJoint, headRb, ref connectedHead, "Head");

        if (!connectedRightArm && canRightArmReconnect && Vector2.Distance(rArm.position, body.position) < snapDistance)
            ConnectPart(rArmToBodySpringJoint, bodyRb, ref connectedRightArm, "Right Arm");

        if (!connectedLeftArm && canLeftArmReconnect && Vector2.Distance(lArm.position, body.position) < snapDistance)
            ConnectPart(lArmToBodySpringJoint, bodyRb, ref connectedLeftArm, "Left Arm");

        if (!connectedRightLeg && canRightLegReconnect && Vector2.Distance(rLeg.position, body.position) < snapDistance)
            ConnectPart(rLegToBodySpringJoint, bodyRb, ref connectedRightLeg, "Right Leg");

        if (!connectedLeftLeg && canLeftLegReconnect && Vector2.Distance(lLeg.position, body.position) < snapDistance)
            ConnectPart(lLegToBodySpringJoint, bodyRb, ref connectedLeftLeg, "Left Leg");
    }

    private void ConnectPart(SpringJoint2D joint, Rigidbody2D partRb, ref bool connectionFlag, string partName)
    {
        if (joint != null && partRb != null)
        {
            joint.connectedBody = partRb;
            joint.enabled = true;
            connectionFlag = true;
            Debug.Log(partName + " connected");
        }
    }

    public void DisconnectHead()
    {
        DisconnectPart(bodyToHeadSpringJoint, "Head");
        connectedHead = false;
    }

    public void DisconnectRightArm()
    {
        DisconnectPart(rArmToBodySpringJoint, "Right Arm");
        connectedRightArm = false;
    }

    public void DisconnectLeftArm()
    {
        DisconnectPart(lArmToBodySpringJoint, "Left Arm");
        connectedLeftArm = false;
    }

    public void DisconnectRightLeg()
    {
        DisconnectPart(rLegToBodySpringJoint, "Right Leg");
        connectedRightLeg = false;
    }

    public void DisconnectLeftLeg()
    {
        DisconnectPart(lLegToBodySpringJoint, "Left Leg");
        connectedLeftLeg = false;
    }

    private void DisconnectPart(SpringJoint2D joint, string partName)
    {
        if (joint != null)
        {
            joint.connectedBody = null;
            joint.enabled = false;
        }

        StartReconnectCooldown(partName);
    }

    private void StartReconnectCooldown(string partName)
    {
        StartCoroutine(ReconnectCooldown(partName));
    }

    private IEnumerator ReconnectCooldown(string partName)
    {
        switch (partName)
        {
            case "Head":
                canHeadReconnect = false;
                break;
            case "Right Arm":
                canRightArmReconnect = false;
                break;
            case "Left Arm":
                canLeftArmReconnect = false;
                break;
            case "Right Leg":
                canRightLegReconnect = false;
                break;
            case "Left Leg":
                canLeftLegReconnect = false;
                break;
        }

        yield return new WaitForSeconds(reconnectDelay);

        switch (partName)
        {
            case "Head":
                canHeadReconnect = true;
                break;
            case "Right Arm":
                canRightArmReconnect = true;
                break;
            case "Left Arm":
                canLeftArmReconnect = true;
                break;
            case "Right Leg":
                canRightLegReconnect = true;
                break;
            case "Left Leg":
                canLeftLegReconnect = true;
                break;
        }
    }
}