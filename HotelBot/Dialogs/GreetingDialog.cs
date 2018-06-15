using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HotelBot.Dialogs
{
	[Serializable]
	public class GreetingDialog : IDialog
	{
		public async Task StartAsync(IDialogContext context)
		{
			await context.PostAsync("Hi, I'm Kyle Bot.");
			await Respond(context);

			context.Wait(MessageReceivedAsync);
		}

		private static async Task Respond(IDialogContext context)
		{
			var username = string.Empty;
			context.UserData.TryGetValue("Name", out username);

			if (string.IsNullOrEmpty(username))
			{
				await context.PostAsync("What is your name?");
				context.UserData.SetValue("GetName", true);
			}
			else
			{
				await context.PostAsync(string.Format("Hi, {0}. How can I help you today?", username));
			}
		}

		private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
		{
			var message = await argument;
			var username = string.Empty;
			var getName = false;

			context.UserData.TryGetValue("Name", out username);
			context.UserData.TryGetValue("GetName", out getName);

			if (getName)
			{
				username = message.Text;
				context.UserData.SetValue("Name", username);
				context.UserData.SetValue("GetName", false);
			}

			await Respond(context);
			context.Done(message);
		}
	}
}