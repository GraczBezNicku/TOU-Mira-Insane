﻿using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modules;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;

namespace TownOfUs.Buttons.Crewmate;

public sealed class TrapperTrapButton : TownOfUsRoleButton<TrapperRole>
{
    public override string Name => "Trap";
    public override string Keybind => "ActionQuaternary";
    public override Color TextOutlineColor => TownOfUsColors.Trapper;
    public override float Cooldown => OptionGroupSingleton<TrapperOptions>.Instance.TrapCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TrapperOptions>.Instance.MaxTraps;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.TrapSprite;

    protected override void OnClick()
    {
        var role = PlayerControl.LocalPlayer.GetRole<TrapperRole>();

        if (role == null) return;

        var pos = PlayerControl.LocalPlayer.transform.position;
        pos.z += 0.001f;

        // TODO: waiting for android assetbundle
        //Trap.CreateTrap(role, pos);

        TouAudio.PlaySound(TouAudio.TrapperPlaceSound);
    }
}
