using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Options;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Utilities;

public static class MiscUtils
{
    public static int KillersAliveCount => Helpers.GetAlivePlayers().Count(x => x.IsImpostor() || x.Is(RoleAlignment.NeutralKilling) || (x.Data.Role is ITouCrewRole { IsPowerCrew: true } &&
        OptionGroupSingleton<GeneralOptions>.Instance.CrewKillersContinue));

    public static int NKillersAliveCount => Helpers.GetAlivePlayers().Count(x => x.Is(RoleAlignment.NeutralKilling));

    public static int NonImpKillersAliveCount => Helpers.GetAlivePlayers().Count(x =>  x.Is(RoleAlignment.NeutralKilling) || (x.Data.Role is ITouCrewRole { IsPowerCrew: true } &&
        OptionGroupSingleton<GeneralOptions>.Instance.CrewKillersContinue));

    public static int ImpAliveCount => Helpers.GetAlivePlayers().Count(x => x.IsImpostor());

    public static int CrewKillersAliveCount => Helpers.GetAlivePlayers().Count(x => x.Data.Role is ITouCrewRole { IsPowerCrew: true } &&
        OptionGroupSingleton<GeneralOptions>.Instance.CrewKillersContinue);

    public static IEnumerable<BaseModifier> AllModifiers => MiraPluginManager.GetPluginByGuid(TownOfUsPlugin.Id)!.Modifiers;

    public static IEnumerable<RoleBehaviour> AllRoles => MiraPluginManager.GetPluginByGuid(TownOfUsPlugin.Id)!.Roles.Values;

    public static ReadOnlyCollection<IModdedOption>? GetModdedOptionsForRole(Type classType)
    {
        var plugin = MiraPluginManager.GetPluginByGuid(TownOfUsPlugin.Id);
        var optionGroup = plugin!.OptionGroups.FirstOrDefault(g => classType.IsAssignableFrom(g.OptionableType));

        return optionGroup?.Children;
    }

    public static string AppendOptionsText(Type classType)
    {
        var options = GetModdedOptionsForRole(classType);
        if (options == null) return string.Empty;

        var builder = new StringBuilder();
        builder.AppendLine("\n\n<b>Options</b>");

        foreach (var option in options)
        {
            switch (option)
            {
                case ModdedToggleOption toggleOption:
                    builder.AppendLine(option.Title + ": " + toggleOption.Value);
                    break;
                case ModdedEnumOption enumOption:
                    builder.AppendLine(enumOption.Title + ": " + enumOption.Values[enumOption.Value]);
                    break;
                case ModdedNumberOption numberOption:
                    builder.AppendLine(numberOption.Title + ": " + numberOption.Value + Helpers.GetSuffix(numberOption.SuffixType));
                    break;
            }
        }

        return builder.ToString();
    }

