using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace IUP.Toolkits.InspectorAttributes.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SceneDropdownFieldAttribute))]
    public sealed class SceneDropdownFieldPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            if (property.propertyType is not
                (SerializedPropertyType.Integer or
                SerializedPropertyType.String))
            {
                HelpBox helpBox = CreateErrorBox(
                    "The attribute [SceneDropdownField] can only be applied to fields of type int or string.");
                root.Add(helpBox);
                return root;
            }

            List<string> scenes = GetScenes();

            bool anySceneInBuildSettings = scenes.Count > 0;
            if (!anySceneInBuildSettings)
            {
                HelpBox helpBox = CreateErrorBox(
                    "No scenes in the build settings, Please ensure that you add your scenes to " +
                    "File->Build Settings");
                root.Add(helpBox);
                return root;
            }

            if (property.propertyType is SerializedPropertyType.String)
            {
                InitRootForStringProperty(root, scenes, property);
            }
            else
            {
                InitRootForIntProperty(root, scenes, property);
            }
            return root;
        }

        private static List<string> GetScenes()
        {
            List<string> scenes = new();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }
            return scenes;
        }

        private void InitRootForIntProperty(
            VisualElement root,
            List<string> scenes,
            SerializedProperty property)
        {
            DropdownField sceneDropdownField = new(property.displayName, scenes, property.intValue);
            sceneDropdownField.BindProperty(property);
            root.Add(sceneDropdownField);
            if (property.intValue >= scenes.Count)
            {
                AddNonExistentElementWarningBoxInRoot(root, sceneDropdownField);
            }
        }

        private void InitRootForStringProperty(
            VisualElement root,
            List<string> scenes,
            SerializedProperty property)
        {
            int selectedIndex = scenes.FindIndex((string scene) => scene == property.stringValue);
            DropdownField sceneDropdownField = new(property.displayName, scenes, selectedIndex);
            sceneDropdownField.BindProperty(property);
            root.Add(sceneDropdownField);
            if (selectedIndex == -1)
            {
                AddNonExistentElementWarningBoxInRoot(root, sceneDropdownField);
            }
        }

        private void AddNonExistentElementWarningBoxInRoot(
            VisualElement root,
            DropdownField sceneDropdownField)
        {
            HelpBox warningBox = new(
                "The selected item is not in the list. " +
                "Check if it is changed in the code or if the scene hierarchy has changed in the build window.",
                HelpBoxMessageType.Warning);
            root.Add(warningBox);

            void RemoveWarningBox(ChangeEvent<string> callback)
            {
                if (sceneDropdownField.index != -1)
                {
                    root.Remove(warningBox);
                    _ = sceneDropdownField.UnregisterValueChangedCallback(RemoveWarningBox);
                }
            }
            _ = sceneDropdownField.RegisterValueChangedCallback(RemoveWarningBox);
        }

        private HelpBox CreateErrorBox(string errorMessage) => new(errorMessage, HelpBoxMessageType.Error);
    }
}
