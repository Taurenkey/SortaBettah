using Dalamud.Plugin;
using KamiLib;
using KamiLib.System;
using SortaBettah.Controllers.Localization;
using SortaBettah.System;
using SortaBettah.Views.Windows;

namespace SortaBettah;

public sealed class SortaBettahPlugin : IDalamudPlugin {
    public static SortaBettahController Controller = null!;

    public SortaBettahPlugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, "SortaBettah");
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));

        Controller = new SortaBettahController();

        CommandController.RegisterMainCommand("/sortakinda", "/sorta");

        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }

    public void Dispose() {
        KamiCommon.Dispose();

        Controller.Dispose();
    }
}