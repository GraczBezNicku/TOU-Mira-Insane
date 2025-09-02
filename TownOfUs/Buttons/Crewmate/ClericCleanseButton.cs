using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;

namespace TownOfUs.Buttons.Crewmate;

public sealed class ClericCleanseButton : TownOfUsRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Cleanse";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Cleric;
    public override float Cooldown => OptionGroupSingleton<ClericOptions>.Instance.CleanseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.CleanseSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfUsPlugin>.Error($"{Name}: Target is null");
            return;
        }

        if (Target.HasModifier<ClericCleanseModifier>())
        {
            Target.RpcRemoveModifier<ClericCleanseModifier>();
        }

        if (PlayerControl.LocalPlayer.TryGetModifier<InsaneModifier>(out var insane))
        {
            if (insane.PlayerIdToFakeCleansedEffects.ContainsKey(Target.PlayerId))
                insane.PlayerIdToFakeCleansedEffects[Target.PlayerId].Clear();
            else
                insane.PlayerIdToFakeCleansedEffects.Add(Target.PlayerId, new List<ClericCleanseModifier.EffectType>());

            int randomEffectCount = UnityEngine.Random.Range(0, Enum.GetValues<ClericCleanseModifier.EffectType>().Length);

            for (int i = 0; i <= randomEffectCount; i++)
            {
                insane.PlayerIdToFakeCleansedEffects[Target.PlayerId]
                    .Add(Enum.GetValues<ClericCleanseModifier.EffectType>().Where(x => !insane.PlayerIdToFakeCleansedEffects[Target.PlayerId].Contains(x)).Random());
            }

            Target.RpcAddModifier<ClericCleanseModifier>(PlayerControl.LocalPlayer);

            if (insane.PlayerIdToFakeCleansedEffects[Target.PlayerId].Count > 0)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Cleric));
            }

            CustomButtonSingleton<ClericBarrierButton>.Instance.ResetCooldownAndOrEffect();

            return;
        }

        Target.RpcAddModifier<ClericCleanseModifier>(PlayerControl.LocalPlayer);

        if (ClericCleanseModifier.FindNegativeEffects(Target).Count > 0)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Cleric));
        }

        CustomButtonSingleton<ClericBarrierButton>.Instance.ResetCooldownAndOrEffect();
    }
}