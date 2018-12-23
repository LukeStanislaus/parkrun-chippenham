using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using JWT;
using JWT.Serializers;
using QuickType;
using System.Data;
using Dapper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace tester
{

    public class Function
    {
        public static string Jwtdecoder(string token)
        {
            //const string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjbGFpbTEiOjAsImNsYWltMiI6ImNsYWltMi12YWx1ZSJ9.8pwBI_HtXqI3UgQHQ_rDRnSQRxFL1SR8fbQoS-5kM5s";
            const string secret = "260370628002-a90up4j8nbbees70evkgcp61a9mu4pfl.apps.googleusercontent.com";

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);


                //var json = decoder.Decode(token, secret, verify: true);
                var json = decoder.Decode(token);
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
        public static string GetSource(string name)
        {
            var name1 = name.Split(" ");
            var name2 = name1[0] + name1[1].ToUpper();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RequestUriString);
            var headername = doc.DocumentNode.SelectNodes("//table[@id='results']").ToList();
            string y = "";
            foreach (var item in headername)
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
            try
            {
                using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(Helper.CnnVal()))
                {
                    // Creates query.
                    var z = Jwtdecoder((((input as dynamic).originalDetectIntentRequest.payload.user.idToken) as object).ToString());
                    var a = token.Welcome.FromJson(z);
                    string str = "insert into races values(@name, @userid)";
                    connection.Execute(str, new
                    {
                        name = a.Name,
                        userid= ((input as dynamic).originalDetectIntentRequest.payload.user.userId)

                    });


                }
                return ReturnMessage((((input as dynamic).originalDetectIntentRequest.payload.user.idToken) as object).ToString());
                //return GetData();
            }
            catch
            {

            }
            try
            {

                var z = Jwtdecoder((((input as dynamic).originalDetectIntentRequest.payload.user.idToken) as object).ToString());
                var a = token.Welcome.FromJson(z);
                
                try
                {
                    return ReturnMessage("Hello, " + a.Name + ". Your parkrun time was " + GetSource(a.Name));
                }
                catch
                {
                      return ReturnMessage("Im afraid that you, "+ a.Name +"  did not run in the last Chippenham parkrun. Although I think you knew that already!");
                }
                //return ReturnMessage(((input as dynamic).originalDetectIntentRequest.payload.user.idToken as object).ToString());
            }
            catch
            {
                return GetData();
            }


        }
    }
}
