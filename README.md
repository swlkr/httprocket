# HttpRocket
__A simple, testable c# http library__

### Usage

using HttpRocket;

var r = new Request("http://google.com");
var response = r.Get();

var r = new Request("http://your-api.com/users");
var response = r.Put("{id: 1, name: 'arnold'}");