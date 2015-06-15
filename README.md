Transformer
===========

![CI status](http://img.shields.io/teamcity/http/powerdeploy-buildserver.cloudapp.net/e/Transformer_CI.svg) ![NuGet Downloads](http://img.shields.io/nuget/dt/Transformer.VisualStudio.svg) ![NuGet Version](http://img.shields.io/nuget/v/Transformer.VisualStudio.svg)

Got tired of ms config-transform? Transformer is a simple template engine (based on string-replacements) which lets you transform any files (like `web.config` or `app.configs` to a specific environment. Config transformation should really be **very simple** and readable: simple placeholder replacements in a template.

![Transformer Live Demo](https://raw.githubusercontent.com/tobiaszuercher/transformer/master/doc/images/transformer-demo.gif)

### How it works
In order to create a template file for your config file `xxxxx.yyy`, just create a file named `xxxxx.template.yyy`.
Example:
* `App.template.config`   --> `App.config`
* `Web.template.config`  --> `Web.config`

With Visual Studio file nesting it looks like other generated code: 

![Visual Studio file nesting](https://raw.githubusercontent.com/tobiaszuercher/transformer/master/doc/images/template_nesting.png)

Inside the template file you can use `${variable}` as placeholders. Example:

```xml
<?xml version="1.0" encoding =" utf-8" ?>
<configuration>
  <appSettings>
    <add key="firstname" value= "${firstname}" />
    <add key="lastname" value= "${lastname}" />
  </appSettings>
</configuration>
```

For each environment create a xml file named to the environment.

`local.xml`

```xml
<?xml version="1.0"?>
<environment description="local environment">
  <variable name="firstname" value="Jack " />
  <variable name="lastname" value="Bauer " />
</environment>
```

Note: you can use `${env}` which contains the name of the environment.

`dev.xml`
```xml
<? xml version=" 1.0" ?>
<environment description="dev system for developers" >
  <variable name="firstname" value="Tobias" />
  <variable name="lastname" value="Zuercher " />
</environment>
```

The environment files must be stored in a folder called `.transformer`. During transformation, the engine will look for the `.transformer` folder in all parent paths starting with the targeting path.

### Quickstart for your Project
  1. `Install-Package Transformer.VisualStudio`
  2. Add a `.transformer` folder in any direct parent folder from your project
  3. Add environment files "dev.xml"
  4. Add Web.template.config
  5. Start adding variables to the template
  6. Use NuGet Package Manager Console: Switch-Environment Cmdlet to switch between environments

### Syntax

#### In templates

| Syntax            | Description                                                                                       |
| -------------     | -------------                                                                                     |
| ${variable}       | variable with the name "variable". if this variable is not defined, a warning message will apear. |
| ${host:localhost} | variiable with the name host. if the variable is not specified, localhost is used.                |
| ${env}            | is an automatic generated variable which contains the name of the environment.                    |

Sometimes you want to have a bigger block in the config if a condition is true (for example turn on WCF Tracing). For this, there is a simple if construct:

```xml
  <!-- [if ${backend.tracing.enabled}] -->
  <system.diagnostics>
    <sources>
      <!-- WCF Message Tracing -->
      <source name="System.ServiceModel.MessageLogging">
      <listeners>
        <add name="messages" type="System.Diagnostics.XmlWriterTraceListener" initializeData="${log.path}WCF_MessageLogging.svclog"/>
      </listeners>
      </source>
    </sources>
  </system.diagnostics>
  <!-- [endif] -->
```

What happens here:
First the variable `${backend.tracing.enabled}` gets resolved to a value. if this value is `"true"`, `"on"`, `"enabled"` or `"1"` then everything which is betweend the `<!-- [if ....]-->` and `<!-- [endif] -->` will be in the transform output, otherwise the whole block is skipped.

Note: There is no else or any operator for the if statement like equals.

#### In environment files
Basic structure:

```xml
<?xml version="1.0">
<environment description="some description about the environment" include="common.xml">
     <variable name="variable1"     value="value1" />
     <variable name="variable"     value="value2" />
     <variable name="combined"     value="Variable1: ${variable1}; Variable2: ${variable2}" />
</environment>
```

### Include Attribute:
The `include` attribute lets you include the variables from another file. Use comma to separate different files. This is useful for variables which have the same value in multiple environments, so you don't have to copy and paste those variables. It's important that the included variables won't overwrite any existing variable! See the Best Practices Chapter for more detaiils how to use this feature properly.

ideas: 
TODO: Tool/Integration/Plugin should support your work and not hide the complexity!


[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/tobiaszuercher/transformer/trend.png)](https://bitdeli.com/free "Bitdeli Badge")

