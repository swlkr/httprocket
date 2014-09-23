using System;
using HttpRocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HttpRocket.Test {
	[TestClass]
	public class RequestTest {

		[TestMethod]
		public void CreateRequestWithAuthorizationHeader() {
			var r = new Request("http://httpstat.us/200") { Headers = new Dictionary<string, string>() { { "Authorization", "OAuth xxxxx" } } };
			r.Get();
			Assert.AreEqual("OAuth xxxxx", r.HttpWebRequest.Headers.Get("Authorization"));
		}
	}
}
