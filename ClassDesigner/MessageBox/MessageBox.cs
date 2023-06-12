using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassDesigner.MessageBox
{
    public static class MessageBox
    {
        public static MessageBoxResult Show(string header = "", string message = "", MessageBoxButtons buttons = MessageBoxButtons.Ok, Window owner = null)
        {
            var mbox = new MessageBoxWindow();
            mbox.Header = header;
            mbox.Message = message;
            if (owner != null) { mbox.Owner = owner; }
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
        public static MessageBoxResult ShowInput(out string input, string header = "", string message = "", MessageBoxButtons buttons = MessageBoxButtons.Ok, Window owner = null)
        {
            var mbox = new MessageBoxWindow();
            mbox.Header = header;
            mbox.Message = message;
            if (owner != null) { mbox.Owner = owner; }
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
            mbox.InputField = true;
            mbox.ShowDialog();
            input = mbox.InputText;
            return mbox.Result;
        }
    }
}
