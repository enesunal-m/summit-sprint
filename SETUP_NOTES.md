# SummitSprint Setup Notes

This document contains detailed setup instructions and troubleshooting information for the SummitSprint Unity project.

## Table of Contents
- [Unity Code Analyzers Setup](#unity-code-analyzers-setup)
- [Development Tools Configuration](#development-tools-configuration)
- [Testing Framework Setup](#testing-framework-setup)
- [Performance Monitoring](#performance-monitoring)
- [UGS Services Configuration](#ugs-services-configuration)
- [Verification Checklist](#verification-checklist)
- [Troubleshooting](#troubleshooting)

## Unity Code Analyzers Setup

### What to Download

Unity doesn't provide standalone code analyzers like Roslyn analyzers. Instead, Unity uses built-in code analysis and IDE integrations. Here's what you should set up:

#### 1. IDE-Specific Analyzers

**For Visual Studio:**
1. Install the **Visual Studio Tools for Unity** extension
2. Enable Unity-specific IntelliSense and debugging
3. Go to `Tools > Options > Text Editor > C# > Code Style` to configure coding standards

**For JetBrains Rider:**
1. Install the **Unity Support** plugin (usually pre-installed)
2. Enable Unity-specific inspections in `Settings > Editor > Inspections > Unity`
3. Configure code style in `Settings > Editor > Code Style > C#`

**For Visual Studio Code:**
1. Install the **C# extension** by Microsoft
2. Install the **Unity Code Snippets** extension
3. Install the **Unity Tools** extension

#### 2. Unity Built-in Code Analysis

Unity includes built-in code analysis features:

1. **Console Window**: Shows compiler warnings and errors
2. **Inspector Warnings**: Shows serialization and component issues
3. **Script Compilation**: Provides immediate feedback on syntax errors

#### 3. Custom Code Analysis Tools (Optional)

If you want additional static analysis, consider:

1. **SonarQube** for Unity projects
2. **CodeMaid** extension for Visual Studio
3. **ReSharper** (paid) for advanced code analysis

### Configuration Steps

1. **Enable Script Debugging**:
   - Go to `Edit > Project Settings > Player`
   - Enable "Script Debugging" in Development builds

2. **Configure Code Formatting**:
   - Use the provided `CodingStandards.cs` as a reference
   - Set up your IDE to follow these conventions

3. **Set Up EditorConfig** (Already created):
   - The `.editorconfig` file in the project root defines coding standards
   - Ensure your IDE respects these settings

## Development Tools Configuration

### Debug UI Setup

The project includes a `DebugUIPanel.cs` for runtime debugging:

```csharp
// Access debug UI at runtime
DebugUIPanel.Instance.ShowPerformanceInfo();
DebugUIPanel.Instance.LogMessage("Debug info", LogLevel.Info);
```

### Performance Monitor Setup

The `PerformanceMonitor.cs` component provides:

1. **Real-time Performance Tracking**:
   - FPS monitoring
   - Memory usage tracking
   - Garbage collection analysis

2. **Performance Overlay**:
   - Enable `showPerformanceOverlay` in the inspector
   - View real-time metrics on screen

3. **Performance Logging**:
   - Automatic logging when performance drops
   - Integration with `GameLogger` system

### Usage Example

```csharp
// Add to any GameObject in your scene
var perfMonitor = gameObject.AddComponent<PerformanceMonitor>();
perfMonitor.enableProfiling = true;
perfMonitor.showPerformanceOverlay = true;
perfMonitor.minAcceptableFrameRate = 30f;
```

## Testing Framework Setup

### Unity Test Runner Configuration

1. **Open Test Runner**:
   - Go to `Window > General > Test Runner`
   - You'll see two tabs: EditMode and PlayMode

2. **Test Assembly Setup**:
   - Tests are located in `Assets/Tests/`
   - Assembly definition file (`Tests.asmdef`) is configured for testing
   - References Unity Testing Framework and UGS services

### Running Tests

1. **In Unity Editor**:
   - Open Test Runner window
   - Click "Run All" to execute all tests
   - Individual tests can be run by clicking on them

2. **From Command Line** (CI/CD):
   ```bash
   Unity -batchmode -quit -projectPath . -runTests -testResults results.xml
   ```

### Test Categories

- **UGS Integration Tests**: Test Unity Gaming Services connectivity
- **Configuration Tests**: Validate game configuration files
- **Performance Tests**: Ensure initialization times are reasonable
- **Error Handling Tests**: Verify graceful error handling

## Performance Monitoring

### Monitoring Components

1. **PerformanceMonitor**: Main performance tracking component
2. **GameLogger**: Centralized logging system
3. **Unity Profiler**: Built-in Unity profiling tools

### Key Metrics Tracked

- **Frame Rate**: Target 30+ FPS on mobile, 60+ FPS on PC
- **Memory Usage**: Monitor for memory leaks and excessive allocation
- **Garbage Collection**: Track GC spikes that cause frame drops
- **Network Performance**: Monitor UGS API call performance

### Performance Thresholds

```csharp
// Default thresholds (configurable in inspector)
minAcceptableFrameRate = 30f;          // FPS
maxAcceptableMemoryMB = 1024f;         // Memory in MB
maxAcceptableGCTimeMS = 10f;           // GC time in milliseconds
```

## UGS Services Configuration

### Required Services

1. **Unity Authentication**: Player authentication and identity
2. **Unity Lobbies**: Multiplayer lobby management
3. **Unity Netcode**: Network communication
4. **Unity Remote Config**: Runtime configuration management

### Configuration Files

- **Development**: `Assets/Resources/GameConfig_Development.asset`
- **Production**: `Assets/Resources/GameConfig_Production.asset`

### Environment Setup

```csharp
// Environment configuration in GameConfig
public enum Environment
{
    Development,
    Staging,
    Production
}
```

## Verification Checklist

### Week 1 Setup Verification

- [ ] **Unity 6 LTS installed and project created**
  - Verify Unity version: `2022.3.x LTS` or later
  - Project uses URP (Universal Render Pipeline)

- [ ] **Required packages installed**
  - Unity Gaming Services SDK
  - Unity Netcode for GameObjects
  - Unity Lobbies
  - Test Framework packages

- [ ] **UGS services configured**
  - Project linked to Unity Cloud Project
  - Services enabled in Unity Dashboard
  - Test authentication working

- [ ] **Repository setup**
  - Git LFS configured for Unity assets
  - `.gitignore` properly configured
  - Initial commit created

- [ ] **Development tools working**
  - Debug UI accessible at runtime
  - Performance monitoring active
  - Console shows no critical errors

- [ ] **Code standards in place**
  - `CodingStandards.cs` created and documented
  - IDE configured for Unity development
  - EditorConfig file active

- [ ] **Basic testing framework**
  - Test Runner accessible
  - Sample tests created and passing
  - CI/CD pipeline configured (may need Unity license)

### Testing Workflow

1. **Make a Code Change**:
   ```bash
   # Example: Modify a script
   git add Assets/Game/Scripts/
   git commit -m "Update: Added new feature"
   git push origin main
   ```

2. **Verify CI Pipeline**:
   - Check GitHub Actions (may fail without Unity license)
   - Review build logs for errors
   - Confirm tests execute

3. **Test UGS Connectivity**:
   - Run authentication test
   - Create and join a test lobby
   - Verify network connectivity

4. **Verify Debug Tools**:
   - Enable performance overlay
   - Check debug UI functionality
   - Review performance logs

## Troubleshooting

### Common Issues

#### 1. UGS Authentication Failures

**Symptoms**: Authentication timeouts, service unavailable errors

**Solutions**:
- Verify internet connection
- Check Unity Dashboard service status
- Ensure project is properly linked to Unity Cloud
- Verify API keys and environment configuration

#### 2. Performance Issues

**Symptoms**: Low FPS, high memory usage, frequent GC

**Solutions**:
- Check Performance Monitor logs
- Enable Unity Profiler for detailed analysis
- Review object pooling implementation
- Optimize texture and mesh assets

#### 3. Test Failures

**Symptoms**: Unit tests failing, timeout errors

**Solutions**:
- Check network connectivity for UGS tests
- Verify test environment configuration
- Review console for detailed error messages
- Ensure proper test data cleanup

#### 4. Build Issues

**Symptoms**: Compilation errors, missing references

**Solutions**:
- Update package dependencies
- Check assembly definition files
- Verify platform-specific settings
- Review build logs for specific errors

### Platform-Specific Notes

#### Windows
- Ensure Visual Studio Unity tools are installed
- Check Windows Defender exclusions for Unity project folder

#### macOS
- Verify Xcode command line tools are installed
- Check macOS security settings for Unity

#### Linux
- Ensure proper graphics drivers are installed
- Verify Unity Hub installation

### CI/CD Setup Status

#### GitHub Actions Status
- **Status**: Configured but may fail without Unity license
- **Requirements**: Unity Professional license for cloud builds
- **Workaround**: Use Unity Personal license with manual builds

#### Unity Cloud Build
- **Status**: Available as alternative to GitHub Actions
- **Advantages**: Includes Unity license, better Unity integration
- **Setup**: Configure through Unity Dashboard

### Unity License Requirements

For CI/CD builds, you need:

1. **Unity Professional License** (recommended)
   - Full CI/CD support
   - No usage restrictions
   - Commercial use allowed

2. **Unity Personal License** (limited)
   - Manual activation required
   - Usage restrictions apply
   - Free for qualifying projects

### Next Steps

1. **Obtain Unity License** for CI/CD
2. **Configure Cloud Build** or GitHub Actions properly
3. **Set up Production Environment** configurations
4. **Implement Network Architecture** for multiplayer
5. **Add Game-Specific Logic** and mechanics

---

## Additional Resources

- [Unity Gaming Services Documentation](https://docs.unity.com/ugs/)
- [Unity Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html)
- [Unity Performance Optimization](https://docs.unity3d.com/Manual/BestPracticeGuides.html)

---

**Last Updated**: Week 1 Setup
**Next Review**: Week 2 Development Phase
