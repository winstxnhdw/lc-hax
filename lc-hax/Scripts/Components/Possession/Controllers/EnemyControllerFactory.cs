using System;
using GameNetcodeStuff;
using UnityEngine;

static class EnemyControllerFactory {
    internal static IEnemyController Create(EnemyAI enemy) => enemy switch {
        CentipedeAI snareFlea => new SnareFleaController(snareFlea),
        FlowermanAI bracken => new BrackenController(bracken),
        ForestGiantAI forestGiant => new ForestGiantController(forestGiant),
        HoarderBugAI hoardingBug => new HoardingBugController(hoardingBug),
        JesterAI jester => new JesterController(jester),
        NutcrackerEnemyAI nutcracker => new NutcrackerController(nutcracker),
        PufferAI sporeLizard => new SporeLizardController(sporeLizard),
        BaboonBirdAI baboonHawk => new BaboonHawkController(baboonHawk),
        SandWormAI earthLeviathan => new EarthLeviathanController(earthLeviathan),
        MouthDogAI eyelessDog => new EyelessDogController(eyelessDog),
        MaskedPlayerEnemy maskedPlayer => new MaskedController(maskedPlayer),
        SpringManAI coilHead => new CoilHeadController(coilHead),
        BlobAI hygrodere => new HygrodereController(hygrodere),
        TestEnemy testEnemy => new TestEnemyController(testEnemy),
        LassoManAI lassoMan => new LassoManController(lassoMan),
        CrawlerAI crawler => new CrawlerController(crawler),
        SandSpiderAI sandSpider => new SandSpiderController(sandSpider),
        RedLocustBees circuitBees => new CircuitBeesController(circuitBees),
        _ => new DefaultEnemyController(enemy)
    };

    sealed class DefaultEnemyController(EnemyAI enemy) : IEnemyController {
        EnemyAI Enemy { get; } = enemy;

        Vector3 IEnemyController.CameraOffset => new(0.0f, 3.0f, -3.0f);
        Vector3 IEnemyController.PositionOffset => Vector3.zero;
        bool IEnemyController.CanMove => true;
        bool IEnemyController.CanRotate => true;
        float IEnemyController.EntranceRange => 2.5f;
        float IEnemyController.SprintMultiplier => 2.8f;
        float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake()?.speed;
        string? IEnemyController.PrimarySkillName => null;
        string? IEnemyController.SecondarySkillName => null;
        bool IEnemyController.AllowAIToChangeOwner => false;

        void IEnemyController.OnPossess() { }

        void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

        void IDisposable.Dispose() { }

        void IEnemyController.OnOutsideStatusChange() { }

        void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

        void IEnemyController.Tick(bool inputPaused) { }

        void IEnemyController.UpdateManual() { }

        void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

        bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

        void IEnemyController.OnPrimaryPressed() { }

        void IEnemyController.OnSecondaryPressed() { }

        void IEnemyController.OnSecondaryHeld() { }

        void IEnemyController.CancelSecondarySkill() { }

        void IEnemyController.ReleaseSecondarySkill() { }

        void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
    }
}
