using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ShimmyMySherbet.RPCDetector.Models
{
    public class HasteAPI
    {
        public string Endpoint = "https://haste.shimmymysherbet.com/";

        public string Upload(string content)
        {
            HttpWebRequest request = WebRequest.CreateHttp(Endpoint + "documents");
            request.Method = "POST";
            using (MemoryStream st = new MemoryStream())
            using (StreamWriter wr = new StreamWriter(st))
            {
                wr.Write(content);
                wr.Flush();

                request.ContentLength = st.Length;
                st.Position = 0;

                using (Stream writes = request.GetRequestStream())
                {
                    st.CopyTo(writes);
                    writes.Flush();
                }

                HttpWebResponse resp = (HttpWebResponse)request.GetResponse();

                string json;
                using (Stream read = resp.GetResponseStream())
                using (StreamReader rd = new StreamReader(read))
                {
                    json = rd.ReadToEnd();
                }
                Rsp r = JsonConvert.DeserializeObject<Rsp>(json);

                return Endpoint + r.key;
            }
        }

        internal class Rsp
        {
            public string key;
        }
    }
}