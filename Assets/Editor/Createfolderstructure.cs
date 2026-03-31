using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Auto-creates the complete folder structure for a Unity horror game project.
/// Place this script in Assets/Editor/ folder.
/// Usage: Unity Menu → Tools → Create Project Folders
/// 
/// Safe to run multiple times - only creates missing folders, won't overwrite existing ones.
/// </summary>
public class CreateFolderStructure
{
    // Define all folders that should exist in the project
    private static readonly List<string> FolderStructure = new List<string>
    {
        // Models
        "Assets/Models",
        "Assets/Models/Environment",
        "Assets/Models/Environment/Walls",
        "Assets/Models/Environment/Floors",
        "Assets/Models/Environment/Props",
        "Assets/Models/Environment/Doors",
        "Assets/Models/Characters",
        "Assets/Models/Characters/Enemies",
        "Assets/Models/Characters/NPCs",
        "Assets/Models/Temp",

        // Textures
        "Assets/Textures",
        "Assets/Textures/Albedo",
        "Assets/Textures/Normal",
        "Assets/Textures/Roughness",
        "Assets/Textures/Metallic",
        "Assets/Textures/Environment",

        // Materials
        "Assets/Materials",
        "Assets/Materials/Stone",
        "Assets/Materials/Metal",
        "Assets/Materials/Wood",
        "Assets/Materials/Organic",

        // Prefabs
        "Assets/Prefabs",
        "Assets/Prefabs/Environment",
        "Assets/Prefabs/Interactive",
        "Assets/Prefabs/Enemies",
        "Assets/Prefabs/Props",

        // Scenes
        "Assets/Scenes",
        "Assets/Scenes/Levels",
        "Assets/Scenes/UI",
        "Assets/Scenes/Testing",

        // Scripts
        "Assets/Scripts",
        "Assets/Scripts/Player",
        "Assets/Scripts/Enemy",
        "Assets/Scripts/Environment",
        "Assets/Scripts/UI",
        "Assets/Scripts/Managers",
        "Assets/Scripts/Utilities",

        // Audio
        "Assets/Audio",
        "Assets/Audio/SFX",
        "Assets/Audio/Music",
        "Assets/Audio/Ambient",

        // Additional folders
        "Assets/Animations",
        "Assets/Shaders",
        "Assets/Editor"
    };

    /// <summary>
    /// Main method - creates all folders. Accessible from Unity menu.
    /// </summary>
    [MenuItem("Tools/Create Project Folders")]
    public static void CreateAllFolders()
    {
        int createdCount = 0;
        int existingCount = 0;

        Debug.Log("🎮 Starting Horror Game Project folder creation...");

        foreach (string folder in FolderStructure)
        {
            if (CreateFolder(folder))
            {
                createdCount++;
                Debug.Log($"✓ Created: {folder}");
            }
            else
            {
                existingCount++;
                Debug.Log($"→ Already exists: {folder}");
            }
        }

        // Refresh the Asset database so new folders appear in Project panel
        AssetDatabase.Refresh();

        Debug.Log($"\n✅ COMPLETE!");
        Debug.Log($"Created: {createdCount} new folders");
        Debug.Log($"Already existed: {existingCount} folders");
        Debug.Log($"Total structure: {FolderStructure.Count} folders");
    }

    /// <summary>
    /// Creates a single folder if it doesn't already exist.
    /// Returns true if folder was created, false if it already existed.
    /// </summary>
    private static bool CreateFolder(string folderPath)
    {
        // Check if folder already exists
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return false; // Folder already exists
        }

        // Split path to get parent directory
        string parentPath = Path.GetDirectoryName(folderPath);
        string folderName = Path.GetFileName(folderPath);

        // Create the folder
        AssetDatabase.CreateFolder(parentPath, folderName);
        return true; // Folder was created
    }

    /// <summary>
    /// Alternative method: Get list of all expected folders (useful for validation)
    /// </summary>
    public static List<string> GetExpectedFolderStructure()
    {
        return new List<string>(FolderStructure);
    }

    /// <summary>
    /// Validate current project structure
    /// </summary>
    [MenuItem("Tools/Validate Project Folders")]
    public static void ValidateFolders()
    {
        Debug.Log("📋 Validating Horror Game Project folder structure...\n");

        int validCount = 0;
        int missingCount = 0;

        foreach (string folder in FolderStructure)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                validCount++;
            }
            else
            {
                missingCount++;
                Debug.LogWarning($"❌ Missing: {folder}");
            }
        }

        Debug.Log($"\n✅ Valid folders: {validCount}");
        Debug.Log($"⚠️  Missing folders: {missingCount}");
        Debug.Log($"Total expected: {FolderStructure.Count}");

        if (missingCount > 0)
        {
            Debug.Log("\n💡 Run Tools → Create Project Folders to auto-create missing ones!");
        }
    }
}