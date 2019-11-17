
#if UNITY_EDITOR

using UnityEditor;

namespace DarkCrystal.UnityIntegration
{
    public partial class DataExplorer
    {
        [CustomEditor(typeof(DataSelector))]
        public class DataSelectorEditor : ExtendedEditor<DataSelector>
        {
            public GuidObject GuidObject => Target.SelectedObject;

            protected override void OnGUI()
            {
                if (GuidObject == null)
                {
                    Layout.Label("No selection");
                    return;
                }

                Inspect(GuidObject);
            }

            public void Inspect(GuidObject guidObject)
            {
                Layout.ReadonlyTextField("Guid", guidObject.Guid.ToString());

                guidObject.ID = EditorGUILayout.TextField("ID", guidObject.ID);
            }
        }
    }
}

#endif