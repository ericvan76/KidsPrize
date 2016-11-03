using Newtonsoft.Json.Converters;

namespace KidsPrize.Converters
{
    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}