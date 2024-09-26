using UnityEditor;

[CustomEditor(typeof(AudioSettings))]
public class AudioSettingsEditor : Editor
{
    private AudioSettings _audioSettings;

    private SerializedProperty _mixer;
    private SerializedProperty _sliderMusic;
    private SerializedProperty _sliderSFX;

    private void OnEnable()
    {
        _audioSettings = (AudioSettings)target;
        {
            _mixer = serializedObject.FindProperty("_mixer");
            _sliderMusic = serializedObject.FindProperty("_sliderMusic");
            _sliderSFX = serializedObject.FindProperty("_sliderSFX");
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(_mixer);
        EditorGUILayout.Space(5);

        _audioSettings.Type = (AudioSettingsType)EditorGUILayout.EnumPopup("Type", _audioSettings.Type);

        switch (_audioSettings.Type)
        {
            case AudioSettingsType.Slider:
                EditorGUILayout.PropertyField(_sliderMusic);
                EditorGUILayout.PropertyField(_sliderSFX);
                break;
            case AudioSettingsType.Image:

                break;
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
    }
}