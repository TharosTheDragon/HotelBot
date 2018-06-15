using HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HotelBot.Dialogs
{
	[LuisModel("772234d4-b444-4d61-8e86-77db96eb58cf", "cb075f1ce3f342a7a25f01ce433b0751")]
	[Serializable]
	public class LUISDialog : LuisDialog<RoomReservation>
	{
		private readonly BuildFormDelegate<RoomReservation> reserveRoom;

		public LUISDialog(BuildFormDelegate<RoomReservation> reserveRoom)
		{
			this.reserveRoom = reserveRoom;
		}

		[LuisIntent("")]
		public async Task None(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("I'm sorry, I don't know what you mean.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Greeting")]
		public async Task Greeting(IDialogContext context, LuisResult result)
		{
			context.Call(new GreetingDialog(), CallBack);
		}

		private async Task CallBack(IDialogContext context, IAwaitable<object> result)
		{
			context.Wait(MessageReceived);
		}

		[LuisIntent("ReserveRoom")]
		public async Task ReserveRoom(IDialogContext context, LuisResult result)
		{
			var enrollmentForm = new FormDialog<RoomReservation>(new RoomReservation(), reserveRoom, FormOptions.PromptInStart);
			context.Call(enrollmentForm, CallBack);
		}

		[LuisIntent("QueryAmenities")]
		public async Task QueryAmenities(IDialogContext context, LuisResult result)
		{
			foreach (var entity in result.Entities.Where(e => e.Type == "Amenity"))
			{
				var value = entity.Entity.ToLower();

				switch (value)
				{
					case "towels": case "pool": case "gym": case "wifi":
						await context.PostAsync("Yes, we have that!");
						context.Wait(MessageReceived);
						return;
				}
			}

			await context.PostAsync("I'm sorry, we don't have that.");
			context.Wait(MessageReceived);
			return;
		}
	}
}