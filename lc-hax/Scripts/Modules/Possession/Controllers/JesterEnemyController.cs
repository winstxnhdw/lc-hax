using System.Linq;
using GameNetcodeStuff;
using Hax;

enum JesterState {
    CLOSED,
    CRANKING,
    OPEN
}

internal class JesterController : IEnemyController<JesterAI> {
    

    public void UsePrimarySkill(JesterAI enemy) {
        enemy.SetBehaviourState(JesterState.CLOSED);
        this.SetNoPlayerChasetimer(enemy, 0f);
    }


    public void OnSecondarySkillHold(JesterAI enemy) {
        if (!enemy.IsBehaviourState(JesterState.CLOSED)) return;
        enemy.SetBehaviourState(JesterState.CRANKING);
    }

    public void ReleaseSecondarySkill(JesterAI enemy) {
        if (!enemy.IsBehaviourState(JesterState.CRANKING)) return;
        enemy.SetBehaviourState(JesterState.OPEN);
    }


    public void Update(JesterAI enemy) => this.SetNoPlayerChasetimer(enemy, 100f);

    public void OnUnpossess(JesterAI enemy) {
        this.SetNoPlayerChasetimer(enemy, 5f);
        enemy.SetBehaviourState(JesterState.CLOSED);
    }

    void SetNoPlayerChasetimer(JesterAI enemy, float value) => enemy.Reflect().SetInternalField("noPlayersToChaseTimer", value);

    public bool IsAbleToMove(JesterAI enemy) => !enemy.IsBehaviourState(JesterState.CRANKING);

    public bool IsAbleToRotate(JesterAI enemy) => !enemy.IsBehaviourState(JesterState.CRANKING);

    public string GetPrimarySkillName(JesterAI _) => "Close box";

    public string GetSecondarySkillName(JesterAI _) => "(HOLD) Begin cranking";

    public float InteractRange(JesterAI _) => 1.0f;
}
