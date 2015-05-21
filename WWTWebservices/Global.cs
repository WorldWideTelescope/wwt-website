using System;
using System.Data;
using System.Configuration;
////using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
////using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Research.WWT
{
    public class Global
    {
        // Regular Expressions	///////////////////////////////
        // Allow nothing -!@#$%^&*()_+.|~=`'""{}:;\<>[?,/]
        public readonly string RegexValidationExpressionAll = "[^" + Regex.Escape(@"!@#$%^*()_+~.=`'{}:;\/<>[?,") + @"\-\|\]\&\""" + "]*";
        // Allow ['-.]
        public readonly string RegexValidationExpressionName = "[^" + Regex.Escape(@"!@#$%^*()_+~=`{}:;\/<>[?,") + @"\|\]\&\""" + "]*";
        // Allow ['-.#,]
        public readonly string RegexValidationExpressionAddress = "[^" + Regex.Escape(@"!@$%^*()_+~=`{}:;\/<>[?") + @"\|\]\&\""" + "]*";
        // Allow [-]
        public readonly string RegexValidationExpressionPostal = "[^" + Regex.Escape(@"!@#$%^*()_+~.=`'{}:;\/<>[?,") + @"\|\]\&\""" + "]*";
        // Allow [:/.-_#?&=+~]
        public readonly string RegexValidationExpressionUrl = "[^" + Regex.Escape(@"!@$%^*()`'{};\<>[,") + @"\|\]\""" + "]*";
        // Allow // Here we'll allow [.-_@]
        public readonly string RegexValidationExpressionEmail = "[^" + Regex.Escape(@"!#$%^*()+~=`'{}:;\/<>[?,") + @"\|\]\&\""" + "]*";
        // Allow only letters, numbers, and spaces
        public readonly string RegexValidationExpressionAlphNumSpace = @"[0-9a-zA-Z\s]*";
        // Allow ['-.#,()[]$!?+=~_]{};\/@
        public readonly string RegexValidationExpressionEmailBody = "[^" + Regex.Escape(@"^`<>") + @"\|" + "]*";
        // Allow [:/.-_#,?&=+~]
        public readonly string RegexValidationExpressionEductionInstituion = "[^" + Regex.Escape(@"!@$%^*()`'{};\<>[,") + @"\|\]\""" + "]*";
        // End Regular Expressions	///////////////////////////////
    }

}
