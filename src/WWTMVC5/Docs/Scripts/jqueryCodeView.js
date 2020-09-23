$(document).ready(
  function () {
      $('a.codeExample').each(
          function (i) {
              $(this).after('<pre class="codeExample"><code></code></pre>');
          }
      )
      $('pre.codeExample').hide();
      $('a.codeExample').toggle(
          function () {
              if (!this.old) {
                  this.old = $(this).html();
              }
              $(this).html('Hide Code');
              parseFile(this);
          },
          function () {
              $(this).html(this.old);
              $(this.nextSibling).hide();
          }
      )
      $('code.codeExample').each(
        function (i) {
            parseInline(this);
        }
      )


      function parseInline(o) {
          code = o.innerHTML;
          code = parseCode(code);
          o.innerHTML = code;
      }

      function parseFile(o) {
          if (!o.nextSibling.hascode) {

              url = getURLParameter("codeExample");
              if (url == "null") {
                  url = o.href;
              }
              $.get(url,
                function (code) {

                    code = parseCode(code);
                    o.nextSibling.innerHTML = '<code>' + code + '</code>';
                    o.nextSibling.hascode = true;
                }


              );
          }
          $(o.nextSibling).show();

      }

      function parseCode(code) {

          // format
          code = code.replace(/</g, '&lt;');
          code = code.replace(/>/g, '&gt;');
          code = code.replace(/\"/g, '&quot;');
          code = code.replace(/\r\n/g, '<br>');
          code = code.replace(/\r?\n/g, '<br> ');
          code = code.replace(/\r/g, '<br> ');
          code = code.replace(/\n/g, '<br> ');
   
          //code = code.replace(/\r/g, '[return]<br> ');
          code = code.replace(/\t/g, '    ');
          code = code.replace(/<br><br>/g, '<br> ');
          code = code.replace(/ /g, '&nbsp;');

          // code keywords
          code = code.replace(/function&nbsp;/g, '<span class="keywords">function</span>&nbsp;');
          code = code.replace(/var&nbsp;/g, '<span class="keywords">var</span>&nbsp;');
          code = code.replace(/if\(/g, '<span class="keywords">if</span>(');
          code = code.replace(/if&nbsp;\(/g, '<span class="keywords">if</span>&nbsp;(');
          code = code.replace(/else/g, '<span class="keywords">else</span>');
          code = code.replace(/for\(/g, '<span class="keywords">for</span>(');
          code = code.replace(/for&nbsp;\(/g, '<span class="keywords">for</span>&nbsp;(');
          code = code.replace(/switch/g, '<span class="keywords">switch</span>');
          code = code.replace(/case/g, '<span class="keywords">case</span>');
          code = code.replace(/break;/g, '<span class="keywords">break;</span>');
          code = code.replace(/return/g, '<span class="keywords">return</span>');
          code = code.replace(/true/g, '<span class="keywords">true</span>');
          code = code.replace(/false/g, '<span class="keywords">false</span>');

          // htmlTags
          code = HtmlTags(code, 'a');
          code = HtmlTags(code, 'b');
          code = HtmlTags(code, 'body');
          code = HtmlTags(code, 'canvas');
          code = HtmlTags(code, 'div');
          code = HtmlTags(code, '!DOCTYPE');
          code = HtmlTags(code, 'form');
          code = HtmlTags(code, 'h1');
          code = HtmlTags(code, 'h2');
          code = HtmlTags(code, 'h3');
          code = HtmlTags(code, 'h4');
          code = HtmlTags(code, 'head');
          code = HtmlTags(code, 'html');
          code = HtmlTags(code, 'img');
          code = HtmlTags(code, 'input');
          code = HtmlTags(code, 'meta');
          code = HtmlTags(code, 'p');
          code = HtmlTags(code, 'PRE');
          code = HtmlTags(code, 'script');
          //code = HtmlTags(code, 'span');  can't use span as it is what is being inserted
          code = HtmlTags(code, 'strong');
          code = HtmlTags(code, 'table');
          code = HtmlTags(code, 'td');
          code = HtmlTags(code, 'th');
          code = HtmlTags(code, 'title');
          code = HtmlTags(code, 'tr');

          // htmlProperties
          code = HtmlProperties(code, "bgcolor");
          code = HtmlProperties(code, "border");
          code = HtmlProperties(code, "cellpadding");
          code = HtmlProperties(code, "cellspacing");
          code = HtmlProperties(code, "charset");
          code = HtmlProperties(code, "checked");
          //code = HtmlProperties(code, "class");  can't use class as it is being inserted
          code = HtmlProperties(code, "colspan");
          code = HtmlProperties(code, "content");
          code = HtmlProperties(code, "height");
          code = HtmlProperties(code, "href");
          code = HtmlProperties(code, "http-equiv");
          code = HtmlProperties(code, "id");
          code = HtmlProperties(code, "name");
          code = HtmlProperties(code, "onclick");
          code = HtmlProperties(code, "onload");
          code = HtmlProperties(code, "src");
          code = HtmlProperties(code, "style");
          code = HtmlProperties(code, "type");
          code = HtmlProperties(code, "value");
          code = HtmlProperties(code, "width");
          code = HtmlProperties(code, "xmlns");

          // quoted strings
          result = code.match(/&quot;(.*?)&quot;/g);
          if (result != null) {
              for (var index = 0; index < result.length; index++) {
                  code = code.replace(result[index], '<span class="string">' + result[index] + '</span>');
              }
          }

          // html comments
          result = code.match(/&lt;!\-\-&nbsp;(.*?)\-\-&gt;/g);
          if (result != null) {
              for (var index = 0; index < result.length; index++) {
                  code = code.replace(result[index], '<span class="comments">' + result[index] + '</span>');
              }
          }

          // single line code comments
          result = code.match(/\/\/&nbsp;(.*?)\<br\>/g);
          if (result != null) {
              for (var index = 0; index < result.length; index++) {
                  code = code.replace(result[index], '<span class="comments">' + result[index] + '</span>');
              }
          }

          return code;
      }

      function HtmlTags(code, tag) {

          // replace start tags
          var regx = new RegExp('&lt;' + tag, 'g');
          var start = '&lt;<span class="htmlTags">' + tag + '</span>';
          code = code.replace(regx, start);

          // replace end tags
          regx = new RegExp(tag + '&gt;', 'g');
          var end = '<span class="htmlTags">' + tag + '</span>&gt;';
          code = code.replace(regx, end);

          return code;
      }

      function HtmlProperties(code, property) {
          var regx = new RegExp(property + '=', 'g');
          var prop = '<span class="htmlProperties">' + property + '</span>=';

          code = code.replace(regx, prop);

          return code;

      }


      function getURLParameter(name) {
          return decodeURI((RegExp(name + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1]);
      }
  }
)
