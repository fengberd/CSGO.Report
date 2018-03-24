using System;
using System.Drawing;

namespace RegisterAssistance.captcha
{
    public interface ICaptchaProcessor
    {
        string submitImage(Image data);
        string getResult(string identifier);
        bool reportSuccess(string identifier);
        bool reportError(string identifier);
    }
}
