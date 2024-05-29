using Hax;

public enum SporeLizardState
{
    IDLE,
    ALERTED,
    HOSTILE
}

internal class SporeLizardController : IEnemyController<PufferAI>
{
    public void UsePrimarySkill(PufferAI enemy)
    {
        enemy.SetBehaviourState(SporeLizardState.HOSTILE);
        enemy.StompServerRpc();
    }

    public void UseSecondarySkill(PufferAI enemy)
    {
        enemy.SetBehaviourState(SporeLizardState.HOSTILE);
        enemy.ShakeTailServerRpc();
    }

    public string GetPrimarySkillName(PufferAI _)
    {
        return "Stomp";
    }

    public string GetSecondarySkillName(PufferAI _)
    {
        return "Smoke";
    }

    public void OnOutsideStatusChange(PufferAI enemy)
    {
        enemy.StopSearch(enemy.roamMap, true);
    }

    private float GetTimeSinceHittingPlayer(PufferAI enemy)
    {
        return enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");
    }

    private void SetTimeSinceHittingPlayer(PufferAI enemy, float value)
    {
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);
    }
}