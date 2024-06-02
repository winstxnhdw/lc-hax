using System;
using GameNetcodeStuff;
using Hax;

[HostCommand("setgift")]
internal class GiftSetCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        var itemName = args[0];
        if (itemName is null)
        {
            Chat.Print("Usage: giftset <item>");
            return;
        }

        itemName = itemName.Replace("_", " ");

        GiftBoxItem? gift;

        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not GiftBoxItem held)
        {
            if (Helper.Grabbables.First(grabbable => grabbable is GiftBoxItem) is not GiftBoxItem map)
            {
                Chat.Print("No Gift found!");
                return;
            }

            if (!localPlayer.GrabObject(map))
            {
                Chat.Print("You must have an empty inventory slot!");
                return;
            }

            gift = map;
        }
        else
        {
            gift = held;
        }

        var scrap = Helper.GetItem(itemName);
        if (scrap is null)
        {
            Chat.Print($"{itemName} not found!");
            return;
        }

        SetGift(gift, scrap);
        Helper.SendFlatNotification($"Gift Content Replaced with {scrap.name}!");
    }

    private void SetGift(GiftBoxItem gift, Item item)
    {
        if (gift is null || item is null) return;
        if (Helper.RoundManager is null) return;
        var giftReflector = gift.Reflect();
        Random random = new((int)gift.targetFloorPosition.x + (int)gift.targetFloorPosition.y);

        var objectInPresentValue = (int)(random.Next(item.minValue + 25, item.maxValue + 35) *
                                         Helper.RoundManager.scrapValueMultiplier);
        _ = giftReflector.SetInternalField("objectInPresent", item.spawnPrefab);
        _ = giftReflector.SetInternalField("objectInPresentValue", objectInPresentValue);
    }
}