using Amazon.Lambda.Core;
using Dapper;
using HtmlAgilityPack;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace tester
{

    public class Function
    {
        public static string Jwtdecoder(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);


                //var json = decoder.Decode(token, secret, verify: true);
                string json = decoder.Decode(token);
                return json;
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
            return "";
        }
        private const string RequestUriString = "http://www.parkrun.org.uk/chippenham/results/latestresults/";

        private const string RequestUriStringAthleteNumber = "http://www.parkrun.org.uk/chippenham/results/athletehistory/?athleteNumber=";
        public static object GetData()
        {
            return JsonConvert.DeserializeObject(@"{
  ""payload"": {
    ""google"": {
      ""expectUserResponse"": true,
      ""richResponse"": {
        ""items"": [
          {
            ""simpleResponse"": {
              ""textToSpeech"": ""PLACEHOLDER""
            }
          }
        ]
      },
      ""userStorage"": ""{\""data\"":{}}"",
      ""systemIntent"": {
        ""intent"": ""actions.intent.PERMISSION"",
        ""data"": {
          ""@type"": ""type.googleapis.com/google.actions.v2.PermissionValueSpec"",
          ""optContext"": ""To address you by name and know your location"",
          ""permissions"": [
            ""NAME"",
            ""DEVICE_PRECISE_LOCATION""
          ]
        }
      }
    }
  }
}");
        }

        public static object ReturnMessage(string message)
        {
            return JsonConvert.DeserializeObject(@"{
                'payload': {
                    'google': {
                        'expectUserResponse': false,
      'richResponse': {
                            'items': [
                              {
            'simpleResponse': {
              'textToSpeech': '" + message + @"'
            }
}
        ]
      }
    }
  }
}");
        }
        public static string GetLast(string name)
        {
            int athleteNumber;
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(Helper.CnnVal()))
            {

                athleteNumber = connection.Query<int>("select athletenumber from nameathletenumber where name=@name1", new
                {
                    name1 = name


                }).First();

            }


            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc1 = web.Load(RequestUriStringAthleteNumber + athleteNumber);
            List<HtmlNode> headername1 = doc1.DocumentNode.SelectNodes("//table[@id='results']").ToList();
            HtmlNode item1 = headername1[2];

            string z = item1.LastChild.ChildNodes[0].ChildNodes[3].InnerText;

            return z;
        }

        public static string GetSource(string name)
        {
            string[] name1 = name.Split(" ");
            string name2 = name1[0] + " " + name1[1].ToUpper();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RequestUriString);
            List<HtmlNode> headername = doc.DocumentNode.SelectNodes("//table[@id='results']").ToList();
            string y = "";

            foreach (HtmlNode item in headername)
            {
                y += item.LastChild.ChildNodes.Where(x => x.ChildNodes[1].InnerText == name2).First().ChildNodes[2].InnerText;
            }

            return y;
        }


        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object FunctionHandler(object input, ILambdaContext context)
        {
            if ((((input as dynamic).queryResult.intent.displayName) as object).ToString() == "Default Welcome Intent")
            {

                string id = ((input as dynamic).originalDetectIntentRequest.payload.user.userId as object).ToString();
                try
                {
                    var name = GetName(id);
                    return Reply(name);
                }
                catch
                {

                    return GetData();

                }

            }
            else
            {

                string id = ((input as dynamic).originalDetectIntentRequest.payload.user.userId as object).ToString();
                string name = ((input as dynamic).originalDetectIntentRequest.payload.user.profile.displayName as object).ToString();
                SetName(id, name);
                return Reply(name);


            }



        }
        public static object Reply(string name)
        {
            try
            {
                string time = GetSource(name);
                return ReturnMessage("Your time in the last parkrun is " + time);

            }
            catch
            {
                return ReturnMessage("I am afraid that you, " + name + ", did not run in the last parkrun. Your" +
                     " last time was " + GetLast(name));
            }
        }
        public static string GetName(string id)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(Helper.CnnVal()))
            {

                return connection.Query<string>("select name from useridname where userid = @id1", new
                {
                    id1 = id

                }).First();

            }
        }
        public static void SetName(string id, string name)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(Helper.CnnVal()))
            {

                connection.Execute("insert into useridname values(@id1, @name1)", new
                {
                    id1 = id,
                    name1= name


                });

            }

        }
    }
}
