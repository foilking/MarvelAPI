using MarvelAPI.Exceptions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;


[assembly: InternalsVisibleTo("MarvelAPI.Test")]

namespace MarvelAPI
{
    public class BaseRequest
    {
        private string _publicApiKey { get; set; }
        private string _privateApiKey { get; set; }
        private bool _useGZip { get; set; }
        protected IRestClient Client;
        
        public BaseRequest(string publicApiKey, string privateApiKey, IRestClient client, bool? useGZip = null)
        {
            _publicApiKey = publicApiKey;
            _privateApiKey = privateApiKey;
            _useGZip = useGZip.HasValue ? useGZip.Value : false;

            Client = client;
        }

        private string CreateHash(string input)
        {
            var hash = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                hash = sBuilder.ToString();
            }
            return hash;
        }

        internal RestRequest CreateRequest(string requestUrl)
        {
            var request = new RestRequest(requestUrl);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();

            request.AddParameter("apikey", _publicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(string.Format("{0}{1}{2}", timestamp, _privateApiKey, _publicApiKey)));

            if (_useGZip)
            {
                request.AddHeader("Accept-Encoded", "gzip");
            }
            else
            {
                request.AddHeader("Accept", "*/*");
            }

            return request;
        }

        internal void HandleResponseErrors<T>(IRestResponse<T> response)
        {
            var code = 0;
            var status = string.Empty;
            var responseStatus = response.ResponseStatus;
            if (responseStatus == ResponseStatus.Error)
            {
                var content = JsonConvert.DeserializeObject<MarvelError>(response.Content);
                switch (content.Code)
                {
                    case "InvalidCredentials":
                        code = 401;
                        status = content.Message;
                        break;
                    case "RequestThrottled":
                        code = 429;
                        status = content.Message;
                        break;
                    default:
                        code = 404;
                        status = content.Message;
                        break;
                }
            }
            else
            {
                var data = response.Data as BaseWrapper;
                if (data != null)
                {
                    code = data.Code;
                    status = data.Status;
                }
            }

            switch (code)
            {
                case 409:
                    throw new ArgumentException(status);
                case 404:
                    throw new NotFoundException(status, response.ErrorException);
                case 401:
                    throw new InvalidCredentialException(status);
                case 429:
                    throw new LimitExceededException(status);
            }
        }
    }
}
