using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using rocketsockets.Impl;
using rocketsockets.Impl.Managed;

namespace rocketsockets.test
{
    public class when_getting_listener_from_managed_factory : with_managed_listener_factory
    {
        protected static ManagedSocketListener listener;
        private Because of = () => 
        {
            listener = factory.CreateListener( scheduler, testConfiguration.Endpoints[0], testConfiguration ) as ManagedSocketListener;
            listener.Close();
        };

        private It should_have_valid_scheduler = () => listener.Scheduler.ShouldNotBeNull();
        private It should_have_valid_server_configuration = () => listener.Configuration.ShouldNotBeNull();
    }
}
