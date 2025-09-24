namespace TodoApi.Configuration
{
	public class AzureAdOptions
	{
		public string TenantId { get; set; } = string.Empty;
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
		public string Instance { get; set; } = string.Empty;
		public string Domain { get; set; } = string.Empty;
	}
}