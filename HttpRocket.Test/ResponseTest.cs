using System;
using HttpRocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace HttpRocket.Test
{
	[TestClass]
	public class ResponseTest
	{
		[TestMethod]
		public void GetShouldReturnCorrectResponse() {
			var r = new Request("http://httpstat.us/200");
			var response = r.Get();
			Assert.AreEqual("200 OK", response.ResponseBody);
		}

		[TestMethod]
		public void ResponseShouldReturnErrorDetails() {
			var r = new Request("http://httpstat.us/500");
			var response = r.Get();
			Assert.AreEqual("500 Internal Server Error", response.ResponseBody);
		}

		[TestMethod]
		public void ErrorResponseShouldReturnCorrectStatusCode() {
			var r = new Request("http://httpstat.us/500");
			var response = r.Get();
			Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
		}

		[TestMethod]
		public void NotFoundResponseShouldReturnCorrectStatusCode() {
			var r = new Request("http://httpstat.us/404");
			var response = r.Get();
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		}

		[TestMethod]
		public void TeapotResponse() {
			var r = new Request("http://httpstat.us/418");
			var response = r.Get();
			Assert.AreEqual("418 I'm a teapot", response.ResponseBody);
		}
	}
}
