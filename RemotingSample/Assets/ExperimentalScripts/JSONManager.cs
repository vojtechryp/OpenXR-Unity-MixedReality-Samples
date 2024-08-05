using UnityEngine;
namespace Vojta.Experiment
{
    public static class JSONManager
    {
        public static bool SaveSessionToJson(string participantID, Session sessionData)
        {
            string jsonData = JsonUtility.ToJson(sessionData, true);
            System.IO.File.WriteAllText(Application.dataPath + $"/Results/{participantID}_results.json", jsonData);

            return true;
        }

        public static Session LoadSessionFromJson(string jsonData)
        {
            Session session = JsonUtility.FromJson<Session>(jsonData);
            return session;
        }
    }
}