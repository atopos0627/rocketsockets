using System;
using hotstack.Owin;
using hotstack.Owin.Impl;
using Symbiote.Core;
using Symbiote.Core.DI;

namespace hotstack.Config
{
    public class HttpDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            var router = new ApplicationRouter();
            return container => 
                       {
                           container.For<HttpConfigurator>().Use<HttpConfigurator>();
                           container.For<IRegisterApplication>().Use( router );
                           container.For<IRouteRequest>().Use( router );
                           //container.For<IViewEngine>().Use<NHamlEngine>().AsSingleton();
                       };
        }
    }
}