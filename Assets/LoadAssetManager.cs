using Google.Play.AssetDelivery;
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

    [SerializeField]
    Image load_image = null;

    private void Start()
    {
        Debug.Log("assetPackNames : " + JsonUtility.ToJson(AndroidAssetPacks.GetCoreUnityAssetPackNames()));

        SetLoad("installtime", installtime_image);

        SetLoad("ondemand", ondemand_image);

        LoadAssetPack<Sprite>("CustomOnDemand", "Assets/AddressableAssets/adv/", "10MB_1.JPG", (asset) =>
        {
            load_image.sprite = asset;
        });
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

    private void LoadAssetPack<T>(string assetPackName, string assetBundlePath, string assetName, Action<T> callBack = null) where T : UnityEngine.Object
    {
        // アセットパック名
        var packRequest = PlayAssetDelivery.RetrieveAssetPackAsync(assetPackName);
        if (packRequest == null)
        {
            Debug.Log(string.Format("ロードしたアセットパックは存在しません。アセットパック名 : {0}", assetPackName));
            return;
        }

        packRequest.Completed += request =>
        {
            if (request.Status == AssetDeliveryStatus.Loaded ||
            request.Status == AssetDeliveryStatus.Available)
            {
                // 
                var bundleCreateRequest = packRequest.LoadAssetBundleAsync(assetBundlePath);
                if (bundleCreateRequest == null)
                {
                    Debug.Log(string.Format("ロードしたアセットバンドルパスは存在しません。パス : {0}", assetBundlePath));
                    return;
                }

                bundleCreateRequest.completed += _ =>
                {
                    var asset = bundleCreateRequest.assetBundle.LoadAsset<T>(assetName);
                    if (asset == null)
                    {
                        Debug.Log(string.Format("ロードしたアセットバンドルは存在しません。アセットバンドル名 : {0}", assetName));
                    }
                    callBack?.Invoke(asset);
                };
            };
        };
    }
}
