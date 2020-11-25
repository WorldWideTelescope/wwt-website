[![Build Status](https://dev.azure.com/aasworldwidetelescope/WWT/_apis/build/status/WorldWideTelescope.wwt-website?branchName=master)](https://dev.azure.com/aasworldwidetelescope/WWT/_build/latest?definitionId=20&branchName=master)

# AAS WorldWide Telescope website

This repository contains the code for the web services backing the
[AAS](https://aas.org/) [WorldWide
Telescope](https://worldwidetelescope.org/home) (WWT) software system.


## Deprecated APIs

As we move the service to a more modern architecture, some endpoints are being deprecated. These include the following:

| Endpoint | Reason | PR |
| -------- | -------| -- |
| `/gettourfile.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/webserviceproxy.aspx` | Offloaded to separate service | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/catalog2.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/demmarsempty.aspx` | Data is from `wwt.nasa.gov` which is no longer available. | [#173](https://github.com/WorldWideTelescope/wwt-website/pull/173) |
| `/wwtweb/gethostname.aspx` | Debugging-only API | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/gettourfile.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/goto2.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/isstle2.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/martiantileempty.aspx` | Data is from `wwt.nasa.gov` which is no longer available. | [#182](https://github.com/WorldWideTelescope/wwt-website/pull/182) |
| `/wwtweb/postmars.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/postmarsdem.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/postmarsdem2.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/sdsstoast.offline.aspx` | Debugging-only API | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/showimage2.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/test.aspx` | Debugging-only API | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/testfailover.aspx` | Debugging-only API | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/webserviceproxy.aspx` | Offloaded to separate service | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |
| `/wwtweb/wmsearthtoday.aspx` | Data is from `wms.jpl.nasa.gov` which is no longer available. | [#181](https://github.com/WorldWideTelescope/wwt-website/pull/181) |
| `/wwtweb/wmsmoon.aspx` | Data is from `onmoon.jpl.nasa.gov` which no longer available. | [#181](https://github.com/WorldWideTelescope/wwt-website/pull/181) |
| `/wwtweb/wmstoast.aspx` | Data is from `ms.mars.asu.edu` which no longer available. | [#181](https://github.com/WorldWideTelescope/wwt-website/pull/181) |
| `/wwtweb/xml2wtt.aspx` | No evidence of current usage | [#205](https://github.com/WorldWideTelescope/wwt-website/pull/205) |


## Developer Instructions

See [docs/dev-environment.md](instructions) for details on how to build and run
locally. Please submit issues (or PRs!) if you run into any problems or
inaccuracies.


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
