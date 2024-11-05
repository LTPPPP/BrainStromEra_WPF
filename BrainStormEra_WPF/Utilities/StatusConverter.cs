using System;
using System.Globalization;
using System.Windows.Data;

namespace BrainStormEra_WPF.Utilities
{
    public class StatusConverter : IValueConverter
    {
        private static readonly Dictionary<int, string> StatusDescriptions = new Dictionary<int, string>
        {
            { 1, "Active" },
            { 2, "Inactive" },
            { 3, "Pending" },
            { 4, "Completed" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int statusId && StatusDescriptions.TryGetValue(statusId, out string description))
            {
                return description;
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var pair in StatusDescriptions)
            {
                if (pair.Value.Equals(value))
                {
                    return pair.Key;
                }
            }
            return 0;
        }
    }
}
