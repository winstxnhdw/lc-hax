using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("weight")]
internal class WeightToggle : ICommand
{
    public void Execute(StringArray _)
    {
        if (WeightMod.Instance is not WeightMod weight) return;
        weight.enabled = !weight.enabled;
        Helper.SendFlatNotification(weight.enabled ? "Weight Mod enabled" : "Weight Mod disabled");
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.carryWeight = 1.0f;
        if (!weight.enabled)
        {
            var totalWeight = 0f;
            foreach (var item in player.ItemSlots)
                if (item != null)
                {
                    totalWeight += Mathf.Clamp(item.itemProperties.weight - 1f, 0f, 10f);
                    ;
                }

            player.carryWeight += totalWeight;
        }
    }
}