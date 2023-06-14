﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS2812BLEDWIFI
{
    public partial class Form1 : Form
    {
        public IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.0.121"), 610);
        //定义网络类型，数据连接类型和网络协议UDP
        public Socket PublicRemote = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Bitmap screen = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var point = new Point();
            point.X = 1440;
            point.Y = 1;
            ConnectItem();
            //MessageBox.Show(GetColorFromScreen(point).R.ToString());
        }
        public void ConnectItem()//连接目标
        {
            //PublicRemote.Close();
            //PublicRemote.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001));//绑定端口号和IP

            //PublicRemote = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            //IPEndPoint ipp = new IPEndPoint(IPAddress.Parse("192.168.0.121"), 610);
            //PublicRemote.Connect(ipp);
        }

        public List<byte[]> SplitList(byte[] superbyte, int size)
        {
            List<byte[]> result = new List<byte[]>();
            int length = superbyte.Length;
            int count = length / size;
            int r = length % size;
            for (int i = 0; i < count; i++)
            {
                byte[] newbyte = new byte[size];
                newbyte = superbyte.Skip(size * i).Take(size).ToArray();// SplitArray(superbyte, size*i, size * i+ size);
                result.Add(newbyte);
            }
            if (r != 0)
            {
                byte[] newbyte = new byte[r];
                newbyte = superbyte.Skip(length - r).Take(r).ToArray();
                result.Add(newbyte);
            }
            return result;
        }

        public Bitmap CaptureFromScreen()
        {
            Rectangle rect = Rectangle.Empty;
            Bitmap bmpScreenCapture = null;
            if (rect == Rectangle.Empty)//capture the whole screen
            {
                rect = Screen.PrimaryScreen.Bounds;
            }
            bmpScreenCapture = new Bitmap(rect.Width, rect.Height);
            Graphics p = Graphics.FromImage(bmpScreenCapture);
            p.CopyFromScreen(rect.X,
                     rect.Y,
                     0, 0,
                     rect.Size,
                     CopyPixelOperation.SourceCopy);
            p.Dispose();
            return bmpScreenCapture;
        }
        public Color GetColorFromScreen(Point p)
        {
            Color c = screen.GetPixel(p.X, p.Y);
            //screen.Dispose();
            return c;
        }
        public void Upload(string message, Socket Remote)
        {
            try
            {
                Remote.SendTo(Tobt(message), Tobt(message).Length, SocketFlags.None, ipep);
                //List<byte[]> a = SplitList(Tobt(message), 1024);
                //Remote.Send(Tobt(message));//向目标机发送消息1
            }
            catch (Exception)
            {
                //ConnectItem();
                throw;
            }
            
        }



        static byte[] Tobt(string mgs)
        {
            byte[] cmgs = Encoding.UTF8.GetBytes(mgs);
            return cmgs;
        }
        public void AutoRun()
        { //上下35 左右20   2560*1440
          //横初始像素 73公差71，竖初始像素 80公差68
            while (true) 
            {
                System.Threading.Thread.Sleep(8);
                var point = new Point();
                screen = CaptureFromScreen();
                String H1 = null;
                String H2 = null;
                String V1 = null;
                String V2 = null;
                //横向
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0) { point.Y = 1296; }
                    else{ point.Y = 144; }
                    for (int j = 0; j < 35; j++)
                    {
                        if (i == 0){ point.X = 2560 - 2 - ((j + 1) * 71);}
                        else { point.X = 0 + 2 + ((j + 1) * 71); }
                        String R = GetColorFromScreen(point).R.ToString();
                        String G = GetColorFromScreen(point).G.ToString();
                        String B = GetColorFromScreen(point).B.ToString();
                        if (i == 0){H1 = H1 + R + "," + G + "," + B + "-";}
                        else { H2 = H2 + R + "," + G + "," + B + "-"; }
                    }
                }
                //纵向
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0) { point.X = 256; }
                    else { point.X = 2304; }
                    for (int j = 0; j < 20; j++)
                    {
                        if (i == 0) { point.Y = 1440 - 2 - ((j + 1) * 68); }
                        else { point.Y = 0 + 2 + ((j + 1) * 68); }
                        String R = GetColorFromScreen(point).R.ToString();
                        String G = GetColorFromScreen(point).G.ToString();
                        String B = GetColorFromScreen(point).B.ToString();
                        if (i == 0) { V1 = V1 + R + "," + G + "," + B + "-"; }
                        else { V2 = V2 + R + "," + G + "," + B + "-"; }
                    }
                }
                String Result = H1 + V1 + H2 + V2 + "1-";
                //MessageBox.Show(Result);
                Upload(Result,PublicRemote);
                screen.Dispose();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button2.Text = "运行中";
            Thread thread = new Thread(new ThreadStart(AutoRun));
            thread.Start();
        }
    }
}
