[![Build Status](https://dev.azure.com/aasworldwidetelescope/WWT/_apis/build/status/WorldWideTelescope.wwt-website?branchName=master)](https://dev.azure.com/aasworldwidetelescope/WWT/_build/latest?definitionId=20&branchName=master)

# AAS WorldWide Telescope website

This repository contains the code for the web services backing the
[AAS](https://aas.org/) [WorldWide
Telescope](https://worldwidetelescope.org/home) (WWT) software system.

**Note** The contents of this repository are in flux as we modernize this
codebase.


## Installation Instructions

**To be revised**. Some *old* notes are in [INSTALL.md](./INSTALL.md).

### Azure Storage Emulator

This project is configured to use [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite), a cross-platform emulator for Azure storage. There are multiple ways to acquire the tool, so please refer to the link given to set up an install.

Access to the storage is done via the `DefaultAzureTokenCredential` that requires `https` protocol for connection. In order to do that, the following steps must be done to enable development:

These are steps taken from [here](https://blog.jongallant.com/2020/04/local-azure-storage-development-with-azurite-azuresdks-storage-explorer/):

1. Install [`mkcert`](https://github.com/FiloSottile/mkcert#installation)
1. Trust the mkcert RootCA.pem and create a certificate

	```
	mkcert -install
	mkcert 127.0.0.1
	```
1. Chose a directory from which to run Azurite. The local emulator data will be stored here
1. Run Azurite with oauth and SSL support:

	```
	azurite --oauth basic --cert 127.0.0.1.pem --key 127.0.0.1-key.pem
	```
1. The app will now run with default settings.

In order to configure the [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) to run, you'll need to do the following:

1. Get the RootCA.pem

	```
	mkcert -CAROOT
	```
1. Open Azure Storage Explorer
1. Go to `Edit->SSL Certificates->Import Certificates` and select the file from the first step.
1. Restart the storage explorer (you will be prompted to do this)

### Configuration
Configuration in this project uses ConfigurationManager.AppSettings. In order to make
it easier to configure outside of web.config, ConfigurationBuilders are supported.
Currently, there are three builders enabled: KeyVault, Environment, and User Secrets.
For more details, see the [project](https://github.com/aspnet/MicrosoftConfigurationBuilders#config-builders-in-this-project)
where they are maintained.


## Getting involved

We love it when people get involved in the WWT community! You can get started by
[participating in our user forum][forum] or by [signing up for our low-traffic
newsletter][newsletter]. If you would like to help make WWT better, our
[Contributor Hub] aims to be your one-stop shop for information about how to
contribute to the project, with the [Contributors’ Guide] being the first thing
you should read. Here on GitHub we operate with a standard [fork-and-pull]
model.

[forum]: https://wwt-forum.org/
[newsletter]: https://bit.ly/wwt-signup
[Contributor Hub]: https://worldwidetelescope.github.io/
[Contributors’ Guide]: https://worldwidetelescope.github.io/contributing/
[fork-and-pull]: https://help.github.com/en/articles/about-collaborative-development-models

All participation in WWT communities is conditioned on your adherence to the
[WWT Code of Conduct], which basically says that you should not be a jerk.

[WWT Code of Conduct]: https://worldwidetelescope.github.io/code-of-conduct/


## Acknowledgments

The AAS WorldWide Telescope system is a [.NET Foundation] project. Work on WWT
has been supported by the [American Astronomical Society] (AAS), the US
[National Science Foundation] (grants [1550701], [1642446], and [2004840]), the [Gordon
and Betty Moore Foundation], and [Microsoft].

[American Astronomical Society]: https://aas.org/
[.NET Foundation]: https://dotnetfoundation.org/
[National Science Foundation]: https://www.nsf.gov/
[1550701]: https://www.nsf.gov/awardsearch/showAward?AWD_ID=1550701
[1642446]: https://www.nsf.gov/awardsearch/showAward?AWD_ID=1642446
[2004840]: https://www.nsf.gov/awardsearch/showAward?AWD_ID=2004840
[Gordon and Betty Moore Foundation]: https://www.moore.org/
[Microsoft]: https://www.microsoft.com/


## Legalities

The WWT code is licensed under the [MIT License]. The copyright to the code is
owned by the [.NET Foundation].

[MIT License]: https://opensource.org/licenses/MIT
