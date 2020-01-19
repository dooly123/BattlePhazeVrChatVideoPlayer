#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRCSDK2;
namespace BattlePhaze.VideoPlayer.Setup.EditorCustom
{
    /// <summary>
    /// BattlePhaze Video generation and player
    /// </summary>
    public class BattlePhazeVideoPlayer : MonoBehaviour
    {
        #region Values
        public bool ColliderTrigger;
        public bool AutoStart;
        public bool AllowNonOwnerControl;
        public bool AssignName = false;
        public int MaxStreamQuality = 720;
        public VRCSDK2.VRC_SyncVideoStream.VideoTextureFormat VideoTextureFormat;
        public GameObject OnEnterTrigger;
        public GameObject OnExitTrigger;
        public Transform ParentUI;
        public VRC_EventHandler.VrcBroadcastType LockedToBroadcast = VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered;
        public Button Play;
        public Button Pause;
        public Button Stop;
        public Button FastForward;
        public Button Rewind;
        public Button Skip;
        public Button Previous;
        public Button Resync;
        public Button PlayFromVideoURL;
        [SerializeField]
        public InputField VideoURL;
        [SerializeField]
        public Slider Volume;
        [SerializeField]
        public VRC_SyncVideoStream VideoPlayer;
        [SerializeField]
        public List<BattlePhazeVideo> BattlePhazeVideoList = new List<BattlePhazeVideo>();
        [SerializeField]
        public List<VRC_SyncVideoStream.VideoEntry> BattlePhazeVideoEntry = new List<VRC_SyncVideoStream.VideoEntry>();
        public bool[] Extended;
        public bool Settings;
        #endregion
        /// <summary>
        /// Generate video system
        /// </summary>
        public void VideoSystemDestructive()
        {
            GenerateParent();
            GenerateVideoPoints();
            GenerateUISystem();
        }
        /// <summary>
        /// Check video system
        /// </summary>
        public void VideoSystemAdaptive()
        {
            GenerateParent();
            GenerateVideoPoints();
            for (int SourceIndex = 0; SourceIndex < BattlePhazeVideoList.Count; SourceIndex++)
            {
                if (BattlePhazeVideoList[SourceIndex].AudioSource)
                {
                    Debug.Log("Make sure your speaker have been added to the Volume Slider", BattlePhazeVideoList[SourceIndex].AudioSource);
                }
            }
            if (VideoURL)
            {
                Debug.Log("Video InputField needs to clear when pressed, this needs to be manually done. a dummy AddURL was added for you to modify, https://imgur.com/a/rYIHhv6", VideoURL);
            }
            PrepareTransplant(Play, "Play");
            PrepareTransplant(Pause, "Pause");
            PrepareTransplant(Stop, "Stop");
            PrepareTransplant(Skip, "Next");
            PrepareTransplant(Previous, "Previous");
            PrepareTransplant(Resync, "Resync");
            //future update use reflection to assign time
            PrepareTransplant(FastForward, "FastForwardSeconds");
            PrepareTransplant(Rewind, "RewindSeconds");
            if (ParentUI)
            {
                if (OnEnterTrigger)
                {
                    ColliderTriggerState("OnEnter", OnEnterTrigger.transform);
                }
                if (OnEnterTrigger)
                {
                    ColliderTriggerState("OnExit", OnExitTrigger.transform);
                }
            }
        }
        /// <summary>
        /// Generate Parent
        /// </summary>
        public void GenerateParent()
        {
            this.transform.gameObject.name = "BattlePhaze Video Player";
            VideoPlayer = this.gameObject.GetOrAddComponent<VRC_SyncVideoStream>();
            VRC_ObjectSync VideoSync = this.gameObject.GetOrAddComponent<VRC_ObjectSync>();
            VideoPlayer.AutoStart = AutoStart;
            VideoPlayer.AllowNonOwnerControl = AllowNonOwnerControl;
            VideoPlayer.videoTextureFormat = VideoTextureFormat;
            VideoPlayer.Videos = null;
            VideoPlayer.Videos = BattlePhazeVideoEntry.ToArray();
            VideoPlayer.MaxStreamQuality = MaxStreamQuality;
        }
        /// <summary>
        /// Generate video points
        /// </summary>
        public void GenerateVideoPoints()
        {
            for (int VideSpeakersIndex = 0; VideSpeakersIndex < BattlePhazeVideoList.Count; VideSpeakersIndex++)
            {
                BattlePhazeVideoList[VideSpeakersIndex] = BattlePhazeVideoList[VideSpeakersIndex];
                if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon)
                {
                    if (BattlePhazeVideoList[VideSpeakersIndex].VideoTypeValue == BattlePhazeVideo.VideoType.Screen)
                    {
                        MeshFilter MeshFilter = BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.GetOrAddComponent<MeshFilter>();
                        if (MeshFilter.sharedMesh == null)
                        {
                            MeshFilter.sharedMesh = BattlePhazeVideoList[VideSpeakersIndex].ScreenMesh;
                        }
                        MeshRenderer MeshRender = BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.GetOrAddComponent<MeshRenderer>();
                        if (MeshRender.sharedMaterial == null)
                        {
                            MeshRender.sharedMaterial = BattlePhazeVideoList[VideSpeakersIndex].ScreenMaterial;
                        }
                        BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.name = "Screen";
                        BattlePhazeVideoList[VideSpeakersIndex].VideoScreen = BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.GetOrAddComponent<VRC_VideoScreen>();
                        BattlePhazeVideoList[VideSpeakersIndex].VideoScreen._videoStream = VideoPlayer;
                    }
                    if (BattlePhazeVideoList[VideSpeakersIndex].VideoTypeValue == BattlePhazeVideo.VideoType.Speaker)
                    {
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource = BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.GetOrAddComponent<AudioSource>();
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSpatialSource = BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.GetOrAddComponent<VRC_SpatialAudioSource>();
                        BattlePhazeVideoList[VideSpeakersIndex].VideoSpeakers = BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.GetOrAddComponent<VRC_VideoSpeaker>();
                        BattlePhazeVideoList[VideSpeakersIndex].VideoSpeakers._videoStream = VideoPlayer;
                        BattlePhazeVideoList[VideSpeakersIndex].VideoSpeakers._channelType = BattlePhazeVideoList[VideSpeakersIndex].ChannelType;
                        BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.name = "Speaker: " + BattlePhazeVideoList[VideSpeakersIndex].VideoSpeakers._channelType.ToString();
                        if (BattlePhazeVideoList[VideSpeakersIndex].EnableSpatilization)
                        {
                            BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.gameObject.name = "Speaker: " + BattlePhazeVideoList[VideSpeakersIndex].VideoSpeakers._channelType.ToString() + ": with Spatilization";
                            BattlePhazeVideoList[VideSpeakersIndex].AudioSpatialSource.Gain = 10;
                            BattlePhazeVideoList[VideSpeakersIndex].AudioSpatialSource.Far = 80;
                            BattlePhazeVideoList[VideSpeakersIndex].AudioSpatialSource.EnableSpatialization = BattlePhazeVideoList[VideSpeakersIndex].EnableSpatilization;
                            BattlePhazeVideoList[VideSpeakersIndex].AudioSpatialSource.VolumetricRadius = BattlePhazeVideoList[VideSpeakersIndex].SpatialVolumetricRadius;
                            BattlePhazeVideoList[VideSpeakersIndex].AudioSpatialSource.Near = BattlePhazeVideoList[VideSpeakersIndex].SpatialNear;
                        }
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.priority = 128;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.volume = BattlePhazeVideoList[VideSpeakersIndex].StartVolume;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.pitch = 1;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.panStereo = 0;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.reverbZoneMix = 1;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.dopplerLevel = 0;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.spread = 0;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.rolloffMode = BattlePhazeVideoList[VideSpeakersIndex].RollOffMode;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.minDistance = BattlePhazeVideoList[VideSpeakersIndex].AudioMinDistance;
                        BattlePhazeVideoList[VideSpeakersIndex].AudioSource.maxDistance = BattlePhazeVideoList[VideSpeakersIndex].AudioMaxFarDistance;
                    }
                    if (BattlePhazeVideoList[VideSpeakersIndex].VideoTypeValue != BattlePhazeVideo.VideoType.Speaker)
                    {
                        if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<VRC_SpatialAudioSource>())
                        {
                            DestroyImmediate(BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<VRC_SpatialAudioSource>());
                        }
                        if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<VRC_VideoSpeaker>())
                        {
                            DestroyImmediate(BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<VRC_VideoSpeaker>());
                        }
                        if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<AudioSource>())
                        {
                            DestroyImmediate(BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<AudioSource>());
                        }
                    }
                    if (BattlePhazeVideoList[VideSpeakersIndex].VideoTypeValue != BattlePhazeVideo.VideoType.Screen)
                    {
                        if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<VRC_VideoScreen>())
                        {
                            DestroyImmediate(BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<VRC_VideoScreen>());
                        }
                        if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<MeshFilter>())
                        {
                            DestroyImmediate(BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<MeshFilter>());
                        }
                        if (BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<MeshRenderer>())
                        {
                            DestroyImmediate(BattlePhazeVideoList[VideSpeakersIndex].VideoItemPositon.GetComponent<MeshRenderer>());
                        }
                    }
                    BattlePhazeVideoList[VideSpeakersIndex] = BattlePhazeVideoList[VideSpeakersIndex];
                }
            }
        }
        /// <summary>
        /// UI system generation
        /// </summary>
        public void GenerateUISystem()
        {
            PrepareTransplant(Play, "Play");
            PrepareTransplant(Pause, "Pause");
            PrepareTransplant(Stop, "Stop");
            PrepareTransplant(Skip, "Next");
            PrepareTransplant(Previous, "Previous");
            PrepareTransplant(Resync, "Resync");
            //future update use reflection to assign time
            PrepareTransplant(FastForward, "FastForwardSeconds");
            PrepareTransplant(Rewind, "RewindSeconds");
            VolumeSlider();
            InputVideoURL();
            if (ParentUI)
            {
                if (OnEnterTrigger)
                {
                    ColliderTriggerState("OnEnter", OnEnterTrigger.transform);
                }
                if (OnEnterTrigger)
                {
                    ColliderTriggerState("OnExit", OnExitTrigger.transform);
                }
            }
        }
        /// <summary>
        /// Volume slider
        /// </summary>
        public void VolumeSlider()
        {
            //for each Speaker add a Onvalue changed event
            for (int SourceIndex = 0; SourceIndex < BattlePhazeVideoList.Count; SourceIndex++)
            {
                if (BattlePhazeVideoList[SourceIndex].AudioSource)
                {
                    Debug.Log("Volume Slider Speakers need to be Manually assigned!!, this is a limitation by unity", BattlePhazeVideoList[SourceIndex].AudioSource);
                }
            }
        }
        /// <summary>
        /// Input video url
        /// </summary>
        public void InputVideoURL()
        {
            OnInputField(VideoURL);
        }
        /// <summary>
        /// double check states and references before open heart surgery
        /// </summary>
        public void PrepareTransplant(Button Event, string EventName)
        {
            if (Event)
            {
                Event.name = "Video System Button: " + EventName;
                if (AssignName)
                {
                    if (Event.GetComponentInChildren<Text>())
                    {
                        Event.GetComponentInChildren<Text>().text = EventName;
                        Event.GetComponentInChildren<Text>().name = EventName+": Text";
                    }
                }
                OnclickState(Event, EventName, GenerateTrigger(EventName));
            }
        }
        /// <summary>
        /// Generates the trigger
        /// </summary>
        /// <param name="RPCName"></param>
        /// <returns></returns>
        public VRC_Trigger GenerateTrigger(string RPCName)
        {
            GameObject RPC = GetChildWithName(this.gameObject, RPCName);
            if (RPC == null)
            {
                RPC = new GameObject(RPCName);
                RPC.transform.parent = transform;
                RPC.transform.localPosition = transform.position;
            }
            VRC_Trigger Trigger = RPC.GetOrAddComponent<VRC_Trigger>();
            Trigger.UsesAdvancedOptions = true;
            Trigger.ShowHelp = true;
            Trigger.Triggers.Clear();
            Trigger.Triggers.Add(Triggerevent(RPCName, LockedToBroadcast));
            return Trigger;
        }
        /// <summary>
        /// digs up the child with a name
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        GameObject GetChildWithName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Trigger event generation
        /// </summary>
        /// <param name="RPCName"></param>
        /// <param name="BroadCastType"></param>
        /// <returns></returns>
        public VRC_Trigger.TriggerEvent Triggerevent(string RPCName, VRC_EventHandler.VrcBroadcastType BroadCastType)
        {
            VRC_Trigger.TriggerEvent EventTrigger = new VRC_Trigger.TriggerEvent
            {
                TriggerType = VRC_Trigger.TriggerType.Custom,
                BroadcastType = BroadCastType,
                Name = RPCName,
                TriggerIndividuals = true
            };
            EventTrigger.Events.Add(VrcEvent(RPCName));
            return EventTrigger;
        }
        /// <summary>
        /// Add a RPC
        /// </summary>
        /// <param name="RPCName"></param>
        /// <returns></returns>
        public VRC_EventHandler.VrcEvent VrcEvent(string RPCName)
        {
            VRC_EventHandler.VrcEvent Event = new VRC_EventHandler.VrcEvent {
                EventType = VRC_EventHandler.VrcEventType.SendRPC,
                Name = RPCName,
                ParameterString = RPCName,
                ParameterInt = 2,
                ParameterObject = VideoPlayer.gameObject
            };
            return Event;
        }
        /// <summary>
        /// Add a Button click state
        /// </summary>
        /// <param name="BtnClick"></param>
        /// <param name="RPCName"></param>
        /// <param name="Trigger"></param>
        public void OnclickState(Button BtnClick, string RPCName, VRC_Trigger Trigger)
        {
            BtnClick.onClick.RemoveAllListeners();
            BtnClick.onClick = new Button.ButtonClickedEvent();
            UnityAction<string> mbListener = new UnityAction<string>(Trigger.ExecuteCustomTrigger);
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(BtnClick.onClick, mbListener, RPCName);
        }
        /// <summary>
        /// Audio Click States
        /// </summary>
        /// <param name="SliderClick"></param>
        /// <param name="RPCName"></param>
        /// <param name="Trigger"></param>
        public void OnVolume(Slider SliderClick, float RPCName, AudioSource[] Source)
        {
            if (SliderClick)
            {
                SliderClick.name = "Video System Volume Slider";
                //SliderClick.onValueChanged.RemoveAllListeners();
                //SliderClick.onValueChanged = new Slider.SliderEvent();
                for (int SourceIndex = 0; SourceIndex < Source.Length; SourceIndex++)
                {
                    //Volume set_Volume does not exist as a variable i can interact with ;
                    //UnityAction<float> VolumeListener = new UnityAction<float>(Source[SourceIndex].set_volume);
                    //UnityEditor.Events.UnityEventTools.AddFloatPersistentListener(SliderClick.onValueChanged, VolumeListener, RPCName);
                }
            }
        }
        /// <summary>
        /// Add a Button click state
        /// </summary>
        /// <param name="BtnClick"></param>
        /// <param name="RPCName"></param>
        /// <param name="Trigger"></param>
        public void OnInputField(InputField InputField)
        {
            if (InputField)
            {
                InputField.name = "Video System Video Input";
                InputField.onEndEdit.RemoveAllListeners();
                InputField.onEndEdit = new InputField.SubmitEvent();
                UnityAction Clear = new UnityAction(VideoPlayer.Clear);
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(InputField.onEndEdit, Clear);
                UnityAction<string> AddURL = new UnityAction<string>(VideoPlayer.AddURL);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(InputField.onEndEdit, AddURL);
                UnityAction<string> Secondary = new UnityAction<string>(VideoPlayer.AddURL);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(InputField.onEndEdit, Secondary);
                Debug.Log("Video InputField needs to clear when pressed, this needs to be manually done. a dummy AddURL was added for you to modify, https://imgur.com/a/rYIHhv6", InputField);
            }
        }
        /// <summary>
        /// Collider Trigger State
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public Collider ColliderTriggerState(string State, Transform Position)
        {
          BoxCollider Collider  = Position.gameObject.GetOrAddComponent<BoxCollider>();
            Collider.isTrigger = true;
          VRC_Trigger Trigger = Collider.gameObject.GetOrAddComponent<VRC_Trigger>();
            Position.gameObject.name = State;
            if (State == "OnEnter")
            {
                Trigger.Triggers.Add(ColliderTriggerEvent(VRC_Trigger.TriggerType.OnEnterTrigger, true));
            }
            else
            {
                if (State == "OnExit")
                {
                    Trigger.Triggers.Add(ColliderTriggerEvent(VRC_Trigger.TriggerType.OnExitTrigger, false));
                }
            }
            return Collider;
        }
        /// <summary>
        /// Collider Trigger Event
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public VRC_Trigger.TriggerEvent ColliderTriggerEvent(VRC_Trigger.TriggerType trigger,bool state)
        {
            VRC_Trigger.TriggerEvent EventTrigger = new VRC_Trigger.TriggerEvent
            {
                TriggerType = trigger,
                BroadcastType  = VRC_EventHandler.VrcBroadcastType.Local,
                Name = "GameObject State",
                TriggerIndividuals = true
            };
            EventTrigger.Events.Add(ColliderVrcEvent(state));
            return EventTrigger;
        }
        /// <summary>
        /// Collider Event
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public VRC_EventHandler.VrcEvent ColliderVrcEvent(bool State)
        {
            VRC_EventHandler.VrcEvent Event = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.SetGameObjectActive,
                Name = "Set Gameobject",
                ParameterBool = State,
                ParameterObject = ParentUI.gameObject
            };
            return Event;
        }
    }
}
#endif