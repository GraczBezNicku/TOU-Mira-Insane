using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using TownOfUs.Events.TouEvents;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class InsaneHunterStalkedModifier(PlayerControl hunter, PlayerControl hunterPickedTarget) : TimedModifier
{
    public override string ModifierName => "Insane Stalked";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkDuration;

    public PlayerControl Hunter { get; set; } = hunter;
    public PlayerControl HunterPickedTarget { get; set; } = hunterPickedTarget;
}
