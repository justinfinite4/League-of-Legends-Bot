
using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueBot.Patterns;
using LeagueBot.ApiHelpers;
using System.IO;

namespace LeagueBot
{
    public class StartCoop : PatternScript
    {
        private const string MODE = "intro";
        
        private Random RandomTextSender;
        
        private const string SELECTED_CHAMPION = "ashe";

        public override void Execute()
        {
            bool
                MethodFound = false
                , develop_mode = false
                , restartneeded = false;

            string[] champs = null;
            int cnt;

            do
            {
                if (bot.isProcessOpen(GAME_PROCESS_NAME) == true)
                    MethodFound = true;
                else
                {
                    bot.log("Waiting for league client process...");
                    bot.waitProcessOpen(CLIENT_PROCESS_NAME);
                    bot.bringProcessToFront(CLIENT_PROCESS_NAME);
                    bot.centerProcess(CLIENT_PROCESS_NAME);

                    bot.log("Client ready.");
                    bot.KillProcess(CLIENT_PROCESS_NAME);
                    bot.wait(13000);

                    bot.waitProcessOpen(CLIENT_PROCESS_NAME);
                    bot.bringProcessToFront(CLIENT_PROCESS_NAME);
                    bot.centerProcess(CLIENT_PROCESS_NAME);
                    bot.log("Client ready.");

                    client.createLobby(MODE);

                    bot.log("Attempting to search for game...");

                    client.startQueue();

                    if (client.leaverbuster())
                    {
                        restartneeded = true;

                        bot.log("Leaverbuster detected");

                        while (client.leaverbuster())
                            bot.wait(500);
                    }

                    if (restartneeded == true)
                    {
                        restartneeded = false;

                        client.startQueue();
                    }

                    while (!client.inChampSelect())
                    {
                        client.acceptQueue();
                        bot.wait(3000);
                    }

                    bot.log("Match found");

                    if (champs == null)
                        champs = io.getChamps();

                    if (champs.Length > 0)
                        foreach (string champ in champs)
                        {
                            bot.log("Attempting to pick " + champ);
                            client.pickChampionByName(champ);
                        }
                    else
                        client.pickChampionByName(SELECTED_CHAMPION);
                    bot.log("waiting for league of legends process...");

                    //bot._outActualTime = 0
                    cnt = 0;

                    do
                    {
                        if (bot.isProcessOpen(GAME_PROCESS_NAME) == true)
                        {
                            MethodFound = true;

                            break;
                        }
                        else
                        {
                            bot.wait(1000);

                            cnt += 1;
                        }
                    }
                    while
                    (
                        cnt < 100
                        && develop_mode == false
                    );
                }

                if (MethodFound == true)
                {
                    bot.waitProcessOpen(GAME_PROCESS_NAME);
                    bot.log("Champion selected, loading game...");

                    bot.executePattern("Coop");
                }
                else
                    bot.log("Failed to load game");
            } while (MethodFound == false);

            bot.executePattern("EndCoop");
        }
    }
}
