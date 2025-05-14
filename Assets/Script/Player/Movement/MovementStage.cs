public enum BodyPartStage
{
    HeadOnly,
    BodyConnected,
    OneArmConnected,
    TwoArmsConnected,
    OneLegConnected,
    FullyConnected
}
public class MovementStage
{
    public BodyPartStage currentStage = BodyPartStage.HeadOnly;
}