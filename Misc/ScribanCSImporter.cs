using UnityEditor;
using UnityEditor.AssetImporters;

using UnityEngine;

namespace CSVToSO
{
    [ScriptedImporter(1, ".sbncs")]
    public class ScribanCSImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            TextAsset sbncs = new TextAsset(System.IO.File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("main", sbncs);
            ctx.SetMainObject(sbncs);
        }

        [MenuItem(
            itemName: "Assets/Create/CSVToSO/C# Template",
            isValidateFunction: false,
            priority: 50)]
        public static void NewScribanCSTemplate()
        {
            ProjectWindowUtil.CreateAssetWithContent(
            "NewCSTemplate.sbncs",
            string.Empty);
        }
    }
}