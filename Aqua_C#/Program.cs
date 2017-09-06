// <del>Python 2.7 --> C#.NET (2016-03-04)</del>
// 소스코드 제공 by 석탄봇
// Project Aqua

using Jint;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading;
using System.Timers;
using System.IO;

namespace Aqua_Bot
{
	class Program
	{
		private static string name = "Aqua";
		private static double version = 0.01;
		private static string owner_chatID = "33242449";
		private static string sireikan_UserID = "cellularhacker1216";

		private static int bot_emotion_sad = 0;
		//private static int bot_emotion_happy = 0;

		private static string botID = "190600725:AAHeJrw8WnTodWfZ1lncrxhn3vmWL2KdjAQ"; //API ID
		private static System.Timers.Timer botGetdataTimer = new System.Timers.Timer();
		private static int lastOffset = 0;
		private static string[] botcommandPharseChar = { " " };
		//private static string[] botQutPhaseChar = { "!" };

		private static string callCommand = "Aqua";
		private static string currentTime = "";

		private static bool isWorking = false;

		private static string System_information_sireikan = "";
		private static string System_information = "";
		private static bool command_mode = false;
		private static string hookChatID = "";
		private static bool ChatIDisHooked = false;
		private static bool testmode = false;
		private static string loadedTime = "";




		static void Main(string[] args)
		{
			FileStream ostrm;
			StreamWriter writer;
			TextWriter oldOut = Console.Out;
			loadedTime = DateTime.Now.ToString("yyyyMMdd-HHmm");

			try
			{
				ostrm = new FileStream("./logs/CH_Aqua_bot-" + loadedTime + ".log", FileMode.OpenOrCreate, FileAccess.Write);
				writer = new StreamWriter(ostrm);
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot open Redirect.txt for writing");
				Console.WriteLine(e.Message);
				return;
			}
			if (testmode == false)  Console.SetOut(writer);

			Console.WriteLine("Loading assets and Modules...");
			SystemInfo_Load();
			SystemInfo_Sireikan_Load();
			botGetdataTimer.Interval = 1000; //1초 루프.
			Aqua_LoadFunc();


			botGetdataTimer.Elapsed += new ElapsedEventHandler(refreshFunc);
			Console.WriteLine("Starting " + name + " " + version);


			botGetdataTimer.Start();



			Console.ReadLine(); //꺼짐방지

			Aqua_exitFunc(owner_chatID);
			if (testmode == false) Console.SetOut(oldOut);
			writer.Close();
			ostrm.Close();

		}

		private static void SystemInfo_Sireikan_Load()
		{
			System_information_sireikan = "UserName: " + Environment.UserName +
			"\n UserDomainName: " + Environment.UserDomainName +
			"\n NetBIOS: " + Environment.MachineName +
			"\n OS Version: " + Environment.OSVersion.VersionString +
			"\n Platform: " + Environment.OSVersion.Platform.ToString() +
			"\n ProcessorCount: " + Environment.ProcessorCount +
			"\n Running Time: " + Environment.TickCount +
			"\n";
			Console.WriteLine("System information(Sireikan) has been Successfully loaded.");
			Console.WriteLine("\n Loaded System Information\n\n" + System_information_sireikan);
		}

		private static void SystemInfo_Load()
		{
			System_information =
			"\n NetBIOS: " + Environment.MachineName +
			"\n OS Version: " + Environment.OSVersion.VersionString +
			"\n Platform: " + Environment.OSVersion.Platform.ToString() +
			"\n ProcessorCount: " + Environment.ProcessorCount +
			"\n Running Time: " + Environment.TickCount +
			"\n";
			Console.WriteLine("System information(Basic) has been Successfully loaded.");
		}





		// Aqua Launch Alert
		static void Aqua_LoadFunc()
		{
			//메시지 전송
			//Console.WriteLine(Uri.EscapeDataString(message));
			currentTime = DateTime.Now.ToString("h:mm:ss tt");
			try
			{
				WebClient wClientVariab = new WebClient();
				string response = wClientVariab.DownloadString("https://api.telegram.org/bot" + botID + "/" + "sendMessage" +
					"?chat_id=" + owner_chatID /* 메시지를 전송할 대상 채팅방 ID */ +
					"&text=" + Uri.EscapeDataString("다녀왔습니다, 주인님.") /* 메시지 */
					);
				Console.WriteLine("Aqua has been loaded!\n" + response + "\n");
			}
			catch (Exception e)
			{
				Console.WriteLine("[" + currentTime + "] " + e + " in " + owner_chatID + " => 서버에 명령 전달을 실패했습니다.");
			}
		}


