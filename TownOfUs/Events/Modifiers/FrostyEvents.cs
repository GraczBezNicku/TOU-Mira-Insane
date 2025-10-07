using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Game.Crewmate;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Options.Modifiers.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;

namespace TownOfUs.Events.Modifiers;

public static class FrostyEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!@event.Target.HasModifier<FrostyModifier>() || @event.Target == @event.Source ||
            MeetingHud.Instance)
        {
            return;
        }

        if (@event.Source.AmOwner)
        {
            LobbyNotificationMessage notif1 = null;
            if (@event.Target.HasModifier<InsaneModifier>())
            {
                notif1 = notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfUsColors.Frosty.ToTextColor()}{@event.Target.Data.PlayerName} was {TownOfUsColors.Insane.ToTextColor()}Insane</color> and Frosty, causing you to be faster for {Math.Round(OptionGroupSingleton<FrostyOptions>.Instance.ChillDuration, 2)} seconds.</color></b>",
                    Color.white, spr: TouModifierIcons.Frosty.LoadAsset());
            }
            else
            {
                notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfUsColors.Frosty.ToTextColor()}{@event.Target.Data.PlayerName} was Frosty, causing you to be slower for {Math.Round(OptionGroupSingleton<FrostyOptions>.Instance.ChillDuration, 2)} seconds.</color></b>",
                    Color.white, spr: TouModifierIcons.Frosty.LoadAsset());
            }

            notif1.AdjustNotification();
        }

        @event.Source.AddModifier<FrozenModifier>(@event.Target);
    }
}