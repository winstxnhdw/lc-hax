#region

using System.Linq;
using Hax;
using UnityEngine;
using UnityEngine.AI;

#endregion

class EarthLeviathanController : IEnemyController<SandWormAI> {
    Vector3 CamOffset { get; } = new(0, 8f, -13f);

    public Vector3 GetCameraOffset(SandWormAI _) => this.CamOffset;


    public void UsePrimarySkill(SandWormAI enemy) {
        if (this.IsEmerged(enemy)) {
            enemy.SetInGround();
        }
        else {
            ForceEmergeAnimation(enemy);
        }
    }

    public void UseSecondarySkill(SandWormAI enemy) {
        if (this.IsEmerged(enemy)) return;
        if (enemy.emergeFromGroundParticle1.isPlaying) {
            enemy.emergeFromGroundParticle1.Stop(true);
        }
        else {
            RoundManager.PlayRandomClip(enemy.creatureSFX, enemy.groundRumbleSFX, true, 1f, 0, 1000);
            enemy.emergeFromGroundParticle1.Play(true);
        }
    }


    public void UseSpecialAbility(SandWormAI enemy) => enemy.creatureVoice.PlayOneShot(enemy.roarSFX[enemy.Reflect().GetInternalField<System.Random>("sandWormRandom").Next(0, enemy.roarSFX.Length)]);

    public bool IsAbleToMove(SandWormAI enemy) => !enemy.inSpecialAnimation;

    public bool IsAbleToRotate(SandWormAI enemy) => !enemy.inSpecialAnimation;


    public string GetPrimarySkillName(SandWormAI worm) {
        if(worm is not SandWormAI enemy) return string.Empty;
        return this.IsEmerged(enemy) ? "Reset" : "Emerge";
    }


    public string GetSecondarySkillName(SandWormAI enemy) {
        if (this.IsEmerged(enemy)) return "";
        if (enemy.emergeFromGroundParticle1.isPlaying) {
            return "Stop Ground Rumble";
        }
        else {
            return "Start Ground Rumble";
        }
    }

    public string GetSpecialAbilityName(SandWormAI _) => "Play Roar Noise";

    


    public bool CanUseEntranceDoors(SandWormAI _) => false;

    public float InteractRange(SandWormAI _) => 0.0f;

    public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;

    public void OnOutsideStatusChange(SandWormAI enemy) => enemy.StopSearch(enemy.roamMap, true);

    // Taken from the Original code and removed server checks.
    public void ForceEmergeAnimation(SandWormAI worm) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.RoundManager is not RoundManager roundManager) return;
        worm.inEmergingState = true;
        float num = roundManager.YRotationThatFacesTheFarthestFromPosition(
            worm.transform.position + Vector3.up * 1.5f, 30f) + Random.Range(-45f, 45f);

        worm.agent.enabled = false;
        worm.inSpecialAnimation = true;
        worm.transform.eulerAngles = new Vector3(0.0f, num, 0.0f);

        for (int attempt = 0; attempt < 6; ++attempt) {
            bool isNaturalSurface = false;
            for (int nodeIndex = 0; nodeIndex < worm.airPathNodes.Length - 1; ++nodeIndex) {
                Vector3 direction = worm.airPathNodes[nodeIndex + 1].position - worm.airPathNodes[nodeIndex].position;
                if (Physics.SphereCast(worm.airPathNodes[nodeIndex].position, 5f, direction, out RaycastHit hitInfo,
                        direction.magnitude,
                        startOfRound.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore)) {
                    isNaturalSurface = startOfRound.naturalSurfaceTags.Any(tag => hitInfo.collider.CompareTag(tag)) ||
                                       (startOfRound.currentLevel.levelID == 12 && hitInfo.collider.CompareTag("Rock"));

                    if (!isNaturalSurface) break;
                }
            }

            if (!isNaturalSurface) {
                num += 60f;
                worm.transform.eulerAngles = new Vector3(0.0f, num, 0.0f);
            }
            else {
                if (Physics.Raycast(worm.endingPosition.position + Vector3.up * 50f, Vector3.down,
                        out RaycastHit hitInfo, 100f,
                        startOfRound.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore)) {
                    worm.endOfFlightPathPosition = RoundManager.Instance.GetNavMeshPosition(
                        hitInfo.point, worm.Reflect().GetInternalField<NavMeshHit>("navHit"), 8f, worm.agent.areaMask);

                    if (!roundManager.GotNavMeshPositionResult)
                        worm.endOfFlightPathPosition = roundManager.GetClosestNode(hitInfo.point).position;
                    break;
                }
            }
        }

        worm.EmergeServerRpc((int)num);
    }

    bool IsEmerged(SandWormAI enemy) => enemy.inEmergingState || enemy.emerged;
}
