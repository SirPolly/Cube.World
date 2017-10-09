using System.IO;
using UnityEditor;
using UnityEngine;

public static class TerrainSplatmap {
    static string _lastFolder = "";

    [MenuItem("UnityCore/Terrain/Export Splatmap...")]
    static void CreatePNG() {
        var texture1 = Selection.activeObject as Texture2D; 
        if (texture1 == null) {
            EditorUtility.DisplayDialog("Select A Splatmap", "You need to select the terrain texture first (a child of your terrain asset).", "OK"); 
            return; 
        }

        var path = EditorUtility.SaveFilePanelInProject("Save Splatmap as PNG", "splatmap-to-png", "png", "Select the location to save the new PNG splatmap.", _lastFolder);
        if (path.Length == 0)
            return;

        _lastFolder = Path.GetDirectoryName(path);

        var texture = Texture2D.Instantiate(texture1);
        var textureColors = texture.GetPixels();
        for (var i = 0; i < textureColors.Length; ++i) {
            textureColors[i].a = 1;
        }

        texture.SetPixels(textureColors);
        texture.Apply();

        if (texture.format != TextureFormat.ARGB32 && texture.format != TextureFormat.RGB24) {
            var newTexture = new Texture2D(texture.width, texture.height);
            newTexture.SetPixels(texture.GetPixels(0), 0);
            texture = newTexture;
        }

        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
    }

    [MenuItem("UnityCore/Terrain/Import Splatmap...")]
    static void Replace() {
        var rawTexture = Selection.activeObject as Texture2D; 
        if (rawTexture == null) { 
            EditorUtility.DisplayDialog("Select The Splatmap", "You need to select the terrain's splatmap from your asset folder first (a child of your terrain asset).", "OK"); 
            return; 
        }

        //
        var path = EditorUtility.OpenFilePanel("Select the PNG to copy over your splatmap.", _lastFolder, "png");
        if (path.Length == 0)
            return;

        _lastFolder = Path.GetDirectoryName(path);

        var bytes = File.ReadAllBytes(path);

        var bakedTexture = new Texture2D(2, 2);
        bakedTexture.LoadImage(bytes);

        if (bakedTexture.format != TextureFormat.ARGB32 && rawTexture.format != TextureFormat.RGB24) {
            var newTexture = new Texture2D(bakedTexture.width, bakedTexture.height);
            newTexture.SetPixels(bakedTexture.GetPixels(0),0);
            bakedTexture = newTexture;
            bakedTexture.Apply();
        }
		
        var textureColors = bakedTexture.GetPixels();
        for (var i = 0; i < textureColors.Length; ++i) {
            var c = textureColors[i];
            if (c.r != 0f || c.g != 0f || c.b != 0f)
                textureColors[i].a = 1f - ( c.r + c.g + c.b );
            else
                textureColors[i] = new Color(0f, 0f, 0f, 0f);
        }
	 
        bakedTexture.SetPixels(textureColors);
        bakedTexture.Apply();

        //
        var bakedData = bakedTexture.EncodeToPNG();
        rawTexture.LoadImage(bakedData);

        AssetDatabase.Refresh();
    }
}