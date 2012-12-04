using System;
using Gwen.Control;

namespace GwenUI
{
    public class GuiWindow : Base
    {
        private int m_WindowCount;
        private readonly Random rand;

        public GuiWindow(Base parent)
            : base(parent)
        {
            rand = new Random();

            Button button1 = new Button(this);
            button1.SetText("Open a Window");
            button1.Clicked += OpenWindow;

            Button button2 = new Button(this);
            button2.SetText("Open a MessageBox");
            button2.Clicked += OpenMsgbox;
            Gwen.Align.PlaceRightBottom(button2, button1, 10);

            m_WindowCount = 1;
        }

        void OpenWindow(Base control)
        {
            WindowControl window = new WindowControl(GetCanvas());
            window.Caption = String.Format("Window {0}", m_WindowCount);
            window.SetSize(rand.Next(200, 400), rand.Next(200, 400));
            window.SetPosition(rand.Next(700), rand.Next(400));

            m_WindowCount++;
        }

        void OpenMsgbox(Base control)
        {
            MessageBox window = new MessageBox(GetCanvas(), String.Format("Window {0}   MessageBox window = new MessageBox(GetCanvas(), String.Format(  MessageBox window = new MessageBox(GetCanvas(), String.Format(", m_WindowCount));
            window.SetPosition(rand.Next(700), rand.Next(400));

            m_WindowCount++;
        }
    }
}

