using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using ResoniteHotReloadLib;
using FrooxEngine.ProtoFlux;
using System.Reflection;

namespace QuickFluxBrowser;
//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class QuickFluxBrowser : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0"; //Changing the version here updates it in all locations needed
	public override string Name => "QuickFluxBrowser";
	public override string Author => "AmasterAmaster";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/resonite-modding-group/ExampleMod/";

	const string harmonyId = "com.AmasterAmaster.QuickFluxBrowser";
	static ModConfiguration config;

	public override void OnEngineInit()
	{
		HotReloader.RegisterForHotReload(this);

		// Get the config if needed
		config = GetConfiguration();

		// Call setup method
		Setup();
	}

	// This is the method that should be used to unload your mod
	// This means removing patches, clearing memory that may be in use etc.
	static void BeforeHotReload()
	{
		// Unpatch Harmony
		Harmony harmony = new Harmony(harmonyId);
		harmony.UnpatchAll(harmonyId);

		// clear any memory that is being used
	}

	// This is called in the newly loaded assembly
	// Load your mod here like you normally would in OnEngineInit
	static void OnHotReload(ResoniteMod modInstance)
	{
		// Get the config if needed
		config = modInstance.GetConfiguration();

		// Call setup method
		Setup();
	}

	static void Setup()
	{
		// Patch Harmony
		Harmony harmony = new Harmony(harmonyId);
		harmony.PatchAll();
	}

	[HarmonyPatch(typeof(InputInterface), "Update", typeof(float))]
	class QuickFluxBrowserPatch
	{
		public static MethodInfo protoflux_Browser = AccessTools.Method(typeof(ProtoFluxTool), "OpenNodeBrowser", new System.Type[] { typeof(IButton), typeof(ButtonEventData) });

		// Print a message when something happens (Just for example)
		public static void Postfix(InputInterface __instance)
		{
			if(__instance.GetKeyDown(Key.I))
			{
				ITool tool = Engine.Current.WorldManager.FocusedWorld.LocalUser.GetActiveTool();
				if(tool is ProtoFluxTool && !Engine.Current.WorldManager.FocusedWorld.LocalUser.HasActiveFocus())
				{
					Engine.Current.WorldManager.FocusedWorld.RunSynchronously(() =>
					{
						protoflux_Browser.Invoke(tool, new object[] { null, null });
					});
				}
			}
		}
	}
}
