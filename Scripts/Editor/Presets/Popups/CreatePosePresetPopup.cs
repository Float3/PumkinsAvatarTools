﻿using Pumkin.AvatarTools;
using Pumkin.DataStructures;
using Pumkin.HelperFunctions;
using UnityEditor;
using UnityEngine;
    
namespace Pumkin.Presets
{
    public class CreatePosePresetPopup : CreatePresetPopupBase
    {
        public static string[] defaultMusclesNames;

        bool muscles_expand = false;
        bool transforms_expand = false;

        SerializedObject serializedPosePreset;
        SerializedProperty pMuscles,
            pTransformPaths,
            pTransformRotations;

        private void OnEnable()
        {
            if(defaultMusclesNames == null || defaultMusclesNames.Length == 0)
                defaultMusclesNames = HumanTrait.MuscleName;

            NewMethod();
        }

        private void NewMethod()
        {
            serializedPosePreset = new SerializedObject(preset);

            pMuscles = serializedPosePreset.FindProperty("muscles");
            pTransformPaths = serializedPosePreset.FindProperty("transformPaths");
            pTransformRotations = serializedPosePreset.FindProperty("transformRotations");
        }

        public static void ShowWindow(PumkinsPosePreset newPreset = null)
        {
            AssignOrCreatePreset<PumkinsPosePreset>(newPreset);

            if(!_window)
            {
                _window = CreateInstance<CreatePosePresetPopup>();
                _window.autoRepaintOnSceneChange = true;
            }

            if(newPreset)
            {
                _window.titleContent = new GUIContent("Edit Pose Preset");
                _overwriteFile = true;
            }
            else
            {
                _window.titleContent = new GUIContent("Create Pose Preset");
            }

            _window.ShowUtility();
        }

        private void OnGUI()
        {
            PumkinsPosePreset preset = (PumkinsPosePreset)CreatePresetPopupBase.preset;
            if(!preset)
                AssignOrCreatePreset<PumkinsPosePreset>(null);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            {
                EditorGUILayout.Space();

                preset.name = EditorGUILayout.TextField("Preset Name", preset.name);

                Helpers.DrawGuiLine();

                PumkinsAvatarTools.DrawAvatarSelectionWithButton(false, false);

                Helpers.DrawGuiLine();

                preset.presetMode = (PumkinsPosePreset.PosePresetMode)EditorGUILayout.EnumPopup("Pose Mode", preset.presetMode);

                Helpers.DrawGuiLine();

                EditorGUI.BeginDisabledGroup(!PumkinsAvatarTools.SelectedAvatar || !preset || string.IsNullOrEmpty(preset.name));
                {
                    _overwriteFile = GUILayout.Toggle(_overwriteFile, "Overwrite File");
                    if(GUILayout.Button("Save Preset", Styles.BigButton))
                    {
                        preset.SetupPreset(preset.name, PumkinsAvatarTools.SelectedAvatar, preset.presetMode);
                        if(preset)
                            preset.SavePreset(_overwriteFile);
                    }
                }
                EditorGUI.EndDisabledGroup();

            }
            EditorGUILayout.EndScrollView();

            CreatePresetPopupBase.preset = preset;
        }

        protected override void RefreshSelectedPresetIndex()
        {
            PumkinsAvatarTools.RefreshPresetIndexByString<PumkinsPosePreset>(preset.name);
        }
    }
}