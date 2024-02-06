using System;

public class EyelessDogController : IEnemyController<MouthDogAI> {
    public void UseSecondarySkill(MouthDogAI enemyInstance) {
        if (enemyInstance.currentBehaviourStateIndex is 2) return;
        Console.WriteLine("Switching to lunge");
        enemyInstance.SwitchToBehaviourServerRpc(2);
    }

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";
}