		// Aqua Shutdown Alert
		static void Aqua_exitFunc(string rcv_owner_chatID)
		{
			//메시지 전송
			//Console.WriteLine(Uri.EscapeDataString(message));
			currentTime = DateTime.Now.ToString("h:mm:ss tt");
			try
			{
				WebClient wClientVariab = new WebClient();
				string response = wClientVariab.DownloadString("https://api.telegram.org/bot" + botID + "/" + "sendMessage" +
					"?chat_id=" + owner_chatID +
					"&text=" + Uri.EscapeDataString("그럼, 잠시 휴식을 취하겠습니다, 주인님.")
					);
				Console.WriteLine("Aqua just go to rest.\n" + response + "\n");
			}
			catch (Exception e)
			{
				Console.WriteLine("[" + currentTime + "] " + e + " in " + owner_chatID + " => 서버에 명령 전달을 실패했습니다.");
			}
		}






		static void refreshFunc(object sender, EventArgs e)
		{


			//새 메시지가 있나 가져옴
			if (isWorking == true)
			{
				return;
			}

			isWorking = true;

			JObject messagesJsonArray = owner_chatIDs("getUpdates", "?offset=" + lastOffset.ToString());
			JArray resultArray = (JArray)messagesJsonArray["result"];

			currentTime = DateTime.Now.ToString("h:mm:ss tt");

			if (resultArray.Count == 0)
			{
				//새 메시지 없음 
			}
			else {
				/*
				//새 메시지가 있으므로 받아옴
				if (resultArray.Count > 10 && lastOffset == 0)
				{
					//내용이 너무 많으므로 타이머를 빨리 돌려서 받아옵니다.
					botGetdataTimer.Interval = 100; //0.1초 루프.

					for (int i = 0; i < resultArray.Count; ++i)
					{
						lastOffset = Int32.Parse((string)resultArray[i]["update_id"]);
					}
					lastOffset += lastOffset == 0 ? 0 : 1;
					owner_chatIDs("getUpdates", "?offset=" + lastOffset.ToString()); //받아와야 처리 끝

					isWorking = false;
					botGetdataTimer.Interval = 2000; //2초 루프.
					return;
				}*/

				for (int i = 0; i < resultArray.Count; ++i)
				{
					//오프셋을 항상 마지막으로 적용
					lastOffset = int.Parse((string)resultArray[i]["update_id"]);
					JObject messagesObject = (JObject)resultArray[i]["message"];
					string botParseTarowner_chatID = (string)messagesObject["text"];
					string botCommandStr = "";
					string[] botCommandParsedArray;

					string targetChatID = (string)messagesObject["chat"]["id"];
					string targetUserID = (string)messagesObject["from"]["username"];
					string targetUserName = (string)messagesObject["from"]["first_name"];





					if (botParseTarowner_chatID == null)
					{
						continue;
					}
					//Console.WriteLine("[" + currentTime + "] " + targetUserName + "(" + targetUserID + ") in " + targetChatID + " => " + botParseTarowner_chatID);

					// 주인의 ChatID가 아닐 때, "[ChatID] UserName : \n 말한내용" 형태로 주인에게 전송
					if (targetChatID != owner_chatID)
					{
						// ChatID를 별칭으로 변환
						// 그룹 채팅ID -> 별칭
						if (targetChatID == "-147157881") targetChatID = "AVN";
						else if (targetChatID == "-122511989") targetChatID = "TEST";
						else if (targetChatID == "-135916593") targetChatID = "일본단톡";
						// 개인 채팅ID -> 별칭
						else if (targetChatID == "185257126") targetChatID = "고영조(까떼)";
						else if (targetChatID == "218361152") targetChatID = "황주성(나비)";


						Console.WriteLine("[" + currentTime + "] " + targetUserName + "(" + targetUserID + ") in " + targetChatID + " => " + botParseTarowner_chatID);
						sendMessage(owner_chatID, "[" + targetChatID + "] " + targetUserName + " : \n" + botParseTarowner_chatID);
					}

					// 주인이 1:1로 메시지를 보냈을 때
					else if (targetChatID == owner_chatID)
					{
						if (command_mode == true)
						{
							Console.WriteLine("[" + currentTime + "] " + targetUserName + "(" + targetUserID + ") in " + targetChatID + " => " + botParseTarowner_chatID);


							//하위 첫번째 명령어까지만 사용하는 경우 사용하는 args
							botCommandParsedArray = botCommandStr.Split(botcommandPharseChar, StringSplitOptions.RemoveEmptyEntries);

							//Console.WriteLine("[" + currentTime + "] " + targetUserName + "(" + targetUserID + ") in " + targetChatID + " => " + botParseTarowner_chatID);

							//하위 첫번째 명령어까지만 사용하는 경우 사용하는 args
							string HookingChatID = "";
							string SelCommand = "";


							//Console.WriteLine("Going to check HookingChatID");
							for (int tmpi = 2; tmpi < botParseTarowner_chatID.Length; ++tmpi)
							{
								HookingChatID += botParseTarowner_chatID[tmpi];
								//Console.WriteLine("botParseTarowner_chatID[tmpi] = " + botParseTarowner_chatID[tmpi]);
							}
							//Console.WriteLine();
							//Console.WriteLine("Going to check SelCommand");
							for (int tmpi = 0; tmpi < botParseTarowner_chatID.Length; ++tmpi)
							{
								if (botParseTarowner_chatID[tmpi] == ' ') break;
								SelCommand += botParseTarowner_chatID[tmpi];
								//Console.WriteLine("botParseTarowner_chatID[tmpi] = " + botParseTarowner_chatID[tmpi]);
							}

							//Console.WriteLine();
							switch (HookingChatID)
							{
								// 그룹 채팅방ID
								case "AVN":
									HookingChatID = "-147157881";
									break;

								case "TEST":
									HookingChatID = "-122511989";
									break;

								case "일본단톡":
									HookingChatID = "-135916593";
									break;

								// 개인ID
								case "고영조":
								case "까떼":
								case "고영조(까떼)":
								case "까떼(고영조)":
									HookingChatID = "185257126";
									break;

								case "나비":
								case "나비그래픽":
								case "황주성":
								case "Nabi":
								case "NabiGraphics":
								case "황주성(나비)":
								case "황주성(Nabi)":
									HookingChatID = "218361152";
									break;

								// 미리 알고있는(등록된) ChatID가 아닐경우, 그냥 그대로 숫자로 표시합니다.
								default:
									break;
							}

							switch (SelCommand)
							{
								// "1 [ChatID]" 형식으로 입력했을 경우
								case "1":
									sendMessage(owner_chatID, "네, 주인님. ChatID를 " + HookingChatID + "로 고정하겠습니다.");
									Console.WriteLine("[" + currentTime + "] Aqua(CH_Aqua_bot) => 네, 주인님. ChatID를 [" + HookingChatID + "]로 고정하겠습니다.");
									hookChatID = HookingChatID;
									ChatIDisHooked = true;
									command_mode = false;
									break;
								// "2" 일 경우 => 고정한 ChatID 출력
								case "2":
									if (hookChatID == "")
									{
										sendMessage(owner_chatID, "주인님, 아직 고정된 ChatID는 없습니다.");
										Console.WriteLine("[" + currentTime + "] Aqua(CH_Aqua_bot) => 주인님, 아직 고정된 ChatID는 없습니다.");
										break;
									}
									sendMessage(owner_chatID, "네, 주인님. 현재 고정된 ChatID는 " + hookChatID + "입니다.");
									Console.WriteLine("[" + currentTime + "] Aqua(CH_Aqua_bot) => 네, 주인님. 현재 고정된 ChatID는 [" + hookChatID + "]입니다.");
									break;
								// "3" 이었을 경우, 프로세스 종료.
								case "3":
									Aqua_exitFunc(owner_chatID);
									System.Environment.Exit(1);
									break;
								default:
									sendMessage(owner_chatID, "주인님, 죄송합니다. 명령을 다시 한 번 확인을 부탁드려도 되겠습니까?" + HookingChatID);
									Console.WriteLine("[" + currentTime + "] Aqua(CH_Aqua_bot) => 주인님, 죄송합니다. 명령을 다시 한 번 확인을 부탁드려도 되겠습니까?" + HookingChatID);
									break;
							}
							command_mode = false;
							continue;
						}


						botCommandParsedArray = botCommandStr.Split(botcommandPharseChar, StringSplitOptions.RemoveEmptyEntries);

						//Console.WriteLine("[" + currentTime + "] " + targetUserName + "(" + targetUserID + ") in " + targetChatID + " => " + botParseTarowner_chatID);

						//하위 첫번째 명령어까지만 사용하는 경우 사용하는 args
						string botGotContent = "";
						string botReplyChatID = "";

						// 고정된 ChatID가 없을때 주인이 보낸 메시지 중 ChatID부분과 보낼메시지로 나눔.
						if (ChatIDisHooked == false)
						{
							for (int tmpi = 1; tmpi < botCommandParsedArray.Length; ++tmpi)
							{
								botGotContent += (i == 1 ? "" : " ") + botCommandParsedArray[tmpi];
							}
							for (int tmpi = 0; tmpi < botParseTarowner_chatID.Length; ++tmpi)
							{
								if (botParseTarowner_chatID[tmpi] == ' ') break;
								botReplyChatID += botParseTarowner_chatID[tmpi];
							}
						}

						//
						else {
							for (int tmpi = 0; tmpi < botParseTarowner_chatID.Length; ++tmpi)
							{
								botGotContent += botParseTarowner_chatID[tmpi];
								//Console.WriteLine("botParseTarowner_chatID[tmpi] = " + botParseTarowner_chatID[tmpi]);
							}
						}
						// 왼쪽: ChatID 고정한게 없을 때 | 오른쪽: ChatID가 고정된게 있을 때, 각각 호출감지
						if (botReplyChatID == "아쿠아" || botGotContent == "아쿠아")
						{
							sendMessage(owner_chatID, "네, 주인님. 무엇을 도와드릴까요?\n"
							+ "1. 채팅방 ID를 고정하거나 변경합니다.\n"
							+ "2. 현재 고정된 ChatID를 확인하시겠습니까?\n"
							+ "3. 잠시 휴식을 취하겠습니다.");
							command_mode = true;
							Console.WriteLine("[" + currentTime + "] Aqua(CH_Aqua_bot) => 주인님께서 저를 부르셨습니다.");
							continue;
						}

						// 고정된 ChatID가 없을 때, Chat별칭을 ChatID로 변경
						// 그룹 채팅ID
						if (botReplyChatID == "AVN") botReplyChatID = "-147157881";
						else if (botReplyChatID == "TEST") botReplyChatID = "-122511989";
						else if (targetChatID == "-135916593") targetChatID = "일본단톡";
						// 개인 채팅ID -> 별칭
						else if (targetChatID == "185257126") targetChatID = "고영조(까떼)";
						else if (targetChatID == "218361152") targetChatID = "황주성(나비)";

						// 주인이 부른게 아닐 때.
						if (command_mode == false)
						{
							if (ChatIDisHooked == false)
							{
								Console.WriteLine("ReplyChatID: [" + botReplyChatID + "], ReplyText: [" + botGotContent + "]");
								sendMessage(botReplyChatID, botGotContent);
							}
							// 고정된 ChatID가 있을 때, 해당 ChatID값으로 주인이 보낸 메시지를 그대로ㅁ 전송 
							else {
								Console.WriteLine("ReplyChatID: [" + hookChatID + "], ReplyText: [" + botGotContent + "]");
								sendMessage(hookChatID, botGotContent);
							}
						}
						//Console.WriteLine("[command_mode] = " + command_mode + ", [ChatIDisHooked] = " + ChatIDisHooked);

					}

				} //end for */

				if (lastOffset != 0)
				{
					lastOffset += 1; //오프셋에 1을 더해야 중복 메시지를 받아오지 않음
				} //오프셋이 0이 아닌경우
			} //결과 끝

			isWorking = false;
			//Console.WriteLine(owner_chatIDs("getUpdates", ""))
		}

