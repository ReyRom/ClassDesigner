using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.MessageBox
{
    public static class MessageBox
    {
        public static MessageBoxResult Show(string header = "", string message = "", MessageBoxButtons buttons = MessageBoxButtons.Ok)
        {
            var mbox = new MessageBoxWindow();
            mbox.Header = header;
            mbox.Message = message;

            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    mbox.OkButton = true;
                    break;
                case MessageBoxButtons.OkCancel:
                    mbox.OkButton = true;
                    mbox.CancelButton = true;
                    break;
                case MessageBoxButtons.YesNo:
                    mbox.YesButton = true;
                    mbox.NoButton = true;
                    break;
            }
            mbox.ShowDialog();

            return mbox.Result;
        }
    }
}
