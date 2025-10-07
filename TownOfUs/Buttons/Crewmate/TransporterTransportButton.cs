using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;

namespace TownOfUs.Buttons.Crewmate;

public sealed class TransporterTransportButton : TownOfUsRoleButton<TransporterRole>
{
    public override string Name => TouLocale.Get("TouRoleTransporterTransport", "Transport");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Transporter;

    public override float Cooldown =>
        OptionGroupSingleton<TransporterOptions>.Instance.TransporterCooldown + MapCooldown;

    public override int MaxUses => (int)OptionGroupSingleton<TransporterOptions>.Instance.MaxNumTransports;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.Transport;
    public int ExtraUses { get; set; }

    public override void ClickHandler()
    {
        if (!CanClick())
        {
            return;
        }

        OnClick();
    }

    protected override void OnClick()
    {
        if (!OptionGroupSingleton<TransporterOptions>.Instance.MoveWithMenu)
        {
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }

        if (Minigame.Instance)
        {
            return;
        }

        var player1Menu = CustomPlayerMenu.Create();
        player1Menu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        player1Menu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

        player1Menu.Begin(
            plr => ((!plr.Data.Disconnected && !plr.Data.IsDead) || Helpers.GetBodyById(plr.PlayerId)) &&
                   (plr.moveable || plr.inVent),
            plr =>
            {
                player1Menu.ForceClose();

                if (plr == null)
                {
                    return;
                }

                var player2Menu = CustomPlayerMenu.Create();
                player2Menu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
                    PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
                player2Menu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
                    PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

                player2Menu.Begin(
                    plr2 => plr2.PlayerId != plr.PlayerId &&
                            (!plr2.HasDied() ||
                             Helpers.GetBodyById(plr2.PlayerId) /*  || MiscUtils.GetFakePlayer(plr2)?.body */) &&
                            (plr2.moveable || plr2.inVent),
                    plr2 =>
                    {
                        player2Menu.Close();
                        if (plr2 == null)
                        {
                            return;
                        }

                        if (Role.Player.HasModifier<InsaneModifier>())
                        {
                            byte newPly1 = plr == PlayerControl.LocalPlayer ? plr.PlayerId : PlayerControl.AllPlayerControls.ToArray().Where(x =>
                                (x.moveable || x.inVent)
                                && (!x.HasDied() || Helpers.GetBodyById(x.PlayerId))).Random().PlayerId;

                            byte newPly2 = plr == PlayerControl.LocalPlayer && newPly1 != plr.PlayerId ? plr.PlayerId : PlayerControl.AllPlayerControls.ToArray().Where(x =>
                                (x.moveable || x.inVent)
                                && (!x.HasDied() || Helpers.GetBodyById(x.PlayerId))
                                && x.PlayerId != newPly1).Random().PlayerId;

                            TransporterRole.RpcTransport(PlayerControl.LocalPlayer, newPly1, newPly2);
                        }
                        else
                            TransporterRole.RpcTransport(PlayerControl.LocalPlayer, plr.PlayerId, plr2.PlayerId);
                    }
                );
                foreach (var panel in player2Menu.potentialVictims)
                {
                    panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
                    if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
                    {
                        panel.NameText.color = Color.white;
                    }
                }
            }
        );
        foreach (var panel in player1Menu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
            {
                panel.NameText.color = Color.white;
            }
        }
    }
}