namespace WWTMVC5
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    public class AttachTokenEndpointBehavior : IEndpointBehavior, IClientMessageInspector
    {
        private string token;

        public AttachTokenEndpointBehavior(string token)
        {
            this.token = token;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var header = new HttpRequestMessageProperty();
            header.Headers.Add("Authorization", "Bearer " + this.token);
            request.Properties.Add(HttpRequestMessageProperty.Name, header);

            return null;
        }
    }
}