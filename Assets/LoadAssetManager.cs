// using Google.Play.AssetDelivery;
//using Google.Android.AppBundle.Editor;
//using Google.Android.AppBundle.Editor.AssetPacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;

public class LoadAssetManager : MonoBehaviour
{
    [SerializeField]
    Image installtime_image = null;

    [SerializeField]
    Image ondemand_image = null;

    private void Start()
    {
        Debug.Log("assetPackNames : " + JsonUtility.ToJson(AndroidAssetPacks.GetCoreUnityAssetPackNames()));

        SetLoad("installtime", installtime_image);

        SetLoad("ondemand", ondemand_image);
    }

    private void SetLoad(string address, Image _image)
    {
        Addressables.LoadAssetAsync<Sprite>(address).Completed += asset =>
        {
            if (asset.Result == null)
            {
                Debug.Log(string.Format("address : {0}は読み込まれませんでした", address));
            }

            _image.sprite = asset.Result;
            Debug.Log(string.Format("address : {0} load complete!", address));
        };
    }

    // // 異なる場合は別途指定
    // private void LoadAsset<T>(string assetPackName, string assetBundlePath, string assetName, Action<T> callBack = null) where T : UnityEngine.Object
    // {
    //     var packRequest = PlayAssetDelivery.RetrieveAssetPackAsync(assetPackName);
    //     packRequest.Completed += request =>
    //     {
    //         if (request.Status == AssetDeliveryStatus.Loaded ||
    //         request.Status == AssetDeliveryStatus.Available)
    //         {
    //             var bundleCreateRequest = packRequest.LoadAssetBundleAsync(assetBundlePath);
    //             bundleCreateRequest.completed += _ =>
    //             {
    //                 var asset = bundleCreateRequest.assetBundle.LoadAsset<T>(assetName);
    //                 callBack?.Invoke(asset);
    //             };
    //         };
    //     };
    // }

    //private static void AssetDeliverySettings()
    //{
    //    var assetPackConfig = new AssetPackConfig();
    //    // アセットパックに含めるディレクトリ
    //    assetPackConfig.AddAssetsFolder("AssetPackName", "相対パス", AssetPackDeliveryMode.InstallTime);
    //    AssetPackConfigSerializer.SaveConfig(assetPackConfig);
    //}

    // private static void BuildAndroidAppBundle()
    // {
    //     var assetPackConfig = AssetPackConfigSerializer.LoadConfig();
    //     // 良しなに設定する
    //     var options = new BuildPlayerOptions()
    //     {
    //         locationPathName = "application.aab",
    //         target = BuildTarget.Android,
    //         targetGroup = BuildTargetGroup.Android
    //     };
    //     Bundletool.BuildBundle(options, assetPackConfig);
    // }
}
