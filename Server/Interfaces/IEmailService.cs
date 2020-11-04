using System.Threading.Tasks;

using MimeKit;

namespace CRM.Server.Interfaces
{
	public interface ISmtpService
	{
		Task SendEmailAsync(MimeMessage message);
	}
}