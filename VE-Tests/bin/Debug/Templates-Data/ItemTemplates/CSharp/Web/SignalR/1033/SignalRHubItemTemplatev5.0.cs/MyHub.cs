﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace $rootnamespace$
{
    public class $safeitemname$ : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }        
    }
}