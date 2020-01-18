#if UNITY_EDITOR
using UnityEngine;
using VRCSDK2;
[System.Serializable]
public class BattlePhazeVideo
{
    public VRC_VideoSpeaker VideoSpeakers;
    public VRC_VideoSpeaker.ChannelType ChannelType;
    public VRC_VideoScreen VideoScreen;
    public VRC_SpatialAudioSource AudioSpatialSource;
    public AudioSource AudioSource;
    public Mesh ScreenMesh;
    public Material ScreenMaterial;
    public Transform VideoItemPositon;
    public VideoType VideoTypeValue;
    public AudioRolloffMode RollOffMode = AudioRolloffMode.Linear;
    public bool EnableSpatilization;
    public float SpatialGain = 10;
    public float SpatialFar = 80;
    public float SpatialNear = 0;
    public float SpatialVolumetricRadius;
    public float AudioMaxFarDistance = 50;
    public float AudioMinDistance = 5;
    public float StartVolume = 0.1f;
    public bool Visable;
    public enum VideoType
    {
        Speaker, Screen
    }
}
#endif