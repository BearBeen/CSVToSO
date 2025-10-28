using System.IO;
using UnityEditor;
using UnityEngine;

namespace CSVToSO
{
    public class FolderPathAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        private const float BTN_WIDTH = 30;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [FolderPath] with a string.");
                return;
            }
            EditorGUI.BeginProperty(position, label, property);

            float textWidth = position.width - 5 - BTN_WIDTH;
            Rect textFieldRect = new Rect(position.x, position.y, textWidth, position.height);
            Rect buttonRect = new Rect(position.x + textWidth + 5, position.y, BTN_WIDTH, position.height);
            property.stringValue = EditorGUI.TextField(textFieldRect, label, property.stringValue);

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Folder")))
            {
                string projectDir = Path.GetDirectoryName(Application.dataPath);
                string currentPath = property.stringValue;
                string startFolder = string.IsNullOrEmpty(currentPath) ? "Assets" : currentPath;
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", startFolder, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    selectedPath = Path.GetRelativePath(projectDir, selectedPath);
                    property.stringValue = selectedPath.NormalizeUnityPath() ;
                    property.serializedObject.ApplyModifiedProperties();
                }
                GUIUtility.ExitGUI();
            }

            EditorGUI.EndProperty();
        }
    }
}