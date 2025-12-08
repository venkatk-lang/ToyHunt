using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelType))]
public class LevelTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelType level = (LevelType)target;

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        // Recalculate whenever anything changes
        if (EditorGUI.EndChangeCheck())
        {
            level.RecalculateCounts();
            EditorUtility.SetDirty(level);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== Level Validation ===", EditorStyles.boldLabel);

        // Show calculated values
        EditorGUILayout.LabelField("Total Grid Slots:", level.totalGridSlots.ToString());
        EditorGUILayout.LabelField("Total Unique Items in Database:", level.totalAvailableItems.ToString());

        // Validation feedback
        if (level.toyDatabase == null)
        {
            EditorGUILayout.HelpBox("Assign a ToyDatabase.", MessageType.Warning);
        }
        else if (level.totalAvailableItems < level.totalGridSlots)
        {
            EditorGUILayout.HelpBox(
                "Not enough unique ToyItems to fill the grid!\n" +
                $"Required: {level.totalGridSlots}, Available: {level.totalAvailableItems}",
                MessageType.Error
            );
        }
        else
        {
            EditorGUILayout.HelpBox("Level configuration is valid.", MessageType.Info);
        }
    }
}