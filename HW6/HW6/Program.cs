﻿using System;
namespace CS422
{
    public class Program
    {
        static void Main()
        {
            WebServer.AddService(new DemoService());
            WebServer.Start(8080, 64);
        }
    }
}

