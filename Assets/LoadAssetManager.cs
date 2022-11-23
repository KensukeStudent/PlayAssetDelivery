using Google.Play.AssetDelivery;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class LoadAssetManager : MonoBehaviour
{
    AssetBundle assetBundle = null;

    [SerializeField]
    Image image = null;

    private void Start()
    {
        StartCoroutine(LoadAssetBundleAsync("CustomOnDemand2"));
    }

    private IEnumerator LoadAssetBundleAsync(string assetBundleName)
    {
        var bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(assetBundleName);

        while (bundleRequest == null)
        {
            yield return null;
        }
        assetBundle = bundleRequest.AssetBundle;
        yield return Addressables.LoadAssetsAsync<Sprite>("", (obj) =>
        {
            if (obj != null)
            {
                image.sprite = obj;
            }
        }).WaitForCompletion();
    }
}
