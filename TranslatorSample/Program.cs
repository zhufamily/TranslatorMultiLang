using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace TranslatorSample
{
    /// <summary>
    /// A sample to handling multiple langauge for Azure Translator
    /// </summary>
    internal class MultiLangSample
    {
        /// <summary>
        /// Translator url
        /// </summary>
        static string _url = ConfigurationManager.AppSettings["translatorUrl"];
        /// <summary>
        /// Translator key
        /// </summary>
        static string _key = ConfigurationManager.AppSettings["translatorKey"];
        /// <summary>
        /// Translator region
        /// </summary>
        static string _region = ConfigurationManager.AppSettings["translatorRegion"];
        /// <summary>
        /// Original text
        /// </summary>
        static string _originalText = File.ReadAllText("multilang.txt");
        /// <summary>
        /// Traget language -- hardcoded for "en" -- just a sample
        /// </summary>
        static string _toLanguage = "en";
        /// <summary>
        /// Http Client
        /// </summary>
        static HttpClient _client = new HttpClient();

        /// <summary>
        /// Sample entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            List<string> chunks = GenerateSentence(_originalText);
            StringBuilder sb = new StringBuilder();
            
            foreach(string chunk in chunks) 
            {
                List<string> fromLangs = DetectLanguage(chunk);
                string copiedChunk = new string(chunk);
                foreach(string lang in fromLangs) 
                {
                    if (lang.ToLower() == _toLanguage.ToLower())
                    {
                        continue;
                    }
                    else
                    {
                        copiedChunk = Translate(copiedChunk, lang);
                    }
                }
                sb.AppendLine(copiedChunk);
            }

            Console.Write(sb.ToString());
        }

        /// <summary>
        /// Chunk by sentence
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        static List<string> GenerateSentence(string inputText)
        { 
            List<string> sentences = new List<string>();
            HttpRequestMessage msg = new HttpRequestMessage();
            msg.Method = HttpMethod.Post;
            msg.RequestUri = new Uri($"{_url}breaksentence?api-version=3.0");
            msg.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            msg.Headers.Add("Ocp-Apim-Subscription-Region", _region);

            object[] data = new object[] { new { Text = inputText } };
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            msg.Content = content;

            HttpResponseMessage response = _client.Send(msg);
            string resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            dynamic respObj = JsonConvert.DeserializeObject<dynamic>(resp);
            JArray sentLen = (JArray)respObj[0]["sentLen"];
            int curPos = 0;
            foreach (JToken slen in sentLen)
            {
                int curLen = (int) slen;
                string sent = inputText.Substring(curPos, curLen);
                sentences.Add(sent); 
                curPos += curLen;
            }
            return sentences;
        }

        /// <summary>
        /// Detect multiple language
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        static List<string> DetectLanguage(string inputText)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            msg.Method = HttpMethod.Post;
            msg.RequestUri = new Uri($"{_url}detect?api-version=3.0");
            msg.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            msg.Headers.Add("Ocp-Apim-Subscription-Region", _region);

            object[] data = new object[] { new { Text = inputText } };
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            msg.Content = content;

            HttpResponseMessage response = _client.Send(msg);
            string resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            dynamic respObj = JsonConvert.DeserializeObject<dynamic>(resp);

            List<string> languages = new List<string>();
            foreach (JObject lang in ((JArray)respObj))
            {
                languages.Add((string)lang["language"]);
                JToken altLangs = lang["alternatives"];
                if (altLangs != null)
                {
                    foreach (JObject altLang in ((JArray)altLangs))
                    {
                        languages.Add((string)altLang["language"]);
                    }
                }
            }
            return languages;
        }

        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="fromLang"></param>
        /// <returns></returns>
        static string Translate(string inputText, string fromLang)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            msg.Method = HttpMethod.Post;
            msg.RequestUri = new Uri($"{_url}translate?api-version=3.0&to={_toLanguage}&from={fromLang}");
            msg.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            msg.Headers.Add("Ocp-Apim-Subscription-Region", _region);

            object[] data = new object[] { new { Text = inputText } };
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            msg.Content = content;

            HttpResponseMessage response = _client.Send(msg);
            string resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            dynamic respObj = JsonConvert.DeserializeObject<dynamic>(resp);
            string translatedText = (string)respObj[0]["translations"][0]["text"];
            return translatedText;
        }
    }
}