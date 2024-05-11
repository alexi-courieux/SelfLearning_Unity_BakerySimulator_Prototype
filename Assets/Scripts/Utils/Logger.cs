public static class Logger
{
    // ReSharper disable Unity.PerformanceAnalysis
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogInfo(object message, UnityEngine.Object context = null)
    {
        UnityEngine.Debug.Log(message, context);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, UnityEngine.Object context = null)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object message, UnityEngine.Object context = null)
    {
        UnityEngine.Debug.LogError(message, context);
    }
}