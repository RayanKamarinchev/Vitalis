using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI.Chat;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.GptResponses;
using Vitalis.Data;
using Vitalis.Data.Entities;

namespace Vitalis.Core.Services
{
    public class MoleculeService : IMoleculeService
    {
        private readonly IConfiguration config;
        private readonly VitalisDbContext context;

        public MoleculeService(IConfiguration _config, VitalisDbContext _context)
        {
            config = _config;
            context = _context;
        }

        public async Task<CompoundInfo> GetInfo(string name)
        {
            var compound = await context.Compounds.FirstOrDefaultAsync(x => x.Name == name);
            if (compound is not null)
            {
                return new CompoundInfo()
                {
                    PhysicalAppearance = compound.PhysicalAppearance,
                    Applications = compound.Applications
                };
            }
            string prompt = $"What is the physical appearance and the applications of " + name;
            ChatClient client = new(model: "gpt-4o-mini", apiKey: config["openAiApiKey"]);
            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "product",
                    jsonSchema: BinaryData.FromBytes("""
                                                     {
                                                         "type": "object",
                                                         "properties": {
                                                            "PhysicalAppearance": { "type": "string" },
                                                            "applications": { "type": "string" }
                                                         },
                                                         "required": ["PhysicalAppearance", "applications"],
                                                         "additionalProperties": false
                                                     }
                                                     """u8.ToArray()),
                    jsonSchemaIsStrict: true)
            };
            List<ChatMessage> messages =
            [
                new UserChatMessage(prompt),
            ];
            ChatCompletion completion = await client.CompleteChatAsync(messages, options);
            string response = completion.Content[0].Text;
            CompoundInfo json = JsonConvert.DeserializeObject<CompoundInfo>(response);

            await context.Compounds.AddAsync(new Compound()
            {
                Name = name,
                PhysicalAppearance = json.PhysicalAppearance,
                Applications = json.Applications
            });
            await context.SaveChangesAsync();

            return json;
        }
    }
}
