using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;

namespace MvcApplication1.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Metro()
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

		public ActionResult Superstore()
		{
			string strWebPage;
			string strURL = "http://director.flyerservices.com/LCL/PublicationDirector.ashx?OrganizationId=797d6dd1-a19f-4f1c-882d-12d6601dc376&BannerId=323a2f8d-132a-4366-909b-12a7216bc5d0&BannerName=RCSSO&Version=text&banner=RCSSO&Language=en&storenum=2810&pgnum=1&pgcat=1&PC=1&T=1&view=text&publicationrunid=15c88c4c-cdc8-4db5-8f41-f068b1a59be2&PublicationId=7c8762e3-d290-45f8-8e6a-130e08b3e734&PublicationType=26&StoreId=20080030-1140-4064-b190-6fca26e3e6d7&Edition=162";
			string uri = string.Empty;
			string htmlResult = string.Empty;

			WebRequest objRequest = HttpWebRequest.Create(strURL);

			HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

			string charset = objResponse.CharacterSet;
			Encoding encoding = Encoding.GetEncoding(charset);

			using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), encoding))
			{
				strWebPage = sr.ReadToEnd();
				sr.Close();
			}

			try
			{
				//strWebPage = strWebPage.Replace("\r\n", "");

				HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

				// There are various options, set as needed
				htmlDoc.OptionFixNestedTags = true;

				// filePath is a path to a file containing the html
				htmlDoc.LoadHtml(strWebPage);

				// Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

				// ParseErrors is an ArrayList containing any errors from the Load statement
				if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
				{
					// Handle any parse errors as required
				}
				else
				{
					List<PostAttribute> postAttributes = new List<PostAttribute>();

					if (htmlDoc.DocumentNode != null)
					{
						HtmlAgilityPack.HtmlNode formNode = htmlDoc.DocumentNode.SelectSingleNode("//form");

						if (formNode != null)
						{
							foreach (var formAttribute in formNode.Attributes)
							{
								if (formAttribute.Name == "action")
								{
									uri = formAttribute.Value;
								}
							}
						}

						HtmlAgilityPack.HtmlNodeCollection inputNodes = htmlDoc.DocumentNode.SelectNodes("//input");

						foreach (var inputNode in inputNodes)
						{
							PostAttribute attribute = new PostAttribute();

							foreach (var nodeAttribute in inputNode.Attributes)
							{
								if (nodeAttribute.Name == "id")
								{
									if (!string.IsNullOrEmpty(nodeAttribute.Value))
									{
										attribute.Id = nodeAttribute.Value;
									}
								}

								if (nodeAttribute.Name == "value")
								{
									if (!string.IsNullOrEmpty(nodeAttribute.Value))
									{
										attribute.Value = nodeAttribute.Value;
									}
								}
							}

							postAttributes.Add(attribute);
						}
					}

					StringBuilder stringBuilder = new StringBuilder();

					foreach (var postAttribute in postAttributes)
					{
						stringBuilder.AppendFormat("&{0}={1}", postAttribute.Id, postAttribute.Value);
					}

					// remove the first "&" and replace it with a "?"
					stringBuilder.Remove(0, 1);
					stringBuilder.Insert(0, "?");

					uri = uri + stringBuilder;

					//WebClient wc = new WebClient();
					//wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
					//htmlResult = wc.UploadString(uri, stringBuilder.ToString());


					WebRequest request = WebRequest.Create(uri);
					request.Method = "POST";
					request.ContentLength = 0;
					using (WebResponse response = request.GetResponse())
					{
						using (Stream responseStream = response.GetResponseStream())
						{
							using (StreamReader responseReader = new StreamReader(responseStream))
							{
								strWebPage = responseReader.ReadToEnd();
								responseReader.Close();
							}
						}
					}


					//WebRequest htmlRequest = HttpWebRequest.Create(uri);
					//htmlRequest.Method = "POST";
					//htmlRequest.ContentLength = 0;

					//HttpWebResponse htmlResponse = (HttpWebResponse)htmlRequest.GetResponse();

					//using (StreamReader sr = new StreamReader(htmlResponse.GetResponseStream()))
					//{
					//    strWebPage = sr.ReadToEnd();
					//    sr.Close();
					//}
				}
			}
			catch (Exception e)
			{
				string message = e.Message;
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

	public class PostAttribute
	{
		public string Id { get; set; }
		public string Value { get; set; }
	}
}
