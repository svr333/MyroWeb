using MyroWebClient.Entities;
using MyroWebClient.Exceptions;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;

namespace MyroWebClient
{
    public class MyroDatabase
    {
        private MyroDataService _myro;

        private string dbLocation = $"Resources/users.json";
        private ConcurrentDictionary<string, User> _users =  new ConcurrentDictionary<string, User>();

        public MyroDatabase(MyroDataService myro)
        {
            _myro = myro;
            LoadUsersInMemory();
        }  

        private void LoadUsersInMemory()
        {
            Directory.CreateDirectory($"Resources");
            if (!File.Exists(dbLocation)) File.WriteAllText(dbLocation, "");
            

            var json = File.ReadAllText(dbLocation);
            _users = JsonConvert.DeserializeObject<ConcurrentDictionary<string, User>>(json);
        }

        public User GetUserByKey(string key)
        {
            _users.TryGetValue(key, out User user);

            return user;
        }

        public void StoreNewUser(string key, User user)
        {
            // VALIDATE IF USER IS CORRECT, IF NOT -> THROW
            if (_myro.GetAllMyroData(user) == null) throw new WrongLoginException();

            _users.AddOrUpdate(key, user, (oldKey, oldUser) => user);

            var json = JsonConvert.SerializeObject(_users);
            File.WriteAllText(dbLocation, json);
        }
    }
}
