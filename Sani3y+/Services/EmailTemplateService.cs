using Microsoft.AspNetCore.Identity;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly UserManager<AppUser> _userManager;
        public EmailTemplateService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public (string Subject, string Body) GetResetPasswordEmail(string resetUrl)
        {
            string subject = "Password Reset Request";
            string body = $@"
                <!DOCTYPE html>
                <html lang='ar' dir='rtl'>
                <head>
                  <meta charset='UTF-8'>
                  <title>إعادة تعيين كلمة المرور</title>
                </head>
                <body style='margin:0;padding:0;background-color:#f7f7f7;font-family:Arial,sans-serif;direction:rtl;'>

                  <table style='width:600px;margin:40px auto;background-color:#ffffff;border:1px solid #e0e0e0;border-radius:8px;'>
                    <tr>
                      <td style='text-align:center;padding:30px 20px 10px;'>
                        <img src='http://sani3ywebapiv1.runasp.net/assets/Frame.png' alt='شعار صنايعي بلس' style='width:120px;display:block;margin:0 auto;' />
                        <p style='color:#007bff;font-size:14px;margin-top:10px;'>sanai3yplus.com</p>
                      </td>
                    </tr>
                    <tr>
                      <td><div style='width:100%;height:2px;background:rgba(0, 0, 0, 0.20);'></div></td>
                    </tr>
                    <tr>
                      <td style='padding:30px 40px;text-align:right;'>
                        <h3 style='margin-top:0;color:#333333;'> عزيزي المستخدم</h3>
                        <p style='font-size:16px;color:#333333;line-height:1.6;'>
                          لقد طلبت إعادة تعيين كلمة المرور الخاصة بك. يُرجى الضغط على الزر أدناه لإتمام العملية. إذا لم تطلب ذلك، يمكنك تجاهل هذا البريد.
                        </p>
                        <div style='text-align:center;margin-top:30px;'>
                          <a href='{resetUrl}' style='background-color:#e9572b;color:#ffffff;text-decoration:none;padding:12px 25px;border-radius:4px;font-size:16px;display:inline-block;'>
                            إعادة تعيين كلمة المرور
                          </a>
                        </div>
                      </td>
                    </tr>
                  </table>

                </body>
                </html>";

            //    string body = $@"
            //<div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>
            //    <h2 style='color: #333;'>Sanal3y plus</h2>
            //    <br>
            //    sanal3yplus.com
            //    <p style='font-size: 16px;'>
            //        لقد طلبت إعادة تعيين كلمة المرور الخاصة بك. يرجى الضغط على الزر أدناه لإتمام العملية.
            //        إذا لم تطلب ذلك، يمكنك تجاهل هذا البريد.
            //    </p>
            //    <div style='text-align: center; margin: 25px 0;'>
            //        <a href='{resetUrl}' 
            //            style='background-color: #ED5B28; 
            //                   color: white; 
            //                   padding: 12px 24px; 
            //                   text-decoration: none; 
            //                   border-radius: 4px; 
            //                   font-weight: bold;'>
            //            إعادة تعيين كلمة المرور
            //        </a>
            //    </div>
            //    <hr>
            //    <p style='font-size: 12px; color: #777; text-align: center;'>
            //        sanal3yplus.com
            //    </p>
            //</div>";

            return (subject, body);
        }
    }
}