    public static IEnumerable<RoleBehaviour> GetRegisteredRoles(RoleAlignment alignment)
    {
        var roles = AllRoles.Where(x => x is ITownOfUsRole role && role.RoleAlignment == alignment);
        var registeredRoles = roles.ToList();

        switch (alignment)
        {
            case RoleAlignment.CrewmateInvestigative:
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Tracker));
                break;
            case RoleAlignment.CrewmateSupport:
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Crewmate));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Scientist));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Noisemaker));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Engineer));
                break;
            case RoleAlignment.ImpostorSupport:
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Impostor));
                break;
            case RoleAlignment.ImpostorConcealing:
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Shapeshifter));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Phantom));
                break;
        }

        return registeredRoles;
    }

    public static IEnumerable<RoleBehaviour> GetRegisteredRoles(ModdedRoleTeams team)
    {
        var roles = AllRoles.Where(x => x is ITownOfUsRole role && role.Team == team);
        var registeredRoles = roles.ToList();

        switch (team)
        {
            case ModdedRoleTeams.Crewmate:
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Crewmate));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Scientist));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Noisemaker));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Engineer));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Tracker));
                break;
            case ModdedRoleTeams.Impostor:
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Impostor));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Shapeshifter));
                registeredRoles.Add(RoleManager.Instance.GetRole(RoleTypes.Phantom));
                break;
        }

        return registeredRoles;
    }

    public static IEnumerable<RoleBehaviour> GetRegisteredGhostRoles()
    {
        var baseGhostRoles = RoleManager.Instance.AllRoles.Where(x => x.IsDead && !AllRoles.Any(y => y.Role == x.Role));
        var ghostRoles = AllRoles.Where(x => x.IsDead).Union(baseGhostRoles);

        return ghostRoles;
    }

    public static RoleBehaviour? GetRegisteredRole(RoleTypes roleType)
    {
        // we want to prioritise the custom roles because the role has the right RoleColour/TeamColor
        var role = AllRoles.FirstOrDefault(x => x.Role == roleType) ?? RoleManager.Instance.AllRoles.FirstOrDefault(x => x.Role == roleType);

        return role;
    }

    public static T? GetRole<T>() where T : RoleBehaviour => PlayerControl.AllPlayerControls.ToArray().ToList().Find(x => x.Data.Role is T)?.Data?.Role as T;

    public static IEnumerable<RoleBehaviour> GetRoles(RoleAlignment alignment) => CustomRoleUtils.GetActiveRoles().Where(x => x is ITownOfUsRole role && role.RoleAlignment == alignment);

    public static PlayerControl? GetPlayerWithModifier<T>() where T : BaseModifier => ModifierUtils.GetPlayersWithModifier<T>().FirstOrDefault();

    public static Color GetRoleColour(string name)
    {
        var pInfo = typeof(TownOfUsColors).GetProperty(name, BindingFlags.Public | BindingFlags.Static);

        if (pInfo == null) return TownOfUsColors.Impostor;
        var colour = (Color)pInfo.GetValue(null)!;

        return colour;
    }

    public static string RoleNameLookup(RoleTypes roleType)
    {
        var role = RoleManager.Instance.GetRole(roleType);
        return role?.NiceName ?? (roleType == RoleTypes.Crewmate ? "Crewmate" : "Impostor");
    }

    public static IEnumerable<RoleBehaviour> GetPotentialRoles()
    {
        var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        var roleOptions = currentGameOptions.RoleOptions;
        var assignmentData = RoleManager.Instance.AllRoles.Select(role => new RoleManager.RoleAssignmentData(role, roleOptions.GetNumPerGame(role.Role), roleOptions.GetChancePerGame(role.Role))).ToList();

        var roleList = assignmentData.Where(x => x.Chance > 0 && x.Role is ICustomRole).Select(x => x.Role);

        var crewmateRole = RoleManager.Instance.AllRoles.FirstOrDefault(x => x.Role == RoleTypes.Crewmate);
        roleList = roleList.AddItem(crewmateRole!);
        //Logger<TownOfUsPlugin>.Error($"GetPotentialRoles - crewmateRole: '{crewmateRole?.NiceName}'");

        var impostorRole = RoleManager.Instance.AllRoles.FirstOrDefault(x => x.Role == RoleTypes.Impostor);
        roleList = roleList.AddItem(impostorRole!);
        //Logger<TownOfUsPlugin>.Error($"GetPotentialRoles - impostorRole: '{impostorRole?.NiceName}'");

        //roleList.Do(x => Logger<TownOfUsPlugin>.Error($"GetPotentialRoles - role: '{x.NiceName}'"));

        return roleList;
    }
    public static void AddFakeChat(NetworkedPlayerInfo BasePlayer, string NameText, string Message, bool ShowHeadsup = false, bool AltColors = false, bool OnLeft = true)
    {
        var Chat = HudManager.Instance.Chat;
        
        var pooledBubble = Chat.GetPooledBubble();

            pooledBubble.transform.SetParent(Chat.scroller.Inner);
            pooledBubble.transform.localScale = Vector3.one;
            if (OnLeft) pooledBubble.SetLeft();
            else pooledBubble.SetRight();
            pooledBubble.SetCosmetics(BasePlayer);
            pooledBubble.NameText.text = NameText;
            pooledBubble.NameText.color = Color.white;
            pooledBubble.NameText.ForceMeshUpdate(true, true);
            pooledBubble.votedMark.enabled = false;
            pooledBubble.Xmark.enabled = false;
            pooledBubble.TextArea.text = Message;
            pooledBubble.TextArea.ForceMeshUpdate(true, true);
            pooledBubble.Background.size = new(5.52f, 0.2f + pooledBubble.NameText.GetNotDumbRenderedHeight() + pooledBubble.TextArea.GetNotDumbRenderedHeight());
            pooledBubble.MaskArea.size = pooledBubble.Background.size - new Vector2(0, 0.03f);
            if (AltColors)
            {
                //pooledBubble.Xmark.enabled = true;
                //pooledBubble.Xmark.transform.localPosition += Vector3.right * 0.8f;
                pooledBubble.Background.color = Color.black;
                pooledBubble.TextArea.color = Color.white;
            }

            pooledBubble.AlignChildren();
            var pos = pooledBubble.NameText.transform.localPosition;
            pooledBubble.NameText.transform.localPosition = pos;
            Chat.AlignAllBubbles();
			if (!Chat.IsOpenOrOpening && Chat.notificationRoutine == null)
			{
				Chat.notificationRoutine = Chat.StartCoroutine(Chat.BounceDot());
			}
			if (ShowHeadsup && !Chat.IsOpenOrOpening)
			{
				SoundManager.Instance.PlaySound(Chat.messageSound, false, 1f, null).pitch = 0.5f + (float)PlayerControl.LocalPlayer.PlayerId / 15f;
				Chat.chatNotification.SetUp(PlayerControl.LocalPlayer, Message);
			}
    }

    public static bool StartsWithVowel(this string word)
    {
        var vowels = new [] {'a', 'e', 'i', 'o', 'u'};
        return vowels.Any(vowel => word.StartsWith(vowel.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
    {
        return PlayerControl.AllPlayerControls.ToArray().Where(
            player => impostors.All(imp => imp.PlayerId != player.PlayerId)).ToList();
    }

    public static List<PlayerControl> GetImpostors(List<NetworkedPlayerInfo> infected)
    {
        return infected.Select(impData => impData.Object).ToList();
    }

    public static List<ushort> GetRolesToAssign(ModdedRoleTeams team, int max = -1, Func<RoleBehaviour, bool>? filter = null)
    {
        var roles = GetRegisteredRoles(team);

        return GetRolesToAssign(roles, max, filter);
    }

    public static List<ushort> GetRolesToAssign(RoleAlignment alignment, int max = -1, Func<RoleBehaviour, bool>? filter = null)
    {
        var roles = GetRegisteredRoles(alignment);

        return GetRolesToAssign(roles, max, filter);
    }

    private static List<ushort> GetRolesToAssign(IEnumerable<RoleBehaviour> roles, int max = -1, Func<RoleBehaviour, bool>? filter = null)
    {
        if (max == 0) return [];

        var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        var roleOptions = currentGameOptions.RoleOptions;

        var assignmentData = roles.Where(x => !x.IsDead && (filter == null || filter(x))).Select(role => new RoleManager.RoleAssignmentData(role, roleOptions.GetNumPerGame(role.Role), roleOptions.GetChancePerGame(role.Role))).ToList();

        List<(ushort RoleType, int Chance)> chosenRoles;

        if (max > 0)
        {
            chosenRoles = GetPossibleRoles(assignmentData, x => x.Chance == 100);

            // Shuffle to ensure that the same 100% roles do not appear in
            // every game if there are more than the maximum.
            chosenRoles.Shuffle();
            // Truncate the list if there are more 100% roles than the max.
            chosenRoles = chosenRoles.GetRange(0, Math.Min(max, chosenRoles.Count));

            if (chosenRoles.Count < max)
            {
                var potentialRoles = GetPossibleRoles(assignmentData, x => x.Chance < 100);

                // Determine which roles appear in this game.
                var optionalRoles = potentialRoles.Where(x => HashRandom.Next(101) < x.Chance).ToList();
                potentialRoles = potentialRoles.Where(x => !optionalRoles.Contains(x)).ToList();

                optionalRoles.Shuffle();
                chosenRoles.AddRange(optionalRoles.GetRange(0, Math.Min(max - chosenRoles.Count, optionalRoles.Count)));

                // If there are not enough roles after that, randomly add
                // ones which were previously eliminated, up to the max.
                if (chosenRoles.Count < max)
                {
                    potentialRoles.Shuffle();
                    chosenRoles.AddRange(potentialRoles.GetRange(0, Math.Min(max - chosenRoles.Count, potentialRoles.Count)));
                }
            }
        }
        else
        {
            var potentialRoles = GetPossibleRoles(assignmentData);
            chosenRoles = potentialRoles.Where(x => HashRandom.Next(101) < x.Chance).ToList();
        }

        var rolesToKeep = chosenRoles.Select(x => x.RoleType).ToList();
        rolesToKeep.Shuffle();

        // Log.Message($"GetRolesToKeep Kept - Count: {rolesToKeep.Count}");
        return rolesToKeep;
    }

    private static List<(ushort RoleType, int Chance)> GetPossibleRoles(List<RoleManager.RoleAssignmentData> assignmentData, Func<RoleManager.RoleAssignmentData, bool>? predicate = null)
    {
        var roles = new List<(ushort, int)>();

        assignmentData.Where(x => predicate == null || predicate(x)).ToList().ForEach((x) =>
        {
            for (var i = 0; i < x.Count; i++)
            {
                roles.Add(((ushort)x.Role.Role, x.Chance));
            }
        });

        return roles;
    }

    public static RoleManager.RoleAssignmentData GetAssignData(RoleTypes roleType)
    {
        var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        var roleOptions = currentGameOptions.RoleOptions;

        var role = GetRegisteredRole(roleType);
        var assignmentData = new RoleManager.RoleAssignmentData(role, roleOptions.GetNumPerGame(role!.Role), roleOptions.GetChancePerGame(role.Role));

        return assignmentData;
    }

    public static PlayerControl? PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.PlayerId == id)
                return player;
        }

        return null;
    }

    public static IEnumerator PerformTimedAction(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return new WaitForEndOfFrame();
        }

        action(1f);
    }

    public static IEnumerator CoFlash(Color color, float waitfor = 1f, float alpha = 0.3f)
    {
        color.a = alpha;
        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = HudManager.Instance.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.SetActive(true);
            fullscreen.color = color;
        }

        yield return new WaitForSeconds(waitfor);

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = HudManager.Instance.FullScreen;
            if (!fullscreen.color.Equals(color)) yield break;

            fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
            fullscreen.enabled = false;
        }
    }

    public static IEnumerator FadeOut(SpriteRenderer? rend, float delay = 0.01f, float decrease = 0.01f)
    {
        if (rend == null)
        {
            yield break;
        }

        var alphaVal = rend.color.a;
        var tmp = rend.color;

        while (alphaVal > 0)
        {
            alphaVal -= decrease;
            tmp.a = alphaVal;
            rend.color = tmp;

            yield return new WaitForSeconds(delay);
        }
    }

    public static IEnumerator FadeIn(SpriteRenderer? rend, float delay = 0.01f, float increase = 0.01f)
    {
        if (rend == null)
        {
            yield break;
        }

        var tmp = rend.color;
        tmp.a = 0;
        rend.color = tmp;

        while (rend.color.a < 1)
        {
            tmp.a = Mathf.Min(rend.color.a + increase, 1f); // Ensure it doesn't go above 1
            rend.color = tmp;

            yield return new WaitForSeconds(delay);
        }
    }

    public static GameObject CreateSpherePrimitive(Vector3 location, float radius)
    {
        // TODO: waiting for android assetbundle
        return new GameObject("Sphere");
        /*
        var spherePrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        spherePrimitive.name = "Sphere Primitive";
        spherePrimitive.transform.localScale = new Vector3(
            radius * ShipStatus.Instance.MaxLightRadius * 2f,
            radius * ShipStatus.Instance.MaxLightRadius * 2f,
            radius * ShipStatus.Instance.MaxLightRadius * 2f);

        Object.Destroy(spherePrimitive.GetComponent<SphereCollider>());

        spherePrimitive.GetComponent<MeshRenderer>().material = AuAvengersAnims.BombMaterial.LoadAsset();
        spherePrimitive.transform.position = location;

        return spherePrimitive;
        */
    }

    public static ArrowBehaviour CreateArrow(Transform parent, Color color)
    {
        var gameObject = new GameObject("Arrow")
        {
            layer = 5
        };
        gameObject.transform.parent = parent;

        var renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = TouAssets.ArrowSprite.LoadAsset();
        renderer.color = color;

        var arrow = gameObject.AddComponent<ArrowBehaviour>();
        arrow.image = renderer;
        arrow.image.color = color;

        return arrow;
    }

    public static IEnumerator BetterBloop(Transform target, float delay = 0, float finalSize = 1f, float duration = 0.5f, float intensity = 1f)
    {
        for (var t = 0f; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }
        var localScale = default(Vector3);
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            var z = 1f + (Effects.ElasticOut(t, duration) - 1f) * intensity;
            z *= finalSize;
            localScale.x = localScale.y = localScale.z = z;
            target.localScale = localScale;
            yield return null;
        }
        localScale.z = localScale.y = localScale.x = finalSize;
        target.localScale = localScale;
    }

    public static void AdjustGhostTasks(PlayerControl player)
    {
        foreach (var task in player.myTasks)
        {
            if (task.TryCast<NormalPlayerTask>() != null)
            {
                var normalPlayerTask = task.Cast<NormalPlayerTask>();

                var updateArrow = normalPlayerTask.taskStep > 0;

                normalPlayerTask.taskStep = 0;
                normalPlayerTask.Initialize();
                if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                {
                    foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
                    {
                        console.Image.color = Color.white;
                    }
                }

                normalPlayerTask.taskStep = 0;
                if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                {
                    normalPlayerTask.taskStep = 1;
                }
                if ((normalPlayerTask.TaskType == TaskTypes.EmptyGarbage || normalPlayerTask.TaskType == TaskTypes.EmptyChute)
                    && (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0 ||
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3 ||
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4))
                {
                    normalPlayerTask.taskStep = 1;
                }

                if (updateArrow)
                {
                    normalPlayerTask.UpdateArrowAndLocation();
                }

                var taskInfo = player.Data.FindTaskById(task.Id);
                taskInfo.Complete = false;
            }
        }
    }

    public static void UpdateLocalPlayerCamera(MonoBehaviour target, Transform lightParent)
    {
        HudManager.Instance.PlayerCam.SetTarget(target);
        PlayerControl.LocalPlayer.lightSource.transform.parent = lightParent;
        PlayerControl.LocalPlayer.lightSource.Initialize(PlayerControl.LocalPlayer.Collider.offset / 2);
    }

    public static void SnapPlayerCamera(MonoBehaviour target)
    {
        var cam = HudManager.Instance.PlayerCam;
        cam.SetTarget(target);
        cam.centerPosition = cam.Target.transform.position;
    }

    public static List<ushort> ReadFromBucket(List<RoleListOption> buckets, List<ushort> roles, RoleListOption roleType, RoleListOption replaceType)
    {
        var result = new List<ushort>();

        while (buckets.Contains(roleType))
        {
            if (roles.Count == 0)
            {
                var count = buckets.RemoveAll(x => x == roleType);
                buckets.AddRange(Enumerable.Repeat(replaceType, count));

                break;
            }

            var addedRole = roles.TakeFirst();
            result.Add(addedRole);

            buckets.Remove(roleType);
        }

        return result;
    }

    public static List<ushort> ReadFromBucket(List<RoleListOption> buckets, List<ushort> roles, RoleListOption roleType)
    {
        var result = new List<ushort>();

        while (buckets.Contains(roleType))
        {
            if (roles.Count == 0)
            {
                buckets.RemoveAll(x => x == roleType);

                break;
            }

            var addedRole = roles.TakeFirst();
            result.Add(addedRole);

            buckets.Remove(roleType);
        }

        return result;
    }
}
