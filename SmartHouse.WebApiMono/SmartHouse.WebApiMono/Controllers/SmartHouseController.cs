﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SmartHouse.Lib;

namespace SmartHouse.WebApiMono
{
	[RoutePrefix("api/SmartHouse")]
	public class SmartHouseController : BaseController
	{
		private readonly IYamahaService YamahaService;
		private readonly IPanodraService PandoraService;
		private readonly ISmartHouseService SmartHouseService;
		private readonly IMPDService MpdService;

		public SmartHouseController(ISettingsService service, IYamahaService yamahaService, IPanodraService pandoraService, ISmartHouseService smartHouseService, IMPDService mpdService) 
			: base(service)
		{
			YamahaService = yamahaService;
			PandoraService = pandoraService;
			SmartHouseService = smartHouseService;
			MpdService = mpdService;
		}

		[HttpGet]
		[Route("TurnOn")]
		public async Task<Result> TurnOn()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.StandBy)
			{
				await YamahaService.TurnOn();
				sb.AppendLine("Yamaha Turn on");
				await Task.Delay(TimeSpan.FromSeconds(8));
			}

			await YamahaService.SetInput("AV2");
			sb.AppendLine("Setting AV2 input");

			await SmartHouseService.SetMode(ModeEnum.Normal);
			sb.AppendLine("Setting Normal mode");

			var state = await SmartHouseService.GetCurrentState();

			if (state == SmartHouseState.Music && MpdService.GetStatus().State != Libmpc.MpdState.Play)
			{
				PandoraService.StopTcp().Wait(1000);
				MpdService.Play();
				sb.AppendLine("Playing MPD");
				await SmartHouseService.SaveState(SmartHouseState.Music);
			}

			else if (!PandoraService.IsPlaying())
			{
				PandoraService.StartTcp().Wait(1000);
				PandoraService.Play();
				sb.AppendLine("Playing pandora radio");	
				await SmartHouseService.SaveState(SmartHouseState.Pandora);
			}

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("TurnOff")]
		public async Task<Result> TurnOff()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (PandoraService.IsPlaying())
			{
				PandoraService.Pause();
				sb.AppendLine("Pausing pandora radio");	
			}

			if (MpdService.GetStatus().State == Libmpc.MpdState.Play)
			{
				MpdService.Stop();
				sb.AppendLine("Stopping MPD");
			}

			if (powerStatus == PowerStatusEnum.On)
			{
				await Task.Delay(TimeSpan.FromSeconds(2));
				await YamahaService.TurnOff();
				sb.AppendLine("Yamaha Turn Off");
			}

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("VolumeUp")]
		public async Task<Result> VolumeUp()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.On)
			{
				await YamahaService.VolumeUp();
				sb.AppendLine("Yamaha Volume Up");
			}
			else
			{
				sb.AppendLine("Yamaha is turned off");
			}

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("VolumeDown")]
		public async Task<Result> VolumeDown()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.On)
			{
				await YamahaService.VolumeDown();
				sb.AppendLine("Yamaha Volume Down");
			}
			else
			{
				sb.AppendLine("Yamaha is turned off");	
			}

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("SetMode")]
		public async Task<Result> SetMode(string mode)
		{
			var modeEnum = (ModeEnum)Enum.Parse(typeof(ModeEnum), mode);

			var result = await SmartHouseService.SetMode(modeEnum);
			return result;
		}

		[HttpGet]
		[Route("Xbox")]
		public async Task<Result> Xbox()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.StandBy)
			{
				await YamahaService.TurnOn();
				sb.AppendLine("Yamaha Turn on");
				await Task.Delay(TimeSpan.FromSeconds(8));
			}

			if (MpdService.GetStatus().State == Libmpc.MpdState.Play)
			{
				MpdService.Stop();
				sb.AppendLine("Stopping MPD");
			}

			if (PandoraService.IsPlaying())
			{
				PandoraService.Pause();
				sb.AppendLine("Pausing pandora radio");
			}				

			await YamahaService.SetInput("HDMI2");
			sb.AppendLine("Set HDMI2 input");

			await SmartHouseService.SaveState(SmartHouseState.XBox);

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("Pandora")]
		public async Task<Result> Pandora()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.StandBy)
			{
				await YamahaService.TurnOn();
				sb.AppendLine("Yamaha Turn on");
				await Task.Delay(TimeSpan.FromSeconds(8));
			}

			if (MpdService.GetStatus().State == Libmpc.MpdState.Play)
			{
				MpdService.Stop();
				sb.AppendLine("Stopping MPD");
			}


			if (!PandoraService.IsPlaying())
			{
				PandoraService.StartTcp().Wait(1000);

				PandoraService.Play();
				sb.AppendLine("Playing pandora radio");
			}

			await YamahaService.SetInput("AV2");
			sb.AppendLine("Set AV2 input");

			await SmartHouseService.SaveState(SmartHouseState.Pandora);

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("Music")]
		public async Task<Result> Music()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.StandBy)
			{
				await YamahaService.TurnOn();
				sb.AppendLine("Yamaha Turn on");
				await Task.Delay(TimeSpan.FromSeconds(8));
			}

			await PandoraService.StopTcp();
			sb.AppendLine("Stopping pandora radio");

			await YamahaService.SetInput("AV2");
			sb.AppendLine("Set AV2 input");

			MpdService.Play();

			await SmartHouseService.SaveState(SmartHouseState.Music);

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("TV")]
		public async Task<Result> TV()
		{
			var sb = new StringBuilder();
			var powerStatus = await YamahaService.PowerStatus();

			if (powerStatus == PowerStatusEnum.StandBy)
			{
				await YamahaService.TurnOn();
				sb.AppendLine("Yamaha Turn on");
				await Task.Delay(TimeSpan.FromSeconds(8));
			}

			if (MpdService.GetStatus().State == Libmpc.MpdState.Play)
			{
				MpdService.Stop();
				sb.AppendLine("Stopping MPD");
			}

			if (PandoraService.IsPlaying())
			{
				PandoraService.Pause();	
				sb.AppendLine("Pause pandora radio");
			}

			await YamahaService.SetInput("AUDIO1");
			sb.AppendLine("Setting AUDIO1 input");

			await SmartHouseService.SaveState(SmartHouseState.TV);

			return new Result()
			{
				ErrorCode = 0,
				Message = sb.ToString(),
				Ok = true
			};
		}

		[HttpGet]
		[Route("GetCurrentState")]
		public async Task<string> GetCurrentState()
		{
			var state = await SmartHouseService.GetCurrentState();
			return state.ToString();
		}

		[HttpGet]
		[Route("RestartOpenVPN")]
		public async Task<Result> RestartOpenVPN()
		{
			var result = await SmartHouseService.RestartOpenVPNServiceTcp();
			return result;
		}

		[HttpGet]
		[Route("PlayAlarm")]
		public async Task<Result> PlayAlarm()
		{
			var result = await SmartHouseService.PlayAlarmTcp();
			return result;
		}
	}
}
