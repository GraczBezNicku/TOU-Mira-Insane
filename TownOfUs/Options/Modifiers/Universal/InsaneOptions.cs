using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace TownOfUs.Options.Modifiers.Universal;

public sealed class InsaneOptions : AbstractOptionGroup<InsaneModifier>
{
    public static bool IsEligibleForInsane(PlayerControl player)
    {
        List<string> eligibleRolesAndModifiers = new List<string>();

        InsaneOptions options = OptionGroupSingleton<InsaneOptions>.Instance;

        if (options.InsaneDetective)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Detective, "Detective"));

        if (options.InsaneSeer)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Seer, "Seer"));

        if (options.InsaneSnitch)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Snitch, "Snitch"));

        if (options.InsaneTrapper)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Trapper, "Trapper"));

        if (options.InsaneMystic)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Mystic, "Mystic"));

        if (options.InsaneAurial)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Aurial, "Aurial"));

        if (options.InsaneOracle)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Oracle, "Oracle"));

        if (options.InsaneMedic)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Medic, "Medic"));

        if (options.InsaneAltruist)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Altruist, "Altruist"));

        if (options.InsaneGuardianAngel)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.GuardianAngel, "Guardian Angel"));

        if (options.InsaneSwapper)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Swapper, "Swapper"));

        if (options.InsaneTransporter)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Transporter, "Transporter"));

        if (options.InsaneBait)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Bait, "Bait"));

        if (options.InsaneSleuth)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Sleuth, "Sleuth"));

        if (options.InsaneTiebreaker)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Tiebreaker, "Tiebreaker"));

        if (options.InsaneCleric)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Cleric, "Cleric"));

        if (options.InsaneHunter)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Hunter, "Hunter"));

        if (options.InsaneFrosty)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Frosty, "Frosty"));

        if (options.InsaneShy)
            eligibleRolesAndModifiers.Add(TouLocale.Get(TouNames.Shy, "Shy"));

        ITownOfUsRole role = player.GetTownOfUsRole();
        BaseModifier[] modifiers = player.GetModifiers<BaseModifier>().ToArray();

        return (role != null && eligibleRolesAndModifiers.Contains(role.RoleName)) || modifiers.Any(x => eligibleRolesAndModifiers.Contains(x.ModifierName));
    }

    public override string GroupName => "Insane";
    public override uint GroupPriority => 29;
    public override Color GroupColor => TownOfUsColors.Insane;

    [ModdedToggleOption("Insane Reveals on Tasks Done")]
    public bool InsaneRevealsOnTasksDone { get; set; } = false;

    [ModdedEnumOption("Insane Reveals To", typeof(InsaneRevealsTo), ["Self"])]
    public InsaneRevealsTo InsaneRevealsTo { get; set; } = InsaneRevealsTo.Self;

    [ModdedToggleOption("Detective Can Be Insane")]
    public bool InsaneDetective { get; set; } = false;
    [ModdedEnumOption("Insane Detective Sees", typeof(InsaneDetecitveSees), ["Opposite", "Random"])]
    public InsaneDetecitveSees InsaneDetectiveSees { get; set; } = InsaneDetecitveSees.Opposite;

    [ModdedToggleOption("Seer Can Be Insane")]
    public bool InsaneSeer { get; set; } = false;
    [ModdedEnumOption("Insane Seer Sees", typeof(InsaneSeerSees), ["Opposite", "Random"])]
    public InsaneSeerSees InsaneSeerSees { get; set; } = InsaneSeerSees.Opposite;

    [ModdedToggleOption("Snitch Can Be Insane")]
    public bool InsaneSnitch { get; set; } = false;

    [ModdedToggleOption("Trapper Can Be Insane")]
    public bool InsaneTrapper { get; set; } = false;
    [ModdedToggleOption("Insane Trapper Can See Dead Roles")]
    public bool InsaneTrapperSeesDead { get; set; } = true;

    [ModdedToggleOption("Mystic Can Be Insane")]
    public bool InsaneMystic { get; set; } = false;

    [ModdedToggleOption("Aurial Can Be Insane")]
    public bool InsaneAurial { get; set; } = false;

    [ModdedToggleOption("Oracle Can Be Insane")]
    public bool InsaneOracle { get; set; } = false;
    [ModdedToggleOption("Insane Oracle's Bless Protects")]
    public bool InsaneOracleBlessProtects { get; set; } = false;

    [ModdedToggleOption("Medic Can Be Insane")]
    public bool InsaneMedic { get; set; } = false;
    [ModdedToggleOption("Insane Medic Protects")]
    public bool InsaneMedicProtects { get; set; } = true;
    [ModdedEnumOption("Insane Medic Report Sees", typeof(InsaneMedicReportSees), ["Opposite", "Random"])]
    public InsaneMedicReportSees InsaneMedicReportSees { get; set; } = InsaneMedicReportSees.Opposite;

    [ModdedToggleOption("Altruist Can Be Insane")]
    public bool InsaneAltruist { get; set; } = false;
    [ModdedEnumOption("Insane Altruist Does", typeof(InsaneAltruistAction), ["Dies and Reports", "Dies", "Reports"])]
    public InsaneAltruistAction InsaneAltruistAbility { get; set; } = InsaneAltruistAction.DiesAndReport;

    [ModdedToggleOption("Cleric Can Be Insane")]
    public bool InsaneCleric { get; set; } = false;
    [ModdedToggleOption("Insane Cleric Protects")]
    public bool InsaneClericProtects { get; set; } = true;
    [ModdedToggleOption("Insane Cleric's Cleanse Works")]
    public bool InsaneClericCleanseWorks { get; set; } = false;

    [ModdedToggleOption("Hunter Can Be Insane")]
    public bool InsaneHunter { get; set; } = false;

    [ModdedToggleOption("Guardian Angel Can Be Insane")]
    public bool InsaneGuardianAngel { get; set; } = false;

    [ModdedToggleOption("Swapper Can Be Insane")]
    public bool InsaneSwapper { get; set; } = false;

    [ModdedToggleOption("Transporter Can Be Insane")]
    public bool InsaneTransporter { get; set; } = false;

    [ModdedToggleOption("Bait Can Be Insane")]
    public bool InsaneBait { get; set; } = false;

    [ModdedToggleOption("Sleuth Can Be Insane")]
    public bool InsaneSleuth { get; set; } = false;

    [ModdedToggleOption("Tiebreaker Can Be Insane")]
    public bool InsaneTiebreaker { get; set; } = false;

    [ModdedToggleOption("Frosty Can Be Insane")]
    public bool InsaneFrosty { get; set; } = false;

    [ModdedToggleOption("Shy Can Be Insane")]
    public bool InsaneShy { get; set; } = false;
}

public enum InsaneRevealsTo
{
    Self,
    Others,
    Everyone
}

public enum InsaneDetecitveSees
{
    Opposite,
    Random
}

public enum InsaneSeerSees
{
    Opposite,
    Random
}

public enum InsaneMedicReportSees
{
    Opposite,
    Random
}

public enum InsaneAltruistAction
{
    DiesAndReport,
    Dies,
    Report,
}
