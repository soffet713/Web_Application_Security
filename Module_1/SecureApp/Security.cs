using System;
using System.Web;

public class Security
{
    public string SanitizeInput(string userInput)
    {
        return HttpUtility.HtmlEncode(userInput);
    }
}