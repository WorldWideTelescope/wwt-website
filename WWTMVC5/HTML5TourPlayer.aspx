<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

</script>

<html>
<head runat="server">
    <title>Title</title>
    <script src="http://cdnjs.cloudflare.com/ajax/libs/jquery/1.10.2/jquery.min.js" type="text/javascript"></script>
    <link href="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/includes/CSS/wwt.min.css?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>" rel="stylesheet" />
    
    <link href="//netdna.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.css" rel="stylesheet">
    <script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Script/ext/bootstrap.js"></script>
    <script type="text/javascript" src="http://www.worldwidetelescope.org/scripts/wwtsdk.aspx"></script>
    <script>
        wwt = { triggerResize: function() {} };
    </script>
    <script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Script/controls/WebControl.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
    <script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Script/controls/TourPlayer.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
    <script>
       
        $(window).on('load', function () {
            var hash = location.hash.replace(/#/, '').split('&&');
            wwt.TourPlayer.init({
                resLoc: '<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>',
                tour: hash[0],
                remote: hash[1]
            });
        });
    </script>
    <style>
        html, body.remote-player {
            height: 100%;
            width: 100%;
            margin: 0;
            padding: 0;
            overflow: hidden;
        }
         .wwt-webcontrol-wrapper, #WorldWideTelescopeControlHost {
             margin: 0;
             /*position: fixed;
             top: 0;left: 0;*/
             height: 100%;
             width: 100%;
         }
    </style>
</head>
<body class="remote-player">
    <form id="HtmlForm" runat="server" class="hide"></form>
    <div id="WorldWideTelescopeControlHost"></div>
</body>
</html>
