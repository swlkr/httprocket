using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpRocket
{
    public interface IHttpRocket
    {
        Response Get();
		Response Put(string requestData);
		Response Post(string requestData);
		Response Delete();
        string ContentType { get; set; }
        string Method { get; set; }
    }

    public class Request : IHttpRocket
    {
        private readonly HttpWebRequest httpWebRequest;
        public string ContentType { get; set; }
        public string Method
        {
            get { return httpWebRequest.Method; }
            set { httpWebRequest.Method = value; }
        }

        public Request(string url)
            : this((HttpWebRequest)WebRequest.Create(url)) { }

        public Request(HttpWebRequest httpWebRequest)
        {
            this.httpWebRequest = httpWebRequest;
            ContentType = "application/x-www-form-urlencoded";
        }

        public Response Get()
        {
            return MakeRequest(WebRequestMethods.Http.Get, null);
        }

		public Response Post(string requestData)
        {
            return MakeRequest(WebRequestMethods.Http.Post, requestData);
        }

		public Response Put(string requestData)
        {
            return MakeRequest(WebRequestMethods.Http.Put, requestData);
        }

		public Response Delete() {
			return MakeRequest("DELETE", null);
		}

		private Response MakeRequest(string method, string requestData)
        {
            httpWebRequest.Method = method;
            httpWebRequest.ContentType = ContentType;
            httpWebRequest.ContentLength = 0;

            try
            {
                SetRequestData(requestData);
				
                using( var response = httpWebRequest.GetResponse())
                using( var responseStream = response.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream)){
	                
					var responseFromServer = responseStreamReader.ReadToEnd();
					return new Response(((HttpWebResponse)response).StatusCode, responseFromServer);

                }
            }
            catch (System.Net.WebException e)
            {
				using (var response = e.Response) {
					using (var responseStream = response.GetResponseStream()) {
						using (var responseStreamReader = new StreamReader(responseStream)) {
							var responseFromServer = responseStreamReader.ReadToEnd();
							return new Response(HttpStatusCode.NotFound, responseFromServer);
						}
					}
				}
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

    public class Response
    {
        private readonly HttpStatusCode httpStatusCode;
        private readonly string responseBody;

        public Response() { }

        public Response(HttpStatusCode httpStatusCode, string responseBody)
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
