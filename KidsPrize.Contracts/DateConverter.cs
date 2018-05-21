using Newtonsoft.Json.Converters;

namespace KidsPrize.Contracts
{
    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}