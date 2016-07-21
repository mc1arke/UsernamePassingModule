# UsernamePassingModule
An HTTPModule for IIS to allow the username of the currently authenticated user to be passed to a downstream server when IIS is running as a reverse proxy of URL rewirter

## Building
Clone the project, open in Visual Studio and build using the normal Visual Studio building process

## Install
To install this module in II7 or above:

1. Copy UsernamePassingModule.dll from output directory (debug/bin or release/bin) to the bin directory of the IIS site you're wanting to pass a username from
2. Add a Web.config, or update your existing Web.config in the root of your site, so it contains a system.WebServer element, containing a modules element, with an add entry for the UsernamePassingModule, similar to:
~~~
  <configuration>
	  <system.webServer>
		  <modules>
			  <add name="Username" type="UsernamePassingModule.UsernamePassingModule"/>
		  </modules>
	  </system.webServer>
  </configuration>
~~~
