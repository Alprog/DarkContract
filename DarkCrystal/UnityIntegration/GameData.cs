
using DarkCrystal.Serialization;

namespace DarkCrystal
{
    [InlineDarkContract]
    public class GameData
    {
        [Key(0, "Entities")]
        public GuidObject.Folder Entities;

        [Key(1, "Locations")]
        public GuidObject.Folder Locations;

        public GameData()
        {
            Entities = new GuidObject.Folder("Entities");
            Locations = new GuidObject.Folder("Locations");
        }
    }
}