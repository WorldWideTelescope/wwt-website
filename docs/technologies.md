# Technologies

There are a number of technologies being used within the service. This document will try to keep the high level tools/SDKs so that people can easily ramp on the project.

## App Configuration
Configuration in this project uses ConfigurationManager.AppSettings. In order to make
it easier to configure outside of web.config, ConfigurationBuilders are supported.
Currently, there are three builders enabled: KeyVault, Environment, and User Secrets.
For more details, see the [project](https://github.com/aspnet/MicrosoftConfigurationBuilders#config-builders-in-this-project)
where they are maintained.