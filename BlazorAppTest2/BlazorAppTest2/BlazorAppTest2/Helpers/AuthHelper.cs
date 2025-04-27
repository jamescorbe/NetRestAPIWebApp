using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NETCore.Encrypt;

namespace BlazorAppTest2.Helpers
{
    
    public class AuthHelper
    {
        private string _fileLocation;

        // not ideal but at least they are stored seperately 
        private string _key;

        public AuthHelper(string fileLocation, string appID) 
        {
            _fileLocation = fileLocation;
            _key = appID;

            GoogleAuth = ReadFromFile();
        }

        public GoogleAuth ReadFromFile()
        {
            if (File.Exists(_fileLocation))
            {
                var fileContents = File.ReadAllText(_fileLocation);
                var authHelper = JsonSerializer.Deserialize<GoogleAuth>(fileContents);
                if (authHelper != null)
                {
                    GoogleAuth.ClientID = EncryptProvider.AESDecrypt(authHelper.ClientID, _key);
                    GoogleAuth.ClientSecret = EncryptProvider.AESDecrypt(authHelper.ClientSecret, _key);

                    return authHelper;
                }
            }

            throw new Exception("Could not get auth from file");
        }
        public void WriteToFile()
        {
            // encrypt before storage
            GoogleAuth.ClientID = EncryptProvider.AESEncrypt(GoogleAuth.ClientID, _key);
            GoogleAuth.ClientSecret = EncryptProvider.AESEncrypt(GoogleAuth.ClientSecret, _key);

            var authHelper = JsonSerializer.Serialize(GoogleAuth);

            if (_fileLocation != string.Empty || File.Exists(_fileLocation))
            {
                File.WriteAllText(_fileLocation, authHelper);
            }
        }

        // 
        public void WriteInitialData(string clientID, string clientSecret)
        {
            GoogleAuth.ClientID = clientID;
            GoogleAuth.ClientSecret = clientSecret;

            WriteToFile();
        }


        public GoogleAuth GoogleAuth { get; set; } = new GoogleAuth();

    }
}
