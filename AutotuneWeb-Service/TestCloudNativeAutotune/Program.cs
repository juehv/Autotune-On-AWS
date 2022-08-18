using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace TestCloudNativeAutotune
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Uri nsUrl = new Uri("https://your-ns-domain-here/");
            NSProfileDetails nsProfile;
            DateTime profileActivation;


            try
            {
                nsProfile = NSProfileDetails.LoadFromNightscout(ref nsUrl, out profileActivation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            nsProfile.CarbRatio = CombineAdjacentTimeBlocks(nsProfile.CarbRatio);
            nsProfile.Sensitivity = CombineAdjacentTimeBlocks(nsProfile.Sensitivity);

            if (nsProfile.CarbRatio.Length > 1)
            {
                var averageValue = Math.Round(nsProfile.CarbRatio.Average(cr => cr.Value), 1);
                nsProfile.CarbRatio = new[] { new NSValueWithTime { TimeAsSeconds = 0, Value = averageValue } };
            }

            if (nsProfile.Sensitivity.Length > 1)
            {
                var averageValue = Math.Round(nsProfile.Sensitivity.Average(sens => sens.Value), 1);
                nsProfile.Sensitivity = new[] { new NSValueWithTime { TimeAsSeconds = 0, Value = averageValue } };

            }

            var oapsProfile = nsProfile.MapToOaps();

            var warnings = new List<string>();

            if (oapsProfile.Dia < 3)
            {
                warnings.Add($"DIA of {oapsProfile.Dia} hours is too short. It has been automatically adjusted to the minimum of 3 hours");
                oapsProfile.Dia = 3;
            }

            if (!TempBasalIncludesRateProperty(nsUrl))
                warnings.Add("Temporary basal records in this Nightscout instance do not include a \"rate\" property - they will not be taken into account by Autotune");

            var json = JsonConvert.SerializeObject(oapsProfile, Formatting.Indented);
            Console.WriteLine(json);
        }


        private static NSValueWithTime[] CombineAdjacentTimeBlocks(NSValueWithTime[] values)
        {
            var combined = new List<NSValueWithTime>();

            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0 || combined.Last().Value != values[i].Value)
                    combined.Add(values[i]);
            }

            return combined.ToArray();
        }

        private static bool TempBasalIncludesRateProperty(Uri url)
        {
            var profileSwitchUrl = new Uri(url, "/api/v1/treatments.json?find[eventType][$eq]=Temp%20Basal&count=10");
            var req = WebRequest.CreateHttp(profileSwitchUrl);
            using (var resp = req.GetResponse())
            using (var stream = resp.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var tempBasals = JsonConvert.DeserializeObject<NSTempBasal[]>(json);

                return tempBasals.Length == 0 || tempBasals.Any(tb => tb.Rate != null);
            }
        }

        class NSTempBasal
        {
            public decimal? Rate { get; set; }
        }

    }
}
