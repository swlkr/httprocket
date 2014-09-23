using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HttpRocket {
	public interface IHttpRocket {
		Response Get();
		Response Put(string requestData);
		Response Post(string requestData);
		Response Delete();
		string ContentType { get; set; }
		string Method { get; set; }
	}

	public class Request : IHttpRocket {
		public HttpWebRequest HttpWebRequest { get; set; }
		public string ContentType { get; set; }
		public string Url { get; set; }
		public string Method { get; set; }
		public Dictionary<string, string> Headers { get; set; }

		public Request(string url) {
			Url = url;
			ContentType = ContentType ?? "application/x-www-form-urlencoded";
			Headers = new Dictionary<string, string>();
		}

		public void CreateRequest(string method, string requestData, Dictionary<string, string> headers) {
			HttpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
			HttpWebRequest.Method = method;
			HttpWebRequest.ContentType = ContentType;
			HttpWebRequest.ContentLength = 0;
			foreach (var header in headers) {
				HttpWebRequest.Headers.Add(header.Key, header.Value);
			}

			SetRequestData(requestData);
		}

		public Response Get() {
			return MakeRequest(WebRequestMethods.Http.Get, null, Headers);
		}

		public Response Post(string requestData) {
			return MakeRequest(WebRequestMethods.Http.Post, requestData, Headers);
		}

		public Response Put(string requestData) {
			return MakeRequest(WebRequestMethods.Http.Put, requestData, Headers);
		}

		public Response Delete() {
			return MakeRequest("DELETE", null, Headers);
		}

		public Response MakeRequest(string method, string requestData) {
			return MakeRequest(method, requestData, new Dictionary<string, string>());
		}

		public Response MakeRequest(string method, string requestData, Dictionary<string, string> headers) {
			CreateRequest(method, requestData, headers);

			try {
				using (var response = HttpWebRequest.GetResponse())
				using (var responseStream = response.GetResponseStream())
				using (var responseStreamReader = new StreamReader(responseStream)) {

					var responseFromServer = responseStreamReader.ReadToEnd();
					return new Response(((HttpWebResponse)response).StatusCode, responseFromServer);

				}
			} catch (System.Net.WebException e) {
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

		public void SetRequestData(string requestData) {
			if (String.IsNullOrEmpty(requestData)) return;

			var requestDataBuffer = Encoding.Default.GetBytes(requestData);
			HttpWebRequest.ContentLength = requestDataBuffer.Length;
			HttpWebRequest.GetRequestStream()
				.Write(requestDataBuffer, 0, requestDataBuffer.Length);
		}
	}

	public class Response {
		private readonly HttpStatusCode httpStatusCode;
		private readonly string responseBody;

		public Response() { }

		public Response(HttpStatusCode httpStatusCode, string responseBody) {
			this.httpStatusCode = httpStatusCode;
			this.responseBody = responseBody;
		}

		public virtual string ResponseBody {
			get { return responseBody; }
		}

		public virtual HttpStatusCode StatusCode {
			get { return httpStatusCode; }
		}
	}
}
