# Request Mocker
This is a simple package that will allow to mock the API endpoint at runtime.

# How to use
Just add the RequestMocker Package, map the expected response to end point and you are done

# Example
The following example demonstrate  how to use this library
1) After importing the Request Mocker library add the Mocker option line in the configuration service section of the startup file
'''
services.AddRequestMocker(x => {
    // this represent an entry in the routing table
	x.Map(HttpMethod.Get, "/sample", new { test = "test" });                  
});
'''
2) Add the ''' app.UseRequestMocker(); ''' to the Configuration function as the first line. This is important since this will overtake any routing, if there is no match found in the routing table it will pass-it to the next part of the pipeline else it will intercept it and replay the assigned object with that rout

