
namespace DarkCrystal
{
    public class Registrator
    {
        public static bool SetObject<T>(T guidObject, bool state) where T : GuidObject
        {
            if (state)
            {
                GuidStorage<T>.Internal.Register(guidObject.Guid, guidObject);
            }
            else
            {
                GuidStorage<T>.Internal.Unregister(guidObject.Guid);
            }
            return true;
        }
    }
}