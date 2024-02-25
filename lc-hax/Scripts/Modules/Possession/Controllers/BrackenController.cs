using GameNetcodeStuff;
using Hax;

enum BrackenState {
    SCOUTING,
    STAND,
    ANGER
}

internal class BrackenController : IEnemyController<FlowermanAI> {
    public void GetCameraPosition(FlowermanAI enemy) {
        PossessionMod.CamOffsetY = 2.5f;
        PossessionMod.CamOffsetZ = -3f;
    }

    bool GetStartingKillAnimationLocalClient(FlowermanAI enemy) => enemy.Reflect().GetInternalField<bool>("startingKillAnimationLocalClient");

    void SetStartingKillAnimationLocalClient(FlowermanAI enemy, bool value) => enemy.Reflect().SetInternalField("startingKillAnimationLocalClient", value);


    public void UsePrimarySkill(FlowermanAI enemy) {
        if (!enemy.carryingPlayerBody) {
            enemy.SetBehaviourState(BrackenState.ANGER);
        }

        enemy.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(BrackenState.STAND);

    public void ReleaseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(BrackenState.SCOUTING);

    public bool IsAbleToMove(FlowermanAI enemy) => !enemy.inSpecialAnimation;

    public string GetPrimarySkillName(FlowermanAI enemy) => enemy.carryingPlayerBody ? "Drop body" : "";

    public string GetSecondarySkillName(FlowermanAI _) => "Stand";

    public float InteractRange(FlowermanAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(FlowermanAI _) => false;
}
