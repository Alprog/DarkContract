
namespace DarkCrystal
{
    public abstract class ISingleton
    {
    }

    public class Singleton<T> : ISingleton where T : class
    {
        public readonly static T Instance;
        
        static Singleton()
        {
            Instance = typeof(T).InvokeDefaultConstructor() as T;
        }

        public static T EnsureInitialization()
        {            
            return Instance;
        }
    }
}