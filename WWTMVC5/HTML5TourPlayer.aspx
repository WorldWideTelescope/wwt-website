<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

</script>

<html>
<head runat="server">
    <title>Title</title>
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet">
	<script src="//code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>
	<script src="//cdnjs.cloudflare.com/ajax/libs/bootbox.js/4.4.0/bootbox.min.js"></script>
    <link href="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/includes/CSS/wwt.min.css?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>" rel="stylesheet" />
    
    <script src="/html5sdk/1.0.0/wwtsdk.min.js"></script>
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
