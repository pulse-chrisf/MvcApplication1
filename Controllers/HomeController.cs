using System;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string strWebPage;
            string strURL = "http://metro.ca/en/on/accessible-flyer.html?method=getAccessibleFlyer";
            
            WebRequest objRequest = HttpWebRequest.Create(strURL);

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            
            string charset = objResponse.CharacterSet;
            Encoding encoding = Encoding.GetEncoding(charset);
            
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), encoding))
            {
                strWebPage = sr.ReadToEnd();
                sr.Close();
            }

            // Check real charset meta-tag in HTML for correct encoding
            int charsetStart = strWebPage.IndexOf("charset=", StringComparison.Ordinal);
            if (charsetStart > 0)
            {
                charsetStart += 8;
                int charsetEnd = strWebPage.IndexOfAny(new[] { ' ', '\"', ';' }, charsetStart);
                string realCharset = strWebPage.Substring(charsetStart, charsetEnd - charsetStart);

                // real charset meta-tag in HTML differs from supplied server header???
                if (realCharset != charset)
                {
                    // get correct encoding
                    Encoding correctEncoding = Encoding.GetEncoding(realCharset);

                    WebRequest objRequest2 = HttpWebRequest.Create(strURL);

                    HttpWebResponse objResponse2 = (HttpWebResponse)objRequest2.GetResponse();

                    using (StreamReader sr = new StreamReader(objResponse2.GetResponseStream(), correctEncoding))
                    {
                        strWebPage = sr.ReadToEnd();
                        sr.Close();
                    }
                }
            }

            ViewBag.Message = strWebPage;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
