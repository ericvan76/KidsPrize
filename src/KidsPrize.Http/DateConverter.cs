using Newtonsoft.Json.Converters;

namespace KidsPrize.Http
{
    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}