using UnityEditor;

public class BundleBuilder : Editor
{
    [MenuItem("Assets/ Build AssetBundle")]
    static void buildAsset()
    {
        BuildPipeline.BuildAssetBundles(@"C:\Users\SteveP1\Desktop\File2", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
    }
}