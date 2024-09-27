using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.Mono;
using HarmonyLib;
using UnityEngine;


namespace Justice_For_Ladder_Bug
{
    [BepInPlugin(pluginGuid, pluginName, pluginversion)]
    public class Mod : BaseUnityPlugin
    {
        public const string pluginGuid = "kestrel.iamyourbeast.justiceforladderbug";
        public const string pluginName = "Justice For Ladder Bug";
        public const string pluginversion = "1.0.0";

        public static ConfigEntry<float> launchChance;
        public static ConfigEntry<float> launchMultiplier;

        public static ConfigEntry<float> shakeFrequency;
        public static ConfigEntry<float> shakeScale;
        public static ConfigEntry<float> shakeLength;

        public static System.Random rand = new System.Random();

        public void Awake() {

            launchChance = Config.Bind("Launch", "Launch Chance", 10f, "The chance for a ladder to launch you, in percent (0-100)");
            launchMultiplier = Config.Bind("Launch", "Launch Multiplier", 2700f, "A multiplier for the launch strength");

            shakeFrequency = Config.Bind("Screenshake", "Screenshake Frequency", 7.5f, "The frequency of the screenshake effect");
            shakeScale = Config.Bind("Screenshake", "Screenshake Scale", 100f, "The scale of the screenshake effect");
            shakeLength = Config.Bind("Screenshake", "Screenshake Length", 3f, "The length of the screenshake effect");

            Logger.LogInfo("Hiiiiiiiiiiii :3");
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.FinishClimbToSurface))]
    public class PatchLadderFling
    {   
        private static void Fling(PlayerMovement instance, Vector3 flingVec) {
            AccessTools.Field(typeof(PlayerMovement), "vMomentum").SetValue(instance, flingVec.y);
            AccessTools.Field(typeof(PlayerMovement), "uprightMovementVector").SetValue(instance, new Vector3(flingVec.x, 0f, flingVec.y));
        }

        [HarmonyPostfix]
        public static void Postfix(PlayerMovement __instance) {
            int rand = Mod.rand.Next(100) + 1;
            //Debug.Log(rand); //bug check: there was no bug im just unlucky lol
            if (rand <= Mathf.Clamp(Mod.launchChance.Value, 0, 100)) { //launchChance% chance of getting flung
                Fling(__instance, new Vector3(10, 0.1f, 10) * Mod.launchMultiplier.Value);
                GameManager.instance.cameraManager.GetShakeManager().AddShake(new CameraShake(Mod.shakeFrequency.Value, Mod.shakeScale.Value, Mod.shakeLength.Value));
            }
        }
    }
}
