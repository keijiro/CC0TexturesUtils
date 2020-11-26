using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace CC0Textures {

static class MetadataEditor
{
    [MenuItem("Assets/CC0 Textures/Fix Normal Map")]
    static void FixNormalMap()
    {
        var sources = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        var assets = new List<Object>();

        foreach (var source in sources)
        {
            // File paths
            var inputPath = AssetDatabase.GetAssetPath(source);
            var outputPath = GetOutputTexturePath(inputPath);

            // Change the importer settings for the input texture.
            // Format -> Color texture, Readable -> yes
            var inputImporter = (TextureImporter)AssetImporter.GetAtPath(inputPath);
            var wasReadable = inputImporter.isReadable;
            inputImporter.textureType = TextureImporterType.Default;
            inputImporter.isReadable = true;

            // Reimport the input texture to apply the changes.
            AssetDatabase.ImportAsset(inputPath);

            // Fix and output
            File.WriteAllBytes(outputPath, FlipNormals(source));
            AssetDatabase.ImportAsset(outputPath);

            // Change the importer settings for the output texture.
            // Format -> Normal map
            var outputImporter = (TextureImporter)AssetImporter.GetAtPath(outputPath);
            outputImporter.textureType = TextureImporterType.NormalMap;

            // Reimport the output texture and add it to the asset list.
            AssetDatabase.ImportAsset(outputPath);
            assets.Add(AssetDatabase.LoadMainAssetAtPath(outputPath));

            // Recover the original input settings.
            inputImporter.textureType = TextureImporterType.NormalMap;
            inputImporter.isReadable = wasReadable;
            AssetDatabase.ImportAsset(inputPath);
        }

        EditorUtility.FocusProjectWindow();
        Selection.objects = assets.ToArray();
    }

    [MenuItem("Assets/CC0 Textures/Fix Normal Map", true)]
    static bool ValidateFixNormalMap()
      => Selection.GetFiltered<Texture2D>(SelectionMode.Assets).Length > 0;

    static string GetOutputTexturePath(string path)
      => Path.Combine(Path.GetDirectoryName(path),
                      Path.GetFileNameWithoutExtension(path) + "_fix.png");

    static byte [] FlipNormals(Texture2D sourceTexture)
    {
        var source = sourceTexture.GetPixels32();

        var width = (uint)sourceTexture.width;
        var height = (uint)sourceTexture.height;

        var buffer = new byte[sourceTexture.width * sourceTexture.height * 3];
        var offs = 0;
        for (var i = 0; i < source.Length; i++)
        {
            var p = source[i];
            buffer[offs++] = p.r;
            buffer[offs++] = (byte)(255 - p.g);
            buffer[offs++] = p.b;
        }

        return ImageConversion.EncodeArrayToPNG
          (buffer, GraphicsFormat.R8G8B8_UNorm, width, height, width * 3);
    }
}

} // namespace CC0Textures
