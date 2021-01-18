using UnityEditor;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Controllers;


namespace VanillaWorks.MessageLogger.Editor
{
    [CustomEditor(typeof(MessagesControllerBase),true)]
    [CanEditMultipleObjects]
    public class MessageControllerBaseEditor : UnityEditor.Editor
    {
        private SerializedProperty _useCustomBehaviour;
        
        private void OnEnable()
        {
            _useCustomBehaviour = serializedObject.FindProperty("_useCustomMessageBehaviour");

        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            
            if (_useCustomBehaviour.boolValue)
            {
                EditorGUILayout.HelpBox(
                    "After selecting 'Use Custom Behaviour' you need to add your custom MessageBehaviour script to your message prefab.",
                    MessageType.Warning);
                Undo.RecordObject(target,"Custom behavior change change");
            }
        }
    }
}