using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Options.Modifiers.Crewmate;

namespace TownOfUs.Modifiers.Game.Crewmate;

public sealed class FrozenModifier(PlayerControl freezer) : TimedModifier
{
    public override string ModifierName => "Frozen";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<FrostyOptions>.Instance.ChillDuration;

    private float SpeedCache { get; set; }
    private DateTime ApplicationTime { get; set; }

    public PlayerControl Freezer { get; set; } = freezer;

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        ApplicationTime = DateTime.UtcNow;
        SpeedCache = Player.MyPhysics.Speed;

        float targetMultiplier = OptionGroupSingleton<FrostyOptions>.Instance.ChillStartSpeed;

        if (Freezer.HasModifier<InsaneModifier>())
        {
            targetMultiplier = 1 + (1 - OptionGroupSingleton<FrostyOptions>.Instance.ChillStartSpeed);
        }

        Player.MyPhysics.Speed *= targetMultiplier;
    }

    public override void OnDeactivate()
    {
        Player.MyPhysics.Speed = SpeedCache;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var timeSpan = DateTime.UtcNow - ApplicationTime;
        var duration = Duration * 1000f;
        Player.MyPhysics.Speed = SpeedCache * 1 - (duration - (float)timeSpan.TotalMilliseconds) *
            (1 - OptionGroupSingleton<FrostyOptions>.Instance.ChillStartSpeed) / duration;
    }
}