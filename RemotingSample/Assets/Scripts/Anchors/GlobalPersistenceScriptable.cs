using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace VRLab.AnchorStore
{
    [CreateAssetMenu(fileName = "GlobalPersistenceScriptable", menuName = "GlobalPersistenceScriptable", order = 0)]
    public class GlobalPersistanceScriptable : ScriptableObject
    {
        [InspectorButton("RebuildAnchorScriptable")]
        public bool RebuildDatabaseFromLatest = false;

        [Header("Current Tags and Anchors")]
        [SerializeField]
        public List<string> CurrentProjectTags = new();
        //[SerializeField]
        //public List<(string, string)> AnchorTagsAndNamesInStoreForThisProject = new();
        [SerializeField]
        [ShowOnly]public List<string> RawAnchorNamesInStoreOnLastUpdate = new();
        [SerializeField]
        [ShowOnly]public List<string> AnchorTagsInStore = new();

        [Header("Anchor Data")]
        [SerializeField]
        public List<PersistantAnchorData> AvailableAnchorDataForProject = new();

        [SerializeField]
        private List<string> latestAnchorStorePersistedNames = new();

        [Header("Added and Removed Anchors")]
        [SerializeField]
        [ShowOnly]public List<string> RawAnchorNamesRemovedFromStore = new();
        [SerializeField]
        [ShowOnly]public List<string> RawAnchorNamesAddedFromStore = new();
        [Header("Added and Removed Anchors")]
        [SerializeField]
        [ShowOnly]public List<string> AnchorTagsRemovedFromStore = new();
        [SerializeField]
        [ShowOnly]public List<string> AnchorTagsAddedFromStore = new();


        public void RebuildAnchorScriptable()
        {
            LogNewAnchorEvent("Rebuild", AnchorLogType.Cleared);
            ClearAllExceptLatest();
            UpdateFromAnchorStore(latestAnchorStorePersistedNames);
        }

        private void ClearAllExceptLatest()
        {
            RawAnchorNamesAddedFromStore = new();
            RawAnchorNamesRemovedFromStore = new();
            RawAnchorNamesInStoreOnLastUpdate = new();
            AnchorTagsInStore = new();
            AnchorTagsAddedFromStore = new();
            AnchorTagsRemovedFromStore = new();
        }

        public void UpdateTagsFromStore()
        {

            List<string> latestAnchorTagsInStore = new();
            // Get list of tags
            foreach (var anchorName in RawAnchorNamesInStoreOnLastUpdate)
            {
                var anchorTag = anchorName.GetTagFromRawName();
                if (anchorTag != "none")
                {
                    if (!latestAnchorTagsInStore.Contains(anchorTag))
                    {
                        latestAnchorTagsInStore.Add(anchorTag);
                    }
                }
            }
            // Check for removed tags
            //foreach (string existingAnchorTag in AnchorTagsInStore)
            //{
            //    if (!latestAnchorStorePersistedNames.Contains(existingAnchorTag))
            //    {
            //        // This anchor name has been removed
            //        LogNewAnchorEvent(existingAnchorTag, AnchorLogType.RemovedTag);
            //        AnchorTagsInStore.Remove(existingAnchorTag);
            //        AnchorTagsRemovedFromStore.Add(existingAnchorTag);
            //    } 
            //}
            // Check for new tags
            foreach (string tagFromStore in latestAnchorTagsInStore)
            {
                if (!AnchorTagsInStore.Contains(tagFromStore))
                {
                    // This anchor is new
                    LogNewAnchorEvent(tagFromStore, AnchorLogType.AddedTag);
                    AnchorTagsInStore.Add(tagFromStore);
                    AnchorTagsAddedFromStore.Add(tagFromStore);
                }
            }
        }

        internal void UpdateFromAnchorStore(IReadOnlyList<string> StorePersistantAnchorNames, List<string> newCategoryName = null)
        {
            if (newCategoryName != null)
            {
                CurrentProjectTags = newCategoryName;
            }

            latestAnchorStorePersistedNames = new(StorePersistantAnchorNames);

            UpdateRawNamesFromStore();
            UpdateTagsFromStore();
            UpdateProjectAnchorsFromStore();
            
            SaveYourself();
        }

        private void UpdateProjectAnchorsFromStore()
        {
            //AnchorTagsAndNamesInStoreForThisProject.Clear();
            //foreach (var anchorName in RawAnchorNamesInStoreOnLastUpdate)
            //{
            //    if (CurrentProjectTags.Contains(anchorName.GetTagFromRawName()))
            //    {
            //        AnchorTagsAndNamesInStoreForThisProject.Add((anchorName.GetTagFromRawName(), anchorName.GetNameFromRawName()));
            //    }
            //}

            //if (Application.isPlaying)
            //{
            //    AvailableAnchorDataForProject.Clear();

            //    foreach (var anchorName in AnchorTagsAndNamesInStoreForThisProject)
            //    {
            //        var anchorData = new PersistantAnchorData(anchorName);
            //        AvailableAnchorDataForProject.Add(anchorData);
            //    }
            //}

        }

        private void UpdateRawNamesFromStore()
        {
            RawAnchorNamesAddedFromStore = new();
            RawAnchorNamesRemovedFromStore = new();

            //foreach (string existingAnchorName in RawAnchorNamesInStoreOnLastUpdate)
            //{
            //    if (!latestAnchorStorePersistedNames.Contains(existingAnchorName))
            //    {
            //        // This anchor name has been removed
            //        LogNewAnchorEvent(existingAnchorName, AnchorLogType.RemovedAnchor);
            //        RawAnchorNamesInStoreOnLastUpdate.Remove(existingAnchorName);
            //        RawAnchorNamesRemovedFromStore.Add(existingAnchorName);
            //    } 
            //}
            foreach (string anchorNameFromStore in latestAnchorStorePersistedNames)
            {
                if (!RawAnchorNamesInStoreOnLastUpdate.Contains(anchorNameFromStore))
                {
                    // This anchor is new
                    LogNewAnchorEvent(anchorNameFromStore, AnchorLogType.AddedAnchor);
                    RawAnchorNamesInStoreOnLastUpdate.Add(anchorNameFromStore);
                    RawAnchorNamesAddedFromStore.Add(anchorNameFromStore);
                }
            }
        }

        internal void SaveYourself()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [Header("Log")]
        [SerializeField]
        [ShowOnly]
        public List<string> LogString;

        internal void LogNewAnchorEvent(string RawAnchorName, AnchorLogType logType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{DateTime.Now.ToShortDateString()}");
            sb.Append(" : ");
            sb.Append($"{RawAnchorName}");
            sb.Append(" - ");
            sb.Append($"{logType}");
            LogString.Add(sb.ToString());
        }
    }

    internal enum AnchorLogType
    {
        AddedAnchor,
        RemovedAnchor,
        AddedTag,
        RemovedTag,
        Cleared
    }

    static class AnchorStringExtensions
    {
        public static string GetTagFromRawName(this string anchorName)
        {
            var anchorNameSplit = anchorName.Split("/");
            return anchorNameSplit.Length > 1 ? anchorNameSplit[0] : "none";
        }

        public static string GetNameFromRawName(this string anchorName)
        {
            var anchorNameSplit = anchorName.Split("/");
            return anchorNameSplit.Length > 1 ? anchorNameSplit[1] : anchorName;
        }
    }
}
