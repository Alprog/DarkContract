
namespace DarkCrystal.FieldSystem
{
    public interface IValueProducer<T>
    {
        T Produce(Entity context);
    }
}