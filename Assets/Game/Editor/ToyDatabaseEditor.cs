using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ToyDatabase))]
public class ToyDatabaseEditor : Editor
{
    private string errorMessage = null;
    private MessageType messageType = MessageType.Info;

    public override void OnInspectorGUI()
    {
        ToyDatabase db = (ToyDatabase)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== Database Validation ===", EditorStyles.boldLabel);

        // Manual validate button
        if (GUILayout.Button("Validate Database"))
        {
            RunValidation(db);
            Repaint();  // FORCE inspector update
        }
        // DRAW HELPBOX
        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, messageType);
        }
    }

    private void RunValidation(ToyDatabase db)
    {
        HashSet<int> usedIds = new HashSet<int>();

        errorMessage = null;
        messageType = MessageType.Info;

        foreach (var type in db.variationTypes)
        {
            if (type == null) continue;

            foreach (var toy in type.items)
            {
                if (toy == null) continue;

                //  Check negative ID
                if (toy.id < 0)
                {
                    errorMessage =
                        $"Invalid ToyItem ID!\n" +
                        $"ID must be >= 0\n" +
                        $"Toy: {toy.name}\n" +
                        $"Type: {type.displayName}";

                    messageType = MessageType.Error;
                    return; // stop immediately for performance
                }

                //  Check duplicate ID
                if (!usedIds.Add(toy.id))
                {
                    errorMessage =
                        $"Duplicate ToyItem ID found!\n" +
                        $"ID = {toy.id}\n" +
                        $"Toy: {toy.name}\n" +
                        $"Type: {type.displayName}";

                    messageType = MessageType.Error;
                    return; // stop immediately
                }
            }
        }

        // All checks passed
        errorMessage = "All ToyItem IDs are valid and unique.";
        messageType = MessageType.Info;
    }
}
