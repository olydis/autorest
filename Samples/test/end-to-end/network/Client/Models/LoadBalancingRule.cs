// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace ApplicationGateway.Models
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// A loag balancing rule for a load balancer.
    /// </summary>
    [JsonTransformation]
    public partial class LoadBalancingRule : SubResource
    {
        /// <summary>
        /// Initializes a new instance of the LoadBalancingRule class.
        /// </summary>
        public LoadBalancingRule()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the LoadBalancingRule class.
        /// </summary>
        /// <param name="protocol">The transport protocol for the external
        /// endpoint. Possible values are 'Udp' or 'Tcp'. Possible values
        /// include: 'Udp', 'Tcp'</param>
        /// <param name="frontendPort">The port for the external endpoint. Port
        /// numbers for each Rule must be unique within the Load Balancer.
        /// Acceptable values are between 1 and 65534.</param>
        /// <param name="id">Resource ID.</param>
        /// <param name="frontendIPConfiguration">A reference to frontend IP
        /// addresses.</param>
        /// <param name="backendAddressPool">A reference to a pool of DIPs.
        /// Inbound traffic is randomly load balanced across IPs in the backend
        /// IPs.</param>
        /// <param name="probe">The reference of the load balancer probe used
        /// by the load balancing rule.</param>
        /// <param name="loadDistribution">The load distribution policy for
        /// this rule. Possible values are 'Default', 'SourceIP', and
        /// 'SourceIPProtocol'. Possible values include: 'Default', 'SourceIP',
        /// 'SourceIPProtocol'</param>
        /// <param name="backendPort">The port used for internal connections on
        /// the endpoint. Acceptable values are between 1 and 65535.</param>
        /// <param name="idleTimeoutInMinutes">The timeout for the TCP idle
        /// connection. The value can be set between 4 and 30 minutes. The
        /// default value is 4 minutes. This element is only used when the
        /// protocol is set to TCP.</param>
        /// <param name="enableFloatingIP">Configures a virtual machine's
        /// endpoint for the floating IP capability required to configure a SQL
        /// AlwaysOn Availability Group. This setting is required when using
        /// the SQL AlwaysOn Availability Groups in SQL server. This setting
        /// can't be changed after you create the endpoint.</param>
        /// <param name="provisioningState">Gets the provisioning state of the
        /// PublicIP resource. Possible values are: 'Updating', 'Deleting', and
        /// 'Failed'.</param>
        /// <param name="name">The name of the resource that is unique within a
        /// resource group. This name can be used to access the
        /// resource.</param>
        /// <param name="etag">A unique read-only string that changes whenever
        /// the resource is updated.</param>
        public LoadBalancingRule(string protocol, int frontendPort, string id = default(string), SubResource frontendIPConfiguration = default(SubResource), SubResource backendAddressPool = default(SubResource), SubResource probe = default(SubResource), string loadDistribution = default(string), int? backendPort = default(int?), int? idleTimeoutInMinutes = default(int?), bool? enableFloatingIP = default(bool?), string provisioningState = default(string), string name = default(string), string etag = default(string))
            : base(id)
        {
            FrontendIPConfiguration = frontendIPConfiguration;
            BackendAddressPool = backendAddressPool;
            Probe = probe;
            Protocol = protocol;
            LoadDistribution = loadDistribution;
            FrontendPort = frontendPort;
            BackendPort = backendPort;
            IdleTimeoutInMinutes = idleTimeoutInMinutes;
            EnableFloatingIP = enableFloatingIP;
            ProvisioningState = provisioningState;
            Name = name;
            Etag = etag;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets a reference to frontend IP addresses.
        /// </summary>
        [JsonProperty(PropertyName = "properties.frontendIPConfiguration")]
        public SubResource FrontendIPConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a reference to a pool of DIPs. Inbound traffic is
        /// randomly load balanced across IPs in the backend IPs.
        /// </summary>
        [JsonProperty(PropertyName = "properties.backendAddressPool")]
        public SubResource BackendAddressPool { get; set; }

        /// <summary>
        /// Gets or sets the reference of the load balancer probe used by the
        /// load balancing rule.
        /// </summary>
        [JsonProperty(PropertyName = "properties.probe")]
        public SubResource Probe { get; set; }

        /// <summary>
        /// Gets or sets the transport protocol for the external endpoint.
        /// Possible values are 'Udp' or 'Tcp'. Possible values include: 'Udp',
        /// 'Tcp'
        /// </summary>
        [JsonProperty(PropertyName = "properties.protocol")]
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the load distribution policy for this rule. Possible
        /// values are 'Default', 'SourceIP', and 'SourceIPProtocol'. Possible
        /// values include: 'Default', 'SourceIP', 'SourceIPProtocol'
        /// </summary>
        [JsonProperty(PropertyName = "properties.loadDistribution")]
        public string LoadDistribution { get; set; }

        /// <summary>
        /// Gets or sets the port for the external endpoint. Port numbers for
        /// each Rule must be unique within the Load Balancer. Acceptable
        /// values are between 1 and 65534.
        /// </summary>
        [JsonProperty(PropertyName = "properties.frontendPort")]
        public int FrontendPort { get; set; }

        /// <summary>
        /// Gets or sets the port used for internal connections on the
        /// endpoint. Acceptable values are between 1 and 65535.
        /// </summary>
        [JsonProperty(PropertyName = "properties.backendPort")]
        public int? BackendPort { get; set; }

        /// <summary>
        /// Gets or sets the timeout for the TCP idle connection. The value can
        /// be set between 4 and 30 minutes. The default value is 4 minutes.
        /// This element is only used when the protocol is set to TCP.
        /// </summary>
        [JsonProperty(PropertyName = "properties.idleTimeoutInMinutes")]
        public int? IdleTimeoutInMinutes { get; set; }

        /// <summary>
        /// Gets or sets configures a virtual machine's endpoint for the
        /// floating IP capability required to configure a SQL AlwaysOn
        /// Availability Group. This setting is required when using the SQL
        /// AlwaysOn Availability Groups in SQL server. This setting can't be
        /// changed after you create the endpoint.
        /// </summary>
        [JsonProperty(PropertyName = "properties.enableFloatingIP")]
        public bool? EnableFloatingIP { get; set; }

        /// <summary>
        /// Gets the provisioning state of the PublicIP resource. Possible
        /// values are: 'Updating', 'Deleting', and 'Failed'.
        /// </summary>
        [JsonProperty(PropertyName = "properties.provisioningState")]
        public string ProvisioningState { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource that is unique within a
        /// resource group. This name can be used to access the resource.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a unique read-only string that changes whenever the
        /// resource is updated.
        /// </summary>
        [JsonProperty(PropertyName = "etag")]
        public string Etag { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Protocol == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Protocol");
            }
        }
    }
}
