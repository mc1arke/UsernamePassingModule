# UsernamePassingModule
An HTTPModule for IIS to allow the username of the currently authenticated user to be passed to a downstream server when IIS is running as a reverse proxy of URL rewriter

## Build
Clone the project, open in Visual Studio and build using the normal Visual Studio building process

## Install
To install this module in II7 (in pipeline mode) or above:

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

## Advanced Configuration
It is possible to pass additional parameters to this module using a custom config setion:
1. If your Web.xml does not already contain a 'configSections' element then add one in the root 'configuration' element of your Web.xml
2. Add a 'section' element to this 'configSections' with an element named 'name' with the value of 'usernamePassingModule' and a second attributed named 'type' and with a value of 'System.Configuration.NameValueSectionHandler'
3. Add a 'usernamePassingModule' element to the root 'configuration' element
4. Add an 'add' element as a child of the 'usernamePaddingModule' element for each configuration element you want to pass, e.g. to configure the name used in the HTTP header add an 'add' element with a paremeter named 'key' and with the value of 'headerName', and a second parametere with the name 'value' and the value set to the name you want the header sent under. This value must be valid for an HTTP header name: containing only Alphanumeric characters, potentially seperated by hyphens.

To setup the module to pass the username header under the name 'Authenticated-User', you Web.config needs to look similar to following:
~~~
   <configuration>
      <configSections>
         <section name="usernamePassingModule" type="System.Configuration.NameValueSectionHandler" />
      </configSections>

      <system.webServer>
         <modules>
            <add name="Username" type="UsernamePassingModule.UsernamePassingModule"/>
         </modules>
      </system.webServer>

      <usernamePassingModule>
         <add key="headerName" value="Authenticated-User"/>
      </usernamePassingModule>

   </configuration>
~~~
