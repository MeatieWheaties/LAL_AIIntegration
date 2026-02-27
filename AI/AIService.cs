namespace LunchAndLearn_AIIntegration.AI
{
    using LunchAndLearn_AIIntegration.Data;
    using LunchAndLearn_AIIntegration.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using OpenAI.Chat;
    using OpenAI.Responses;
    using System.Text.Json;

    public sealed class AiService
    {
        readonly IDbContextFactory<DB> _db;
        private readonly ChatClient _chat;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        public AiService(IDbContextFactory<DB> db, ChatClient client)
        {
            _db = db;
            _chat = client;
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

        public async Task<string> AskAsync(string userText, CancellationToken ct = default)
        {
            return "";
            //var options = new CreateResponseOptions
            //{

            //};

            //options.InputItems.Add(ResponseItem.CreateUserMessageItem(userText));

            //var response = await _client.CreateResponseAsync(options, ct);
            //// SDK shape varies a bit by version; the README shows iterating OutputItems.
            //// A common pattern is to pull the first message text you find:
            //var msg = response.Value.OutputItems.OfType<MessageResponseItem>().FirstOrDefault();
            //return msg?.Content?.FirstOrDefault()?.Text ?? "";
        }

        const string CONTEXT = "This kobold is often in gardens and digging in dirt.";

        const string OBJECTIVE = "Determine if this kobold has mentioned any edible foods that are acceptable for human standards. List any foods found in the Items array. Leave a response for how useful the kobold's input is. Ignore crude and bad grammar. Focus just on actionable items.";

        const string OUTPUT_SCHEMA = """
{
  "type": "object",
  "additionalProperties": false,
  "properties": {
    "response": { "type": "string" },
    "items": { "type": "array", "items": { "type": "string" } }
  },
  "required": ["response", "items"]
}
""";

        public async Task AssessKobold(int id)
        {
            try
            {
                await using var _ = await _db.CreateDbContextAsync();

                var _kobold = _.Kobolds.FirstOrDefault(x => x.Id == id);

                if (_kobold == null) return;

                var _assess = await AskStructuredAsync<Input, Output>(OBJECTIVE, new Input
                {
                    KoboldName = _kobold.Name,
                    KoboldMessage = _kobold.Message,
                    Context = CONTEXT,
                }, BinaryData.FromString(OUTPUT_SCHEMA));

                var _test = _assess;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
