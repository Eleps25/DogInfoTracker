using System.ComponentModel;
using System.Reflection;

namespace DogInfoTracker {
    internal class EnumHelper {
        public static string GetEnumDescription(Enum enumValue) {
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
            DescriptionAttribute attr = field?.GetCustomAttribute<DescriptionAttribute>();

            return attr != null ? attr.Description : enumValue.ToString();
        }
    }
}
