using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;

namespace mybot.Controllers
{
    public class RockPaperScissorsController : ApiController
    {
        private static Random _rnd = new Random();
        private IList<string> _moves = new List<string> { "sten", "sax", "påse" };
        
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.Message)
            {
                var moveIsValid = _moves.Contains(activity.Text.ToLower());

                if (!moveIsValid)
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply($"Ehh... vad är det där för drag?!"));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                var player1Move = activity.Text.ToLower();
                var player2Move = GetRandomMove();

                var reply = activity.CreateReply($"Mitt drag är {player2Move}");
                await connector.Conversations.ReplyToActivityAsync(reply);

                var result = GetResult(player1Move, player2Move);

                string resultString;
                if (result == GameResult.Player1Wins)
                    resultString = "Du vann!";
                else if (result == GameResult.Player2Wins)
                    resultString = "Du förlorade!";
                else
                    resultString = "Lika!";
                
                var winner = activity.CreateReply(resultString);
                await connector.Conversations.ReplyToActivityAsync(winner);

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        private string GetRandomMove()
        {
            return _moves[_rnd.Next(0, 3)];
        }

        private GameResult GetResult(string p1Move, string p2Move)
        {
            if (p1Move == p2Move)
            {
                return GameResult.Draw;
            }

            if (p1Move == "sten")
                return p2Move == "sax" ? GameResult.Player1Wins : GameResult.Player2Wins;

            if (p1Move == "sax")
                return p2Move == "påse" ? GameResult.Player1Wins : GameResult.Player2Wins;

            //if (p1Move == "påse")
            return p2Move == "sten" ? GameResult.Player1Wins : GameResult.Player2Wins;
        }
    }

    public enum GameResult
    {
        Player1Wins,
        Draw,
        Player2Wins
    }
}
