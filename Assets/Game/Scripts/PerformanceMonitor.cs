using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// Monitors game performance metrics and logs warnings when performance degrades.
/// This helps identify performance bottlenecks during development and testing.
/// </summary>
public class PerformanceMonitor : MonoBehaviour
{
    #region Inspector Settings

    [Header("Monitoring Settings")]
    [Tooltip("Enable performance profiling and logging")]
    public bool enableProfiling = true;

    [Tooltip("How often to sample performance metrics (in seconds)")]
    [Range(0.1f, 5f)]
    public float profilingInterval = 1f;

    [Tooltip("Show performance overlay on screen")]
    public bool showPerformanceOverlay = false;

    [Header("Performance Thresholds")]
    [Tooltip("Minimum acceptable frame rate before logging warnings")]
    [Range(15f, 60f)]
    public float minAcceptableFrameRate = 30f;

    [Tooltip("Maximum acceptable memory usage in MB before logging warnings")]
    [Range(256f, 4096f)]
    public float maxAcceptableMemoryMB = 1024f;

    [Tooltip("Maximum acceptable garbage collection time in ms")]
    [Range(1f, 50f)]
    public float maxAcceptableGCTimeMS = 10f;

    #endregion

    #region Private Fields

    private float lastProfilingTime;
    private int frameCountSample;
    private float totalFrameTime;
    private long totalMemoryAllocated;
    private float gcTime;
    private bool isInitialized = false;

    // Performance history for trend analysis
    private float[] frameRateHistory = new float[10];
    private float[] memoryHistory = new float[10];
    private int historyIndex = 0;

