using System.Threading;
using Cysharp.Threading.Tasks;
using Google.Play.AssetDelivery;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BundleLoader : MonoBehaviour
{
    /// <summary>
    /// �i�����̃e�L�X�g
    /// </summary>
    [SerializeField] private Text _progressText;

    /// <summary>
    /// �V�[���J�ڃ{�^��
    /// </summary>
    [SerializeField] private Button _sceneChangeButton;

    /// <summary>
    /// �ǂݍ��񂾃A�Z�b�g�o���h�����̃V�[���̃p�X�Q�p�ϐ�
    /// </summary>
    private string[] _scenePaths;

    /// <summary>
    /// �ǂݍ��񂾃A�Z�b�g�o���h���p�ϐ�
    /// </summary>
    private AssetBundle _assetBundle;

    private void Start()
    {
        _sceneChangeButton.gameObject.SetActive(false);
        _sceneChangeButton.onClick.AddListener(onClick);
        LoadAssetBundleAsync("movie", this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void OnDestroy()
    {
        _sceneChangeButton.onClick.RemoveListener(onClick);
    }

    /// <summary>
    /// �A�Z�b�g�o���h�������[�h����
    /// </summary>
    /// <param name="assetBundleName">�A�Z�b�g�o���h���̖��O</param>
    /// <param name="ct">�L�����Z���g�[�N��</param>
    /// <returns></returns>
    private async UniTask LoadAssetBundleAsync(string assetBundleName, CancellationToken ct)
    {
        var bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(assetBundleName);


        //Wi-Fi�̐ڑ����`�F�b�N
        await checkWifiAsync(bundleRequest, ct);

        while (!bundleRequest.IsDone)
        {
            //Wi-Fi�̐ڑ����`�F�b�N
            await checkWifiAsync(bundleRequest, ct);

            //�i����
            _progressText.text = bundleRequest.DownloadProgress * 100f + "%";

            await UniTask.Yield(ct);
        }

        if (bundleRequest.Error != AssetDeliveryErrorCode.NoError)
        {
            //��������̃G���[�͂����ŏE��
            await UniTask.Yield(ct);
        }

        //�{�^���\��
        _sceneChangeButton.gameObject.SetActive(true);

        //DL�ɐ���������A�Z�b�g�o���h���̒��g���Q�Ƃ���
        _assetBundle = bundleRequest.AssetBundle;
        _scenePaths = _assetBundle.GetAllScenePaths();
    }

    /// <summary>
    /// Wi-Fi�̐ڑ����`�F�b�N����
    /// </summary>
    /// <param name="playAssetBundleRequest">���N�G�X�g�����A�Z�b�g�o���h��</param>
    /// <param name="ct">�L�����Z���g�[�N��</param>
    private async UniTask checkWifiAsync(PlayAssetBundleRequest playAssetBundleRequest, CancellationToken ct)
    {
        //�A�Z�b�g�o���h����150MB�ȏ�̏ꍇ�AWi-Fi�̐ڑ����O��ƂȂ�@�ڑ��������ꍇ�̓��[�U�[��Wi-Fi������DL���Ă��ǂ����m�F����B
        //150MB�����Ă��邩�ǂ����͎����Ŕ���
        if (playAssetBundleRequest.Status == AssetDeliveryStatus.WaitingForWifi)
        {
            //Wi-Fi������DL���Ă��ǂ����m�F
            var userConfirmationOperation = PlayAssetDelivery.ShowCellularDataConfirmation();

            //���[�U�[�̓��͂�҂�
            await userConfirmationOperation.ToUniTask(cancellationToken: ct).AttachExternalCancellation(ct);

            if ((userConfirmationOperation.Error != AssetDeliveryErrorCode.NoError) ||
                (userConfirmationOperation.GetResult() != ConfirmationDialogResult.Accepted))
            {
                // ���[�U�[�����ۂ������̏���
            }

            // Wi-Fi�ɐڑ����ꂽ�A��������"���ڑ��ł���"�����F�����̂�҂�
            await UniTask.WaitWhile(() => playAssetBundleRequest.Status != AssetDeliveryStatus.WaitingForWifi, cancellationToken: ct);
        }
    }

    ///�{�^���������̏���
    private void onClick()
    {
        SceneManager.LoadScene(_scenePaths[0]);
    }
}