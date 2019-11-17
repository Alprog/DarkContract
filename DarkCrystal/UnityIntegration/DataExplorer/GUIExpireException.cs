
namespace DarkCrystal.UnityIntegration
{
    public class GUIExpireException : System.Exception
    {
        public void Process()
        {
#if UNITY_EDITOR
            Utils.RefreshEditors();
#endif
        }
    }
}