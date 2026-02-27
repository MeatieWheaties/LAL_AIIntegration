namespace LunchAndLearn_AIIntegration.AI
{
    using LunchAndLearn_AIIntegration.Data;
    using LunchAndLearn_AIIntegration.Data.Models;
    using LunchAndLearn_AIIntegration.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using OpenAI.Chat;
    using OpenAI.Responses;
    using System.Text.Json;

    public sealed class AiService
    {
        readonly IDbContextFactory<DB> _db;

        readonly AIRepository _aiRepo;
        private readonly ChatClient _chat;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        public AiService(IDbContextFactory<DB> db, ChatClient client, AIRepository aiRepo)
        {
            _db = db;
            _chat = client;
            _aiRepo = aiRepo;
        }

        public async Task<TOut> AskStructuredAsync<TIn, TOut>(
        string prompt,
        TIn input,
        BinaryData jsonSchema,                // schema for TOut
        string schemaName = "result",
        CancellationToken ct = default)
        {
            // 1) Serialize input to JSON
            var inputJson = JsonSerializer.Serialize(input, JsonOpts);

            // 2) Build messages
            List<ChatMessage> messages =
            [
                new SystemChatMessage("You are a precise JSON generator. Return ONLY JSON that matches the schema."),
            new UserChatMessage(
                $"{prompt}\n\n" +
                "Input JSON:\n" +
                inputJson
            ),
        ];

            // 3) Enforce JSON schema output (Structured Outputs)
            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: schemaName,
                    jsonSchema: jsonSchema,
                    jsonSchemaIsStrict: true)
            };

            // 4) Call model
            ChatCompletion completion = await _chat.CompleteChatAsync(messages, options, ct);

            // 5) Deserialize output JSON into TOut
            var json = completion.Content[0].Text;
            var result = JsonSerializer.Deserialize<TOut>(json, JsonOpts);

            if (result is null)
                throw new InvalidOperationException("Model returned empty/invalid JSON.");

            return result;
        }

        public async Task AssessKobold(int id)
        {
            try
            {
                await using var _ = await _db.CreateDbContextAsync();

                var _kobold = _.Kobolds.FirstOrDefault(x => x.Id == id);

                if (_kobold == null) return;

                var _ai = await _aiRepo.GetConfigAsync();

                var _assess = await AskStructuredAsync<Input, Output>(_ai.RenderContext(), new Input
                {
                    KoboldName = _kobold.Name,
                    KoboldMessage = _kobold.Message,
                }, BinaryData.FromString(Output.SCHEMA));

                var _assessment = new Assessment
                {
                    KoboldId = _kobold.Id,
                    Response = _assess.Response,
                    ConfidenceLevel = _assess.ConfidenceLevel,
                    Items = new List<Item>()
                };
                foreach(var item in _assess.Items)
                {
                    _assessment.Items.Add(new Item
                    {
                        Name = item
                    });
                }

                await _aiRepo.CommitAssessment(_assessment);
            }
            catch (Exception ex)
            {
                var _catch = ex;
            }
        }

        public async Task AssessFoodRecomendations(string request, List<Item> items)
        {
            try
            {
                await using var _ = await _db.CreateDbContextAsync();

                var _assess = await AskStructuredAsync<string, Output>(request,
                    RenderFoodList(items), BinaryData.FromString(Output.SCHEMA));

                var _test = _assess;

            } catch (Exception ex)
            {

            }
        }
        
        string RenderFoodList(List<Item> items)
        {
            var _result = "";

            for(int x = 0; x < items.Count; x++)
            {
                if (x == 0)
                    _result += items[x].Name;
                else _result += ", " + items[x].Name;
            }

            return _result;
        }
    }
}
