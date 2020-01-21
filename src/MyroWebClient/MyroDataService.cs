using MyroWebClient.Entities;
using MyroWebClient.Exceptions;

namespace MyroWebClient
{
    public class MyroDataService
    {
        private HttpService _httpService;
        private DataParser _dataParser;

        public MyroDataService(HttpService httpService, DataParser dataParser)
        {
            _httpService = httpService;
            _dataParser = dataParser;
        }

        public Grades GetAllMyroData(User user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.SchoolAbreviation))
            {
                throw new IncompleteFormsException();
            }

            var rawData = _httpService.GetRawData(user.UserName, user.Password, user.SchoolAbreviation);
            return _dataParser.ConvertDataToObject(rawData);
        }
    }
}
