using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRCSDK2;
using UnityEngine.Video;
using UnityEngine.UI;

namespace BattlePhaze.VideoPlayer.Setup.EditorCustom
{
    [CustomEditor(typeof(BattlePhazeVideoPlayer))]
    public class BattlePhazeVideoPlayerEditor : Editor
    {
        public Color32 PrimaryWhiteColor = new Color32(242, 85, 228, 255);
        public Color32 SecondaryColor = new Color32(140, 140, 140, 255);
        public GUIStyle FoldOutStyle;
        public GUIStyle StyleButton;
        public GUIStyle TextStyling;
        public GUIStyle StyleTextInput;
        public GUIStyle EnumStyling;
        public GUIStyle HorizontalLine;
        public GUIStyle LargeTextStyling;
        public BattlePhazeVideoPlayer BattlePhazeVideoPlayerManager;
        public SerializedProperty BattlePhazeVideoEntryProperty;
        public SerializedObject m_manager;
        public SerializedProperty BattlePhazeVideoTypeProperty;
        public bool rerun = false;
        public float Version = 0.01f;

        public override void OnInspectorGUI()
        {
            BattlePhazeVideoPlayerManager = (BattlePhazeVideoPlayer)target;
            if (rerun == false)
            {
                UIDesign();
            }
            m_manager = new SerializedObject(BattlePhazeVideoPlayerManager);
            m_manager.Update();
            BattlePhazeVideoEntryProperty = m_manager.FindProperty("BattlePhazeVideoEntry");
            BattlePhazeVideoTypeProperty = m_manager.FindProperty("BattlePhazeVideoList");
            VideoPlayerDisplay();
            BattlePhazeUIPipeline();
            if (GUILayout.Button("Update Video System (adaptive)", StyleButton))
            {
                BattlePhazeVideoPlayerManager.VideoSystemAdaptive();
                EditorUtility.SetDirty(BattlePhazeVideoPlayerManager);
            }
            if (GUILayout.Button("Generate Video System (destructive)", StyleButton))
            {
                BattlePhazeVideoPlayerManager.VideoSystemDestructive();
                EditorUtility.SetDirty(BattlePhazeVideoPlayerManager);
            }
            serializedObject.ApplyModifiedProperties();
        }
        public void UIDesign()
        {
            rerun = true;
            StyleButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(3, 3, 3, 3),
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            StyleButton.normal.background = SetColor(40,40, SecondaryColor);
            StyleButton.normal.textColor = PrimaryWhiteColor;
            TextStyling = new GUIStyle(GUI.skin.textArea)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(3, 3, 3, 3),
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            TextStyling.fixedWidth = 300;
            TextStyling.fixedHeight = 20;
            TextStyling.normal.background = SetColor(2, 2, new Color(0.9f, 0.9f, 0.9f, 0f));
            TextStyling.clipping = TextClipping.Overflow;
            TextStyling.normal.textColor = PrimaryWhiteColor;
            StyleTextInput = new GUIStyle(GUI.skin.textArea)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(3, 3, 3, 3),
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            StyleTextInput.fixedHeight = 20;
            StyleTextInput.normal.background = SetColor(2, 2, new Color(0.7f, 0.7f, 0.7f, 1f));
            StyleTextInput.clipping = TextClipping.Overflow;
            StyleTextInput.normal.textColor = PrimaryWhiteColor;
            FoldOutStyle = new GUIStyle(EditorStyles.foldout)
            {
                margin = new RectOffset(3, 3, 3, 3),
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            FoldOutStyle.normal.textColor = PrimaryWhiteColor;
            FoldOutStyle.active.textColor = PrimaryWhiteColor;
            FoldOutStyle.focused.textColor = PrimaryWhiteColor;
            FoldOutStyle.hover.textColor = PrimaryWhiteColor;
            FoldOutStyle.onNormal.textColor = PrimaryWhiteColor;
            FoldOutStyle.onActive.textColor = PrimaryWhiteColor;
            FoldOutStyle.onFocused.textColor = PrimaryWhiteColor;
            FoldOutStyle.onHover.textColor = PrimaryWhiteColor;
            LargeTextStyling = new GUIStyle(EditorStyles.label)
            {
                margin = new RectOffset(3, 3, 3, 3),
                fontSize = 30,
                fontStyle = FontStyle.Bold
            };
            LargeTextStyling.normal.textColor = PrimaryWhiteColor;
            LargeTextStyling.active.textColor = PrimaryWhiteColor;
            LargeTextStyling.focused.textColor = PrimaryWhiteColor;
            LargeTextStyling.hover.textColor = PrimaryWhiteColor;
            LargeTextStyling.onNormal.textColor = PrimaryWhiteColor;
            LargeTextStyling.onActive.textColor = PrimaryWhiteColor;
            LargeTextStyling.onFocused.textColor = PrimaryWhiteColor;
            LargeTextStyling.onHover.textColor = PrimaryWhiteColor;
            EnumStyling = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Bold
            };
            EnumStyling.normal.background = SetColor(2, 2, new Color(0.4f, 0.4f, 0.4f, 0.5f));
            EnumStyling.normal.textColor = PrimaryWhiteColor;
            HorizontalLine = new GUIStyle();
            HorizontalLine.normal.background = SetColor(2, 2, new Color(0.7f, 0.7f, 0.7f, 1f));
            HorizontalLine.margin = new RectOffset(0, 0, 4, 4);
            HorizontalLine.fixedHeight = 12;
        }
        public void VideoPlayerDisplay()
        {
            GUILayout.Label("BattlePhaze Video", LargeTextStyling);
            GUILayout.Label("Version " + Version, TextStyling);
            GUILayout.Box(GUIContent.none,HorizontalLine);
            BattlePhazeVideoListing();
            GUILayout.Space(10);
            if (GUILayout.Button("Add a video", StyleButton))
            {
                BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.Add(new VRCSDK2.VRC_SyncVideoStream.VideoEntry());
                BattlePhazeVideoPlayerManager.Extended = new bool[BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.Count];
                EditorUtility.SetDirty(BattlePhazeVideoPlayerManager);
                serializedObject.Update();
            }
            BattlePhazeVideoEntryListing();
            GUILayout.Space(10);
            if (GUILayout.Button("Add a video Receiver", StyleButton))
            {
                BattlePhazeVideoPlayerManager.BattlePhazeVideoList.Add(new BattlePhazeVideo());
                EditorUtility.SetDirty(BattlePhazeVideoPlayerManager);
                serializedObject.Update();
            }
            GUILayout.Space(10);
        }
        public void BattlePhazeVideoListing()
        {
            for (int VideoSpeakersIndex = 0; VideoSpeakersIndex < BattlePhazeVideoEntryProperty.arraySize; VideoSpeakersIndex++)
            {
                if (BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].Source == VideoSource.Url)
                {
                    EditorGUILayout.BeginHorizontal();
                    BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex] = EditorGUILayout.Foldout(BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex], "Video External " + BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].URL, true, FoldOutStyle);
                    if (GUILayout.Button("Remove Video", StyleButton))
                    {
                        BattlePhazeVideoPlayerManager.Extended = new bool[BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.Count];
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.RemoveAt(VideoSpeakersIndex);
                    }
                        GUILayout.EndVertical();
                }
                else
                {
                    if (BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].VideoClip)
                    {
                        EditorGUILayout.BeginHorizontal();
                        BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex] = EditorGUILayout.Foldout(BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex], "Video Internal " + BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].VideoClip.name, true, FoldOutStyle);
                        if (GUILayout.Button("Remove Video", StyleButton))
                        {
                            BattlePhazeVideoPlayerManager.Extended = new bool[BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.Count];
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.RemoveAt(VideoSpeakersIndex);
                        }
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex] = EditorGUILayout.Foldout(BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex], "Video Internal ", true, FoldOutStyle);
                        if (GUILayout.Button("Remove Video", StyleButton))
                        {
                            BattlePhazeVideoPlayerManager.Extended = new bool[BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.Count];
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry.RemoveAt(VideoSpeakersIndex);
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.Space(4);
                if (BattlePhazeVideoPlayerManager.Extended[VideoSpeakersIndex])
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Video Source", TextStyling);
                    BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].Source = (VideoSource)EditorGUILayout.EnumPopup(BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].Source, EnumStyling);
                    GUILayout.EndVertical();
                    if (BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].Source == VideoSource.Url)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Content Type", TextStyling);
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].SyncType = (VRC_SyncVideoStream.VideoSyncType)EditorGUILayout.EnumPopup(BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].SyncType, EnumStyling);
                        GUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("URL", TextStyling);
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].URL = EditorGUILayout.TextField(BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].URL, StyleTextInput);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].Source == VideoSource.VideoClip)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Video Clip", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].VideoClip = (VideoClip)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].VideoClip, typeof(VideoClip), true);
                            GUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Play Back Speed", TextStyling);
                    BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].PlaybackSpeed = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoEntry[VideoSpeakersIndex].PlaybackSpeed, StyleTextInput);
                    GUILayout.EndVertical();
                }
                GUILayout.Space(5);
            }
        }
        public void BattlePhazeVideoEntryListing()
        {
            if (BattlePhazeVideoTypeProperty != null)
            {
                for (int VideoPlayerIndex = 0; VideoPlayerIndex < BattlePhazeVideoTypeProperty.arraySize; VideoPlayerIndex++)
                {
                    //SerializedProperty m_explanation = SerializedOptionsList.FindPropertyRelative("m_explanation");
                    EditorGUILayout.BeginHorizontal();
                    BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].Visable = EditorGUILayout.Foldout(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].Visable, "Output " + BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoTypeValue.ToString(), true, FoldOutStyle);
                    if (GUILayout.Button("Remove Output", StyleButton))
                    {
                        BattlePhazeVideoPlayerManager.Extended = new bool[BattlePhazeVideoPlayerManager.BattlePhazeVideoList.Count];
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoList.RemoveAt(VideoPlayerIndex);
                    }
                    GUILayout.EndVertical();
                    if (BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].Visable)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Type", TextStyling);
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoTypeValue = (BattlePhazeVideo.VideoType)EditorGUILayout.EnumPopup(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoTypeValue, EnumStyling);
                        GUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Position", TextStyling);
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoItemPositon = (Transform)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoItemPositon, typeof(Transform), true);
                        GUILayout.EndVertical();
                        if (BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoTypeValue == BattlePhazeVideo.VideoType.Speaker)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Audio Channel Mode", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].ChannelType = (VRC_VideoSpeaker.ChannelType)EditorGUILayout.EnumPopup(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].ChannelType, EnumStyling);
                            GUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Audio Maximum Distance", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].AudioMaxFarDistance = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].AudioMaxFarDistance, StyleTextInput);
                            GUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Audio Minimum Distance", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].AudioMinDistance = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].AudioMinDistance, StyleTextInput);
                            GUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Enable Spatilization", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].EnableSpatilization = EditorGUILayout.Toggle(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].EnableSpatilization);
                            GUILayout.EndVertical();
                            if (BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].EnableSpatilization)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("Spatial Volumetric Radius", TextStyling);
                                BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialVolumetricRadius = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialVolumetricRadius, StyleTextInput);
                                GUILayout.EndVertical();
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("Spatial Near Distance", TextStyling);
                                BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialNear = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialNear, StyleTextInput);
                                GUILayout.EndVertical();
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("Spatial Far Distance", TextStyling);
                                BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialFar = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialFar, StyleTextInput);
                                GUILayout.EndVertical();
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("Spatial Gain", TextStyling);
                                BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialGain = EditorGUILayout.FloatField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].SpatialGain, StyleTextInput);
                                GUILayout.EndVertical();
                            }
                        }
                        if (BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].VideoTypeValue == BattlePhazeVideo.VideoType.Screen)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Material", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].ScreenMaterial = (Material)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].ScreenMaterial, typeof(Material), false);
                            GUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Mesh", TextStyling);
                            BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].ScreenMesh = (Mesh)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex].ScreenMesh, typeof(Mesh), false);
                            GUILayout.EndVertical();
                        }
                        BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex] = BattlePhazeVideoPlayerManager.BattlePhazeVideoList[VideoPlayerIndex];
                    }
                }
            }
        }
        public void BattlePhazeUIPipeline()
        {
            BattlePhazeVideoPlayerManager.Settings = EditorGUILayout.Foldout(BattlePhazeVideoPlayerManager.Settings, "Settings", true, FoldOutStyle);
            if (BattlePhazeVideoPlayerManager.Settings)
            {
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Auto Naming", TextStyling);
                BattlePhazeVideoPlayerManager.AssignName = EditorGUILayout.Toggle(BattlePhazeVideoPlayerManager.AssignName);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Allow Non Owner Control", TextStyling);
                BattlePhazeVideoPlayerManager.AllowNonOwnerControl = EditorGUILayout.Toggle(BattlePhazeVideoPlayerManager.AllowNonOwnerControl);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Auto Start the Player", TextStyling);
                BattlePhazeVideoPlayerManager.AutoStart = EditorGUILayout.Toggle(BattlePhazeVideoPlayerManager.AutoStart);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Max Stream Quality", TextStyling);
                BattlePhazeVideoPlayerManager.MaxStreamQuality = EditorGUILayout.IntField(BattlePhazeVideoPlayerManager.MaxStreamQuality);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Video Format", TextStyling);
                BattlePhazeVideoPlayerManager.LockedToBroadcast = (VRC_EventHandler.VrcBroadcastType)EditorGUILayout.EnumPopup(BattlePhazeVideoPlayerManager.LockedToBroadcast, EnumStyling);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Video Format", TextStyling);
                BattlePhazeVideoPlayerManager.VideoTextureFormat = (VRCSDK2.VRC_SyncVideoStream.VideoTextureFormat)EditorGUILayout.EnumPopup(BattlePhazeVideoPlayerManager.VideoTextureFormat,EnumStyling);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("On Trigger Enter Trigger", TextStyling);
                BattlePhazeVideoPlayerManager.OnEnterTrigger = (GameObject)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.OnEnterTrigger, typeof(GameObject), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("On Trigger Exit Trigger", TextStyling);
                BattlePhazeVideoPlayerManager.OnEnterTrigger = (GameObject)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.OnExitTrigger, typeof(GameObject), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Parent Of UI Window", TextStyling);
                BattlePhazeVideoPlayerManager.ParentUI = (Transform)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.ParentUI, typeof(Transform), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Play", TextStyling);
                BattlePhazeVideoPlayerManager.Play = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Play, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Pause", TextStyling);
                BattlePhazeVideoPlayerManager.Pause = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Pause, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Stop", TextStyling);
                BattlePhazeVideoPlayerManager.Stop = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Stop, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("FastForward", TextStyling);
                BattlePhazeVideoPlayerManager.FastForward = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.FastForward, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Rewind", TextStyling);
                BattlePhazeVideoPlayerManager.Rewind = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Rewind, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Skip", TextStyling);
                BattlePhazeVideoPlayerManager.Skip = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Skip, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Previous", TextStyling);
                BattlePhazeVideoPlayerManager.Previous = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Previous, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Resync", TextStyling);
                BattlePhazeVideoPlayerManager.Resync = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Resync, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Play From Video URL", TextStyling);
                BattlePhazeVideoPlayerManager.PlayFromVideoURL = (Button)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.PlayFromVideoURL, typeof(Button), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Volume URL", TextStyling);
                BattlePhazeVideoPlayerManager.VideoURL = (InputField)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.VideoURL, typeof(InputField), true);
                GUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Volume Slider", TextStyling);
                BattlePhazeVideoPlayerManager.Volume = (Slider)EditorGUILayout.ObjectField(BattlePhazeVideoPlayerManager.Volume, typeof(Slider), true);
                GUILayout.EndVertical();
            }
            GUILayout.Space(10);
        }
        /// <summary>
        /// Used for setting a color of a Texture2D
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public Texture2D SetColor(int x, int y, Color32 color)
        {
            Texture2D tex = new Texture2D(x, y);
            Color[] pix = new Color[x * y];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }
}