using System;
using HttpRocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpRocket.Test {
	[TestClass]
	public class RequestTest {

		[TestMethod]
		public void CreateRequestWithAuthorizationHeader() {
			var r = new Request("http://httpstat.us/200");
			r.CreateRequest("GET", "", new System.Collections.Generic.Dictionary<string, string>() { { "Authorization", "OAuth xxxxx" } });
			Assert.AreEqual("OAuth xxxxx", r.HttpWebRequest.Headers.Get("Authorization"));
		}
	}
}
