using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuBackgroundVideo : MonoBehaviour
{
    private RawImage targetImage;
    private Texture fallbackTexture;
    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;

    public void Configure(RawImage image, VideoClip videoClip)
    {
        targetImage = image;
        fallbackTexture = image != null ? image.texture : null;

        if (targetImage == null || videoClip == null)
            return;

        int width = videoClip.width > 0 ? (int)videoClip.width : 1280;
        int height = videoClip.height > 0 ? (int)videoClip.height : 720;

        renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
        {
            name = "Main Menu Animated Background",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            useMipMap = false,
            autoGenerateMips = false
        };
        renderTexture.Create();

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;

        if (Application.isPlaying)
            videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer preparedPlayer)
    {
        if (targetImage != null && renderTexture != null)
            targetImage.texture = renderTexture;

        preparedPlayer.Play();
    }

    void OnVideoError(VideoPlayer failedPlayer, string message)
    {
        if (targetImage != null)
            targetImage.texture = fallbackTexture;

        Debug.LogWarning($"MainMenuBackgroundVideo: เล่นวิดีโอพื้นหลังไม่ได้ ({message})");
    }

    void OnDisable()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Pause();
    }

    void OnEnable()
    {
        if (!Application.isPlaying || videoPlayer == null)
            return;

        if (videoPlayer.isPrepared)
            videoPlayer.Play();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
            videoPlayer.targetTexture = null;
        }

        if (targetImage != null && targetImage.texture == renderTexture)
            targetImage.texture = fallbackTexture;

        if (renderTexture == null)
            return;

        renderTexture.Release();

        if (Application.isPlaying)
            Destroy(renderTexture);
        else
            DestroyImmediate(renderTexture);
    }
}
