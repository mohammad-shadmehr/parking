using Newtonsoft.Json;

namespace Parking.Infrastructure.Helpers
{
    public static class JsonHelper
    {
        public static T Deserialise<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static string Serialise<T>(T input)
        {
            return JsonConvert.SerializeObject(input);
        }
    }
}
