using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;
using System.Xml.XPath;


class Program
{
   public static async Task Main()
    {
        Console.Clear();
        string url = "https://www.dnd5eapi.co/api/2014/monsters/";
        int totalXP;
        
        Console.WriteLine("Pup-Tart and his band of shibas are headed into a dungeon. How many monsters await them?");
        
        //Returns the number of inhabitants, then searches the API for them
        int inhabitants;
        while (!int.TryParse(Console.ReadLine(), out inhabitants) || inhabitants <= 0)
            {
                Console.WriteLine("Please enter a valid number of monsters.");
            }

        Task monster = Monster(url, inhabitants);

        Console.WriteLine($"{inhabitants} monsters? Hope they're up for the challenge!");

        await monster;
    }

    public static async Task Monster(string url, int inhabitants)
    {
        await Task.Delay(2000);
        int totalXP = 0;
        string creatureIndex = "";

        //API Call
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            //Deserialize the JSON response into a list of creatures
            var monsterResponse = JsonSerializer.Deserialize<MonsterResponse>(responseBody);

         /* This loop runs a number of times equal to the inhabitants value entered at the beginning.
            It  then pulls a random monster, then pulls the index number to further search in the API. */
            Random random = new Random();
            for (int i = 0; i < inhabitants; i++)
            {
                int x = random.Next(0,monsterResponse.results.Count);
                creatureIndex = monsterResponse.results[x].index;
                Task stats = MonsterStats(creatureIndex);

                await stats;
                int xp = await MonsterStats(creatureIndex);

                totalXP += xp;
            }

        }
        Console.WriteLine($"The total amount of XP Pup-Tart and his shiba band can gain from this dungeon is {totalXP} XP. They might level a few times!");
    }

    public static async Task<int> MonsterStats(string index)
    {
        string url = ("https://www.dnd5eapi.co/api/2014/monsters/"+index);
         using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            //Deserialize the JSON response into a list of creatures
            var creature = JsonSerializer.Deserialize<CreatureDetails>(responseBody);

            Console.WriteLine($"{creature.name} ({creature.size} {creature.type})\nChallenge Rating {creature.challenge_rating} ({creature.xp} XP)\n");
            return creature.xp;
        }
    }

    public class MonsterResponse
    {
       public int count {get; set;}
       public List<Creature> results {get; set;} 
    }

    public class Creature
    {
        public string? index {get; set;}
        public string? name {get; set;}
        public string? url {get; set;}
    }

    public class CreatureDetails
    {
        public string? name {get; set;}
        public string? size {get; set;}
        public string? type {get; set;}
        public double? challenge_rating {get; set;}
        public int xp {get; set;}
    }
}