		static void sendMessage(string chatID, string message, string parseMode = "", string replyID = "", bool isSlientMessage = true)
		{
			//메시지 전송
			//Console.WriteLine(Uri.EscapeDataString(message));

			// 주인의 채팅방ID였을경우, 알림을 강제로 활성화.
			currentTime = DateTime.Now.ToString("h:mm:ss tt");
			try
			{
				WebClient wClientVariab = new WebClient();
				string response = wClientVariab.DownloadString("https://api.telegram.org/bot" + botID + "/" + "sendMessage" +
					"?chat_id=" + chatID /* 메시지를 전송할 대상 채팅방 ID */ +
					"&text=" + Uri.EscapeDataString(message) /* 메시지 */ +
					"&parse_mode=" + Uri.EscapeDataString(parseMode) /* 마크다운/HTML (형식 있는 메시지) 로 전송 여부 */ +
					"&reply_to_message_id=" + replyID /* 누군가에 대한 답장일 경우 사용. */ +
					"&disable_notification=" + Uri.EscapeDataString(isSlientMessage.ToString()) /* 알람 없이 메시지를 보낼 경우 사용. */
					);
			}
			catch (Exception e)
			{
				Console.WriteLine("[" + currentTime + "] " + e + "in " + chatID + " => 서버에 명령 전달을 실패했습니다.");
			}
		}




		static JObject owner_chatIDs(string actionStr, string actionParameters)
		{
			WebClient wClientVariab = new WebClient();
			string response = wClientVariab.DownloadString("https://api.telegram.org/bot" + botID + "/" + actionStr + actionParameters);

			JObject jObjResult = JObject.Parse(response);

			if ((bool)jObjResult["ok"] == false)
			{
				Console.WriteLine(" API Failed - ", actionStr, actionParameters);
			}

			//test
			return jObjResult;
		}
	}
}