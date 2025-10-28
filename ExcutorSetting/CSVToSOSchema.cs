#if UNITY_EDITOR

using System.Text.RegularExpressions;

using CsvHelper;

using UnityEditor;

using UnityEngine;

namespace CSVToSO
{
    [CreateAssetMenu(fileName = "CSVToSOSchema", menuName = "CSVToSO/CSVToSOSchema")]
    public partial class CSVToSOSchema : ScriptableObject
    {
        [SerializeField] protected string _match;
        [SerializeField] protected AbsConfigParser _configParser;
        [SerializeField] protected CSVToSOExecutor _excutor;

        protected Regex _fileNameRegex;

        protected Regex fileNameRegex
        {
            get
            {
                if (_fileNameRegex == null)
                {
                    _fileNameRegex = new Regex(_match);
                }
                return _fileNameRegex;
            }
        }

        public bool isSupportGenCS => _configParser is ISupportGenCS;
        public bool isSupportGenSO => _configParser is ISupportGenSO;

        public bool IsTemplate(string fileName)
        {
            return fileNameRegex.IsMatch(fileName);
        }

        public void GenAllCS()
        {
            if (_configParser is not ISupportGenCS genCS)
            {
                Debug.LogError("This Schema does not support generate .cs script file !!!");
                return;
            }
            _excutor.GenAllCS(IsTemplate, genCS);
        }

        public void GenAllSO()
        {
            if (_configParser is not ISupportGenSO genSO)
            {
                Debug.LogError("This Schema does not support generate ScriptableObject .asset file !!!");
                return;
            }
            _excutor.GenAllSO(IsTemplate, genSO);
        }
    }

    [CustomEditor(typeof(CSVToSOSchema))]
    public class CSVToSOSchemaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CSVToSOSchema csvToSOSchema = serializedObject.targetObject as CSVToSOSchema;
            if (!csvToSOSchema) return;
            using var _1 = new EditorGUI.DisabledScope(EditorApplication.isCompiling);
            using var _2 = new EditorGUILayout.HorizontalScope();
            if (csvToSOSchema.isSupportGenCS)
            {
                if (GUILayout.Button(new GUIContent("GenAllCS", "Generate all the.cs file by all the matched name csv file")))
                {
                    (serializedObject.targetObject as CSVToSOSchema).GenAllCS();
                }
            }
            if (csvToSOSchema.isSupportGenSO)
            {
                if (GUILayout.Button(new GUIContent("GenAllSO", "Generate all the ScriptableObject assets by all the matched name csv file")))
                {
                    (serializedObject.targetObject as CSVToSOSchema).GenAllSO();
                }
            }
        }
    }
}
#endif