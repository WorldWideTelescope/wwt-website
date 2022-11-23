[![Build Status](https://dev.azure.com/aasworldwidetelescope/WWT/_apis/build/status/WorldWideTelescope.wwt-website?branchName=master)](https://dev.azure.com/aasworldwidetelescope/WWT/_build/latest?definitionId=20&branchName=master)

# AAS WorldWide Telescope website

This repository contains the code for the web services backing the
[AAS](https://aas.org/) [WorldWide
Telescope](https://worldwidetelescope.org/home) (WWT) software system.

You can monitor metrics for the WWT web services on [the public WWT metrics
dashboard][dashboard], made possible by support from [Datadog].

[dashboard]: https://p.us3.datadoghq.com/sb/cf4ddee0-e5ae-11ec-90f8-da7ad0900003-c64423f0e5e0627e2eb777abe3e591b0
[Datadog]: https://datadoghq.com/


## Developer Instructions

See [these instructions](docs/dev-environment.md) for details on how to build
and run locally. Please submit issues (or PRs!) if you run into any problems or
inaccuracies.

If you have .NET 6 installed, the basic CLI build command is:

```
dotnet build -c Release wwt-website-net6.slnf
```

You can build and run the server in a Docker container with:

```
docker build -t aasworldwidetelescope/core-data:latest .
docker run --rm -p 8080:80 --name wwtcoredata aasworldwidetelescope/core-data:latest
# browse to http://localhost:8080/wwtweb/isstle.aspx
```

However, most API endpoints won't work since they need to be wired up to the
backing data storage. To test locally with WWT production assets, launch the Docker
container with the following environment variables:

```
-e UseAzurePlateFiles=true -e AzurePlateFileStorageAccount=[secret]
```

where the secret can be obtained from the running app's configuration KeyVault.

See comments in the `azure-pipelines.yml` file for descriptions of how deployment
works.

[This file](docs/deprecations.md) logs old APIs that have been deprecated or
removed. The implementations of many of these can be found in the Git history if
you’re willing to dig.


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

The AAS WorldWide Telescope system is a [.NET Foundation] project managed by the
non-profit [American Astronomical Society] (AAS). Work on WWT has been supported
by the AAS, the US [National Science Foundation], and other partners. See [the
WWT user website][acks] for details.

[.NET Foundation]: https://dotnetfoundation.org/
[American Astronomical Society]: https://aas.org/
[National Science Foundation]: https://www.nsf.gov/
[acks]: https://worldwidetelescope.org/about/acknowledgments/


## Legalities

The WWT code is licensed under the [MIT License]. The copyright to the code is
owned by the [.NET Foundation].

[MIT License]: https://opensource.org/licenses/MIT
