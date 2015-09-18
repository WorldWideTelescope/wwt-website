On a Windows 7 or 8.1 machine (should also work for Windows 10):

1.	If you do not already have Visual Studio, you may download Visual Studio Community Edition for free here: 
http://visualstudio.com  (2015 is the latest version)

2.	You need the Internet Information Services (IIS) web server enabled on your machine
(search for Turn Windows Features On or Off) 
Check these: 
Internet Information Services
Web Management Tools -> IIS Management Console
World Wide Web Services -> Application Development Features -> ASP.NET 4.5
Security-> Windows Authentication

3.	Install Azure SDK 2.6 (https://azure.microsoft.com/blog/2015/04/29/announcing-the-azure-sdk-2-6-for-net/)
4.	Clone the WWT Website Git Repository found here: https://github.com/WorldWideTelescope/wwt-website.git
Either download as ZIP (then Unzip into the directory of your choice) or 
Clone in Desktop (if you have GitHub for Windows installed)
5.	Run Visual Studio as Administrator (right-click on Visual Studio and select Run As Administrator)
6.	Now you need to open the solution file in Visual Studio:
File-> Open-> Project/Solution -> [location of your copy of the Git Repository] -> wwt-website<-master> -> WWTWebSiteOnly.sln
7.	Click OK on any Security Warnings
8.	Click on Build -> Build Solution

It should build with no errors (you may get some warnings...probably OK)

9.	In order to view the newly built website in your browser, you will first have to update some permissions on your machine:
Launch the IIS Manager.   Make Sure you are in “Features View” (selected near the bottom)
On the left, click [Machine Name] -> Sites -> Default Web Site
Under IIS section, double-click Authentication
Right-Click Windows Authentication.  Select Enable.  If prompted for Extended Protection, select Accept and click OK.

Now you must get the proper permissions on the content folder.    
On the left, Right-Click Default Web Site, select Edit Permissions

A WWTMVC5 Properties sheet should appear. 
Select the Security tab.  Click the Edit button.   Click the Add button.
Click the Locations button.   Select your machine name at the top of the Locations window.  Click OK.
Enter IIS_IUSRS as the object name in the Select Users or Groups window.
Click Check Names. It should show:    [Machine Name]\IIS_IUSRS
Click OK. A dialog will show the chosen permissions.  Click OK.

Then Click OK again.

10.	Launch your web browser.    Enter localhost and go.    
It will probably spend about a minute working, then the WWT website should render.
