# How to contribute to the WWT Web Site

Please see http://worldwidetelescope.org/Learn/WebDevelopment#addnewpage for detailed instructions.

In general, there are two ways to make an edit to the website:

* On a Windows machine, install Visual Studio Community Edition (see README for WorldWideTelescope/wwt-windows-client) and then make sure you have IIS installed.  Then when you make a local change you can see the result locally to make sure it is what you want (and before you submit a pull request).
* If you aren't on a Windows machine, you can edit the appropriate file with a text editor.  Then when you submit a pull request, indicate in the request the intention of your edit and then say that you have not tested this in a test web environment and ask the committer to test things out for you first.

Note that there is a master template page that is used for (almost) all pages but *not* for the home page: `WWTMVC5/Views/Shared/_ContentPage.cshtml`
