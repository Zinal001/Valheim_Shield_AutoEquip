using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.zinals.valheim.shieldautoequip
{
    internal static class Patches
    {
        internal static HarmonyLib.Harmony HarmonyInstance { get; private set; }

        internal static void Patch()
        {
            HarmonyInstance = new HarmonyLib.Harmony("tech.zinals.valheim.shieldautoequip");
            HarmonyInstance.PatchAll(typeof(Patches));
        }


        [HarmonyLib.HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem)), HarmonyLib.HarmonyPostfix()]
        private static void Humanoid_EquipItem_Postfix(Humanoid __instance, ItemDrop.ItemData item, ref bool __result, ItemDrop.ItemData ___m_leftItem)
        {
            if(__result && item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon && ___m_leftItem == null)
            {
                if (Plugin.DetectedWeapons.Any() && !Plugin.DetectedWeapons.Contains(item.m_shared.m_name))
                    return;

                List<ItemDrop.ItemData> shieldItems = new List<ItemDrop.ItemData>();
                __instance.GetInventory().GetAllItems(ItemDrop.ItemData.ItemType.Shield, shieldItems);
                shieldItems = shieldItems.Where(s => (!s.m_shared.m_useDurability || s.m_durability > 0f) && (!Plugin.EquippableShields.Any() || Plugin.EquippableShields.Contains(s.m_shared.m_name))).OrderByDescending(s => s.GetBlockPower(__instance.GetSkillFactor(Skills.SkillType.Blocking))).ThenByDescending(s => s.m_durability).ToList();
                if (shieldItems.Any())
                {
                    Plugin.PluginLogger.LogInfo($"Equipping shield {shieldItems[0].m_shared.m_description}");
                    __instance.EquipItem(shieldItems[0], true);
                }
            }
        }

    }
}
