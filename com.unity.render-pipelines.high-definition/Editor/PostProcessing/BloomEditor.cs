using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    [VolumeComponentEditor(typeof(Bloom))]
    sealed class BloomEditor : VolumeComponentWithQualityEditor
    {
        SerializedDataParameter m_Threshold;
        SerializedDataParameter m_Intensity;
        SerializedDataParameter m_Scatter;
        SerializedDataParameter m_Tint;
        SerializedDataParameter m_DirtTexture;
        SerializedDataParameter m_DirtIntensity;

        // Advanced settings
        SerializedDataParameter m_HighQualityPrefiltering;
        SerializedDataParameter m_HighQualityFiltering;
        SerializedDataParameter m_Resolution;
        SerializedDataParameter m_Anamorphic;

        public override bool hasAdvancedMode => true;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Bloom>(serializedObject);

            m_Threshold = Unpack(o.Find(x => x.threshold));
            m_Intensity = Unpack(o.Find(x => x.intensity));
            m_Scatter = Unpack(o.Find(x => x.scatter));
            m_Tint = Unpack(o.Find(x => x.tint));
            m_DirtTexture = Unpack(o.Find(x => x.dirtTexture));
            m_DirtIntensity = Unpack(o.Find(x => x.dirtIntensity));

            m_HighQualityPrefiltering = Unpack(o.Find("m_HighQualityPrefiltering"));
            m_HighQualityFiltering = Unpack(o.Find("m_HighQualityFiltering"));
            m_Resolution = Unpack(o.Find("m_Resolution"));
            m_Anamorphic = Unpack(o.Find(x => x.anamorphic));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Bloom", EditorStyles.miniLabel);
            PropertyField(m_Threshold);
            PropertyField(m_Intensity);
            PropertyField(m_Scatter);
            PropertyField(m_Tint);

            EditorGUILayout.LabelField("Lens Dirt", EditorStyles.miniLabel);
            PropertyField(m_DirtTexture, EditorGUIUtility.TrTextContent("Texture"));
            PropertyField(m_DirtIntensity, EditorGUIUtility.TrTextContent("Intensity"));

            if (isInAdvancedMode)
            {
                EditorGUILayout.LabelField("Advanced Tweaks", EditorStyles.miniLabel);

                PropertyField(m_Anamorphic);
            }

            base.OnInspectorGUI();
        }

        public override void OnQualityGUI()
        {
            if (isInAdvancedMode)
            {
                PropertyField(m_Resolution);
                PropertyField(m_HighQualityPrefiltering);
                PropertyField(m_HighQualityFiltering);
            }
        }

        public override QualitySettingsBlob SaveCustomQualitySettingsAsObject(QualitySettingsBlob settings = null)
        {
            if (settings == null)
                settings = new QualitySettingsBlob();

            settings.Save<int>(m_Resolution);
            settings.Save<bool>(m_HighQualityPrefiltering);
            settings.Save<bool>(m_HighQualityFiltering);

            return settings;
        }

        public override void LoadSettingsFromObject(QualitySettingsBlob settings)
        {
            settings.TryLoad<int>(ref m_Resolution);
            settings.TryLoad<bool>(ref m_HighQualityPrefiltering);
            settings.TryLoad<bool>(ref m_HighQualityFiltering);
        }

        public override void LoadSettingsFromQualityPreset(RenderPipelineSettings settings, int level)
        {
            CopySetting(ref m_Resolution, (int)settings.postProcessQualitySettings.BloomRes[level]);
            CopySetting(ref m_HighQualityPrefiltering, settings.postProcessQualitySettings.BloomHighQualityPrefiltering[level]);
            CopySetting(ref m_HighQualityFiltering, settings.postProcessQualitySettings.BloomHighQualityFiltering[level]);
        }
    }
}
