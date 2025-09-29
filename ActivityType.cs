using System.ComponentModel;

namespace DogInfoTracker {
    public enum ActivityType {
        [Description("Veterinář")]
        Veterinary,

        [Description("Krmení")]
        Food,

        [Description("Vážení")]
        Weight,

        [Description("Ostatní")]
        Other = 100
    }
}
