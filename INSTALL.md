# INSTALL.md

### On a Windows 7 or 8.1 machine (should also work for Windows 10):

1. If you do not already have Visual Studio, you may [download Visual Studio Community Edition for free](http://visualstudio.com) (2015 is the latest version).

2. You need the Internet Information Services (IIS) web server enabled on your machine.  To turn it on, go to the Control Panel and search for "Turn Windows Features On or Off."
Check these: 
Internet Information Services
Web Management Tools -> IIS Management Console
World Wide Web Services -> Application Development Features -> ASP.NET 4.5
Security-> Windows Authentication

3. Install (Azure SDK
2.6)[https://azure.microsoft.com/blog/2015/04/29/announcing-the-azure-sdk-2-6-for-net/].
An easy way to do this is to go into the Visual Studio menu: "Tools" ->
"Extensions and Updates" and update first SQL Server (to get LocalDB
installed) and then the Azure SDK, which depends on LocalDB.

4. Clone the [WWT Website Git
Repository](https://github.com/WorldWideTelescope/wwt-website.git).
You can "Clone in Desktop" if you have GitHub for Windows installed.

5. Run Visual Studio as Administrator (right-click on Visual Studio and
select Run As Administrator).

6. Open the solution file in Visual Studio:
File-> Open-> Project/Solution -> /path/to/repo/wwt-website<-master> -> WWTWebSiteOnly.sln

7. Click OK on any Security Warnings

8. Click on Build -> Build Solution.  It should build with no errors
(you may get some warnings...probably OK).

9. In order to view the newly built website in your browser, you will first have to update some permissions on your machine:
   - Launch the IIS Manager.   Make Sure you are in “Features View” (selected near the bottom)
   - On the left, click [Machine Name] -> Sites -> Default Web Site
   - Under IIS section, double-click Authentication
   - Right-Click Windows Authentication.  Select Enable.  If prompted for Extended Protection, select Accept and click OK.

10. Get the proper permissions on the content folder:
    - On the left, Right-Click Default Web Site, select Edit Permissions
    - A WWTMVC5 Properties sheet should appear. 
    - Select the Security tab.  Click the Edit button.   Click the Add button.
    - Click the Locations button.   Select your machine name at the top of the Locations window.  Click OK.
    - Enter IIS\_IUSRS as the object name in the Select Users or Groups window.
    - Click Check Names. It should show:    [Machine Name]\IIS\_IUSRS
    - Click OK. A dialog will show the chosen permissions.  Click OK.
    - Then Click OK again.

11. Launch your web browser.  Enter localhost and go.  It will probably spend about a minute working, then the WWT website should render.



### On Linux, use a Windows virtual machine:

1. Install [VirtualBox](https://www.virtualbox.org/wiki/Linux_Downloads).

2. Get [a no-cost .ova file of Windows from the Microsoft Edge
team](https://dev.modern.ie/tools/vms/linux/).

3. Import the .ova file into VirtualBox.

4. In your new Windows virtual machine, follow the instructions above.
