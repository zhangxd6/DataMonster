using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;

[assembly: OwinStartup(typeof(Server.Startup))]

namespace Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
                FileSystem = new PhysicalFileSystem("wwwroot")
            };

            app.UseFileServer(options);
            app.MapSignalR();
        }
    }
}
