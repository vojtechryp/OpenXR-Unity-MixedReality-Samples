using System.IO;
using UnityEditor;
using UnityEngine;
using Vojta.Experiment;

public static class JSONManager
{
    private static string GetJsonPath(string participantId)
    {
        return $"Assets/Results/Json/{participantId}.json";
    }

    private static string GetScriptablePath(string participantId)
    {
        return $"Assets/Results/Session/{participantId}.asset";
    }

    public static void SaveSessionToJson(string participantId, Session session)
    {
        string jsonPath = GetJsonPath(participantId);
        string jsonData = JsonUtility.ToJson(session, true);
        File.WriteAllText(jsonPath, jsonData);

        string scriptablePath = GetScriptablePath(participantId);
        AssetDatabase.SaveAssets();
        AssetDatabase.CreateAsset(session, scriptablePath);
        AssetDatabase.SaveAssets();

        Debug.Log("Session data saved to JSON and ScriptableObject.");
    }
}
