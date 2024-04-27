using Il2CppRhythm;
using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace BackgroundMusicSoftener
{
    public class Main : MelonMod
    {
        private MelonPreferences_Category prefsCategory;
        private MelonPreferences_Entry<bool> muteStateEntry;
        private MelonPreferences_Entry<float> volumeEntry;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

            // Initialize preferences and set custom file path
            prefsCategory = MelonPreferences.CreateCategory("BackgroundMusicSoftener");
            prefsCategory.SetFilePath("UserData/BackgroundMusicSoftener.cfg");
            muteStateEntry = prefsCategory.CreateEntry("MuteState", false);
            volumeEntry = prefsCategory.CreateEntry("VolumeLevel", 0.2f); // Default volume level is 20%

            // Load and apply the saved mute state
            MuteAudio(muteStateEntry.Value);

            // Load and apply the saved volume level
            SetVolume(volumeEntry.Value);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // Ensure the audio is muted based on the saved preference when the scene loads
            MuteAudio(muteStateEntry.Value);

            // mute on the sceen after a run
            SetVolume(volumeEntry.Value);
        }
        
        public override void OnLateUpdate()
        {
            // Apply the saved volume level
            // Needs to be here because OnSceneWasLoaded or OnSceneWasInitialized doesn't work all the time
            SetVolume(volumeEntry.Value);

            // Toggle mute state when the 'M' key is pressed
            if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                ToggleMute();
            }
            // Toggle mute state when the 'M' key is pressed
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            {
                ToggleVolume();
            }
        }

        [HarmonyPostfix] // Postfix is used to run our code after the method finishes
        public static void Postfix(BeatmapInfo __result, int playerIndex)
        {
            // Log the returned BeatmapInfo. Adjust logging based on your needs.
            if (__result != null)
            {
                MelonLogger.Msg($"GetBeatmapInfo called for player index {playerIndex}. Beatmap ID: {__result.id}, Name: {__result.name}");
            }
            else
            {
                MelonLogger.Msg($"GetBeatmapInfo returned null for player index {playerIndex}");
            }
        }


        private void SetVolume(float volume)
        {
            // Set the audio volume state based on the passed volume parameter
            GameObject backgroundAudioSourceObj = GameObject.Find("BackgroundAudioSource");
            if (backgroundAudioSourceObj != null)
            {
                AudioSource audioSource = backgroundAudioSourceObj.GetComponent<AudioSource>();

                if (audioSource != null)
                {
                    if (audioSource.volume != volume) // To make it work in OnLateUpdate
                    {
                        LoggerInstance.Msg($"Background music volume currently is: {audioSource.volume * 100}%");
                        audioSource.volume = volume; // Set the audio volume
                        LoggerInstance.Msg($"Background music volume set to: {volume * 100}%");
                    }

                }
            }
        }
        private void ToggleVolume()
        {
            GameObject backgroundAudioSourceObj = GameObject.Find("BackgroundAudioSource");
            if (backgroundAudioSourceObj != null)
            {
                AudioSource audioSource = backgroundAudioSourceObj.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    // Toggle the volume between 50% and 100%
                    audioSource.volume = audioSource.volume == 0.2f ? 1.0f : 0.2f;
                    volumeEntry.Value = audioSource.volume; // Save the new volume level to preferences
                    LoggerInstance.Msg($"Background music volume level toggled to: {audioSource.volume * 100}%");
                }
            }
        }

        private void MuteAudio(bool mute)
        {
            // Set the audio mute state based on the passed mute parameter
            GameObject backgroundAudioSourceObj = GameObject.Find("BackgroundAudioSource");
            if (backgroundAudioSourceObj != null)
            {
                AudioSource audioSource = backgroundAudioSourceObj.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.mute = mute; // Set the audio mute state
                    LoggerInstance.Msg($"Background music has been muted: {mute}");
                }
            }
        }

        private void ToggleMute()
        {
            GameObject backgroundAudioSourceObj = GameObject.Find("BackgroundAudioSource");
            if (backgroundAudioSourceObj != null)
            {
                AudioSource audioSource = backgroundAudioSourceObj.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.mute = !audioSource.mute; // Toggle the mute state
                    muteStateEntry.Value = audioSource.mute; // Save the new mute state to preferences
                    LoggerInstance.Msg($"Background music mute state: {audioSource.mute}");
                }
            }
        }
    }
}
