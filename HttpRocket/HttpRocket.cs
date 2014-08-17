using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpRocket
{
    public interface IHttpRocket
    {
        HttpRocketResponse Get();
        HttpRocketResponse Put(string requestData);
        HttpRocketResponse Post(string requestData);
        string ContentType { get; set; }
        string Method { get; set; }
    }

    public class HttpRocket : IHttpRocket
    {
        private readonly HttpWebRequest httpWebRequest;
        public string ContentType { get; set; }
        public string Method
        {
            get { return httpWebRequest.Method; }
            set { httpWebRequest.Method = value; }
        }

        public HttpRocket(string url)
            : this((HttpWebRequest)WebRequest.Create(url)) { }

        public HttpRocket(HttpWebRequest httpWebRequest)
        {
            this.httpWebRequest = httpWebRequest;
            ContentType = "application/x-www-form-urlencoded";
        }

        public HttpRocketResponse Get()
        {
            return MakeRequest(WebRequestMethods.Http.Get, null);
        }

        public HttpRocketResponse Post(string requestData)
        {
            return MakeRequest(WebRequestMethods.Http.Post, requestData);
        }

        public HttpRocketResponse Put(string requestData)
        {
            return MakeRequest(WebRequestMethods.Http.Put, requestData);
        }

        private HttpRocketResponse MakeRequest(string method, string requestData)
        {
            httpWebRequest.Method = method;
            httpWebRequest.ContentType = ContentType;
            httpWebRequest.ContentLength = 0;

            try
            {
                SetRequestData(requestData);

                var response = httpWebRequest.GetResponse();
                var responseStream = response.GetResponseStream();
                var responseStreamReader = new StreamReader(responseStream);
                var responseFromServer = responseStreamReader.ReadToEnd();

                responseStreamReader.Close();
                responseStream.Close();
                response.Close();

                return new HttpRocketResponse(((HttpWebResponse)response).StatusCode, responseFromServer);
            }
            catch (System.Net.WebException e)
            {
                return new HttpRocketResponse(HttpStatusCode.NotFound, e.Message);
            }
        }

        private void SetRequestData(string requestData)
        {
            if (String.IsNullOrEmpty(requestData)) return;

            var requestDataBuffer = Encoding.Default.GetBytes(requestData);
            httpWebRequest.ContentLength = requestDataBuffer.Length;
            httpWebRequest.GetRequestStream()
                .Write(requestDataBuffer, 0, requestDataBuffer.Length);
        }
    }

    public class HttpRocketResponse
    {
        private readonly HttpStatusCode httpStatusCode;
        private readonly string responseBody;

        public HttpRocketResponse() { }

        public HttpRocketResponse(HttpStatusCode httpStatusCode, string responseBody)
        {
            this.httpStatusCode = httpStatusCode;
            this.responseBody = responseBody;
        }

        public virtual string ResponseBody
        {
            get { return responseBody; }
        }

        public virtual HttpStatusCode StatusCode
        {
            get { return httpStatusCode; }
        }
    }
}
