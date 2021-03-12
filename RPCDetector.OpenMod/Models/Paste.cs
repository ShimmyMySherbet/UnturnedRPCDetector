using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace ShimmyMySherbet.RPCDetector.Models
{
    public class PasteMessage
    {
        public bool encrypted = false;
        public PasteSection[] sections;

        public PasteMessage(string content)
        {
            PasteSection s = new PasteSection();
            s.contents = content;
            sections = new PasteSection[] { s };
        }
    }

    public class PasteSection
    {
        public string name = "RPC Detector Report";
        public string syntax = "text";
        public string contents;
    }

    public class PasteResponse
    {
        public string id;
        public string link;
    }

    public class PasteAPI
    {
        public static PasteResponse Upload(string content)
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://api.paste.ee/v1/pastes?key=aRVTkwqPgdnHqFp7GiZfppd4OS1uVLEWRUSmcEUTw");
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new PasteMessage(content)));
            request.ContentLength = payload.Length;
            using (var s = request.GetRequestStream())
            {
                s.Write(payload, 0, payload.Length);
                s.Flush();
            }
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            string js;
            using (var st = resp.GetResponseStream())
            using (StreamReader reader = new StreamReader(st))
            {
                js = reader.ReadToEnd();
            }
            PasteResponse re = JsonConvert.DeserializeObject<PasteResponse>(js);
            return re;
        }
    }
}