using System;
using HttpRocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpRocket.Test
{
	[TestClass]
	public class ResponseTest
	{
		[TestMethod]
		public void GetTest() {
			var r = new Request("http://httpstat.us/200");
			var response = r.Get();
			Assert.AreEqual("200 OK", response.ResponseBody);
		}

		[TestMethod]
		public void ServerShouldReturnErrorDetails() {
			var r = new Request("http://httpstat.us/500");
			var response = r.Get();
			Assert.AreEqual("500 Internal Server Error", response.ResponseBody);
		}

		[TestMethod]
		public void Teapot() {
			var r = new Request("http://httpstat.us/418");
			var response = r.Get();
			Assert.AreEqual("418 I'm a teapot", response.ResponseBody);
		}
	}
}
