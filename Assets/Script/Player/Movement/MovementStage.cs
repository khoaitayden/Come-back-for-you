using UnityEngine;

public enum BodyPartStage
{
    HeadOnly,
    BodyConnected,
    RightArmConnected,
    TwoArmsConnected,
    FullyConnected
}

public class MovementStage : MonoBehaviour
{
    [Header("Movement Scripts")]
    public MonoBehaviour headMovementScript;
    public MonoBehaviour headBodyMovementScript;
    public MonoBehaviour headBodyAndOneArmScript;
    public MonoBehaviour headBodyAndTwoArmsScript;
    public MonoBehaviour fullBodyScript;
    [SerializeField ] private Camera camera;

    private BodyPartStage _currentStage = BodyPartStage.HeadOnly;
    private bool _scriptsInitialized = false;

    public BodyPartStage CurrentStage
    {
        get { return _currentStage; }
        set
        {
            if (_currentStage != value || !_scriptsInitialized)
            {
                _currentStage = value;
                SetActiveMovementScript();
            }
        }
    }

    void Awake()
    {
        SetActiveMovementScript();
        _scriptsInitialized = true;
    }

    private void SetActiveMovementScript()
    {
        DisableAllMovementScripts();
        EnableCurrentStageScript();
    }

    private void DisableAllMovementScripts()
    {
        if (headMovementScript != null) headMovementScript.enabled = false;
        if (headBodyMovementScript != null) headBodyMovementScript.enabled = false;
        if (headBodyAndOneArmScript != null) headBodyAndOneArmScript.enabled = false;
        if (headBodyAndTwoArmsScript != null) headBodyAndTwoArmsScript.enabled = false;
        if (fullBodyScript != null) fullBodyScript.enabled = false;
    }

    private void EnableCurrentStageScript()
    {
        switch (_currentStage)
        {
            case BodyPartStage.HeadOnly:
                if (headMovementScript != null) headMovementScript.enabled = true;
                camera.orthographicSize = 8f;
                break;
            case BodyPartStage.BodyConnected:
                if (headBodyMovementScript != null) headBodyMovementScript.enabled = true;
                camera.orthographicSize = 10f;
                break;
            case BodyPartStage.RightArmConnected:
                if (headBodyAndOneArmScript != null) headBodyAndOneArmScript.enabled = true;
                camera.orthographicSize = 12f;
                break;
            case BodyPartStage.TwoArmsConnected:
                if (headBodyAndTwoArmsScript != null) headBodyAndTwoArmsScript.enabled = true;
                camera.orthographicSize = 14f;
                break;
            case BodyPartStage.FullyConnected:
                if (fullBodyScript != null) fullBodyScript.enabled = true;
                camera.orthographicSize = 16f;
                break;
        }
    }
}