using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KenshinChat.Server.Models
{
    public class ApiDbContext
    {
        private string datastore;

        public List<User> Users { get; set; }

        public ApiDbContext()
        {
            this.datastore = "./Datastore";
            InitializeFieldValues();
        }

        private void InitializeFieldValues()
        {
            //Get Users
            string filename = "/users.json";
            string json = File.ReadAllText(this.datastore + filename);
            this.Users = JsonConvert.DeserializeObject<List<User>>(json);
        }

        public void SaveChanges()
        {
            //Save Users
            string json = JsonConvert.SerializeObject(this.Users);
            string filename = "/users.json";
            File.WriteAllText(this.datastore + filename, json);
        }
    }
}
