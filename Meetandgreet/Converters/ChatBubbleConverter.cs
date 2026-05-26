using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace Meetandgreet.Converters
{
    public class ChatBubbleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 'value' will be the SenderId. Let's compare it to your current user.
            int senderId = (int)value;

            // Replace '1' with your logged-in user's ID
            if (parameter.ToString() == "Color")
                return (senderId == 1) ? "#FF6B81" : "#A0A0A0";

            if (parameter.ToString() == "Align")
                return (senderId == 1) ? HorizontalAlignment.Right : HorizontalAlignment.Left;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}