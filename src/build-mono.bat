# Build core Handlebars library
mcs -recurse:Handlebars\*.cs -out:..\build\mono\Handlebars.dll -target:library -unsafe+ -o+ -r:reference\mono\Ninject.dll,reference\mono\Newtonsoft.Json.dll,System.Configuration.dll -nowarn:0414,0169,0618
# Build the node launcher and proxy
mcs -recurse:Engine\NodeEngine\*.cs -out:..\build\mono\NodeEngine.dll -target:library -unsafe+ -o+ -r:reference\mono\Ninject.dll,reference\mono\Newtonsoft.Json.dll,System.Configuration.dll,System.Net.dll,System.Net.Http.dll,..\build\mono\Handlebars.dll -nowarn:0414,0169,0618
# Build the proxy exe for execution on macs
mcs -recurse:src\Proxy\Handlebars.Proxy\*.cs -out:build\mono\handlebars-proxy.exe -target:exe -unsafe+ -o+ -r:src\reference\mono\Ninject.dll,src\reference\mono\Newtonsoft.Json.dll,System.Configuration.dll,System.Net.dll,System.Net.Http.dll,build\mono\Handlebars.dll,build\mono\NodeEngine.dll,src\reference\mono\NDesk.Options.dll,src\reference\mono\Microsoft.Owin.dll,src\reference\mono\Microsoft.Owin.Hosting.dll,src\reference\mono\Owin.dll -nowarn:0414,0169,0618