#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class AutoEnumUpdator : MonoBehaviour
{
    public static void GenerateEnumFromStringArray(string originalFilePath, string[] enumNames)
    {
        string absolutePath = Path.GetFullPath(originalFilePath);
        if (!File.Exists(absolutePath))
        {
            Debug.LogError($"Cannot founded {absolutePath}");
            return;
        }


        // Build enum file contents
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// Auto-generated. Do not edit manually.");
        sb.AppendLine($"public enum {Path.GetFileNameWithoutExtension(absolutePath)}");
        sb.AppendLine("{");
        foreach (var raw in enumNames)
        {
            string safe = Sanitize(raw);
            sb.AppendLine($"    {safe},");
        }
        sb.AppendLine("}");
        // Write file
        File.WriteAllText(absolutePath, sb.ToString());

        // Refresh Unity to import the new script
        AssetDatabase.Refresh();

        Debug.Log($"Generated enum at: {absolutePath}");
    }

    private static string Sanitize(string raw)
    {
        // Replace invalid chars for C# identifiers
        var s = new string(raw.Replace(" ", "_")
                              .Replace("-", "_")
                              .ToCharArray());
        if (char.IsDigit(s[0]))
            s = "_" + s;
        return s;
    }
}
#endif