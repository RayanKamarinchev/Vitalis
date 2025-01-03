using Microsoft.Extensions.Configuration;
using NCDK.SMARTS;
using NCDK.Smiles;
using NCDK;
using System.Text.RegularExpressions;
using NCDK.IO.Formats;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;
using Newtonsoft.Json;
using OpenAI.Chat;
using Vitalis.Core.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Vitalis.Core.Services
{
    public class MoleculeService : IMoleculeService
    {
        private readonly IConfiguration config;
        private readonly SmilesParser smilesParser = new SmilesParser();
        private readonly SmartsPattern benzenePattern = SmartsPattern.Create("c1ccccc1");
        private readonly SmartsPattern acidPropertiesAlkynePattern = SmartsPattern.Create("[C;H1]#[C;H0]");
        private readonly SmartsPattern soldiumAlkynePattern = SmartsPattern.Create("[C;Na1]#[C;H0]");
        private readonly SmartsPattern geminalHalogensPattern = SmartsPattern.Create("[CX4]([F,Cl,Br,I])[F,Cl,Br,I]");
        private readonly SmartsPattern alcoholsPattern = SmartsPattern.Create("[CX4][OH]");
        private readonly SmartsPattern alkenesPattern = SmartsPattern.Create("[C]=[C]");
        private readonly SmartsPattern alkynesPattern = SmartsPattern.Create("[C]#[C]");
        private readonly SmartsPattern halogensPattern = SmartsPattern.Create("[C][Cl,Br]");
        private readonly SmartsPattern carbonylPattern = SmartsPattern.Create("[C]=[O]");
        private readonly SmartsPattern aminesPattern = SmartsPattern.Create("[NX3;!$(N=*)]");
        private readonly SmartsPattern soldiumAlkoxidePattern = SmartsPattern.Create("[Na+].[O-][C]");
        private readonly SmartsPattern nitrilesPattern = SmartsPattern.Create("[C]#N");
        private readonly SmartsPattern amidesPattern = SmartsPattern.Create("[CX3](=[O])[NX3]");
        private readonly SmartsPattern nitroarenesPattern = SmartsPattern.Create("[a][N+](=O)[O-]");
        private readonly SmartsPattern aminoarenesPattern = SmartsPattern.Create("[a][NH2]");
        private readonly SmartsPattern diazoniumPattern = SmartsPattern.Create("[a][N+]#N");
        private readonly SmartsPattern carboxylicAcidPattern = SmartsPattern.Create("[CX3](=O)[OX2H]");
        private readonly SmartsPattern hexagonPattern = SmartsPattern.Create("C1CCCCC1");

        public MoleculeService(IConfiguration _config)
        {
            config = _config;
        }

        public List<Reaction> GetPossibleReactions(string reactant)
        {
            //cycloalkanes 2 halogens
            //dehalogenation
            //benxene preparation
            var mol = smilesParser.ParseSmiles(reactant);

            List<Reaction> reactions = new List<Reaction>();
            AddReaction("Cl2", "", "hv");
            AddReaction("Br\u2082", "", "hv");
            AddReaction("HNO3", "", "t");
            AddReaction("H2SO4", "", "t");

            //dehydration
            AddReaction("", "", "t");

            if (hexagonPattern.Matches(mol))
            {
                AddReaction("", "", "t, cat");
            }


            if (halogensPattern.Matches(mol))
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
            if (alkenesPattern.Matches(mol) || alkynesPattern.Matches(mol))
            {
                AddReaction("HCl", "", "");
                AddReaction("HBr", "", "");
                AddReaction("H2", "Ni", "t, p");
                AddReaction("HCN", "", "");
                AddReaction("H2O", "H+", "t, p");
                AddReaction("H2O + KMnO4", "", "");
                AddReaction("KMnO4", "", "OH-, t", "H2SO4");
                AddReaction("O2", "", "300 C", "Ag");
                AddReaction("O2", "PdCl2 . CuCl2", "t");

            }

            if (alkynesPattern.Matches(mol))
            {
                AddReaction("", "cat. Lindlar", "aqua");
                if (acidPropertiesAlkynePattern.Matches(mol))
                {
                    AddReaction("NaNH2", "", "");
                }
            }

            //alchohol
            if (alcoholsPattern.Matches(mol))
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
            if (carbonylPattern.Matches(mol))
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

            if (carboxylicAcidPattern.Matches(mol))
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
            if (benzenePattern.Matches(mol))
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

            if (diazoniumPattern.Matches(mol))
            {
                AddReaction("CuCN", "", "t");
                AddReaction("H3PO2 + H2O", "", "t");
                AddReaction("H2O", "", "t");
                AddReaction("CuBr", "", "t");
            }

            if (nitrilesPattern.Matches(mol))
            {
                AddReaction("LiAlH4", "", "");
                AddReaction("H2O", "", "");
            }

            if (amidesPattern.Matches(mol))
            {
                AddReaction("LiAlH4", "", "");
            }

            if (nitroarenesPattern.Matches(mol))
            {
                AddReaction("H2", "Ni", "t, p");
            }

            if (aminoarenesPattern.Matches(mol))
            {
                //диазониеви соли
                //TODO might fail
                AddReaction("NaNO2 + HCl", "", "t = 0C");
            }

            if (geminalHalogensPattern.Matches(mol))
            {
                AddReaction("NaNH2", "strong base", "t");
                AddReaction("H2O", "", "");
            }

            if (soldiumAlkynePattern.Matches(mol))
            {
                AddReaction("ClCH2R", "", "");
            }

            if (soldiumAlkoxidePattern.Matches(mol))
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



        public async Task<string> PredictProduct(string reactant, string reagent, string catalyst, string conditions, string followUp)
        {
            string reactionPrompt = $"{reactant} + {reagent}, catalyst: {catalyst}, under conditions: {conditions}";
            if (followUp != "")
            {
                reactionPrompt += $", next step: {followUp}";
            }

            ChatClient client = new(model: "gpt-4o", apiKey: config["openAiApiKey"]);

            string prompt = "output json with the structure formula and the valid SMILES formula of the main product of the reaction:\n" + reactionPrompt;
            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "product",
                    jsonSchema: BinaryData.FromBytes("""
                                                     {
                                                         "type": "object",
                                                         "properties": {
                                                            "reactant1": { "type": "string" },
                                                            "reactant2": { "type": "string" },
                                                            "explanation": { "type": "string" },
                                                            "reactionEquation": { "type": "string" },
                                                            "product": { "type": "string" },
                                                            "productInSMILES": { "type": "string" }
                                                         },
                                                         "required": ["reactant1", "reactant2", "explanation", "reactionEquation", "product", "productInSMILES"],
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
            //convert response to json
            Response json = JsonConvert.DeserializeObject<Response>(response);
            string res = json.ProductInSMILES.Replace(" ", String.Empty);

            return res;
        }
    }
}
