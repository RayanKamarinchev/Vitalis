using Microsoft.Extensions.Configuration;
using NCDK.SMARTS;
using NCDK.Smiles;
using NCDK;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using NCDK.IO.Formats;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;
using Newtonsoft.Json;
using OpenAI.Chat;
using PubChem.NET;
using Vitalis.Core.Models.GptResponses;
using Vitalis.Data;
using Vitalis.Data.Entities;
using static Vitalis.Core.Infrastructure.Constants;

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

        public List<Reaction> GetPossibleReactions(string reactant)
        {
            //cycloalkanes 2 halogens
            //dehalogenation
            //benxene preparation
            var mol = ChemConstants.smilesParser.ParseSmiles(reactant);

            List<Reaction> reactions = new List<Reaction>();
            AddReaction("Cl2", "", "hv");
            AddReaction("Br2", "", "hv");
            AddReaction("HNO3", "", "t");

            if (ChemConstants.hexagonPattern.Matches(mol))
            {
                AddReaction("", "", "t, cat");
            }


            if (ChemConstants.halogensPattern.Matches(mol))
            {
                AddReaction("KOH", "", "alcohol");
                AddReaction("KOH", "", "aqua");
                AddReaction("H2O", "", "");
                AddReaction("NH3", "", "");
                AddReaction("NaCN", "", "");

                AddReaction("Li", "", "");
                AddReaction("Mg", "", "");
            }

            //alkene alkine
            if (ChemConstants.alkenesPattern.Matches(mol) || ChemConstants.alkynesPattern.Matches(mol))
            {
                
                //dehydrogenation
                AddReaction("", "", "t");
                AddReaction("HCl", "", "");
                AddReaction("HBr", "", "");
                AddReaction("H2", "Ni", "t, p");
                AddReaction("HCN", "", "");
                AddReaction("H2O", "H+", "t, p");
                AddReaction("H2O + KMnO4", "", "");//TODO
                AddReaction("KMnO4", "", "OH-, t", "H2SO4");
                AddReaction("O2", "", "300 C", "Ag");
                AddReaction("O2", "PdCl2 . CuCl2", "t");

            }

            if (ChemConstants.alkynesPattern.Matches(mol))
            {
                //dehydrogenation
                AddReaction("", "", "t");
                AddReaction("H2", "cat. Lindlar", "aqua");
                if (ChemConstants.acidPropertiesAlkynePattern.Matches(mol))
                {
                    AddReaction("NaNH2", "", "");
                }
            }

            //alchohol
            if (ChemConstants.alcoholsPattern.Matches(mol))
            {
                //dehydration
                AddReaction("", "H2SO4", "t = 180C");
                AddReaction("", "Pt", "t");

                AddReaction("Na", "", "t");

                AddReaction("HCl", "", "");
                AddReaction("HBr", "", "");
                AddReaction("NH3", "Al2O3", "t");
                AddReaction("HCN", "", "");
                AddReaction("CH3CH3", "", "");

                AddReaction("PBr3", "", "t < 0C");
                AddReaction("PCl3", "", "t < 0C");
                AddReaction("PCl5", "", "t < 0C");
                AddReaction("PBr5", "", "t < 0C");
                AddReaction("SOCl2", "", "");

                //eterification
                AddReaction("CH3CH2OH", "H2SO4", "t = 140C");

                AddReaction("CH3COOH", "H+", "t");

                AddReaction("KMnO4", "K2Cr2O7", "");

                //Iodoform
                AddReaction("NaOH + I2", "", "");
            }

            //carbonyl derrivatives
            if (ChemConstants.carbonylPattern.Matches(mol))
            {
                AddReaction("LiAlH4", "", "");
                AddReaction("H2O", "", "");
                AddReaction("CH3CH2OH", "", "");
                AddReaction("HCN", "", "");
                //complex
                AddReaction("NH2CH3", "", "");

                AddReaction("NaHSO3", "", "");
                AddReaction("KMnO4", "", "");
                AddReaction("", "NaBH4", "", "H+");
                //questionable
                AddReaction("NaOH", "", "");
            }

            if (ChemConstants.carboxylicAcidPattern.Matches(mol))
            {
                AddReaction("Na", "", "t");
                AddReaction("NaOH", "", "t");
                AddReaction("NH3", "", "t");
                AddReaction("CH3CH2OH", "", "t");

                AddReaction("PCl3", "", "t");
                AddReaction("PCl5", "", "t");
                AddReaction("SOCl2", "", "t");

                AddReaction("LiAlH4", "", "");
                //decarboxylation
                AddReaction("", "", "t");
            }

            //has benzene nucleus
            if (ChemConstants.benzenePattern.Matches(mol))
            {
                AddReaction("CH3COCl", "AlCl3 (lewis acid)", "");
                AddReaction("H2", "", "t, p");
                AddReaction("KMnO4", "", "OH-, t", "H2SO4");
                AddReaction("HCl", "", "");
                AddReaction("HBr", "", "");
                AddReaction("HCN", "", "");
            }

            //if (aminesPattern.Matches(mol))
            //{
                
            //}

            if (ChemConstants.diazoniumPattern.Matches(mol))
            {
                AddReaction("CuCN", "", "t");
                AddReaction("H3PO2 + H2O", "", "t");
                AddReaction("H2O", "", "t");
                AddReaction("CuBr", "", "t");
            }

            if (ChemConstants.nitrilesPattern.Matches(mol))
            {
                AddReaction("LiAlH4", "", "");
                AddReaction("H2O", "", "");
            }

            if (ChemConstants.amidesPattern.Matches(mol))
            {
                AddReaction("LiAlH4", "", "");
            }

            if (ChemConstants.nitroarenesPattern.Matches(mol))
            {
                AddReaction("H2", "Ni", "t, p");
            }

            if (ChemConstants.aminoarenesPattern.Matches(mol))
            {
                //диазониеви соли
                //TODO might fail
                AddReaction("NaNO2 + HCl", "", "t = 0C");
            }

            if (ChemConstants.geminalHalogensPattern.Matches(mol))
            {
                AddReaction("NaNH2", "strong base", "t");
                AddReaction("H2O", "", "");
            }

            if (ChemConstants.soldiumAlkynePattern.Matches(mol))
            {
                AddReaction("ClCH2R", "", "");
            }

            if (ChemConstants.soldiumAlkoxidePattern.Matches(mol))
            {
                AddReaction("ClCH2CH3", "", "");
                AddReaction("BrCH2CH3", "", "");
            }

            return reactions;

            void AddReaction(string reagent, string catalyst, string conditions, string followUp = "")
            {
                reactions.Add(new Reaction(reagent, catalyst, conditions, followUp));
            }
        }

        public async Task<string> PredictProduct(string reactant, Reaction reaction)
        {
            
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
