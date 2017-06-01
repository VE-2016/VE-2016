using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinExplorer.Services.Extensions
{

    public class Publisher
    {
        public string publisherId { get; set; }
        public string publisherName { get; set; }
        public string displayName { get; set; }
        public string flags { get; set; }
    }

    public class Extension
    {
        public Publisher publisher { get; set; }
        public string extensionId { get; set; }
        public string extensionName { get; set; }
        public string displayName { get; set; }
        public string flags { get; set; }
        public string lastUpdated { get; set; }
        public string publishedDate { get; set; }
        public string releaseDate { get; set; }
        public string shortDescription { get; set; }
        public int deploymentType { get; set; }
    }

    public class MetadataItem
    {
        public string name { get; set; }
        public int count { get; set; }
    }

    public class ResultMetadata
    {
        public string metadataType { get; set; }
        public List<MetadataItem> metadataItems { get; set; }
    }

    public class Result
    {
        public List<Extension> extensions { get; set; }
        public object pagingToken { get; set; }
        public List<ResultMetadata> resultMetadata { get; set; }
    }

    public class RootObject
    {
        public List<Result> results { get; set; }
    }

    public class ExtensionManager
    {

        public RootObject GetExtensions()
        {
            JsonTextReader reader = null;


            try { 

             string exts = LoadExtensions();


            JObject result = JObject.Parse(exts);
            reader = new JsonTextReader(new System.IO.StringReader(result.ToString()));
            reader.SupportMultipleContent = true;
        }
        catch(Exception)
        {}

            if (reader == null)
                return null;

            JsonSerializer serializer = new JsonSerializer();

            RootObject r  = serializer.Deserialize<RootObject>(reader);

            return r;
}


        public string LoadExtensions()
        {
            string uri = "https://marketplace.visualstudio.com:443//_apis//public//gallery//extensionquery";

            var request = (HttpWebRequest)WebRequest.Create("https://marketplace.visualstudio.com:443//_apis//public//gallery//extensionquery");

            //var post = @"{""filters"":[{""criteria"":[""filterType"":""5"",""value"":""templates""}],""pageNumber"":""1"",""pageSize"":""5000"",""sortBy"":""1"",""sortOrder"":""0""}],""assetTypes"":[]}";

            var post = @"{""filters"":[{""criteria"":[{""filterType"":""5"",""value"":""templates""}]}]}";

            //   richTextBox1.Text = post;

            var postData = post;// richTextBox1.Text;

            var data = System.Text.ASCIIEncoding.UTF8.GetBytes(postData);


            //var httpClient = new WebClient();
            //httpClient.Headers.Add("Content-Type", "application/json");
            //httpClient.Headers.Add("Accept", "api-version=3.0-preview.1");

            //var responses = httpClient.UploadData(uri, "POST", data);

            //string v = Encoding.UTF8.GetString(responses);

            //richTextBox1.AppendText(v);

            //return;

            request.Method = WebRequestMethods.Http.Post;
            ///request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Accept = "api-version=3.0-preview.1";


            //var writer = new System.IO.StreamWriter(request.GetRequestStream());
            //writer.Write(postData);
            //writer.Close();
            //var response = request.GetResponse();
            //var reader = new System.IO.StreamReader(response.GetResponseStream());
            //var responseText = reader.ReadToEnd();




            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);

            }

            try
            {

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                //richTextBox1.Text = responseString;

                return responseString;

            }
            catch (WebException ex)
            {
                
            }

            return "";
        }



    }
}