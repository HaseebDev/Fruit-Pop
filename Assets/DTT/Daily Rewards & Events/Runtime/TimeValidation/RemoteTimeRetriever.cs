using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace DTT.DailyRewards
{
    /// <summary>
    /// A class that attempts to retrieve a unix timestamp from a remote API.
    /// </summary>
    public class RemoteTimeRetriever
    {
        /// <summary>
        /// The url of the API to check against.
        /// </summary>
        protected readonly string URL;

        /// <summary>
        /// The key to use for json, if left null, then
        /// assume it is a string answer.
        /// </summary>
        protected readonly string JsonKey;
        
        /// <summary>
        /// how long to cache the results for;
        /// </summary>
        /// <returns></returns>
        protected readonly int cacheTimeout = 3;

        /// <summary>
        /// The last time retrieved by the API
        /// </summary>
        protected int lastCachedTime = 0;
        
        /// <summary>
        /// At what time the cached time was retrieved
        /// (using local unix Time).
        /// </summary>
        protected int lastCachedAt = 0;

        /// <summary>
        /// Setup the remote time retriever.
        /// </summary>
        /// <param name="url">The url of the API.</param>
        /// <param name="jsonKey">The json key, if given for the unix time, else assume it's just a string</param>
        /// <param name="cacheTimeoutInSeconds"></param>
        public RemoteTimeRetriever(string url, string jsonKey = null, int cacheTimeoutInSeconds = -1)
        {
            URL = url;
            JsonKey = jsonKey;
            cacheTimeout = cacheTimeoutInSeconds;
        }

        /// <summary>
        /// Attempts to retrieve a unix timestamp from a remote API
        /// given by the URL and optionally the JSON key.
        /// </summary>
        /// <param name="time">the time retrieved from the API if found, else null</param>
        public void Retrieve(Action<int?> time)
        {
            int currentSystemTime = UnixHelper.GetCurrentUnixTime();
            if (lastCachedAt + cacheTimeout > currentSystemTime)
            {
                time?.Invoke(lastCachedTime + (currentSystemTime - lastCachedAt));
                return;
            }
            try
            {
                AsyncGetResponse(response =>
                {
                    int? result = HandleResponse(response);
                    Debug.Log($"response {response}\n result: {result}");
                    if (result != null)
                    {
                        lastCachedTime = (int)result;
                        lastCachedAt = UnixHelper.GetCurrentUnixTime();
                    }
                    time?.Invoke(result);
                });
            }
            catch (Exception e)
            {
                Debug.LogError("Failure to get time from the remote API");
                Debug.LogException(e);
                time?.Invoke(null);

            }
        }

        /// <summary>
        /// Attempt to get a response from the API provided.
        /// </summary>
        /// <param name="response">The Response from the API.</param>
        private void AsyncGetResponse(Action<string> response)
        {
            UnityWebRequest www = UnityWebRequest.Get(URL);
            UnityWebRequestAsyncOperation request = www.SendWebRequest();
            request.completed += (result) => OnCompleteDownload();

            void OnCompleteDownload()
            {
                // Response code 200 = SUCCESS / OK.
                if (www.responseCode != 200)
                {
                    Debug.Log(www.error);
                    response?.Invoke(null);
                }
                else
                {
                    response?.Invoke(www.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Attempt to convert the API response to a unix time
        /// Using either a JSON key or a literal string.
        /// </summary>
        /// <param name="response">Unix time if possible </param>
        /// <returns>An int if found, else it </returns>
        private int? HandleResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }
            return string.IsNullOrEmpty(JsonKey) ? ExtractStringTime(response) : ExtractJsonTime(JsonKey, response);
        }

        /// <summary>
        /// Try parse an exact string to a unix timestamp.
        /// </summary>
        /// <param name="response">Response from the api.</param>
        /// <returns>.Unix timestamp if able to convert, else null.</returns>
        private int? ExtractStringTime(string response)
        {
            string justNumbersString = new string(response.Where(char.IsDigit).ToArray());

            if (int.TryParse(justNumbersString, out int result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Attempt to extract a unix timestamp from a json object.
        /// </summary>
        /// <param name="key">the key of the unix timestamp.</param>
        /// <param name="jsonResponse">The response of the API.</param>
        /// <returns>The unix time if possible.</returns>
        private int? ExtractJsonTime(string key, string jsonResponse)
        {
            JsonNode json = Json.Parse(jsonResponse);
            string time = json[key].Value;
            return ExtractStringTime(time);
        }
    }
}