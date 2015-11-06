using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ResKueMe.SMS
{
    public class SMSUtils
    {
        public void SendText(List<string> numbers, string message)
        {
            try
            {
                if (numbers != null)
                {
                    string recipients = string.Empty;
                    foreach (string number in numbers)
                    {
                        recipients += ";" + number;
                    }
                    var smsComposeTask = new SmsComposeTask();
                    smsComposeTask.To = recipients;
                    smsComposeTask.Body = message;
                    smsComposeTask.Show();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Error in sending text to users " + ex.Message); 
            }
        }
    }
}
