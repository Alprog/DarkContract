
namespace DarkCrystal
{ 
    public abstract class RefCountedObject
    {
        private int ReferenceCount;

        public void Retain()
        {
            ReferenceCount++;
        }

        public void Release()
        {
            ReferenceCount--;
            if (ReferenceCount == 0)
            {
                Dispose();
            }
        }

        public abstract void Dispose();
    }
}