using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DDD.Core.Sample.AuthServer.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace DDD.Core.Sample.AuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //RSA��֤�鳤��2048���ϣ��������쳣
            //����AccessToken�ļ���֤��
            var rsa = new RSACryptoServiceProvider();
            //�������ļ���ȡ����֤��
            rsa.ImportCspBlob(Convert.FromBase64String(Configuration["SigningCredential"]));
            //IdentityServer4��Ȩ��������
            services.AddIdentityServer()
                .AddSigningCredential(new RsaSecurityKey(rsa)) //���ü���֤��
                                                               //.AddTemporarySigningCredential()    //���Ե�ʱ���ʹ����ʱ��֤��
                .AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
                .AddInMemoryClients(InMemoryConfiguration.Clients())
                //�����client credentialsģʽ��ô�Ͳ���Ҫ������֤User��
                .AddResourceOwnerValidator<UserValidator>(); //User��֤�ӿ�

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();  //����asp.net core �ܵ�

            app.UseRouting(routes =>
            {
                routes.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
