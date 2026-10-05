using HarmonyLib;
using UnityEngine;

namespace SchoolChange.Patches
{
  // Player's height patch
    [HarmonyPatch(typeof(CharacterController), "height", MethodType.Setter)]
    internal static class CharacterControllerHeightPatch
    {
        internal static CharacterController Target;
        internal static float? Override;

        private static bool Prefix(CharacterController __instance, float value)
        {
            if (!Override.HasValue) return true;
            if (__instance != Target) return true;
            return Mathf.Approximately(value, Override.Value);
        }
    }
}