    // GUI display variables
    private GUIStyle performanceStyle;
    private Rect performanceRect;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeMonitor();
    }

    private void Start()
    {
        SetupPerformanceTracking();
    }

    private void Update()
    {
        if (!enableProfiling || !isInitialized) return;

        UpdateFrameCounters();

        if (Time.time - lastProfilingTime >= profilingInterval)
        {
            SamplePerformanceMetrics();
            AnalyzePerformance();
            ResetCounters();
        }
    }

    private void OnGUI()
    {
        if (showPerformanceOverlay && enableProfiling && isInitialized)
        {
            DisplayPerformanceOverlay();
        }
    }

    private void OnDestroy()
    {
        CleanupMonitor();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Enable or disable performance monitoring at runtime.
    /// </summary>
    /// <param name="enable">True to enable monitoring</param>
    public void EnableProfiling(bool enable)
    {
        enableProfiling = enable;

        if (GameLogger.Instance != null)
        {
            GameLogger.Log($"Performance profiling {(enable ? "enabled" : "disabled")}",
                GameLogger.LogLevel.Info, "Performance");
        }
        else
        {
            Debug.Log($"Performance profiling {(enable ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    /// Get current performance statistics.
    /// </summary>
    /// <returns>Performance stats as a formatted string</returns>
    public string GetPerformanceStats()
    {
        if (!isInitialized) return "Performance monitor not initialized";

        float avgFrameRate = frameCountSample > 0 ? frameCountSample / profilingInterval : 0f;
        float memoryMB = totalMemoryAllocated / 1048576f;

        return $"FPS: {avgFrameRate:F1} | Memory: {memoryMB:F1}MB | GC Time: {gcTime:F2}ms";
    }

    /// <summary>
    /// Force a performance sample right now.
    /// </summary>
    public void ForceSample()
    {
        if (isInitialized)
        {
            SamplePerformanceMetrics();
            AnalyzePerformance();
        }
    }

    /// <summary>
    /// Reset all performance counters and history.
    /// </summary>
    public void ResetPerformanceHistory()
    {
        frameRateHistory = new float[10];
        memoryHistory = new float[10];
        historyIndex = 0;

        if (GameLogger.Instance != null)
        {
            GameLogger.Log("Performance history reset", GameLogger.LogLevel.Info, "Performance");
        }
    }

    #endregion

    #region Private Methods

    private void InitializeMonitor()
    {
        // Initialize GUI style for overlay
        performanceRect = new Rect(10, 10, 300, 120);

        // Reset counters
        ResetCounters();

        isInitialized = true;

        if (GameLogger.Instance != null)
        {
            GameLogger.Log("Performance monitor initialized", GameLogger.LogLevel.Info, "Performance");
        }
        else
        {
            Debug.Log("Performance monitor initialized");
        }
    }

    private void SetupPerformanceTracking()
    {
        // Configure Unity's profiler if available
        if (Application.isEditor)
        {
            // Enable deep profiling in editor for more detailed metrics
            Profiler.enabled = true;
        }
    }

    private void UpdateFrameCounters()
    {
        frameCountSample++;
        totalFrameTime += Time.unscaledDeltaTime;
    }

    private void SamplePerformanceMetrics()
    {
        // Memory usage sampling
        totalMemoryAllocated = Profiler.GetTotalAllocatedMemory(false);

        // Garbage collection time (approximation)
        gcTime = Profiler.GetMonoUsedSize() > 0 ? Time.unscaledDeltaTime * 1000f : 0f;
    }

    private void AnalyzePerformance()
    {
        // Calculate metrics
        float avgFrameRate = frameCountSample > 0 ? frameCountSample / profilingInterval : 0f;
        float memoryMB = totalMemoryAllocated / 1048576f;

        // Update history
        UpdatePerformanceHistory(avgFrameRate, memoryMB);

        // Check thresholds and log warnings
        CheckFrameRateThreshold(avgFrameRate);
        CheckMemoryThreshold(memoryMB);
        CheckGarbageCollectionThreshold(gcTime);

        // Log performance info for debugging
        LogPerformanceMetrics(avgFrameRate, memoryMB, gcTime);
    }

    private void UpdatePerformanceHistory(float frameRate, float memoryMB)
    {
        frameRateHistory[historyIndex] = frameRate;
        memoryHistory[historyIndex] = memoryMB;
        historyIndex = (historyIndex + 1) % frameRateHistory.Length;
    }

    private void CheckFrameRateThreshold(float currentFrameRate)
    {
        if (currentFrameRate < minAcceptableFrameRate)
        {
            string message = $"Low framerate detected: {currentFrameRate:F1} FPS (threshold: {minAcceptableFrameRate:F1})";

            if (GameLogger.Instance != null)
            {
                GameLogger.Log(message, GameLogger.LogLevel.Warning, "Performance");
            }
            else
            {
                Debug.LogWarning($"[Performance] {message}");
            }
        }
    }

    private void CheckMemoryThreshold(float currentMemoryMB)
    {
        if (currentMemoryMB > maxAcceptableMemoryMB)
        {
            string message = $"High memory usage detected: {currentMemoryMB:F1}MB (threshold: {maxAcceptableMemoryMB:F1}MB)";

            if (GameLogger.Instance != null)
            {
                GameLogger.Log(message, GameLogger.LogLevel.Warning, "Performance");
            }
            else
            {
                Debug.LogWarning($"[Performance] {message}");
            }
        }
    }

    private void CheckGarbageCollectionThreshold(float currentGCTime)
    {
        if (currentGCTime > maxAcceptableGCTimeMS)
        {
            string message = $"High GC time detected: {currentGCTime:F2}ms (threshold: {maxAcceptableGCTimeMS:F1}ms)";

            if (GameLogger.Instance != null)
            {
                GameLogger.Log(message, GameLogger.LogLevel.Warning, "Performance");
            }
            else
            {
                Debug.LogWarning($"[Performance] {message}");
            }
        }
    }

    private void LogPerformanceMetrics(float avgFrameRate, float memoryMB, float gcTimeMs)
    {
        // Only log if performance is concerning OR if we're in debug mode
        bool logMetrics = avgFrameRate < minAcceptableFrameRate ||
                         memoryMB > maxAcceptableMemoryMB ||
                         Application.isEditor;

        if (logMetrics)
        {
            string message = $"Performance Metrics - FPS: {avgFrameRate:F1}, Memory: {memoryMB:F1}MB, GC: {gcTimeMs:F2}ms";

            if (GameLogger.Instance != null)
            {
                var logLevel = (avgFrameRate < minAcceptableFrameRate || memoryMB > maxAcceptableMemoryMB)
                    ? GameLogger.LogLevel.Warning
                    : GameLogger.LogLevel.Info;

                GameLogger.Log(message, logLevel, "Performance");
            }
            else
            {
                Debug.Log($"[Performance] {message}");
            }
        }
    }

    private void ResetCounters()
    {
        frameCountSample = 0;
        totalFrameTime = 0f;
        lastProfilingTime = Time.time;
        gcTime = 0f;
    }

    private void DisplayPerformanceOverlay()
    {
        if (performanceStyle == null)
        {
            performanceStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { textColor = Color.white },
                fontSize = 12,
                alignment = TextAnchor.UpperLeft
            };
        }

        float avgFrameRate = frameCountSample > 0 ? frameCountSample / profilingInterval : Time.frameCount / Time.time;
        float memoryMB = totalMemoryAllocated / 1048576f;

        Color originalColor = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.7f);
        GUI.Box(performanceRect, "", performanceStyle);
        GUI.color = originalColor;

        string overlayText = $"PERFORMANCE MONITOR\n" +
                           $"FPS: {avgFrameRate:F1} (Min: {minAcceptableFrameRate:F1})\n" +
                           $"Memory: {memoryMB:F1}MB (Max: {maxAcceptableMemoryMB:F1}MB)\n" +
                           $"GC Time: {gcTime:F2}ms\n" +
                           $"Profiling: {(enableProfiling ? "ON" : "OFF")}";

        performanceStyle.normal.textColor = avgFrameRate < minAcceptableFrameRate ? Color.red : Color.white;
        GUI.Label(performanceRect, overlayText, performanceStyle);
    }

    private void CleanupMonitor()
    {
        if (GameLogger.Instance != null)
        {
            GameLogger.Log("Performance monitor shutting down", GameLogger.LogLevel.Info, "Performance");
        }

        isInitialized = false;
    }

    #endregion

    #region Static Utility Methods

    /// <summary>
    /// Get the current memory usage in MB.
    /// Static method for easy access from other scripts.
    /// </summary>
    /// <returns>Memory usage in megabytes</returns>
    public static float GetCurrentMemoryUsageMB()
    {
        return Profiler.GetTotalAllocatedMemory(false) / 1048576f;
    }

    /// <summary>
    /// Get the current frame rate.
    /// Static method for easy access from other scripts.
    /// </summary>
    /// <returns>Current frame rate</returns>
    public static float GetCurrentFrameRate()
    {
        return Time.frameCount / Time.time;
    }

    #endregion
}
