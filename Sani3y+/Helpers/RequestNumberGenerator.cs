namespace Sani3y_.Helpers
{
	public class RequestNumberGenerator
	{
		public static string GenerateRequestNumber()
		{
			string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
			string prefix = "REQ";
			Random random = new Random();
			int randomPart = random.Next(100, 1000);
			return $"{prefix}-{datePart}-{randomPart}";
		}
	}
}
