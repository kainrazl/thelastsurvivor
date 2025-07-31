using UnityEditor;
using UnityEditor.Build;

public static class BuildTools
{
#if ZOMBIES
    [MenuItem("BuildVersion/ZOMBIES (Active)")]
    public static void dummy(){ }

    [MenuItem("BuildVersion/Set AZTEK")]
    public static void SetAztek()
    {
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, "AZTEK");
    }
#elif AZTEK
    [MenuItem ("BuildVersion/AZTEK (Active)")]
    public static void dummy(){ }

    [MenuItem("BuildVersion/Set ZOMBIES")]
    public static void SetZombies()
    {
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, "ZOMBIES");
    }
#else
    [MenuItem("BuildVersion/Set AZTEK")]
    public static void SetAztek()
    {
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, "AZTEK");
    }

    [MenuItem("BuildVersion/Set ZOMBIES")]
    public static void SetZombies()
    {
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, "ZOMBIES");
    }
#endif
}
